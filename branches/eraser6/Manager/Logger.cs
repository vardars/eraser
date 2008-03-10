using System;
using System.Collections.Generic;
using System.Text;

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
		INFORMATION,

		/// <summary>
		/// Notice messages.
		/// </summary>
		NOTICE,

		/// <summary>
		/// Warning messages.
		/// </summary>
		WARNING,

		/// <summary>
		/// Error messages.
		/// </summary>
		ERROR,

		/// <summary>
		/// Fatal errors.
		/// </summary>
		FATAL
	}

	/// <summary>
	/// Represents a log entry.
	/// </summary>
	public struct LogEntry
	{
		/// <summary>
		/// Creates a LogEntry structure.
		/// </summary>
		/// <param name="message">The log message.</param>
		/// <param name="level">The type of log entry.</param>
		public LogEntry(string message, LogLevel level)
		{
			Message = message;
			Level = level;
			Timestamp = DateTime.Now;
		}

		/// <summary>
		/// The type of log entry.
		/// </summary>
		public readonly LogLevel Level;

		/// <summary>
		/// The time which the message was logged.
		/// </summary>
		public readonly DateTime Timestamp;

		/// <summary>
		/// The log message.
		/// </summary>
		public readonly string Message;
	}
}
