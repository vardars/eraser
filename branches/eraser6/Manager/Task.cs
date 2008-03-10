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
			/// <returns></returns>
			internal abstract List<string> GetPaths();

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
			internal override List<string> GetPaths()
			{
				List<string> result = new List<string>();
				result.Add(Path);
				return result;
			}
		}

		/// <summary>
		/// Represents a folder and its files which are to be erased.
		/// </summary>
		public class Folder : FilesystemObject
		{
			internal override List<string> GetPaths()
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

				//Then exclude each file.
				if (ExcludeMask.Length != 0)
				{
					string regex = Regex.Escape(ExcludeMask).Replace("\\*", ".*").
						Replace("\\?", ".");
					Regex excludePattern = new Regex(regex, RegexOptions.IgnoreCase);
					foreach (FileInfo file in files)
						if (excludePattern.Matches(file.FullName).Count == 0)
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
			set { id = value; }
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

		/// <summary>
		/// The prototype for events handling the progress changed event.
		/// </summary>
		/// <param name="e">The new progress value.</param>
		public delegate void ProgressEventFunction(TaskProgressEventArgs e);

		/// <summary>
		/// The event object holding all event handlers.
		/// </summary>
		public event ProgressEventFunction ProgressChanged;

		/// <summary>
		/// Broadcasts a ProgressChanged event.
		/// </summary>
		/// <param name="e">The new progress value.</param>
		internal void OnProgressChanged(TaskProgressEventArgs e)
		{
			if (ProgressChanged != null)
				ProgressChanged.Invoke(e);
		}

		private uint id;
		private string name;
		private Schedule schedule = Schedule.RunNow;
		private List<ErasureTarget> entries = new List<ErasureTarget>();
		private List<LogEntry> log = new List<LogEntry>();
	}

	/// <summary>
	/// A Event argument object containing the progress of the task.
	/// </summary>
	public class TaskProgressEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="overallProgress">The overall progress of the task.</param>
		/// <param name="currentItemProgress">The progress for the individual
		/// component of the task.</param>
		public TaskProgressEventArgs(uint overallProgress, uint currentItemProgress)
		{
			this.overallProgress = overallProgress;
			this.currentItemProgress = currentItemProgress;
		}

		/// <summary>
		/// A number from 0 to 100 detailing the overall progress of the task.
		/// </summary>
		public uint OverallProgress
		{
			get { return overallProgress; }
		}

		internal uint overallProgress;
		internal uint currentItemProgress;
		internal string currentItemName;
		internal uint currentPass;
		internal uint totalPasses;
	}
}
