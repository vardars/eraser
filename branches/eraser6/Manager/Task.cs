using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Eraser.Manager
{
	/// <summary>
	/// Deals with an erase task.
	/// </summary>
	public class Task
	{
		/// <summary>
		/// Represents a generic target of erasure
		/// </summary>
		public abstract class ErasureTarget
		{
			/// <summary>
			/// The method used for erasing the file. If the variable is equal to
			/// ErasureMethodManager.Default then the default is queried for the
			/// task type.
			/// </summary>
			public ErasureMethod Method
			{
				get { return method; }
				set { method = value; }
			}

			/// <summary>
			/// Retrieves the text to display representing this task.
			/// </summary>
			public abstract string UIText
			{
				get;
			}

			private ErasureMethod method = null;
		}

		/// <summary>
		/// Class representing a tangible object (file/folder) to be erased.
		/// </summary>
		public abstract class FilesystemObject : ErasureTarget
		{
			/// <summary>
			/// Retrieves the list of files/folders to erase as a list.
			/// </summary>
			/// <param name="totalSize">Returns the total size in bytes of the
			/// items.</param>
			/// <returns>A list containing the paths to all the files to be erased.</returns>
			internal abstract List<string> GetPaths(out long totalSize);

			/// <summary>
			/// The path to the file or folder referred to by this object.
			/// </summary>
			public string Path
			{
				get { return path; }
				set { path = value; }
			}

			public override string UIText
			{
				get { return Path; }
			}

			private string path;
		}

		/// <summary>
		/// Class representing a unused space erase.
		/// </summary>
		public class UnusedSpace : ErasureTarget
		{
			public string Drive;

			public override string UIText
			{
				get { return string.Format("Unused disk space ({0})", Drive); }
			}
		}

		/// <summary>
		/// Class representing a file to be erased.
		/// </summary>
		public class File : FilesystemObject
		{
			internal override List<string> GetPaths(out long totalSize)
			{
				List<string> result = new List<string>();
				result.Add(Path);

				totalSize = new FileInfo(Path).Length;
				return result;
			}
		}

		/// <summary>
		/// Represents a folder and its files which are to be erased.
		/// </summary>
		public class Folder : FilesystemObject
		{
			internal override List<string> GetPaths(out long totalSize)
			{
				//Get a list to hold all the resulting paths.
				List<string> result = new List<string>();

				//Open the root of the search, including every file matching the pattern
				DirectoryInfo dir = new DirectoryInfo(Path);

				//List recursively all the files which match the include pattern.
				string includeMask = IncludeMask;
				if (includeMask.Length == 0)
					includeMask = "*.*";
				FileInfo[] files = dir.GetFiles(includeMask, SearchOption.AllDirectories);

				//Then exclude each file and finalize the list and total file size
				totalSize = 0;
				if (ExcludeMask.Length != 0)
				{
					string regex = Regex.Escape(ExcludeMask).Replace("\\*", ".*").
						Replace("\\?", ".");
					Regex excludePattern = new Regex(regex, RegexOptions.IgnoreCase);
					foreach (FileInfo file in files)
						if (excludePattern.Matches(file.FullName).Count == 0)
						{
							totalSize += file.Length;
							result.Add(file.FullName);
						}
				}
				else
					foreach (FileInfo file in files)
					{
						totalSize += file.Length;
						result.Add(file.FullName);
					}
				
				//Return the filtered list.
				return result;
			}

			/// <summary>
			/// A wildcard expression stating the condition for the set of files to include.
			/// The include mask is applied before the exclude mask is applied. If this value
			/// is empty, all files and folders within the folder specified is included.
			/// </summary>
			public string IncludeMask
			{
				get { return includeMask; }
				set { includeMask = value; }
			}

			/// <summary>
			/// A wildcard expression stating the condition for removing files from the set
			/// of included files. If this value is omitted, all files and folders extracted
			/// by the inclusion mask is erased.
			/// </summary>
			public string ExcludeMask
			{
				get { return excludeMask; }
				set { excludeMask = value; }
			}

			/// <summary>
			/// Determines if Eraser should delete the folder after the erase process.
			/// </summary>
			public bool DeleteIfEmpty
			{
				get { return deleteIfEmpty; }
				set { deleteIfEmpty = value; }
			}

			private string includeMask;
			private string excludeMask;
			private bool deleteIfEmpty;
		}

		/// <summary>
		/// The unique identifier for this task. This ID will be persistent across
		/// executions.
		/// </summary>
		public uint ID
		{
			get { return id; }
		}

		/// <summary>
		/// The Executor object which is managing this task.
		/// </summary>
		public Executor Executor
		{
			get { return executor; }
		}

		/// <summary>
		/// The name for this task. This is just an opaque value for the user to
		/// recognize the task.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// The name of the task, used for display in UI elements.
		/// </summary>
		public string UIText
		{
			get
			{
				//Simple case, the task name was given by the user.
				if (Name.Length != 0)
					return Name;

				string result = string.Empty;
				if (Entries.Count < 3)
					//Simpler case, small set of data.
					foreach (Task.ErasureTarget tgt in Entries)
						result += tgt.UIText + ", ";
				else
					//Ok, we've quite a few entries, get the first, the mid and the end.
					for (int i = 0; i < Entries.Count; i += Entries.Count / 3)
						result += Entries[i].UIText + ", ";
				return result.Substring(0, result.Length - 2);
			}
		}

		/// <summary>
		/// Gets the status of the task - whether it is being executed.
		/// </summary>
		public bool Executing
		{
			get { return executing; }
		}

		/// <summary>
		/// The set of data to erase when this task is executed.
		/// </summary>
		public List<ErasureTarget> Entries
		{
			get { return entries; }
			set { entries = value; }
		}

		/// <summary>
		/// The schedule for running the task.
		/// </summary>
		public Schedule Schedule
		{
			get { return schedule; }
			set { schedule = value; }
		}

		/// <summary>
		/// Retrieves the log for this task.
		/// </summary>
		public List<LogEntry> Log
		{
			get
			{
				lock (log)
					return log.GetRange(0, log.Count);
			}
		}

		/// <summary>
		/// Logs the message and its associated information into the task Log.
		/// </summary>
		/// <param name="entry">The log entry structure representing the log
		/// message.</param>
		internal void LogEntry(LogEntry entry)
		{
			lock (log)
				log.Add(entry);
		}

		#region Events
		/// <summary>
		/// The prototype for events handling just a task object as the event argument.
		/// </summary>
		/// <param name="e">The task object.</param>
		public delegate void TaskEventFunction(TaskEventArgs e);

		/// <summary>
		/// The prototype for events handling the progress changed event.
		/// </summary>
		/// <param name="e">The new progress value.</param>
		public delegate void ProgressEventFunction(TaskProgressEventArgs e);

		/// <summary>
		/// The start of the execution of a task.
		/// </summary>
		public event TaskEventFunction TaskStarted;

		/// <summary>
		/// The event object holding all event handlers.
		/// </summary>
		public event ProgressEventFunction ProgressChanged;

		/// <summary>
		/// The completion of the execution of a task.
		/// </summary>
		public event TaskEventFunction TaskFinished;

		/// <summary>
		/// Broadcasts the task execution start event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnTaskStarted(TaskEventArgs e)
		{
			if (TaskStarted != null)
				TaskStarted(e);
			executing = true;
		}

		/// <summary>
		/// Broadcasts a ProgressChanged event.
		/// </summary>
		/// <param name="e">The new progress value.</param>
		internal void OnProgressChanged(TaskProgressEventArgs e)
		{
			if (ProgressChanged != null)
				ProgressChanged(e);
		}

		/// <summary>
		/// Broadcasts the task execution completion event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnTaskFinished(TaskEventArgs e)
		{
			if (TaskFinished != null)
				TaskFinished(e);
			executing = false;
		}
		#endregion

		internal uint id;
		internal Executor executor;
		internal bool cancelled = false;

		private string name;
		private bool executing;

		private Schedule schedule = Schedule.RunNow;
		private List<ErasureTarget> entries = new List<ErasureTarget>();
		private List<LogEntry> log = new List<LogEntry>();
	}

	/// <summary>
	/// A base event class for all event arguments involving a task.
	/// </summary>
	public class TaskEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="task">The task being run.</param>
		public TaskEventArgs(Task task)
		{
			this.task = task;
		}

		/// <summary>
		/// The executing task.
		/// </summary>
		public Task Task
		{
			get { return task; }
		}

		private Task task;
	}

	/// <summary>
	/// A Event argument object containing the progress of the task.
	/// </summary>
	public class TaskProgressEventArgs : TaskEventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="task">The task being run.</param>
		/// <param name="overallProgress">The overall progress of the task.</param>
		/// <param name="currentItemProgress">The progress for the individual
		/// component of the task.</param>
		public TaskProgressEventArgs(Task task, int overallProgress,
			int currentItemProgress)
			: base(task)
		{
			this.overallProgress = overallProgress;
			this.currentItemProgress = currentItemProgress;
		}

		/// <summary>
		/// A number from 0 to 100 detailing the overall progress of the task.
		/// </summary>
		public int OverallProgress
		{
			get { return overallProgress; }
		}

		/// <summary>
		/// The amount of time left for the operation to complete, in seconds.
		/// </summary>
		public int TimeLeft
		{
			get { return timeLeft; }
		}

		/// <summary>
		/// The current erasure target - the current item being erased.
		/// </summary>
		public Task.ErasureTarget CurrentTarget
		{
			get { return currentTarget; }
		}

		/// <summary>
		/// A number from 0 to 100 detailing the overall progress of the item.
		/// </summary>
		public int CurrentItemProgress
		{
			get { return currentItemProgress; }
		}

		/// <summary>
		/// The file name of the item being erased.
		/// </summary>
		public string CurrentItemName
		{
			get { return currentItemName; }
		}

		/// <summary>
		/// The pass number of a multi-pass erasure method.
		/// </summary>
		public int CurrentPass
		{
			get { return currentPass; }
		}

		/// <summary>
		/// The total number of passes to complete before this erasure method is
		/// completed.
		/// </summary>
		public int TotalPasses
		{
			get { return totalPasses; }
		}

		internal int overallProgress;
		internal int timeLeft;
		internal Task.ErasureTarget currentTarget;
		internal int currentItemProgress;
		internal string currentItemName;
		internal int currentPass = 1;
		internal int totalPasses;
	}
}
