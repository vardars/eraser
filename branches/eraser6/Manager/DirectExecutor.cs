using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.IO;

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
				this.Main();
			});

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
					task.ID = unusedIds[0];
					unusedIds.RemoveAt(0);
				}
				else
					task.ID = ++nextId;
			}

			//Add the task to the set of tasks
			lock (tasksLock)
			{
				tasks.Add(task.ID, task);

				//If the task is scheduled to run now, break the waiting thread and
				//run it immediately
				if (task.Schedule == Schedule.RunNow)
				{
					scheduledTasks.Add(DateTime.Now, task);
					schedulerInterrupt.Set();
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
			}
			return true;
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
					try
					{
						//Run the task
						foreach (Task.ErasureTarget target in task.Entries)
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

					//If the task is a recurring task, reschedule it since we are done.
					if (task.Schedule is RecurringSchedule)
						((RecurringSchedule)task.Schedule).Reschedule(DateTime.Now);
				}

				//Wait for half a minute to check for the next scheduled task.
				schedulerInterrupt.WaitOne(30000, false);
			}
		}

		/// <summary>
		/// Executes a unused space erase.
		/// </summary>
		/// <param name="target">The target of the unused space erase.</param>
		private void EraseUnusedSpace(Task task, Task.UnusedSpace target)
		{
			throw new NotImplementedException("Unused space erasures are not "+
				"currently implemented");
		}

		/// <summary>
		/// Erases a file or folder on the volume.
		/// </summary>
		/// <param name="target">The target of the erasure.</param>
		private void EraseFilesystemObject(Task task, Task.FilesystemObject target)
		{
			List<string> paths = target.GetPaths();
			TaskProgressEventArgs eventArgs = new TaskProgressEventArgs(0, 0);

			//Get the erasure method if the user specified he wants the default.
			ErasureMethod method = target.Method;
			if (method == ErasureMethodManager.Default)
				method = ErasureMethodManager.GetInstance(Globals.Settings.DefaultFileErasureMethod);

			//Iterate over every path, and erase the path.
			for (int i = 0; i < paths.Count; ++i)
			{
				//Update the task progress
				eventArgs.overallProgress = (uint)(i * 100) / (uint)paths.Count;
				eventArgs.currentItemName = paths[i];
				eventArgs.currentItemProgress = 0;
				eventArgs.totalPasses = method.Passes;
				task.OnProgressChanged(eventArgs);

				//Make sure the file does not have any attributes which may
				//affect the erasure process
				FileInfo info = new FileInfo(paths[i]);
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
				if ((info.Attributes & FileAttributes.ReadOnly) != 0)
					info.Attributes &= ~FileAttributes.ReadOnly;

				//Create the file stream, and call the erasure method
				//to write to the stream.
				using (FileStream strm = new FileStream(info.FullName,
					FileMode.Open, FileAccess.Write, FileShare.None,
					8, FileOptions.WriteThrough))
				{
					method.Erase(strm, PRNGManager.GetInstance(Globals.Settings.ActivePRNG),
						delegate(uint currentProgress, uint currentPass)
						{
							eventArgs.currentPass = currentPass;
							eventArgs.currentItemProgress = currentProgress;
							task.OnProgressChanged(eventArgs);
						}
					);

					//Set the length of the file to 0.
					strm.Seek(0, SeekOrigin.Begin);
					strm.SetLength(0);
				}

				//Remove the file.
				RemoveFile(info);
			}
		}

		/// <summary>
		/// Securely removes the filename of the file.
		/// </summary>
		/// <param name="info">The FileInfo object representing the file.</param>
		private void RemoveFile(FileInfo info)
		{
			//Set the date of the file to be invalid to prevent forensic
			//detection
			info.CreationTime = info.LastWriteTime = info.LastAccessTime =
				DateTime.MinValue;
			info.Attributes = FileAttributes.Normal;
			info.Attributes = FileAttributes.NotContentIndexed;

			//Rename the file a few times to erase the record from the MFT.
			for (uint i = 0; i < FilenameErasePasses; ++i)
			{
				//Get a random file name
				PRNG prng = null;
				byte[] newFileNameAry = new byte[info.Name.Length];
				prng.NextBytes(newFileNameAry);
				string newFileName = (new System.Text.ASCIIEncoding()).
					GetString(newFileNameAry);

				//Validate the name
				const string validFileNameChars = "0123456789abcdefghijklmnopqrs" +
					"tuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
				for (int j = 0, k = newFileName.Length; j < k; ++j)
					if (!Char.IsLetterOrDigit(newFileName[j]))
					{
						newFileName.Insert(j, validFileNameChars[
							(int)newFileName[j] % validFileNameChars.Length].ToString());
						newFileName.Remove(j + 1, 1);
					}

				//Rename the file.
				info.MoveTo(info.DirectoryName + Path.DirectorySeparatorChar + newFileName);
			}
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
