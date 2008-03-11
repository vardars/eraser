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
	[Serializable]
	public struct LogEntry : ISerializable
	{
		#region Serialization code
		public LogEntry(SerializationInfo info, StreamingContext context)
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
