using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.IO;

using Eraser.Util;
using System.Security.Principal;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Eraser.Manager
{
	/// <summary>
	/// The DirectExecutor class is used by the Eraser GUI directly when the program
	/// is run without the help of a Service.
	/// </summary>
	public class DirectExecutor : Executor, IDisposable
	{
		public DirectExecutor()
		{
			thread = new Thread(delegate()
				{
					Main();
				}
			);

			thread.Start();
			Thread.Sleep(0);
		}

		void IDisposable.Dispose()
		{
			thread.Abort();
			schedulerInterrupt.Set();
		}

		public override void AddTask(ref Task task)
		{
			lock (unusedIdsLock)
			{
				if (unusedIds.Count != 0)
				{
					task.id = unusedIds[0];
					unusedIds.RemoveAt(0);
				}
				else
					task.id = ++nextId;
			}

			//Set the executor of the task
			task.executor = this;

			//Add the task to the set of tasks
			lock (tasksLock)
			{
				tasks.Add(task.ID, task);

				//If the task is scheduled to run now, break the waiting thread and
				//run it immediately
				if (task.Schedule == Schedule.RunNow)
				{
					QueueTask(task);
				}
				//If the task is scheduled, add the next execution time to the list
				//of schduled tasks.
				else if (task.Schedule != Schedule.RunOnRestart)
				{
					scheduledTasks.Add((task.Schedule as RecurringSchedule).NextRun, task);
				}
			}
		}

		public override bool DeleteTask(uint taskId)
		{
			lock (tasksLock)
			{
				if (!tasks.ContainsKey(taskId))
					return false;

				lock (unusedIdsLock)
					unusedIds.Add(taskId);
				tasks.Remove(taskId);

				for (int i = 0; i != scheduledTasks.Count; ++i)
					if (scheduledTasks.Values[i].id == taskId)
						scheduledTasks.RemoveAt(i);
			}

			return true;
		}

		public override void ReplaceTask(Task task)
		{
			lock (tasksLock)
			{
				//Replace the task in the global set
				if (!tasks.ContainsKey(task.ID))
					return;

				tasks[task.ID] = task;

				//Then replace the task if it is in the queue
				for (int i = 0; i != scheduledTasks.Count; ++i)
					if (scheduledTasks.Values[i].id == task.ID)
						scheduledTasks.Values[i] = task;
			}
		}

		public override void QueueTask(Task task)
		{
			lock (tasksLock)
			{
				//Set the task variable to indicate that the task is already
				//waiting to be executed.
				task.queued = true;

				//Queue the task to be run immediately.
				scheduledTasks.Add(DateTime.Now, task);
				schedulerInterrupt.Set();
			}
		}

		public override void QueueRestartTasks()
		{
			lock (tasksLock)
			{
				foreach (Task task in tasks.Values)
					if (task.Schedule == Schedule.RunOnRestart)
						QueueTask(task);
			}
		}

		public override void CancelTask(Task task)
		{
			lock (currentTask)
			{
				if (currentTask == task)
				{
					currentTask.cancelled = true;
					return;
				}
			}

			lock (tasksLock)
				for (int i = 0; i != scheduledTasks.Count; ++i)
					if (scheduledTasks.Values[i] == task)
					{
						scheduledTasks.RemoveAt(i);
						return;
					}

			throw new ArgumentOutOfRangeException("The task to be cancelled must " +
				"either be currently executing or queued.");
		}

		public override Task GetTask(uint taskId)
		{
			lock (tasksLock)
			{
				if (!tasks.ContainsKey(taskId))
					return null;
				return tasks[taskId];
			}
		}

		public override List<Task> GetTasks()
		{
			lock (tasksLock)
			{
				Task[] result = new Task[tasks.Count];
				tasks.Values.CopyTo(result, 0);
				return new List<Task>(result);
			}
		}

		public override void SaveTaskList(Stream stream)
		{
			lock (tasksLock)
				new BinaryFormatter().Serialize(stream, tasks);
		}

		public override void LoadTaskList(Stream stream)
		{
			lock (tasksLock)
			{
				//Load the list into the dictionary
				tasks = (Dictionary<uint, Task>)new BinaryFormatter().Deserialize(stream);

				//Ignore the next portion if there are no tasks
				if (tasks.Count == 0)
					return;

				lock (unusedIdsLock)
				{
					//Find gaps in the numbering
					nextId = 1;
					foreach (uint id in tasks.Keys)
					{
						tasks[id].executor = this;
						while (id > nextId)
							unusedIds.Add(++nextId);
						++nextId;
					}

					//Decrement the ID, since the next ID will be preincremented
					//before use.
					--nextId;
				}
			}
		}

		/// <summary>
		/// The thread entry point for this object. This object operates on a queue
		/// and hence the thread will sequentially execute tasks.
		/// </summary>
		private void Main()
		{
			//The waiting thread will utilize a polling loop to check for new
			//scheduled tasks. This will be checked every 30 seconds. However,
			//when the thread is waiting for a new task, it can be interrupted.
			while (thread.ThreadState != ThreadState.AbortRequested)
			{
				//Check for a new task
				Task task = null;
				lock (tasksLock)
				{
					if (scheduledTasks.Count != 0 &&
						(scheduledTasks.Values[0].Schedule == Schedule.RunNow ||
						 scheduledTasks.Keys[0] <= DateTime.Now))
					{
						task = scheduledTasks.Values[0];
						scheduledTasks.RemoveAt(0);
					}
				}

				if (task != null)
				{
					//Set the currently executing task.
					currentTask = task;

					try
					{
						//Broadcast the task started event.
						task.queued = false;
						task.cancelled = false;
						task.OnTaskStarted(new TaskEventArgs(task));

						//Run the task
						foreach (Task.ErasureTarget target in task.Targets)
							try
							{
								if (target is Task.UnusedSpace)
									EraseUnusedSpace(task, (Task.UnusedSpace)target);
								else if (target is Task.FilesystemObject)
									EraseFilesystemObject(task, (Task.FilesystemObject)target);
								else
									throw new ArgumentException("Unknown erasure target.");
							}
							catch (FatalException)
							{
								throw;
							}
							catch (Exception e)
							{
								task.LogEntry(new LogEntry(e.Message, LogLevel.ERROR));
							}
					}
					catch (FatalException e)
					{
						task.LogEntry(new LogEntry(e.Message, LogLevel.FATAL));
					}
					finally
					{
						//If the task is a recurring task, reschedule it since we are done.
						if (task.Schedule is RecurringSchedule)
							((RecurringSchedule)task.Schedule).Reschedule(DateTime.Now);

						//If the task is an execute on restart task, it is only run
						//once and can now be restored to an immediately executed task
						if (task.Schedule == Schedule.RunOnRestart)
							task.Schedule = Schedule.RunNow;

						//And the task finished event.
						task.OnTaskFinished(new TaskEventArgs(task));
					}
				}

				//Wait for half a minute to check for the next scheduled task.
				schedulerInterrupt.WaitOne(30000, false);
			}
		}

		private class WriteStatistics
		{
			public WriteStatistics()
			{
				startTime = DateTime.Now;
			}

			public int Speed
			{
				get
				{
					if (DateTime.Now == startTime)
						return 0;

					if ((DateTime.Now - lastSpeedCalc).Seconds < 10 && lastSpeed != 0)
						return lastSpeed;

					lastSpeedCalc = DateTime.Now;
					lastSpeed = (int)(dataWritten / (DateTime.Now - startTime).TotalSeconds);
					return lastSpeed;
				}
			}

			public long DataWritten
			{
				get { return dataWritten; }
				set { dataWritten = value; }
			}

			private DateTime startTime;
			private DateTime lastSpeedCalc;
			private long dataWritten;
			private int lastSpeed;
		}

		/// <summary>
		/// Executes a unused space erase.
		/// </summary>
		/// <param name="target">The target of the unused space erase.</param>
		private void EraseUnusedSpace(Task task, Task.UnusedSpace target)
		{
			//Check for sufficient privileges to run the unused space erasure.
			if (!Permissions.IsAdministrator())
			{
				string exceptionString = "The program does not have the required permissions " +
					"to erase the unused space on disk";
				if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
					Environment.OSVersion.Version >= new Version(6, 0))
				{
					exceptionString += ". Run the program as administrator and retry the operation.";
				}
				throw new Exception(exceptionString);
			}

			//If the user is under disk quotas, log a warning message
			if (Drive.HasQuota(target.Drive))
				task.LogEntry(new LogEntry("The drive which is having its unused space erased has " +
					"disk quotas active. This will prevent the complete erasure of unused space and " +
					"will pose a security concern", LogLevel.WARNING));

			//Get the erasure method if the user specified he wants the default.
			ErasureMethod method = target.Method;
			if (method == ErasureMethodManager.Default)
				method = ErasureMethodManager.GetInstance(
					ManagerLibrary.Instance.Settings.DefaultUnusedSpaceErasureMethod);

			TaskProgressEventArgs eventArgs = new TaskProgressEventArgs(task, 0, 0);
			eventArgs.currentTarget = target;
			eventArgs.currentItemName = "Cluster tips";
			eventArgs.totalPasses = method.Passes;
			eventArgs.timeLeft = -1;
			task.OnProgressChanged(eventArgs);

			//Erase the cluster tips of every file on the drive.
			if (target.EraseClusterTips)
			{
				EraseClusterTips(task, target, method,
					delegate(int currentFile, string currentFilePath, int totalFiles)
					{
						eventArgs.currentItemName = "(Tips) " + currentFilePath;
						eventArgs.currentItemProgress = (int)((float)currentFile / totalFiles * 100);
						eventArgs.overallProgress = eventArgs.CurrentItemProgress / 10;
						task.OnProgressChanged(eventArgs);
					}
				);
			}

			//Make a folder to dump our temporary files in
			DirectoryInfo info = new DirectoryInfo(target.Drive).Root;
			{
				string directoryName;
				do
					directoryName = GetRandomFileName(18);
				while (Directory.Exists(info.FullName + Path.DirectorySeparatorChar + directoryName));
				info = info.CreateSubdirectory(directoryName);
			}

			try
			{
				//Set the folder's compression flag off since we want to use as much
				//space as possible
				if (Eraser.Util.File.IsCompressed(info.FullName))
					Eraser.Util.File.SetCompression(info.FullName, false);

				//Determine the total amount of data that needs to be written.
				WriteStatistics statistics = new WriteStatistics();
				long totalSize = method.CalculateEraseDataSize(
					null, Drive.GetFreeSpace(info.Root.FullName));

				//Continue creating files while there is free space.
				eventArgs.currentItemName = "Unused space";
				task.OnProgressChanged(eventArgs);
				while (Drive.GetFreeSpace(info.Root.FullName) > 0)
				{
					//Generate a non-existant file name
					string currFile;
					do
						currFile = info.FullName + Path.DirectorySeparatorChar +
							GetRandomFileName(18);
					while (System.IO.File.Exists(currFile));
					
					//Create the stream
					using (FileStream stream = new FileStream(currFile,
						FileMode.CreateNew, FileAccess.Write))
					{

						//Set the length of the file to be the amount of free space left
						//or the maximum size of one of these dumps.
						stream.SetLength(Math.Min(ErasureMethod.FreeSpaceFileUnit,
							Drive.GetFreeSpace(info.Root.FullName)));

						//Then run the erase task
						method.Erase(stream, long.MaxValue,
							PRNGManager.GetInstance(ManagerLibrary.Instance.Settings.ActivePRNG),
							delegate(long lastWritten, int currentPass)
							{
								statistics.DataWritten += lastWritten;
								eventArgs.currentPass = currentPass;
								eventArgs.currentItemProgress = (int)(statistics.DataWritten * 100 / totalSize);
								eventArgs.overallProgress = (int)((10 + eventArgs.currentItemProgress * 0.8));

								if (statistics.Speed == 0)
									eventArgs.timeLeft = -1;
								else
									eventArgs.timeLeft = (int)((totalSize - statistics.DataWritten) / statistics.Speed);
								task.OnProgressChanged(eventArgs);

								lock (currentTask)
									if (currentTask.cancelled)
										throw new FatalException("The task was cancelled.");
							}
						);
					}
				}

				//If the drive is using NTFS, try to squeeze entries into the MFT as well
				eventArgs.currentItemName = "Resident MFT entries";
				task.OnProgressChanged(eventArgs);

				try
				{
					for ( ; ; )
					{
						//Open this stream
						using (FileStream strm = new FileStream(info.FullName + Path.DirectorySeparatorChar +
							GetRandomFileName(18), FileMode.CreateNew, FileAccess.Write))
						{
							//Stretch the file size to the size of one MFT record
							strm.SetLength(1);

							//Then run the erase task
							method.Erase(strm, long.MaxValue,
								PRNGManager.GetInstance(ManagerLibrary.Instance.Settings.ActivePRNG),
								null);
						}
					}
				}
				catch (IOException)
				{
					//OK, enough squeezing.
				}
			}
			finally
			{
				eventArgs.currentItemName = "Removing temporary files";
				task.OnProgressChanged(eventArgs);

				//Remove the folder holding all our temporary files.
				RemoveFolder(info);
			}


			//NotImplemented: clear directory entries: Eraser.cpp@2321
		}

		private delegate void SubFoldersHandler(DirectoryInfo info);
		private delegate void ClusterTipsEraseProgress(int currentFile,
			string currentFilePath, int totalFiles);
		private static void EraseClusterTips(Task task, Task.UnusedSpace target,
			ErasureMethod method, ClusterTipsEraseProgress callback)
		{
			//List all the files which can be erased.
			List<string> files = new List<string>();
			SubFoldersHandler subFolders = null;
			subFolders = delegate(DirectoryInfo info)
			{
				try
				{
					foreach (FileInfo file in info.GetFiles())
						if (Eraser.Util.File.IsProtectedSystemFile(file.FullName))
							task.LogEntry(new LogEntry(string.Format("{0} did not have its cluster tips " +
								"erased, because it is a system file", file.FullName), LogLevel.INFORMATION));
						else
						{
							foreach (string i in Util.File.GetADSes(file))
								files.Add(file.FullName + ':' + i);
							files.Add(file.FullName);
						}

					foreach (DirectoryInfo subDir in info.GetDirectories())
						subFolders(subDir);
				}
				catch (UnauthorizedAccessException)
				{
				}
				catch (IOException)
				{
				}
			};

			subFolders(new DirectoryInfo(target.Drive));

			//For every file, erase the cluster tips.
			for (int i = 0, j = files.Count; i != j; ++i)
			{
				EraseFileClusterTips(files[i], method);
				callback(i, files[i], files.Count);
			}
		}

		/// <summary>
		/// Erases the cluster tips of the given file.
		/// </summary>
		/// <param name="file">The file to erase.</param>
		/// <param name="method">The erasure method to use.</param>
		private static void EraseFileClusterTips(string file, ErasureMethod method)
		{
			//Get the file access times
			StreamInfo streamInfo = new StreamInfo(file);
			DateTime lastAccess = DateTime.MinValue, lastWrite = DateTime.MinValue,
			         created = DateTime.MinValue;
			{
				FileInfo info = streamInfo.File;
				if (info != null)
				{
					lastAccess = info.LastAccessTime;
					lastWrite = info.LastWriteTime;
					created = info.CreationTime;
				}
			}

			//Create the stream, lengthen the file, then tell the erasure method
			//to erase the tips.
			using (FileStream stream = streamInfo.Open(FileMode.Open, FileAccess.Write))
			{
				long fileLength = stream.Length;
				long fileArea = GetFileArea(file);

				try
				{
					stream.SetLength(fileArea);
					stream.Seek(fileLength, SeekOrigin.Begin);

					//Erase the file
					method.Erase(stream, long.MaxValue, PRNGManager.GetInstance(
						ManagerLibrary.Instance.Settings.ActivePRNG), null);
				}
				finally
				{
					//Make sure the file is restored!
					stream.SetLength(fileLength);
				}
			}

			//Set the file times
			FileInfo fileInfo = streamInfo.File;
			if (fileInfo != null)
			{
				fileInfo.LastAccessTime = lastAccess;
				fileInfo.LastWriteTime = lastWrite;
				fileInfo.CreationTime = created;
			}
		}

		/// <summary>
		/// Erases a file or folder on the volume.
		/// </summary>
		/// <param name="target">The target of the erasure.</param>
		private void EraseFilesystemObject(Task task, Task.FilesystemObject target)
		{
			//Retrieve the list of files to erase.
			long dataTotal = 0;
			List<string> paths = target.GetPaths(out dataTotal);
			TaskProgressEventArgs eventArgs = new TaskProgressEventArgs(task, 0, 0);

			//Get the erasure method if the user specified he wants the default.
			ErasureMethod method = target.Method;
			if (method == ErasureMethodManager.Default)
				method = ErasureMethodManager.GetInstance(ManagerLibrary.Instance.Settings.DefaultFileErasureMethod);

			//Calculate the total amount of data required to finish the wipe.
			dataTotal = method.CalculateEraseDataSize(paths, dataTotal);

			//Record the start of the erasure pass so we can calculate speed of erasures
			WriteStatistics statistics = new WriteStatistics();

			//Iterate over every path, and erase the path.
			for (int i = 0; i < paths.Count; ++i)
			{
				//Update the task progress
				eventArgs.overallProgress = (i * 100) / paths.Count;
				eventArgs.currentTarget = target;
				eventArgs.currentItemName = paths[i];
				eventArgs.currentItemProgress = 0;
				eventArgs.totalPasses = method.Passes;
				task.OnProgressChanged(eventArgs);

				//Make sure the file does not have any attributes which may affect
				//the erasure process
				bool isReadOnly = false;
				StreamInfo info = new StreamInfo(paths[i]);
				if ((info.Attributes & FileAttributes.Compressed) != 0 ||
					(info.Attributes & FileAttributes.Encrypted) != 0 ||
					(info.Attributes & FileAttributes.SparseFile) != 0 ||
					(info.Attributes & FileAttributes.ReparsePoint) != 0)
				{
					//Log the error
					throw new ArgumentException("Compressed, encrypted, or sparse" +
						"files cannot be erased with Eraser.");
				}

				//Remove the read-only flag, if it is set.
				if (isReadOnly = info.IsReadOnly)
					info.IsReadOnly = false;

				try
				{
					//Create the file stream, and call the erasure method to write to
					//the stream.
					using (FileStream strm = info.Open(FileMode.Open, FileAccess.Write,
						FileShare.None, FileOptions.WriteThrough))
					{
						//Set the end of the stream after the wrap-round the cluster size
						strm.SetLength(GetFileArea(paths[i]));

						//If the stream is empty, there's nothing to overwrite. Continue
						//to the next entry
						if (strm.Length != 0)
						{
							//Then erase the file.
							long itemWritten = 0,
								 itemTotal = method.CalculateEraseDataSize(null, strm.Length);
							method.Erase(strm, long.MaxValue,
								PRNGManager.GetInstance(ManagerLibrary.Instance.Settings.ActivePRNG),
								delegate(long lastWritten, int currentPass)
								{
									statistics.DataWritten += lastWritten;
									eventArgs.currentPass = currentPass;
									eventArgs.currentItemProgress = (int)
										((itemWritten += lastWritten) * 100 / itemTotal);
									eventArgs.overallProgress = (int)
										(statistics.DataWritten * 100 / dataTotal);

									if (statistics.Speed != 0)
										eventArgs.timeLeft = (int)
											(dataTotal - statistics.DataWritten) / statistics.Speed;
									else
										eventArgs.timeLeft = -1;
									task.OnProgressChanged(eventArgs);

									lock (currentTask)
										if (currentTask.cancelled)
											throw new FatalException("The task was cancelled.");
								}
							);
						}

						//Set the length of the file to 0.
						strm.Seek(0, SeekOrigin.Begin);
						strm.SetLength(0);
					}

					//Remove the file.
					FileInfo fileInfo = info.File;
					if (fileInfo != null)
						RemoveFile(fileInfo);
				}
				finally
				{
					//Re-set the read-only flag
					info.IsReadOnly = isReadOnly;
				}
			}

			//If the user requested a folder removal, do it.
			if (target is Task.Folder)
			{
				Task.Folder fldr = (Task.Folder)target;
				if (fldr.DeleteIfEmpty)
					RemoveFolder(new DirectoryInfo(fldr.Path));
			}
		}

		/// <summary>
		/// Retrieves the size of the file on disk, calculated by the amount of
		/// clusters allocated by it.
		/// </summary>
		/// <param name="filePath">The path to the file.</param>
		/// <returns>The area of the file.</returns>
		private static long GetFileArea(string filePath)
		{
			StreamInfo info = new StreamInfo(filePath);
			uint clusterSize = Drive.GetClusterSize(info.Directory.Root.FullName);
			return (info.Length + (clusterSize - 1)) & ~(clusterSize - 1);
		}

		/// <summary>
		/// Securely removes files.
		/// </summary>
		/// <param name="info">The FileInfo object representing the file.</param>
		private static void RemoveFile(FileInfo info)
		{
			//Set the date of the file to be invalid to prevent forensic
			//detection
			info.CreationTime = info.LastWriteTime = info.LastAccessTime =
				new DateTime(1800, 1, 1, 0, 0, 0);
			info.Attributes = FileAttributes.Normal;
			info.Attributes = FileAttributes.NotContentIndexed;

			//Rename the file a few times to erase the record from the MFT.
			for (int i = 0; i < FilenameErasePasses; ++i)
			{
				//Rename the file.
				string newPath = info.DirectoryName + Path.DirectorySeparatorChar +
					GetRandomFileName(info.Name.Length);

				//Try to rename the file. If it fails, it is probably due to another
				//process locking the file. Defer, then rename again.
				try
				{
					info.MoveTo(newPath);
				}
				catch (IOException)
				{
					Thread.Sleep(100);
					--i;
				}
			}

			//Then delete the file.
			info.Delete();
		}

		private static void RemoveFolder(DirectoryInfo info)
		{
			foreach (FileInfo file in info.GetFiles())
				RemoveFile(file);
			foreach (DirectoryInfo dir in info.GetDirectories())
				RemoveFolder(dir);

			//Then clean up this folder.
			for (int i = 0; i < FilenameErasePasses; ++i)
			{
				//Rename the folder.
				string newPath = info.Parent.FullName + Path.DirectorySeparatorChar +
					GetRandomFileName(info.Name.Length);

				//Try to rename the file. If it fails, it is probably due to another
				//process locking the file. Defer, then rename again.
				try
				{
					info.MoveTo(newPath);
				}
				catch (IOException)
				{
					Thread.Sleep(100);
					--i;
				}
			}

			//Remove the folder
			info.Delete();
		}

		/// <summary>
		/// Generates a random file name with the given length.
		/// </summary>
		/// <param name="length">The length of the file name to generate.</param>
		/// <returns>A random file name.</returns>
		private static string GetRandomFileName(int length)
		{
			//Get a random file name
			PRNG prng = PRNGManager.GetInstance(ManagerLibrary.Instance.Settings.ActivePRNG);
			byte[] newFileNameAry = new byte[length];
			prng.NextBytes(newFileNameAry);

			//Validate the name
			string validFileNameChars = "0123456789abcdefghijklmnopqrstuvwxyz" +
				"ABCDEFGHIJKLMNOPQRSTUVWXYZ _+=-()[]{}',`~!";
			for (int j = 0, k = newFileNameAry.Length; j < k; ++j)
				newFileNameAry[j] = (byte)validFileNameChars[
					(int)newFileNameAry[j] % validFileNameChars.Length];

			return new System.Text.UTF8Encoding().GetString(newFileNameAry);
		}

		/// <summary>
		/// The thread object.
		/// </summary>
		private Thread thread;

		/// <summary>
		/// The lock preventing concurrent access for the tasks list and the
		/// tasks queue.
		/// </summary>
		private object tasksLock = new object();

		/// <summary>
		/// The list of tasks. Includes all immediate, reboot, and recurring tasks
		/// </summary>
		private Dictionary<uint, Task> tasks = new Dictionary<uint, Task>();

		/// <summary>
		/// The queue of tasks. This queue is executed when the first element's
		/// timestamp (the key) has been past. This list assumes that all tasks
		/// are sorted by timestamp, smallest one first.
		/// </summary>
		private SortedList<DateTime, Task> scheduledTasks =
			new SortedList<DateTime, Task>();

		/// <summary>
		/// The currently executing task.
		/// </summary>
		Task currentTask;

		/// <summary>
		/// The list of task IDs for recycling.
		/// </summary>
		private List<uint> unusedIds = new List<uint>();

		/// <summary>
		/// Lock preventing concurrent access for the IDs.
		/// </summary>
		private object unusedIdsLock = new object();

		/// <summary>
		/// Incrementing ID. This value is incremented by one every time an ID
		/// is required by no unused IDs remain.
		/// </summary>
		private uint nextId = 0;

		/// <summary>
		/// An automatically reset event allowing the addition of new tasks to
		/// interrupt the thread's sleeping state waiting for the next recurring
		/// task to be due.
		/// </summary>
		AutoResetEvent schedulerInterrupt = new AutoResetEvent(true);
	}
}
