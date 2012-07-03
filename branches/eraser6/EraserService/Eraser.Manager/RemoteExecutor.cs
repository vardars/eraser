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
using System.Collections.Generic;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters;
using System.Security.Principal;
using System.Threading;
using System.Globalization;

using Eraser.Util;

namespace Eraser.Manager
{
	/// <summary>
	/// The RemoteExecutorServer class is the server half required for remote execution
	/// of tasks.
	/// </summary>
	public class RemoteExecutorServer : DirectExecutor
	{
		/// <summary>
		/// Our Remote Server ID which will be used for obtaining a named mutex.
		/// </summary>
		public const string ServerID =
			"Eraser-FB6C5A7D-E47F-475f-ABA4-58F4D24BB67E-RemoteExecutor-";

		/// <summary>
		/// Our Remote Server name, prevent collisions!
		/// </summary>
		public static readonly string ServerName =
			ServerID + WindowsIdentity.GetCurrent().User.ToString();

		/// <summary>
		/// The name of the Executor object, needed when retrieving the global instance.
		/// </summary>
		public const string ExecutorName = "Executor.rem";

		/// <summary>
		/// Constructor.
		/// </summary>
		public RemoteExecutorServer()
		{
			//Only one server can exist per computer at any one time.
			bool isFirstInstance;
			Mutex globalMutex = new Mutex(true, ServerID, out isFirstInstance);
			if (!isFirstInstance)
			{
				throw new InvalidOperationException(S._("Only one RemoteExecutorServer can be " +
					"instantiated at any one time on one computer."));
			}
			GlobalMutex = globalMutex;

			//Create the server.
			Dictionary<string, string> properties = new Dictionary<string, string>();
			properties["portName"] = ServerName;

			BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
			serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
			BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
			ServerChannel = new IpcChannel(properties, clientProvider, serverProvider);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerChannel.StopListening(null);
				if (GlobalMutex != null)
				{
					GlobalMutex.Close();
					GlobalMutex = null;
				}
			}

			base.Dispose(disposing);
		}

		#pragma warning disable 642
		/// <summary>
		/// Whether an instance of the remote executor server is running.
		/// </summary>
		public static bool IsRunning
		{
			get
			{
				//Do we already have the global mutex?
				if (GlobalMutex != null)
					return true;

				//Try to obtain our mutex on the port.
				bool isFirstInstance = false;
				try
				{
					using (new Mutex(true, ServerID, out isFirstInstance))
						;
				}
				catch (UnauthorizedAccessException)
				{
					isFirstInstance = false;
				}

				return !isFirstInstance;
			}
		}
		#pragma warning restore 642

		public override void Run()
		{
			base.Run();

			//Register the server channel.
			ChannelServices.RegisterChannel(ServerChannel, true);

			//Expose the DirectExecutor object for remote calls.
			RemotingServices.Marshal(this, ExecutorName);

			//Expose a factory for Task objects.
			RemotingConfiguration.RegisterActivatedServiceType(typeof(Task));
		}

		/// <summary>
		/// The IPC Channel used for communications.
		/// </summary>
		private IpcChannel ServerChannel;

		/// <summary>
		/// The global mutex to allow only one server instance to run on a machine at any one time.
		/// </summary>
		private static Mutex GlobalMutex;
	}

	/// <summary>
	/// The RemoteExecutorClient class is the client half required for remote execution
	/// of tasks, sending requests to the server running on the local computer.
	/// </summary>
	/// <remarks>If a RemoteExecutorClient object has been constructed, all Task objects can
	/// only be used with RemoteExecutorServer.
	/// 
	/// TODO: For this restriction to be lifted, every Executor needs to have a factory method
	/// to get the Task type for each executor.
	/// See http://msdn.microsoft.com/en-us/library/ff650208.aspx
	/// </remarks>
	public class RemoteExecutorClient : Executor
	{
		public RemoteExecutorClient()
		{
			//Create the channel.
			Dictionary<string, string> properties = new Dictionary<string, string>();
			properties["portName"] = RemoteExecutorServer.ServerID + Guid.NewGuid().ToString();
			BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
			serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
			BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
			IpcChannel channel = new IpcChannel(properties, clientProvider, serverProvider);

			//Register the channel.
			ChannelServices.RegisterChannel(channel, true);

			//Register the Client-activated Task class.
			RemotingConfiguration.RegisterActivatedClientType(typeof(Task),
				"ipc://" + RemoteExecutorServer.ServerName);
			Run();
		}

		protected override void Dispose(bool disposing)
		{
			Client = null;
			base.Dispose(disposing);
		}

		public override void Run()
		{
			//This function should be idempotent since our constructor will call us.
			if (Client != null)
				return;

			try
			{
				//TODO: is there a better way to verify that we can connect to our Remote instance?
				//Create an instance of the remote object.
				Task task2 = new Task();
				Client = (RemoteExecutorServer)Activator.GetObject(typeof(RemoteExecutorServer),
					string.Format(CultureInfo.InvariantCulture, "ipc://{0}/{1}",
						RemoteExecutorServer.ServerName, RemoteExecutorServer.ExecutorName));
				Client.TaskAdded += OnTaskAdded;
				Client.TaskDeleted += OnTaskDeleted;
				IsConnected = true;
			}
			catch (RemotingException)
			{
			}
		}

		public override void QueueTask(Task task)
		{
			Client.QueueTask(task);
		}

		public override void ScheduleTask(Task task)
		{
			Client.ScheduleTask(task);
		}

		public override void UnqueueTask(Task task)
		{
			Client.UnqueueTask(task);
		}

		public override void QueueRestartTasks()
		{
			Client.QueueRestartTasks();
		}
		
		internal override bool IsTaskQueued(Task task)
		{
			return Client.IsTaskQueued(task);
		}

		[OneWay]
		public void OnTaskAdded(object sender, TaskEventArgs e)
		{
			OnTaskAdded(e);
		}

		[OneWay]
		public void OnTaskDeleted(object sender, TaskEventArgs e)
		{
			OnTaskDeleted(e);
		}

		public override ExecutorTasksCollection Tasks
		{
			get
			{
				return Client.Tasks;
			}
		}

		/// <summary>
		/// This will tell the server process to exit. As such, this client connection will be
		/// invalid after this call.
		/// </summary>
		public void Exit()
		{
			Client.Dispose();
			Dispose();
		}

		/// <summary>
		/// Checks whether the executor instance has connected to a server.
		/// </summary>
		public bool IsConnected
		{
			get;
			private set;
		}

		/// <summary>
		/// The DirectExecutor proxy object used for calls to the server.
		/// </summary>
		private DirectExecutor Client;
	}
}