/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Kasra Nassiri <cjax@users.sourceforge.net>
 * Modified By: Joel Low <lowjoel@users.sourceforge.net>
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
using System.IO;
using System.Text;
using System.IO.Pipes;
using System.Threading;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;

namespace Eraser.Manager
{
	/// <summary>
	/// Represents a request to the RemoteExecutorServer instance
	/// </summary>
	[Serializable]
	internal class RemoteRequest
	{
		/// <summary>
		/// List of supported functions
		/// </summary>
		public enum Function : uint
		{
			ADD_TASK = 0,
			GET_TASK,
			GET_TASKS,
			CANCEL_TASK,
			DELETE_TASK,
			QUEUE_TASK,
			REPLACE_TASK,
			SCHEDULE_TASK,
			SAVE_TASK_LIST,
			LOAD_TASK_LIST,
			QUEUE_RESTART_TASK,
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="func">The function this command is wanting to execute.</param>
		/// <param name="data">The parameters for the command, serialised using a
		/// BinaryFormatter</param>
		public RemoteRequest(Function func, byte[] data)
		{
			Func = func;
			Data = data;
		}

		/// <summary>
		/// The function that this request is meant to call.
		/// </summary>
		public Function Func;

		/// <summary>
		/// The parameters associated with the function call.
		/// </summary>
		public byte[] Data;
	};

	/// <summary>
	/// The RemoteExecutorServer class is the server half required for remote execution
	/// of tasks.
	/// </summary>
	public class RemoteExecutorServer : DirectExecutor
	{
		/// <summary>
		/// Our Remote Server name, prevent collisions!
		/// </summary>
		public const string ServerName = "Eraser-FB6C5A7D-E47F-475f-ABA4-58F4D24BB67E-RemoteExecutor";

		/// <summary>
		/// The thread which will answer pipe connections
		/// </summary>
		private Thread thread = null;

