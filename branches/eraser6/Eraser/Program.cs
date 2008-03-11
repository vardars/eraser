using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Eraser.Manager;
using Microsoft.Win32;
using System.IO;

namespace Eraser
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (eraserClient = new DirectExecutor())
			{
				//Load the task list
				RegistryKey key = Application.UserAppDataRegistry;
				byte[] savedTaskList = (byte[])key.GetValue("TaskList", new byte[] { });
				using (MemoryStream stream = new MemoryStream(savedTaskList))
				{
					eraserClient.LoadTaskList(stream);
				}

				//Run the program
				Application.Run(new MainForm());

				//Save the task list
				using (MemoryStream stream = new MemoryStream())
				{
					eraserClient.SaveTaskList(stream);
					key.SetValue("TaskList", stream.ToArray(), RegistryValueKind.Binary);
				}
			}
		}

		public static DirectExecutor eraserClient;
	}
}
