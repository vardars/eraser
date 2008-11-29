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
	[Serializable]
	internal class RemoteHeader
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
		public RemoteHeader(Function func, byte[] data)
		{
			Func = func;
			Data = data;
		}

		public Function Func;
		public byte[] Data;
	};

	// we allways pass complete tasks accross our server/clinet
	// streams
	public class RemoteExecutorServer : DirectExecutor
	{
		public const string ServerName = "localhost";

		private Thread thread = null;
		private NamedPipeServerStream server =
			new NamedPipeServerStream(ServerName, PipeDirection.InOut, 32,
				PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

		public RemoteExecutorServer()
		{
			thread = new Thread(Main);
			thread.Start();

			Thread.Sleep(0);
		}

		public override void Dispose()
		{
			Abort();
			base.Dispose();
		}

		public void Abort()
		{
			thread.Abort();
		}

		private void Main()
		{
			while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
			{
				IAsyncResult asyncWait = server.BeginWaitForConnection(
					server.EndWaitForConnection, null);

				while (!asyncWait.AsyncWaitHandle.WaitOne(15))
					if (Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
						break;

				//If we still aren't connected that means the connection failed to establish.
				if (!server.IsConnected)
					continue;

				//Read the request into the buffer.
				RemoteHeader request = null;
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
					request = (RemoteHeader)new BinaryFormatter().Deserialize(new MemoryStream(buffer));
				}

				uint taskId = 0;
				Task task = null;
				Stream stream = null;
				object returnValue = null;

				#region Deserialise
				switch (request.Func)
				{
					// void \+ task
					case RemoteHeader.Function.CANCEL_TASK:
					// void \+ task
					case RemoteHeader.Function.QUEUE_TASK:
					// void \+ task
					case RemoteHeader.Function.REPLACE_TASK:
					// void \+ task
					case RemoteHeader.Function.SCHEDULE_TASK:
					// void \+ ref task
					case RemoteHeader.Function.ADD_TASK:
						using (MemoryStream mStream = new MemoryStream(request.Data))
							task = (Task)new BinaryFormatter().Deserialize(mStream);
						returnValue = new object();
						break;

					// bool \+ taskid
					case RemoteHeader.Function.DELETE_TASK:
					// task \+ taskid
					case RemoteHeader.Function.GET_TASK:
						using (MemoryStream mStream = new MemoryStream(request.Data))
							taskId = (uint)new BinaryFormatter().Deserialize(mStream);
						break;

					// void \+ stream
					case RemoteHeader.Function.LOAD_TASK_LIST:
					// void \+ stream
					case RemoteHeader.Function.SAVE_TASK_LIST:
						using (MemoryStream mStream = new MemoryStream(request.Data))
							stream = (Stream)new BinaryFormatter().Deserialize(mStream);
						returnValue = new object();
						break;

					// list<task> \+ void
					case RemoteHeader.Function.GET_TASKS:
					// void \+ void
					case RemoteHeader.Function.QUEUE_RESTART_TASK:
						returnValue = new object();
						break;

					default:
						throw new FatalException("Unknown RemoteExecutorClient.Function");
				}
				#endregion

				#region Invoke
				switch (request.Func)
				{
					// void \+ task
					case RemoteHeader.Function.CANCEL_TASK:
						CancelTask(task);
						break;

					// void \+ task
					case RemoteHeader.Function.QUEUE_TASK:
						QueueTask(task);
						break;

					// void \+ task
					case RemoteHeader.Function.REPLACE_TASK:
						ReplaceTask(task);
						break;

					// void \+ task
					case RemoteHeader.Function.SCHEDULE_TASK:
						ScheduleTask(task);
						break;

					// void \+ ref task
					case RemoteHeader.Function.ADD_TASK:
						AddTask(ref task);
						break;

					// bool \+ taskid
					case RemoteHeader.Function.DELETE_TASK:
						returnValue = DeleteTask(taskId);
						break;

					// task \+ taskid
					case RemoteHeader.Function.GET_TASK:
						returnValue = GetTask(taskId);
						break;

					// void \+ stream
					case RemoteHeader.Function.LOAD_TASK_LIST:
						LoadTaskList(stream);
						break;

					// void \+ stream
					case RemoteHeader.Function.SAVE_TASK_LIST:
						SaveTaskList(stream);
						break;

					// list<task> \+ void
					case RemoteHeader.Function.GET_TASKS:
						returnValue = GetTasks();
						break;

					// void \+ void
					case RemoteHeader.Function.QUEUE_RESTART_TASK:
						QueueRestartTasks();
						break;

					default:
						throw new FatalException("Unknown RemoteExecutorClient.Function");
				#endregion
				}

				// return the returnValue and disconnect
				using (MemoryStream mStream = new MemoryStream())
				{
					new BinaryFormatter().Serialize(mStream, returnValue);
					byte[] buffer = mStream.ToArray();
					byte[] bufferLength = BitConverter.GetBytes(buffer.Length);
					server.Write(bufferLength, 0, sizeof(int));
					server.Write(buffer, 0, buffer.Length);
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

		private object SendRequest(RemoteHeader header)
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
				result = new BinaryFormatter().Deserialize(mStream);
			}

			return result;
		}

		public override bool DeleteTask(uint taskId)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, taskId);
			return (bool)SendRequest(new RemoteHeader(RemoteHeader.Function.DELETE_TASK,
				mStream.GetBuffer()));
		}

		public override List<Task> GetTasks()
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, null);
			return (List<Task>)SendRequest(new RemoteHeader(RemoteHeader.Function.GET_TASKS,
				mStream.GetBuffer()));
		}

		public override Task GetTask(uint taskId)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, taskId);
			return (Task)SendRequest(new RemoteHeader(RemoteHeader.Function.GET_TASK,
				mStream.GetBuffer()));
		}

		public override void LoadTaskList(Stream stream)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, stream);
			SendRequest(new RemoteHeader(RemoteHeader.Function.LOAD_TASK_LIST,
				mStream.GetBuffer()));
		}

		public override void AddTask(ref Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteHeader(RemoteHeader.Function.ADD_TASK,
				mStream.GetBuffer()));
		}

		public override void CancelTask(Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteHeader(RemoteHeader.Function.CANCEL_TASK,
				mStream.GetBuffer()));
		}

		public override void QueueRestartTasks()
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, null);
			SendRequest(new RemoteHeader(RemoteHeader.Function.QUEUE_RESTART_TASK,
				mStream.GetBuffer()));
		}

		public override void QueueTask(Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteHeader(RemoteHeader.Function.QUEUE_TASK,
				mStream.GetBuffer()));
		}

		public override void ReplaceTask(Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteHeader(RemoteHeader.Function.REPLACE_TASK,
				mStream.GetBuffer()));
		}

		public override void Run()
		{
		}

		public override void ScheduleTask(Task task)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, task);
			SendRequest(new RemoteHeader(RemoteHeader.Function.SCHEDULE_TASK,
				mStream.GetBuffer()));
		}

		public override void SaveTaskList(Stream stream)
		{
			MemoryStream mStream = new MemoryStream();
			new BinaryFormatter().Serialize(mStream, stream);
			SendRequest(new RemoteHeader(RemoteHeader.Function.SAVE_TASK_LIST,
				mStream.GetBuffer()));
		}
	}
}
