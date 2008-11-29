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
	public class RemoteHeader
	{
		public enum Function : uint
		{
			RUN = 0,
			ADD_TASK,
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

		public Function Func;
		public Stream SerializationStream = new MemoryStream();
	};

	// we allways pass complete tasks accross our server/clinet
	// streams
	public class RemoteExecutorServer : DirectExecutor
	{
		public const string ServerName = "localhost";

		private Thread thread = null;
		private NamedPipeServerStream server =
			new NamedPipeServerStream(ServerName, PipeDirection.InOut, 32,
				PipeTransmissionMode.Message, PipeOptions.None);

		public RemoteExecutorServer()
		{
			thread = new Thread(Main);
			thread.Start();

			Thread.Sleep(0);
		}

		public override void Dispose()
		{
 			base.Dispose();
			Abort();
		}

		public void Abort()
		{
			thread.Abort();
		}

		private void Main()
		{
			byte[] buffer = new byte[32768];
			MemoryStream mstream = new MemoryStream();

			while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
			{
				if (!server.IsConnected)
					server.WaitForConnection();

				//Read the header into the buffer.
				int lastRead = 0;
				while ((lastRead = server.Read(buffer, 0, buffer.Length)) > 0)
					mstream.Write(buffer, 0, lastRead);

				//Deserialise the header of the request.
				object returnValue = null;
				RemoteHeader data = (RemoteHeader)new BinaryFormatter().Deserialize(mstream);
				data.SerializationStream.Position = 0;

				uint taskId = 0;
				Task task = null;
				Stream stream = null;

				#region Deserialise
				switch (data.Func)
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
						task = (Task)new BinaryFormatter().Deserialize(data.SerializationStream);
						returnValue = new object();
						break;

					// bool \+ taskid
					case RemoteHeader.Function.DELETE_TASK:
					// task \+ taskid
					case RemoteHeader.Function.GET_TASK:
						taskId = (uint)new BinaryFormatter().Deserialize(data.SerializationStream);
						break;

					// void \+ stream
					case RemoteHeader.Function.LOAD_TASK_LIST:
					// void \+ stream
					case RemoteHeader.Function.SAVE_TASK_LIST:
						stream = (Stream)new BinaryFormatter().Deserialize(data.SerializationStream);
						returnValue = new object();
						break;

					// list<task> \+ void
					case RemoteHeader.Function.GET_TASKS:
					// void \+ void
					case RemoteHeader.Function.QUEUE_RESTART_TASK:
					// void \+ void
					case RemoteHeader.Function.RUN:
						returnValue = new object();
						break;

					default:
						throw new FatalException("Unknown RemoteExecutorClient.Function");
				}
				#endregion

				#region Invoke
				switch (data.Func)
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

					// void \+ void
					case RemoteHeader.Function.RUN:
						Run();
						break;

					default:
						throw new FatalException("Unknown RemoteExecutorClient.Function");
					#endregion
				}

				// return the returnValue and disconnect
				using (MemoryStream ms = new MemoryStream())
				{
					new BinaryFormatter().Serialize(ms, returnValue);
					server.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
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

		private object Communicate(RemoteHeader header)
		{
			// initialise client and connect to the server
			object results = null;

			// wait for a connection for at least 5s
			client.Connect(5000);

			// serialise the data
			using (MemoryStream ms = new MemoryStream())
			{
				byte[] buffer = new byte[32768];
				new BinaryFormatter().Serialize(ms, header);

				//Write the request to the pipe
				client.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);

				//Read the response from the server
				int lastRead = 0;
				ms.Position = 0;
				while ((lastRead = client.Read(buffer, 0, buffer.Length)) != 0)
					ms.Write(buffer, 0, lastRead);
				
				//Deserialise the response
				results = new BinaryFormatter().Deserialize(ms);
			}

			return results;
		}

		public override bool DeleteTask(uint taskId)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.DELETE_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, taskId);
			return (bool)Communicate(rh);
		}

		public override List<Task> GetTasks()
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.GET_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			return (List<Task>)Communicate(rh);
		}

		public override Task GetTask(uint taskId)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.GET_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			return (Task)Communicate(rh);
		}

		public override void LoadTaskList(Stream stream)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.LOAD_TASK_LIST;
			new BinaryFormatter().Serialize(rh.SerializationStream, stream);
			Communicate(rh);
		}

		public override void AddTask(ref Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.ADD_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, task);
			Communicate(rh);
		}

		public override void CancelTask(Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.CANCEL_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, task);
			Communicate(rh);
		}

		public override void QueueRestartTasks()
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.QUEUE_RESTART_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void QueueTask(Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.QUEUE_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void ReplaceTask(Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.REPLACE_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void Run()
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.RUN;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void ScheduleTask(Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.SCHEDULE_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void SaveTaskList(Stream stream)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Func = RemoteHeader.Function.SAVE_TASK_LIST;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}
	}
}