		/// <summary>
		/// Our pipe instance which handles connections.
		/// </summary>
		private NamedPipeServerStream server =
			new NamedPipeServerStream(ServerName, PipeDirection.InOut, 4,
				PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

		/// <summary>
		/// Constructor.
		/// </summary>
		public RemoteExecutorServer()
		{
			thread = new Thread(Main);
			thread.Start();

			Thread.Sleep(0);
		}

		public override void Dispose()
		{
			thread.Abort();
			base.Dispose();
		}

		/// <summary>
		/// The polling loop dealing with every server connection.
		/// </summary>
		private void Main()
		{
			while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
			{
				//Wait for a connection to be established
				if (!server.IsConnected)
				{
					IAsyncResult asyncWait = server.BeginWaitForConnection(
						server.EndWaitForConnection, null);
					while (!server.IsConnected && !asyncWait.AsyncWaitHandle.WaitOne(15))
						if (Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
							break;
				}

				//If we still aren't connected that means the connection failed to establish.
				if (!server.IsConnected)
					continue;

				//Read the request into the buffer.
				RemoteRequest request = null;
				using (MemoryStream mstream = new MemoryStream())
				{
					byte[] buffer = new byte[65536];
					server.Read(buffer, 0, sizeof(int));
					int messageSize = BitConverter.ToInt32(buffer, 0);
					while (messageSize > 0)
					{
						int lastRead = server.Read(buffer, 0, Math.Min(messageSize, buffer.Length));
						messageSize -= lastRead;
						mstream.Write(buffer, 0, lastRead);
					}

					//Deserialise the header of the request.
					mstream.Position = 0;
					request = (RemoteRequest)new BinaryFormatter().Deserialize(new MemoryStream(buffer));
				}

				#region Deserialise
				object parameter = null;
				switch (request.Func)
				{
					// void \+ task
					case RemoteRequest.Function.CANCEL_TASK:
					case RemoteRequest.Function.QUEUE_TASK:
					case RemoteRequest.Function.REPLACE_TASK:
					case RemoteRequest.Function.SCHEDULE_TASK:
					case RemoteRequest.Function.ADD_TASK:
						using (MemoryStream mStream = new MemoryStream(request.Data))
							parameter = new BinaryFormatter().Deserialize(mStream);
						break;

					// bool \+ taskid
					case RemoteRequest.Function.DELETE_TASK:
					// task \+ taskid
					case RemoteRequest.Function.GET_TASK:
						using (MemoryStream mStream = new MemoryStream(request.Data))
							parameter = new BinaryFormatter().Deserialize(mStream);
						break;

					// void \+ stream
					case RemoteRequest.Function.LOAD_TASK_LIST:
					case RemoteRequest.Function.SAVE_TASK_LIST:
						using (MemoryStream mStream = new MemoryStream(request.Data))
							parameter = new BinaryFormatter().Deserialize(mStream);
						break;

					// list<task> \+ void
					case RemoteRequest.Function.GET_TASKS:
					// void \+ void
					case RemoteRequest.Function.QUEUE_RESTART_TASK:
						break;

					default:
						throw new FatalException("Unknown RemoteExecutorClient.Function");
				}
				#endregion

				#region Invoke
				object returnValue = null;
				switch (request.Func)
				{
					// void \+ task
					case RemoteRequest.Function.CANCEL_TASK:
						CancelTask((Task)parameter);
						break;

					// void \+ task
					case RemoteRequest.Function.QUEUE_TASK:
						QueueTask((Task)parameter);
						break;

					// void \+ task
					case RemoteRequest.Function.REPLACE_TASK:
						ReplaceTask((Task)parameter);
						break;

					// void \+ task
					case RemoteRequest.Function.SCHEDULE_TASK:
						ScheduleTask((Task)parameter);
						break;

					// void \+ ref task
					case RemoteRequest.Function.ADD_TASK:
					{
						Task task = (Task)parameter;
						AddTask(ref task);
						break;
					}

					// bool \+ taskid
					case RemoteRequest.Function.DELETE_TASK:
						returnValue = DeleteTask((uint)parameter);
						break;

					// task \+ taskid
					case RemoteRequest.Function.GET_TASK:
						returnValue = GetTask((uint)parameter);
						break;

					// void \+ stream
					case RemoteRequest.Function.LOAD_TASK_LIST:
						LoadTaskList((Stream)parameter);
						break;

					// void \+ stream
					case RemoteRequest.Function.SAVE_TASK_LIST:
						SaveTaskList((Stream)parameter);
						break;

					// list<task> \+ void
					case RemoteRequest.Function.GET_TASKS:
						returnValue = GetTasks();
						break;

					// void \+ void
					case RemoteRequest.Function.QUEUE_RESTART_TASK:
						QueueRestartTasks();
						break;

					default:
						throw new FatalException("Unknown RemoteExecutorClient.Function");
				#endregion
				}

				//Return the result of the invoked function, if any.
				if (returnValue != null)
					using (MemoryStream mStream = new MemoryStream())
					{
						new BinaryFormatter().Serialize(mStream, returnValue);
						byte[] buffer = mStream.ToArray();
						byte[] bufferLength = BitConverter.GetBytes(buffer.Length);
						server.Write(bufferLength, 0, sizeof(int));
						server.Write(buffer, 0, buffer.Length);
					}
				else
				{
					byte[] buffer = BitConverter.GetBytes(0);
					server.Write(buffer, 0, sizeof(int));
				}

				// we are done, disconnect
				server.Disconnect();
			}
		}
	}

	public class RemoteExecutorClient : Executor
	{
		private NamedPipeClientStream client =
			new NamedPipeClientStream(".", RemoteExecutorServer.ServerName,
				PipeDirection.InOut);

		public RemoteExecutorClient()
		{
		}

		public override void Dispose()
		{
			client.Close();
			client.Dispose();
		}

		/// <summary>
		/// Connects to the remote server.
		/// </summary>
		/// <returns>True if the connection to the remote server was established.</returns>
		public bool Connect()
		{
			try
			{
				client.Connect(250);
			}
			catch (TimeoutException)
			{
			}

			return client.IsConnected;
		}

		private object SendRequest(RemoteRequest header)
		{
			//Connect to the server
			object result = null;
			client.Connect(5000);

			using (MemoryStream mStream = new MemoryStream())
			{
				//Serialise the request
				new BinaryFormatter().Serialize(mStream, header);

				//Write the request to the pipe
				byte[] buffer = mStream.ToArray();
				byte[] bufferLength = BitConverter.GetBytes(buffer.Length);
				client.Write(bufferLength, 0, sizeof(int));
				client.Write(buffer, 0, buffer.Length);

				//Read the response from the pipe
				mStream.Position = 0;
				buffer = new byte[32768];
				client.Read(buffer, 0, sizeof(int));
				int responseLength = BitConverter.ToInt32(buffer, 0);
				while (responseLength > 0)
					responseLength -= client.Read(buffer, 0, Math.Min(buffer.Length, responseLength));

				//Deserialise the response
				mStream.Position = 0;
				if (mStream.Length > 0)
					result = new BinaryFormatter().Deserialize(mStream);
			}

			return result;
		}

		public override bool DeleteTask(uint taskId)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, taskId);
			return (bool)SendRequest(new RemoteRequest(RemoteRequest.Function.DELETE_TASK,
				mStream.GetBuffer()));
		}

		public override List<Task> GetTasks()
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, null);
			return (List<Task>)SendRequest(new RemoteRequest(RemoteRequest.Function.GET_TASKS,
				mStream.GetBuffer()));
		}

		public override Task GetTask(uint taskId)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, taskId);
			return (Task)SendRequest(new RemoteRequest(RemoteRequest.Function.GET_TASK,
				mStream.GetBuffer()));
		}

		public override void LoadTaskList(Stream stream)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, stream);
			SendRequest(new RemoteRequest(RemoteRequest.Function.LOAD_TASK_LIST,
				mStream.GetBuffer()));
		}

		public override void AddTask(ref Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteRequest(RemoteRequest.Function.ADD_TASK,
				mStream.GetBuffer()));
		}

		public override void CancelTask(Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteRequest(RemoteRequest.Function.CANCEL_TASK,
				mStream.GetBuffer()));
		}

		public override void QueueRestartTasks()
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, null);
			SendRequest(new RemoteRequest(RemoteRequest.Function.QUEUE_RESTART_TASK,
				mStream.GetBuffer()));
		}

		public override void QueueTask(Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteRequest(RemoteRequest.Function.QUEUE_TASK,
				mStream.GetBuffer()));
		}

		public override void ReplaceTask(Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteRequest(RemoteRequest.Function.REPLACE_TASK,
				mStream.GetBuffer()));
		}

		public override void Run()
		{
		}

		public override void ScheduleTask(Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteRequest(RemoteRequest.Function.SCHEDULE_TASK,
				mStream.GetBuffer()));
		}

		public override void SaveTaskList(Stream stream)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, stream);
			SendRequest(new RemoteRequest(RemoteRequest.Function.SAVE_TASK_LIST,
				mStream.GetBuffer()));
		}
	}
}
