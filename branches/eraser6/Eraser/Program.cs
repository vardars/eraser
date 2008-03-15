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
			Application.SafeTopLevelCaptionFormat = "Eraser";

			using (ManagerLibrary library = new ManagerLibrary())
			using (eraserClient = new DirectExecutor())
			{
				//Set the defaults for the library
				library.Settings = new Settings();

				//Load the task list
				RegistryKey key = Application.UserAppDataRegistry;
				byte[] savedTaskList = (byte[])key.GetValue("TaskList", new byte[] { });
				using (MemoryStream stream = new MemoryStream(savedTaskList))
				{
					try
					{
						if (savedTaskList.Length != 0)
							eraserClient.LoadTaskList(stream);
					}
					catch (Exception)
					{
						key.DeleteValue("TaskList");
						MessageBox.Show("Could not load task list. All task entries have " +
							"been lost.", "Eraser", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
				}

				//Create the main form
				MainForm form = new MainForm();

				//Run tasks which are meant to be run on restart
				int restartPos = Environment.CommandLine.ToLower().IndexOf("restart");
				if ((restartPos > 1 &&
					Environment.CommandLine[restartPos - 1] == '/') ||
					(restartPos > 2 &&
					Environment.CommandLine[restartPos - 1] == '-' &&
					Environment.CommandLine[restartPos - 2] == '-'))
				{
					eraserClient.QueueRestartTasks();
				}

				//Run the program
				Application.Run(form);

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

	public class Settings : Manager.Settings
	{
		public Settings()
		{
			RegistryKey key = Application.UserAppDataRegistry;
			ActivePRNG = new Guid((string)
				key.GetValue("PRNG", (object)Guid.Empty));
			EraseLockedFilesOnRestart =
				(int)key.GetValue("EraseOnRestart", (object)true) != 0;
			ConfirmEraseOnRestart =
				(int)key.GetValue("ConfirmEraseOnRestart", (object)true) != 0;
			DefaultFileErasureMethod = new Guid((string)
				key.GetValue("DefaultFileErasureMethod", (object)Guid.Empty));
			DefaultUnusedSpaceErasureMethod = new Guid((string)
				key.GetValue("DefaultUnusedSpaceErasureMethod", (object)Guid.Empty));
			ExecuteMissedTasksImmediately =
				(int)key.GetValue("ExecuteMissedTasksImmediately", (object)true) != 0;
		}

		~Settings()
		{
			RegistryKey key = Application.UserAppDataRegistry;
			key.SetValue("PRNG", ActivePRNG);
			key.SetValue("EraseOnRestart", EraseLockedFilesOnRestart,
				RegistryValueKind.DWord);
			key.SetValue("ConfirmEraseOnRestart", ConfirmEraseOnRestart,
				RegistryValueKind.DWord);
			key.SetValue("DefaultFileErasureMethod", DefaultFileErasureMethod);
			key.SetValue("DefaultUnusedSpaceErasureMethod",
				DefaultUnusedSpaceErasureMethod);
			key.SetValue("ExecuteMissedTasksImmediately",
				ExecuteMissedTasksImmediately, RegistryValueKind.DWord);
		}
	}
}
