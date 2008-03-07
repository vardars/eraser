using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	/// <summary>
	/// Deals with an erase task.
	/// </summary>
	public class Task
	{
		/// <summary>
		/// Class representing the data to be erased.
		/// </summary>
		public abstract class Data
		{
			/// <summary>
			/// Retrieves the list of files/folders to erase as a list.
			/// </summary>
			/// <returns></returns>
			internal abstract List<string> GetPaths();
		}

		/// <summary>
		/// Class representing a free space erase.
		/// </summary>
		public class FreeSpace
		{
			public string Drive;
		}

		/// <summary>
		/// Class representing a file to be erased.
		/// </summary>
		public class File : Data
		{
			internal override List<string> GetPaths()
			{
				return null;
			}

			/// <summary>
			/// The path to the file referred to by this object.
			/// </summary>
			public string Path
			{
				get { return path; }
				set { path = value; }
			}

			private string path;
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
		/// The name for this task. This is just an opaque value for the user to recognize
		/// the task.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// The set of data to erase when this task is executed.
		/// </summary>
		public List<object> Entries
		{
			get { return entries; }
			set { entries = value; }
		}

		private uint id;
		private string name;
		private List<object> entries;
	}
}
