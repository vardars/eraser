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
	// we allways pass complete tasks accross our server/clinet
	// streams
	public class RemoteExecutorServer : DirectExecutor
	{
		public const string ServerName = "EraserRemoteExecutorServer";

		private Thread thread = null;
		private NamedPipeServerStream server = 
			new NamedPipeServerStream(ServerName, PipeDirection.InOut, 32,
				PipeTransmissionMode.Message, PipeOptions.Asynchronous);

		public RemoteExecutorServer()
			: base()
		{
			thread = new Thread(Main);
			thread.Start();

			Thread.Sleep(0);
		}

		~RemoteExecutorServer()
		{
			thread.Interrupt();
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
				if(!server.IsConnected)
					server.WaitForConnection(); 

				while (server.Position < server.Length)
					mstream.Write(buffer, 0, server.Read(buffer, 0, buffer.Length));

				object returnValue = null;
				using (RemoteExecutorClient.RemoteHeader data = (RemoteExecutorClient.RemoteHeader)
					new BinaryFormatter().Deserialize(mstream))
				{

					data.SerializationStream.Position = 0;

					uint taskId = 0;
					Task task = null;
					Stream stream = null;

					#region Deserialise
					switch (data.Function)
					{
						// void \+ task
						case RemoteExecutorClient.Function.CANCEL_TASK:
						// void \+ task
						case RemoteExecutorClient.Function.QUEUE_TASK:
						// void \+ task
						case RemoteExecutorClient.Function.REPLACE_TASK:
						// void \+ task
						case RemoteExecutorClient.Function.SCHEDULE_TASK:
						// void \+ ref task
						case RemoteExecutorClient.Function.ADD_TASK:
							task = (Task)new BinaryFormatter().Deserialize(data.SerializationStream);
							returnValue = null;
							break;

						// bool \+ taskid
						case RemoteExecutorClient.Function.DELETE_TASK:
						// task \+ taskid
						case RemoteExecutorClient.Function.GET_TASK:
							taskId = (uint)new BinaryFormatter().Deserialize(data.SerializationStream);
							break;

						// void \+ stream
						case RemoteExecutorClient.Function.LOAD_TASK_LIST:
						// void \+ stream
						case RemoteExecutorClient.Function.SAVE_TASK_LIST:
							stream = (Stream)new BinaryFormatter().Deserialize(data.SerializationStream);
							returnValue = null;
							break;

						// list<task> \+ void
						case RemoteExecutorClient.Function.GET_TASKS:
						// void \+ void
						case RemoteExecutorClient.Function.QUEUE_RESTART_TASK:
						// void \+ void
						case RemoteExecutorClient.Function.RUN:
							returnValue = null;
							break;

						default:
							throw new FatalException("Unknown RemoteExecutorClient.Function");
					}
					#endregion

					#region Invoke
					switch (data.Function)
					{
						// void \+ task
						case RemoteExecutorClient.Function.CANCEL_TASK:
							CancelTask(task);
							break;

						// void \+ task
						case RemoteExecutorClient.Function.QUEUE_TASK:
							QueueTask(task);
							break;

						// void \+ task
						case RemoteExecutorClient.Function.REPLACE_TASK:
							ReplaceTask(task);
							break;

						// void \+ task
						case RemoteExecutorClient.Function.SCHEDULE_TASK:
							ScheduleTask(task);
							break;

						// void \+ ref task
						case RemoteExecutorClient.Function.ADD_TASK:
							AddTask(ref task);
							break;

						// bool \+ taskid
						case RemoteExecutorClient.Function.DELETE_TASK:
							returnValue = DeleteTask(taskId);
							break;

						// task \+ taskid
						case RemoteExecutorClient.Function.GET_TASK:
							returnValue = GetTask(taskId);
							break;

						// void \+ stream
						case RemoteExecutorClient.Function.LOAD_TASK_LIST:
							LoadTaskList(stream);
							break;

						// void \+ stream
						case RemoteExecutorClient.Function.SAVE_TASK_LIST:
							SaveTaskList(stream);
							break;

						// list<task> \+ void
						case RemoteExecutorClient.Function.GET_TASKS:
							returnValue = GetTasks();
							break;

						// void \+ void
						case RemoteExecutorClient.Function.QUEUE_RESTART_TASK:
							QueueRestartTasks();
							break;

						// void \+ void
						case RemoteExecutorClient.Function.RUN:
							Run();
							break;

						default:
							throw new FatalException("Unknown RemoteExecutorClient.Function");
					}
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
		public static int Instances = 0;
		public const string ClientName = "EraserRemoteExecutorClient";

		private NamedPipeClientStream client = 
			new NamedPipeClientStream(RemoteExecutorServer.ServerName,
				ClientName, PipeDirection.InOut);

		public enum Function : uint
		{
			ADD_TASK = 0,
			CANCEL_TASK,
			DELETE_TASK,
			GET_TASK,
			GET_TASKS,
			RUN,
			QUEUE_RESTART_TASK,
			QUEUE_TASK,
			REPLACE_TASK,
			LOAD_TASK_LIST,
			SCHEDULE_TASK,
			SAVE_TASK_LIST,
		}

		public RemoteExecutorClient()
		{
			Instances += 1;
		}

		public override void Dispose()
		{
			client.Close();
			client.Dispose();
		}

		public class RemoteHeader : IDisposable
		{
			public void Dispose()
			{
			}

			public Function Function;
			public Stream SerializationStream = new MemoryStream();
		};

		private object Communicate(RemoteHeader header)
		{
			// initialise client and connect to the server
			object results = null;
			IAsyncResult asyncResult;
			
			client.Connect(10000);

			client.ReadMode = PipeTransmissionMode.Message;
			client.Position = 0;			

			// serialise the data
			using (MemoryStream ms = new MemoryStream())
			{
				byte[] buffer = new byte[32768];

				new BinaryFormatter().Serialize(ms, header);
				long clinetPos = client.Position;

				// write async
				(asyncResult = client.BeginWrite(ms.GetBuffer(), 0, ms.GetBuffer().Length,
					delegate(IAsyncResult ar)
					{
						// completed
						client.EndWrite(ar);

						ms.Position = 0;
						ms.Capacity = (int)(client.Length - (client.Position = clinetPos));

						while (client.Position < client.Length)
							ms.Write(buffer, 0, client.Read(buffer, 0, buffer.Length));

						results = new BinaryFormatter().Deserialize(ms);

					}, this)).AsyncWaitHandle.WaitOne();
			}

			// TODO: return the proper results
			return results;
		}

		public override bool DeleteTask(uint taskId)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.DELETE_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, taskId);
			return (bool)Communicate(rh);
		}

		public override List<Task> GetTasks()
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.GET_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			return (List<Task>)Communicate(rh);
		}

		public override Task GetTask(uint taskId)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.GET_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			return (Task)Communicate(rh);
		}

		public override void LoadTaskList(Stream stream)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.LOAD_TASK_LIST;
			new BinaryFormatter().Serialize(rh.SerializationStream, stream);
			Communicate(rh);
		}

		public override void AddTask(ref Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.ADD_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, task);
			Communicate(rh);
		}

		public override void CancelTask(Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.CANCEL_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, task);
			Communicate(rh);
		}

		public override void QueueRestartTasks()
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.QUEUE_RESTART_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void QueueTask(Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.QUEUE_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void ReplaceTask(Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.REPLACE_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void Run()
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.RUN;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void ScheduleTask(Task task)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.SCHEDULE_TASK;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}

		public override void SaveTaskList(Stream stream)
		{
			RemoteHeader rh = new RemoteHeader();
			rh.Function = Function.SAVE_TASK_LIST;
			new BinaryFormatter().Serialize(rh.SerializationStream, null);
			Communicate(rh);
		}
	}
}
