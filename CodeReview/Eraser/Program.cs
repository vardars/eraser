/* 
 * $Id$
 * Copyright 2008-2010 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
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
using System.Collections.Generic;
using System.Windows.Forms;

using System.IO;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Principal;

using ComLib.Arguments;

using Eraser.Manager;
using Eraser.Util;

namespace Eraser
{
	internal static partial class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] commandLine)
		{
			//Immediately parse command line arguments
			ComLib.BoolMessageItem argumentParser = Args.Parse(commandLine, "/", ":");
			Args parsedArguments = (Args)argumentParser.Item;

			//We default to a GUI if:
			// - The parser did not succeed.
			// - The parser resulted in an empty arguments list
			// - The parser's argument at index 0 is not equal to the first argument (this
			//   is when the user is passing GUI options -- command line options always
			//   start with the action, e.g. Eraser help, or Eraser addtask
			if (!argumentParser.Success || parsedArguments.IsEmpty ||
				parsedArguments.Positional[0] != parsedArguments.Raw[0])
			{
				GUIMain(commandLine);
			}
			else
			{
				return CommandMain(commandLine);
			}

			//Return zero to signify success
			return 0;
		}

		/// <summary>
		/// Runs Eraser as a command-line application.
		/// </summary>
		/// <param name="commandLine">The command line parameters passed to Eraser.</param>
		private static int CommandMain(string[] commandLine)
		{
			//True if the user specified a quiet command.
			bool isQuiet = false;

			try
			{
				ConsoleProgram program = new ConsoleProgram(commandLine);
				isQuiet = program.Arguments.Quiet;

				using (ManagerLibrary library = new ManagerLibrary(new Settings()))
					program.Run();

				return 0;
			}
			catch (UnauthorizedAccessException)
			{
				return 5; //ERROR_ACCESS_DENIED
			}
			catch (Win32Exception e)
			{
				Console.WriteLine(e.Message);
				return e.ErrorCode;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return 1;
			}
			finally
			{
				//Flush the buffered output to the console
				Console.Out.Flush();

				//Don't ask for a key to press if the user specified Quiet
				if (!isQuiet)
				{
					Console.Write("\nPress enter to continue . . . ");
					Console.Out.Flush();
					Console.ReadLine();
				}

				KernelApi.FreeConsole();
			}
		}

		/// <summary>
		/// Runs Eraser as a GUI application.
		/// </summary>
		/// <param name="commandLine">The command line parameters passed to Eraser.</param>
		private static void GUIMain(string[] commandLine)
		{
			//Create a unique program instance ID for this user.
			string instanceId = "Eraser-BAD0DAC6-C9EE-4acc-8701-C9B3C64BC65E-GUI-" +
				WindowsIdentity.GetCurrent().User.ToString();

			//Then initialise the instance and initialise the Manager library.
			using (GuiProgram program = new GuiProgram(commandLine, instanceId))
			using (ManagerLibrary library = new ManagerLibrary(new Settings()))
			{
				program.InitInstance += OnGUIInitInstance;
				program.NextInstance += OnGUINextInstance;
				program.ExitInstance += OnGUIExitInstance;
				program.Run();
			}
		}

		/// <summary>
		/// Triggered when the Program is started for the first time.
		/// </summary>
		/// <param name="sender">The sender of the object.</param>
		/// <param name="e">Event arguments.</param>
		private static void OnGUIInitInstance(object sender, InitInstanceEventArgs e)
		{
			GuiProgram program = (GuiProgram)sender;
			eraserClient = new RemoteExecutorServer();

			//Set our UI language
			EraserSettings settings = EraserSettings.Get();
			System.Threading.Thread.CurrentThread.CurrentUICulture =
				new CultureInfo(settings.Language);
			Application.SafeTopLevelCaptionFormat = S._("Eraser");

			//Load the task list
			SettingsCompatibility.Execute();
			try
			{
				if (System.IO.File.Exists(TaskListPath))
				{
					using (FileStream stream = new FileStream(TaskListPath, FileMode.Open,
						FileAccess.Read, FileShare.Read))
					{
						eraserClient.Tasks.LoadFromStream(stream);
					}
				}
			}
			catch (FileLoadException)
			{
			}
			catch (SerializationException ex)
			{
				System.IO.File.Delete(TaskListPath);
				MessageBox.Show(S._("Could not load task list. All task entries have " +
					"been lost. The error returned was: {0}", ex.Message), S._("Eraser"),
					MessageBoxButtons.OK, MessageBoxIcon.Error,
					MessageBoxDefaultButton.Button1,
					S.IsRightToLeft(null) ? MessageBoxOptions.RtlReading : 0);
			}

			//Create the main form
			program.MainForm = new MainForm();
			foreach (string param in program.CommandLine)
			{
				//Run tasks which are meant to be run on restart
				switch (param)
				{
					case "--atRestart":
						eraserClient.QueueRestartTasks();
						goto case "--quiet";

					//Hide the main form if the user specified the quiet command
					//line
					case "--quiet":
						e.ShowMainForm = false;
						break;
				}
			}

			//Run the eraser client.
			eraserClient.Run();
		}

		/// <summary>
		/// Triggered when a second instance of Eraser is started.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">Event argument.</param>
		private static void OnGUINextInstance(object sender, NextInstanceEventArgs e)
		{
			//Another instance of the GUI Program has been started: show the main window
			//now as we still do not have a facility to handle the command line arguments.
			GuiProgram program = (GuiProgram)sender;

			//Invoke the function if we aren't on the main thread
			if (program.MainForm.InvokeRequired)
			{
				program.MainForm.Invoke(
					(GuiProgram.NextInstanceEventHandler)OnGUINextInstance,
					sender, e);
				return;
			}

			program.MainForm.Show();
		}

		/// <summary>
		/// Triggered when the first instance of Eraser is exited.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">Event argument.</param>
		private static void OnGUIExitInstance(object sender, EventArgs e)
		{
			//Save the task list
			if (!Directory.Exists(Program.AppDataPath))
				Directory.CreateDirectory(Program.AppDataPath);
			using (FileStream stream = new FileStream(TaskListPath, FileMode.Create,
				FileAccess.Write, FileShare.None))
			{
				eraserClient.Tasks.SaveToStream(stream);
			}

			//Dispose the eraser executor instance
			eraserClient.Dispose();
		}

		/// <summary>
		/// The global Executor instance.
		/// </summary>
		public static Executor eraserClient;

		/// <summary>
		/// Path to the Eraser application data path.
		/// </summary>
		public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(
			Environment.SpecialFolder.LocalApplicationData), @"Eraser 6");

		/// <summary>
		/// File name of the Eraser task list.
		/// </summary>
		private const string TaskListFileName = @"Task List.ersx";

		/// <summary>
		/// Path to the Eraser task list.
		/// </summary>
		public static readonly string TaskListPath = Path.Combine(AppDataPath, TaskListFileName);

		/// <summary>
		/// Path to the Eraser settings key (relative to HKCU)
		/// </summary>
		public const string SettingsPath = @"SOFTWARE\Eraser\Eraser 6";
	}
}
