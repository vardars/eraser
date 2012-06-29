/* 
 * $Id$
 * Copyright 2008-2012 The Eraser Project
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
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Collections.Generic;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Security.AccessControl;

namespace Eraser.Manager
{
	/// <summary>
	/// Represents a request to the RemoteExecutorServer instance
	/// </summary>
	[Serializable]
	internal class RemoteExecutorRequest
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="func">The function this command is wanting to execute.</param>
		/// <param name="data">The parameters for the command, serialised using a
		/// BinaryFormatter</param>
		public RemoteExecutorRequest(RemoteExecutorFunction func, params object[] data)
		{
			Func = func;
			Data = data;
		}

		/// <summary>
		/// The function that this request is meant to call.
		/// </summary>
		public RemoteExecutorFunction Func { get; set; }

		/// <summary>
		/// The parameters associated with the function call.
		/// </summary>
		public object[] Data { get; private set; }
	};

	/// <summary>
	/// List of supported functions
	/// </summary>
	public enum RemoteExecutorFunction
	{
		QueueTask,
		ScheduleTask,
		UnqueueTask,

		AddTask,
		DeleteTask,
		//UpdateTask,
		GetTaskCount,
		GetTask
	}

	/// <summary>
	/// The RemoteExecutorServer class is the server half required for remote execution
	/// of tasks.
	/// </summary>
	public class RemoteExecutorServer : DirectExecutor
	{
		/// <summary>
		/// Our Remote Server name, prevent collisions!
		/// </summary>
		public static readonly string ServerName =
			"Eraser-FB6C5A7D-E47F-475f-ABA4-58F4D24BB67E-RemoteExecutor-" +
			WindowsIdentity.GetCurrent().User.ToString();

		/// <summary>
		/// Constructor.
		/// </summary>
		public RemoteExecutorServer()
		{
			ServerChannel = new IpcChannel("localhost:9090");
		}

		protected override void Dispose(bool disposing)
		{
			ServerChannel.StopListening(null);

			base.Dispose(disposing);
		}

		public override void Run()
		{
			base.Run();

			//Register the server channel.
			ChannelServices.RegisterChannel(ServerChannel, true);

			// Show the name of the channel.
			Console.WriteLine("The name of the channel is {0}.",
				ServerChannel.ChannelName);

			// Show the priority of the channel.
			Console.WriteLine("The priority of the channel is {0}.",
				ServerChannel.ChannelPriority);

			// Show the URIs associated with the channel.
			ChannelDataStore channelData = (ChannelDataStore)
				ServerChannel.ChannelData;
			foreach (string uri in channelData.ChannelUris)
			{
				Console.WriteLine("The channel URI is {0}.", uri);
			}

			// Expose an object for remote calls.
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(RemoteExecutorServer), ServerName,
				System.Runtime.Remoting.WellKnownObjectMode.Singleton);

			// Parse the channel's URI.
			string[] urls = ServerChannel.GetUrlsForUri("RemoteObject.rem");
			if (urls.Length > 0)
			{
				string objectUrl = urls[0];
				string objectUri;
				string channelUri = ServerChannel.Parse(objectUrl, out objectUri);
				Console.WriteLine("The object URI is {0}.", objectUri);
				Console.WriteLine("The channel URI is {0}.", channelUri);
				Console.WriteLine("The object URL is {0}.", objectUrl);
			}
		}

		/// <summary>
		/// The IPC Channel used for communications.
		/// </summary>
		private IpcChannel ServerChannel;
	}

	/// <summary>
	/// The RemoteExecutorServer class is the client half required for remote execution
	/// of tasks, sending requests to the server running on the local computer.
	/// </summary>
	public class RemoteExecutorClient : Executor
	{
		public RemoteExecutorClient()
		{
			// Create the channel.
			IpcChannel channel = new IpcChannel();

			// Register the channel.
			ChannelServices.RegisterChannel(channel, true);

			// Register as client for remote object.
			WellKnownClientTypeEntry remoteType = new WellKnownClientTypeEntry(
				typeof(RemoteExecutorServer),
				"ipc://localhost:9090/" + RemoteExecutorServer.ServerName);
			RemotingConfiguration.RegisterWellKnownClientType(remoteType);

			// Create a message sink.
			string objectUri;
			System.Runtime.Remoting.Messaging.IMessageSink messageSink =
				channel.CreateMessageSink(
					"ipc://localhost:9090/" + RemoteExecutorServer.ServerName, null,
					out objectUri);
			Console.WriteLine("The URI of the message sink is {0}.",
				objectUri);
			if (messageSink != null)
			{
				Console.WriteLine("The type of the message sink is {0}.",
					messageSink.GetType().ToString());
			}

			// Create an instance of the remote object.
			RemoteExecutorServer server = (RemoteExecutorServer)
				Activator.GetObject(typeof(RemoteExecutorServer),
				"ipc://localhost:9090/" + RemoteExecutorServer.ServerName);

		}

		protected override void Dispose(bool disposing)
		{
			if (client == null)
				return;

			if (disposing)
			{
				client.Close();
			}

			client = null;
			base.Dispose(disposing);
		}

		public override void Run()
		{
			try
			{
				client.Connect(0);
			}
			catch (TimeoutException)
			{
			}
		}

		/// <summary>
		/// Sends a request to the executor server.
		/// </summary>
		/// <typeparam name="ReturnType">The expected return type of the request.</typeparam>
		/// <param name="function">The requested operation.</param>
		/// <param name="args">The arguments for the operation.</param>
		/// <returns>The return result from the object as if it were executed locally.</returns>
		internal ReturnType SendRequest<ReturnType>(RemoteExecutorFunction function, params object[] args)
		{
			//Connect to the server
			object result = null;

			using (MemoryStream mStream = new MemoryStream())
			{
				//Serialise the request
				new BinaryFormatter().Serialize(mStream, new RemoteExecutorRequest(function, args));

				//Write the request to the pipe
				byte[] buffer = mStream.ToArray();
				client.Write(buffer, 0, buffer.Length);

				//Read the response from the pipe
				mStream.Position = 0;
				buffer = new byte[65536];
				client.ReadMode = PipeTransmissionMode.Message;
				do
				{
					int lastRead = client.Read(buffer, 0, buffer.Length);
					mStream.Write(buffer, 0, lastRead);
				}
				while (!client.IsMessageComplete);

				//Check if the server says there is a response. If so, read it.
				if (BitConverter.ToInt32(mStream.ToArray(), 0) == 1)
				{
					mStream.Position = 0;
					do
					{
						int lastRead = client.Read(buffer, 0, buffer.Length);
						mStream.Write(buffer, 0, lastRead);
					}
					while (!client.IsMessageComplete);

					//Deserialise the response
					mStream.Position = 0;
					if (mStream.Length > 0)
						result = new BinaryFormatter().Deserialize(mStream);
				}
			}

			return (ReturnType)result;
		}

		public override void QueueTask(Task task)
		{
			SendRequest<object>(RemoteExecutorFunction.QueueTask, task);
		}

		public override void ScheduleTask(Task task)
		{
			SendRequest<object>(RemoteExecutorFunction.ScheduleTask, task);
		}

		public override void UnqueueTask(Task task)
		{
			SendRequest<object>(RemoteExecutorFunction.UnqueueTask, task);
		}

		public override void QueueRestartTasks()
		{
			throw new NotImplementedException();
		}

		internal override bool IsTaskQueued(Task task)
		{
			throw new NotImplementedException();
		}

		public override ExecutorTasksCollection Tasks
		{
			get
			{
				return tasks;
			}
		}

		/// <summary>
		/// Checks whether the executor instance has connected to a server.
		/// </summary>
		public bool IsConnected 
		{
			get { return client.IsConnected; }
		}

		/// <summary>
		/// The list of tasks belonging to this executor instance.
		/// </summary>
		private RemoteExecutorClientTasksCollection tasks;

		/// <summary>
		/// The named pipe used to connect to another running instance of Eraser.
		/// </summary>
		private NamedPipeClientStream client;

		private class RemoteExecutorClientTasksCollection : ExecutorTasksCollection
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="executor">The <see cref="RemoteExecutor"/> object owning
			/// this list.</param>
			public RemoteExecutorClientTasksCollection(RemoteExecutorClient executor)
				: base(executor)
			{
			}

			/// <summary>
			/// Sends a request to the executor server.
			/// </summary>
			/// <typeparam name="ReturnType">The expected return type of the request.</typeparam>
			/// <param name="function">The requested operation.</param>
			/// <param name="args">The arguments for the operation.</param>
			/// <returns>The return result from the object as if it were executed locally.</returns>
			private ReturnType SendRequest<ReturnType>(RemoteExecutorFunction function, params object[] args)
			{
				RemoteExecutorClient client = (RemoteExecutorClient)Owner;
				return client.SendRequest<ReturnType>(function, args);
			}

			#region IList<Task> Members
			public override int IndexOf(Task item)
			{
				throw new NotSupportedException();
			}

			public override void Insert(int index, Task item)
			{
				throw new NotSupportedException();
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public override Task this[int index]
			{
				get
				{
					return SendRequest<Task>(RemoteExecutorFunction.GetTask, index);
				}
				set
				{
					throw new NotSupportedException();
				}
			}
			#endregion

			#region ICollection<Task> Members
			public override void Add(Task item)
			{
				item.Executor = Owner;
				SendRequest<object>(RemoteExecutorFunction.AddTask, item);

				//Call all the event handlers who registered to be notified of tasks
				//being added.
				Owner.OnTaskAdded(new TaskEventArgs(item));
			}

			public override void Clear()
			{
				throw new NotSupportedException();
			}

			public override bool Contains(Task item)
			{
				throw new NotSupportedException();
			}

			public override void CopyTo(Task[] array, int arrayIndex)
			{
				throw new NotSupportedException();
			}

			public override int Count
			{
				get { return SendRequest<int>(RemoteExecutorFunction.GetTaskCount); }
			}

			public override bool Remove(Task item)
			{
				item.Cancel();
				item.Executor = null;
				SendRequest<object>(RemoteExecutorFunction.DeleteTask, item);

				//Call all event handlers registered to be notified of task deletions.
				Owner.OnTaskDeleted(new TaskEventArgs(item));
				return true;
			}
			#endregion

			#region IEnumerable<Task> Members
			public override IEnumerator<Task> GetEnumerator()
			{
				throw new NotSupportedException();
			}
			#endregion

			public override void SaveToStream(Stream stream)
			{
				throw new NotSupportedException();
			}

			public override void LoadFromStream(Stream stream)
			{
				throw new NotSupportedException();
			}
		}
	}
}