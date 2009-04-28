/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By: Kasra Nassiri <cjax@users.sourceforge.net> @17/10/2008
 * Modified By: 
 * 
 * This file is part of Eraser.
 * 
 * Eraser is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 * 
 * Eraser is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

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
	public class DirectExecutor : Executor
	{
		public DirectExecutor()
		{
			thread = new Thread(Main);
		}

		public override void Dispose()
		{
			thread.Abort();
			schedulerInterrupt.Set();
		}

		public override void Run()
		{
			thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			thread.Start();
		}

		public override void AddTask(ref Task task)
		{
			lock (unusedIdsLock)
			{
				if (unusedIds.Count != 0)
				{
					task.ID = unusedIds[0];
					unusedIds.RemoveAt(0);
				}
				else
					task.ID = ++nextId;
			}

			//Set the executor of the task
			task.Executor = this;

			//Add the task to the set of tasks
			lock (tasksLock)
				tasks.Add(task.ID, task);

			//Call all the event handlers who registered to be notified of tasks
			//being added.
			OnTaskAdded(task);

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
				ScheduleTask(task);
			}
		}

		public override bool DeleteTask(uint taskId)
		{
			lock (tasksLock)
			{
				if (!tasks.ContainsKey(taskId))
					return false;

				Task task = tasks[taskId];
				lock (unusedIdsLock)
					unusedIds.Add(taskId);
				tasks.Remove(taskId);

				for (int i = 0; i != scheduledTasks.Count; )
					if (scheduledTasks.Values[i].ID == taskId)
						scheduledTasks.RemoveAt(i);
					else
						++i;

				//Call all event handlers registered to be notified of task deletions.
				OnTaskDeleted(task);
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
					if (scheduledTasks.Values[i].ID == task.ID)
					{
						scheduledTasks.RemoveAt(i);
						if (task.Schedule is RecurringSchedule)
							ScheduleTask(task);
						else if (task.Schedule == Schedule.RunNow)
							QueueTask(task);
					}
			}
		}

		public override void QueueTask(Task task)
		{
			lock (tasksLock)
			{
				//Set the task variable to indicate that the task is already
				//waiting to be executed.
				task.Queued = true;

				//Queue the task to be run immediately.
				scheduledTasks.Add(DateTime.Now, task);
				schedulerInterrupt.Set();
			}
		}

		public override void ScheduleTask(Task task)
		{
			RecurringSchedule schedule = (RecurringSchedule)task.Schedule;
			if (schedule.MissedPreviousSchedule &&
				ManagerLibrary.Instance.Settings.ExecuteMissedTasksImmediately)
				//OK, we've missed the schedule and the user wants the thing
				//to follow up immediately.
				scheduledTasks.Add(DateTime.Now, task);
			else
				scheduledTasks.Add(schedule.NextRun, task);
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
					currentTask.Cancelled = true;
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

			throw new ArgumentOutOfRangeException(S._("The task to be cancelled must " +
				"either be currently executing or queued."));
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
						Task currentTask = tasks[id];
						currentTask.Executor = this;
						while (id > nextId)
							unusedIds.Add(nextId++);
						++nextId;

						//Check if the task is recurring. If it is, check if we missed it.
						if (currentTask.Schedule is RecurringSchedule)
							ScheduleTask(currentTask);
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
						//Prevent the system from sleeping.
						KernelAPI.SetThreadExecutionState(KernelAPI.EXECUTION_STATE.ES_CONTINUOUS |
							KernelAPI.EXECUTION_STATE.ES_SYSTEM_REQUIRED);

						//Broadcast the task started event.
						task.Queued = false;
						task.Cancelled = false;
						task.OnTaskStarted(new TaskEventArgs(task));
						OnTaskProcessing(task);

						//Start a new log session to separate this session's events
						//from previous ones.
						task.Log.NewSession();

						//Run the task
						TaskProgressManager progress = new TaskProgressManager(currentTask);
						foreach (Task.ErasureTarget target in task.Targets)
							try
							{
								progress.Event.CurrentTarget = target;
								++progress.Event.CurrentTargetIndex;
								if (target is Task.UnusedSpace)
									EraseUnusedSpace(task, (Task.UnusedSpace)target, progress);
								else if (target is Task.FilesystemObject)
									EraseFilesystemObject(task, (Task.FilesystemObject)target, progress);
								else
									throw new ArgumentException(S._("Unknown erasure target."));
							}
							catch (FatalException)
							{
								throw;
							}
							catch (Exception e)
							{
								task.Log.Add(new LogEntry(e.Message, LogLevel.ERROR));
							}
					}
					catch (FatalException e)
					{
						task.Log.Add(new LogEntry(e.Message, LogLevel.FATAL));
					}
					catch (Exception e)
					{
						task.Log.Add(new LogEntry(e.Message, LogLevel.ERROR));
					}
					finally
					{
						//Allow the system to sleep again.
						KernelAPI.SetThreadExecutionState(KernelAPI.EXECUTION_STATE.ES_CONTINUOUS);

						//If the task is a recurring task, reschedule it since we are done.
						if (task.Schedule is RecurringSchedule)
							((RecurringSchedule)task.Schedule).Reschedule(DateTime.Now);

						//If the task is an execute on restart task, it is only run
						//once and can now be restored to an immediately executed task
						if (task.Schedule == Schedule.RunOnRestart)
							task.Schedule = Schedule.RunNow;

						//And the task finished event.
						task.OnTaskFinished(new TaskEventArgs(task));
						OnTaskProcessed(currentTask);
					}

					currentTask = null;
				}

				//Wait for half a minute to check for the next scheduled task.
				schedulerInterrupt.WaitOne(30000, false);
			}
		}

		/// <summary>
		/// Manages the progress for any operation.
		/// </summary>
		private class ProgressManager
		{
			/// <summary>
			/// Starts measuring the speed of the task.
			/// </summary>
			public void Start()
			{
				startTime = DateTime.Now;
			}

			/// <summary>
			/// Tracks the amount of the operation completed.
			/// </summary>
			public long Completed
			{
				get
				{
					return completed;
				}
				set
				{
					lastCompleted += value - completed;
					completed = value;
				}
			}

			/// <summary>
			/// The amount to reach before the operation completes.
			/// </summary>
			public long Total
			{
				get
				{
					return total;
				}
				set
				{
					total = value;
				}
			}

			/// <summary>
			/// Gets the percentage of the operation completed.
			/// </summary>
			public float Progress
			{
				get
				{
					return (float)((double)Completed / Total);
				}
			}

			/// <summary>
			/// Computes the speed of the erase, in units of completion per second,
			/// based on the information collected in the previous 15 seconds.
			/// </summary>
			public int Speed
			{
				get
				{
					if (DateTime.Now == startTime)
						return 0;

					if ((DateTime.Now - lastSpeedCalc).Seconds < 15 && lastSpeed != 0)
						return lastSpeed;

					lastSpeed = (int)(lastCompleted / (DateTime.Now - lastSpeedCalc).TotalSeconds);
					lastSpeedCalc = DateTime.Now;
					lastCompleted = 0;
					return lastSpeed;
				}
			}

			/// <summary>
			/// Calculates the estimated amount of time left based on the total
			/// amount of information to erase and the current speed of the erase
			/// </summary>
			public TimeSpan TimeLeft
			{
				get
				{
					if (Speed == 0)
						return new TimeSpan(0, 0, -1);
					return new TimeSpan(0, 0, (int)((Total - Completed) / Speed));
				}
			}

			/// <summary>
			/// The starting time of the operation, used to determine average speed.
			/// </summary>
			private DateTime startTime;

			/// <summary>
			/// The last time a speed calculation was computed so that speed is not
			/// computed too often.
			/// </summary>
			private DateTime lastSpeedCalc;

			/// <summary>
			/// The last calculated speed of the operation.
			/// </summary>
			private int lastSpeed;

			/// <summary>
			/// The amount of the operation completed since the last speed computation.
			/// </summary>
			private long lastCompleted;

			/// <summary>
			/// The amount of the operation completed.
			/// </summary>
			private long completed;

			/// <summary>
			/// The amount to reach before the operation is completed.
			/// </summary>
			private long total;
		}

		/// <summary>
		/// Provides a common interface to track the progress made by the Erase functions.
		/// </summary>
		private class TaskProgressManager : ProgressManager
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			public TaskProgressManager(Task task)
			{
				foreach (Task.ErasureTarget target in task.Targets)
					Total += target.TotalData;

				Event = new TaskProgressEventArgs(task);
				Start();
			}

			/// <summary>
			/// The TaskProgressEventArgs object representing the progress of the current
			/// task.
			/// </summary>
			public TaskProgressEventArgs Event
			{
				get
				{
					return evt;
				}
				set
				{
					evt = value;
				}
			}

			private TaskProgressEventArgs evt;
		}

		#region Unused Space erasure functions
		/// <summary>
		/// Executes a unused space erase.
		/// </summary>
		/// <param name="task">The task currently being executed</param>
		/// <param name="target">The target of the unused space erase.</param>
		/// <param name="progress">The progress manager object managing the progress of the task</param>
		private void EraseUnusedSpace(Task task, Task.UnusedSpace target, TaskProgressManager progress)
		{
			//Check for sufficient privileges to run the unused space erasure.
			if (!Permissions.IsAdministrator())
			{
				if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
					Environment.OSVersion.Version >= new Version(6, 0))
				{
					throw new Exception(S._("The program does not have the required permissions " +
						"to erase the unused space on disk. Run the program as an administrator " +
						"and retry the operation."));
				}
				else
					throw new Exception(S._("The program does not have the required permissions " +
						"to erase the unused space on disk"));
			}

			//If the user is under disk quotas, log a warning message
			if (VolumeInfo.FromMountpoint(target.Drive).HasQuota)
				task.Log.Add(new LogEntry(S._("The drive which is having its unused space erased " +
					"has disk quotas active. This will prevent the complete erasure of unused " +
					"space and will pose a security concern"), LogLevel.WARNING));

			//Get the erasure method if the user specified he wants the default.
			ErasureMethod method = target.Method;
			
			//Erase the cluster tips of every file on the drive.
			if (target.EraseClusterTips)
			{
				progress.Event.CurrentItemName = S._("Cluster tips");
				progress.Event.CurrentTargetTotalPasses = method.Passes;
				progress.Event.TimeLeft = progress.TimeLeft;
				task.OnProgressChanged(progress.Event);

				ProgressManager tipProgress = new ProgressManager();
				tipProgress.Start();
				EraseClusterTips(task, target, method,
					delegate(int currentFile, string currentFilePath, int totalFiles)
					{
						tipProgress.Total = totalFiles;
						tipProgress.Completed = currentFile;

						progress.Event.CurrentItemName = S._("(Tips) {0}", currentFilePath);
						progress.Event.CurrentItemProgress = tipProgress.Progress;
						progress.Event.CurrentTargetProgress = progress.Event.CurrentItemProgress / 10;
						progress.Event.TimeLeft = tipProgress.TimeLeft;
						task.OnProgressChanged(progress.Event);

						lock (currentTask)
							if (currentTask.Cancelled)
								throw new FatalException(S._("The task was cancelled."));
					}
				);
			}

			//Make a folder to dump our temporary files in
			DirectoryInfo info = new DirectoryInfo(target.Drive);
			VolumeInfo volInfo = VolumeInfo.FromMountpoint(target.Drive);
			FileSystem fsManager = FileSystem.Get(volInfo);
			info = info.CreateSubdirectory(FileSystem.GenerateRandomFileName(info, 18));

			try
			{
				//Set the folder's compression flag off since we want to use as much
				//space as possible
				if (Eraser.Util.File.IsCompressed(info.FullName))
					Eraser.Util.File.SetCompression(info.FullName, false);

				//Determine the total amount of data that needs to be written.
				long totalSize = method.CalculateEraseDataSize(null, volInfo.TotalFreeSpace);

				//Continue creating files while there is free space.
				progress.Event.CurrentItemName = S._("Unused space");
				task.OnProgressChanged(progress.Event);
				while (volInfo.AvailableFreeSpace > 0)
				{
					//Generate a non-existant file name
					string currFile = FileSystem.GenerateRandomFileName(info, 18);

					//Create the stream
					using (FileStream stream = new FileStream(currFile, FileMode.CreateNew,
						FileAccess.Write, FileShare.None, 8, FileOptions.WriteThrough))
					{
						//Set the length of the file to be the amount of free space left
						//or the maximum size of one of these dumps.
						long streamLength = Math.Min(ErasureMethod.FreeSpaceFileUnit,
							volInfo.AvailableFreeSpace);

						//Handle IO exceptions gracefully, because the filesystem
						//may require more space than demanded by us for file allocation.
						while (true)
							try
							{
								stream.SetLength(streamLength);
								break;
							}
							catch (IOException)
							{
								if (streamLength > volInfo.ClusterSize)
									streamLength -= volInfo.ClusterSize;
								else
									throw;
							}

						//Then run the erase task
						method.Erase(stream, long.MaxValue,
							PRNGManager.GetInstance(ManagerLibrary.Instance.Settings.ActivePRNG),
							delegate(long lastWritten, int currentPass)
							{
								progress.Completed += lastWritten;
								progress.Event.CurrentItemPass = currentPass;
								progress.Event.CurrentItemProgress = progress.Progress;
								if (target.EraseClusterTips)
									progress.Event.CurrentTargetProgress = (float)
										(0.1f + progress.Event.CurrentItemProgress * 0.8f);
								else
									progress.Event.CurrentTargetProgress = (float)
										(progress.Event.CurrentItemProgress * 0.9f);
								progress.Event.TimeLeft = progress.TimeLeft;
								task.OnProgressChanged(progress.Event);

								lock (currentTask)
									if (currentTask.Cancelled)
										throw new FatalException(S._("The task was cancelled."));
							}
						);
					}
				}

				//Erase old resident file system table files
				progress.Event.CurrentItemName = S._("Old resident file system table files");
				task.OnProgressChanged(progress.Event);
				fsManager.EraseOldFilesystemResidentFiles(volInfo, method, null);
			}
			finally
			{
				//Remove the folder holding all our temporary files.
				progress.Event.CurrentItemName = S._("Removing temporary files");
				task.OnProgressChanged(progress.Event);
				fsManager.DeleteFolder(info);
			}

			//Then clean the old file system entries
			progress.Event.CurrentItemName = S._("Old file system entries");
			ProgressManager fsEntriesProgress = new ProgressManager();
			fsEntriesProgress.Start();
			fsManager.EraseDirectoryStructures(volInfo,
				delegate(int currentFile, int totalFiles)
				{
					lock (currentTask)
						if (currentTask.Cancelled)
							throw new FatalException(S._("The task was cancelled."));

					//Compute the progress
					fsEntriesProgress.Total = totalFiles;
					fsEntriesProgress.Completed = currentFile;

					//Set the event parameters, then broadcast the progress event.
					progress.Event.TimeLeft = fsEntriesProgress.TimeLeft;
					progress.Event.CurrentItemProgress = fsEntriesProgress.Progress;
					progress.Event.CurrentTargetProgress = (float)(
						0.9 + progress.Event.CurrentItemProgress / 10);
					task.OnProgressChanged(progress.Event);
				}
			);
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
				//Check if we've been cancelled
				if (task.Cancelled)
					throw new FatalException(S._("The task was cancelled."));

				try
				{
					//Skip this directory if it is a reparse point
					if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
					{
						task.Log.Add(new LogEntry(S._("Files in {0} did not have their cluster " +
							"tips erased because it is a hard link or a symbolic link.",
							info.FullName), LogLevel.INFORMATION));
						return;
					}

					foreach (FileInfo file in info.GetFiles())
						if (Util.File.IsProtectedSystemFile(file.FullName))
							task.Log.Add(new LogEntry(S._("{0} did not have its cluster tips " +
								"erased, because it is a system file", file.FullName),
								LogLevel.INFORMATION));
						else if ((file.Attributes & FileAttributes.ReparsePoint) != 0)
							task.Log.Add(new LogEntry(S._("{0} did not have its cluster tips " +
								"erased because it is a hard link or a symbolic link.",
								file.FullName), LogLevel.INFORMATION));
						else if ((file.Attributes & FileAttributes.Compressed) != 0 ||
							(file.Attributes & FileAttributes.Encrypted) != 0 ||
							(file.Attributes & FileAttributes.SparseFile) != 0)
						{
							task.Log.Add(new LogEntry(S._("{0} did not have its cluster tips " +
								"erased because it is compressed, encrypted or a sparse file.",
								file.FullName), LogLevel.INFORMATION));
						}
						else
						{
							try
							{
								foreach (string i in Util.File.GetADSes(file))
									files.Add(file.FullName + ':' + i);

								files.Add(file.FullName);
							}
							catch (IOException e)
							{
								task.Log.Add(new LogEntry(S._("{0} did not have its cluster tips erased " +
									"because of the following error: {1}", info.FullName, e.Message),
									LogLevel.ERROR));
							}
						}

					foreach (DirectoryInfo subDirInfo in info.GetDirectories())
						subFolders(subDirInfo);
				}
				catch (UnauthorizedAccessException e)
				{
					task.Log.Add(new LogEntry(S._("{0} did not have its cluster tips erased " +
						"because of the following error: {1}", info.FullName, e.Message),
						LogLevel.ERROR));
				}
				catch (IOException e)
				{
					task.Log.Add(new LogEntry(S._("{0} did not have its cluster tips erased " +
						"because of the following error: {1}", info.FullName, e.Message),
						LogLevel.ERROR));
				}
			};

			subFolders(new DirectoryInfo(target.Drive));

			//For every file, erase the cluster tips.
			for (int i = 0, j = files.Count; i != j; ++i)
			{
				//Get the file attributes for restoring later
				StreamInfo info = new StreamInfo(files[i]);
				FileAttributes fileAttr = info.Attributes;

				try
				{
					//Reset the file attributes.
					info.Attributes = FileAttributes.Normal;
					EraseFileClusterTips(files[i], method);
				}
				catch (Exception e)
				{
					task.Log.Add(new LogEntry(S._("{0} did not have its cluster tips erased. " +
						"The error returned was: {1}", files[i], e.Message), LogLevel.ERROR));
				}
				finally
				{
					info.Attributes = fileAttr;
				}
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
			DateTime lastAccess = DateTime.MinValue,
			         lastWrite = DateTime.MinValue,
			         created = DateTime.MinValue;
			//Create the stream, lengthen the file, then tell the erasure method
			//to erase the tips.
			using (FileStream stream = streamInfo.Open(FileMode.Open, FileAccess.Write,
				FileShare.None, FileOptions.WriteThrough))
			{
				long fileLength = stream.Length;
				long fileArea = GetFileArea(file);

				try
				{
					//Get the file access times
					FileInfo info = streamInfo.File;
					if (info != null)
					{
						lastAccess = info.LastAccessTime;
						lastWrite = info.LastWriteTime;
						created = info.CreationTime;
					}

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
		#endregion

		#region Filesystem Object erasure functions
		/// <summary>
		/// Erases a file or folder on the volume.
		/// </summary>
		/// <param name="task">The task currently being processed.</param>
		/// <param name="target">The target of the erasure.</param>
		/// <param name="progress">The progress manager for the current task.</param>
		private void EraseFilesystemObject(Task task, Task.FilesystemObject target,
			TaskProgressManager progress)
		{
			//Retrieve the list of files to erase.
			long dataTotal = 0;
			List<string> paths = target.GetPaths(out dataTotal);

			//Get the erasure method if the user specified he wants the default.
			ErasureMethod method = target.Method;

			//Calculate the total amount of data required to finish the wipe.
			dataTotal = method.CalculateEraseDataSize(paths, dataTotal);

			//Iterate over every path, and erase the path.
			for (int i = 0; i < paths.Count; ++i)
			{
				//Update the task progress
				progress.Event.CurrentTargetProgress = i / (float)paths.Count;
				progress.Event.CurrentTarget = target;
				progress.Event.CurrentItemName = paths[i];
				progress.Event.CurrentItemProgress = 0;
				progress.Event.CurrentTargetTotalPasses = method.Passes;
				task.OnProgressChanged(progress.Event);
				
				//Get the filesystem provider to handle the secure file erasures
				FileSystem fsManager = FileSystem.Get(VolumeInfo.FromMountpoint(paths[i]));

				//Remove the read-only flag, if it is set.
				StreamInfo info = new StreamInfo(paths[i]);
				bool isReadOnly = false;
				if (isReadOnly = info.IsReadOnly)
					info.IsReadOnly = false;

				try
				{
					//Make sure the file does not have any attributes which may affect
					//the erasure process
					if ((info.Attributes & FileAttributes.Compressed) != 0 || 
						(info.Attributes & FileAttributes.Encrypted) != 0 ||
						(info.Attributes & FileAttributes.SparseFile) != 0)
					{
						//Log the error
						task.Log.Add(new LogEntry(S._("The file {0} could not be erased " +
							"because the file was either compressed, encrypted or a sparse file.",
							info.FullName), LogLevel.ERROR));
					}

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
									dataTotal -= lastWritten;
									progress.Completed += lastWritten;
									progress.Event.CurrentItemPass = currentPass;
									progress.Event.CurrentItemProgress = (float)
										((itemWritten += lastWritten) / (float)itemTotal);
									progress.Event.CurrentTargetProgress =
										(i + progress.Event.CurrentItemProgress) /
										(float)paths.Count;
									progress.Event.TimeLeft = progress.TimeLeft;
									task.OnProgressChanged(progress.Event);

									lock (currentTask)
										if (currentTask.Cancelled)
											throw new FatalException(S._("The task was cancelled."));
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
						fsManager.DeleteFile(fileInfo);
				}
				catch (UnauthorizedAccessException)
				{
					task.Log.Add(new LogEntry(S._("The file {0} could not be erased because the " +
						"file's permissions prevent access to the file.", info.FullName),
						LogLevel.ERROR));
				}
				catch (FileLoadException)
				{
					task.Log.Add(new LogEntry(S._("The file {0} could not be erased because the " +
						"file is currently in use.", info.FullName), LogLevel.ERROR));
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
				progress.Event.CurrentItemName = S._("Removing folders...");
				task.OnProgressChanged(progress.Event);

				Task.Folder fldr = (Task.Folder)target;
				if (fldr.DeleteIfEmpty)
				{
					FileSystem fsManager = FileSystem.Get(VolumeInfo.FromMountpoint(fldr.Path));
					fsManager.DeleteFolder(new DirectoryInfo(fldr.Path));
				}
			}

			//If the user was erasing the recycle bin, clear the bin.
			if (target is Task.RecycleBin)
			{
				progress.Event.CurrentItemName = S._("Emptying recycle bin...");
				task.OnProgressChanged(progress.Event);

				ShellAPI.SHEmptyRecycleBin(IntPtr.Zero, null,
					ShellAPI.SHEmptyRecycleBinFlags.SHERB_NOCONFIRMATION |
					ShellAPI.SHEmptyRecycleBinFlags.SHERB_NOPROGRESSUI |
					ShellAPI.SHEmptyRecycleBinFlags.SHERB_NOSOUND);
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
			VolumeInfo volume = VolumeInfo.FromMountpoint(info.Directory.FullName);
			long clusterSize = volume.ClusterSize;
			return (info.Length + (clusterSize - 1)) & ~(clusterSize - 1);
		}
		#endregion

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
