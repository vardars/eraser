/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
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
using System.Text;
using System.Runtime.Serialization;

namespace Eraser.Manager
{
	/// <summary>
	/// The levels of logging allowing for the filtering of messages.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Informative messages.
		/// </summary>
		Information,

		/// <summary>
		/// Notice messages.
		/// </summary>
		Notice,

		/// <summary>
		/// Warning messages.
		/// </summary>
		Warning,

		/// <summary>
		/// Error messages.
		/// </summary>
		Error,

		/// <summary>
		/// Fatal errors.
		/// </summary>
		Fatal
	}

	/// <summary>
	/// The Logger class which handles log entries and manages entries.
	/// 
	/// The class has the notion of entries and sessions. Each session contains one
	/// or more (log) entries. This allows the program to determine if the last
	/// session had errors or not.
	/// </summary>
	[Serializable]
	public class Logger : ISerializable
	{
		#region Serialization code
		protected Logger(SerializationInfo info, StreamingContext context)
		{
			entries = (Dictionary<DateTime, List<LogEntry>>)
				info.GetValue("Entries", typeof(Dictionary<DateTime, List<LogEntry>>));
			foreach (DateTime key in entries.Keys)
				lastSession = key;
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Entries", entries);
		}
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public Logger()
		{
			entries = new Dictionary<DateTime, List<LogEntry>>();
		}

		/// <summary>
		/// The prototype of a registrant of the Log event.
		/// </summary>
		/// <param name="e"></param>
		public delegate void LogEventFunction(LogEntry e);

		/// <summary>
		/// All the registered event handlers for the log event of this task.
		/// </summary>
		public event LogEventFunction OnLogged;

		/// <summary>
		/// All the registered event handlers for handling when a new session has been
		/// started.
		/// </summary>
		public event EventHandler OnNewSession;

		/// <summary>
		/// Retrieves the log for this task.
		/// </summary>
		public Dictionary<DateTime, List<LogEntry>> Entries
		{
			get
			{
				return entries;
			}
		}

		/// <summary>
		/// Retrieves the log entries from the previous session.
		/// </summary>
		public List<LogEntry> LastSessionEntries
		{
			get
			{
				lock (entries)
					return entries[lastSession];
			}
		}

		/// <summary>
		/// Adds a new session to the log.
		/// </summary>
		internal void NewSession()
		{
			lock (entries)
			{
				lastSession = DateTime.Now;
				entries.Add(lastSession, new List<LogEntry>());
			}

			if (OnNewSession != null)
				OnNewSession(this, new EventArgs());
		}

		/// <summary>
		/// Logs the message and its associated information into the current session.
		/// </summary>
		/// <param name="entry">The log entry structure representing the log
		/// message.</param>
		internal void Add(LogEntry entry)
		{
			lock (entries)
			{
				if (entries.Count == 0)
					NewSession();
				entries[lastSession].Add(entry);
			}

			if (OnLogged != null)
				OnLogged(entry);
		}

		/// <summary>
		/// Clears the log entries from the log.
		/// </summary>
		public void Clear()
		{
			lock (entries)
			{
				entries.Clear();
				lastSession = DateTime.MinValue;
			}
		}

		/// <summary>
		/// The log entries.
		/// </summary>
		private Dictionary<DateTime, List<LogEntry>> entries;

		/// <summary>
		/// The last session
		/// </summary>
		private DateTime lastSession;
	}

	/// <summary>
	/// Represents a log entry.
	/// </summary>
	[Serializable]
	public struct LogEntry : ISerializable
	{
		#region Serialization code
		private LogEntry(SerializationInfo info, StreamingContext context)
			: this()
		{
			Level = (LogLevel)info.GetValue("Level", typeof(LogLevel));
			Timestamp = (DateTime)info.GetValue("Timestamp", typeof(DateTime));
			Message = (string)info.GetValue("Message", typeof(string));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Level", Level);
			info.AddValue("Timestamp", Timestamp);
			info.AddValue("Message", Message);
		}
		#endregion

		/// <summary>
		/// Creates a LogEntry structure.
		/// </summary>
		/// <param name="message">The log message.</param>
		/// <param name="level">The type of log entry.</param>
		public LogEntry(string message, LogLevel level)
			: this()
		{
			Message = message;
			Level = level;
			Timestamp = DateTime.Now;
		}

		/// <summary>
		/// The type of log entry.
		/// </summary>
		public LogLevel Level { get; private set; }

		/// <summary>
		/// The time which the message was logged.
		/// </summary>
		public DateTime Timestamp { get; private set; }

		/// <summary>
		/// The log message.
		/// </summary>
		public string Message { get; private set; }
	}
}
