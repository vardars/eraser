using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

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
		/// Class representing a free space erase.
		/// </summary>
		public class FreeSpace : ErasureTarget
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

		private uint id;
		private string name;
		private Schedule schedule = Schedule.RunNow;
		private List<ErasureTarget> entries = new List<ErasureTarget>();
	}
}
