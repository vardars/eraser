/* 
 * $Id$
 * Copyright 2008 The Eraser Project
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

using Eraser.Manager;
using Eraser.Util;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.Reflection;

namespace Eraser
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] commandLine)
		{
			//Trivial case: no command parameters
			if (commandLine.Length == 0)
				GUIMain(false);

			//Determine if the sole parameter is --restart; if it is, start the GUI
			//passing isRestart as true. Otherwise, we're a console application.
			else if (commandLine.Length == 1)
			{
				if (commandLine[0] == "/restart" || commandLine[0] == "--restart")
				{
					GUIMain(true);
				}
				else
				{
					CommandMain(commandLine);
				}
			}

			//The other trivial case: definitely a console application.
			else
				CommandMain(commandLine);
		}

		/// <summary>
		/// Runs Eraser as a command-line application.
		/// </summary>
		/// <param name="commandLine">The command line parameters passed to Eraser.</param>
		private static void CommandMain(string[] commandLine)
		{
			//Create a console for our GUI app.
			KernelAPI.AllocConsole();
			Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
			Console.SetIn(new StreamReader(Console.OpenStandardInput()));

			//Map commands to our functions.
			Dictionary<string, CommandHandler> handlers =
				new Dictionary<string, CommandHandler>();
			handlers.Add("addtask", CommandAddTask);
			handlers.Add("querymethods", CommandQueryMethods);
			handlers.Add("help", delegate(Dictionary<string, string> arguments)
				{
					CommandHelp();
				});

			try
			{
				//Get the command.
				if (commandLine.Length < 1)
				{
					CommandHelp();
					return;
				}
				else if (!handlers.ContainsKey(commandLine[0]))
					throw new ArgumentException("Unknown action: " + commandLine[0]);	

				//Parse the command line.
				Dictionary<string, string> cmdParams = new Dictionary<string, string>();
				for (int i = 1; i != commandLine.Length; ++i)
				{
					string param = commandLine[i];
					if (param.Length == 0)
						continue;

					if (param[0] == '/' || param[0] == '-')
					{
						//Ignore the second hyphen if the user specified --X
						if (param[0] == '-' && param.Length >= 2 && param[1] == '-')
							param = param.Substring(2);
						else
							param = param.Substring(1);

						//Separate the key/value at the first equal sign.
						int eqIdx = param.IndexOf('=');
						if (eqIdx != -1)
							cmdParams.Add(param.Substring(0, eqIdx), param.Substring(eqIdx + 1));
						else
							cmdParams.Add(param, null);
					}
					else
					{
						throw new ArgumentException("Invalid command line parameter: " +
							param);
					}
				}

				//Call the function
				handlers[commandLine[0]](cmdParams);
			}
			catch (ArgumentException e)
			{
				Console.WriteLine(e.Message + "\n");
				CommandUsage();
			}
			finally
			{
				//Flush the buffer since we may have got buffered output.
				Console.Out.Flush();

				Console.Write("\nPress any key to continue . . . ");
				Console.Out.Flush();
				Console.ReadLine();

				//We are no longer using the console, release it.
				KernelAPI.FreeConsole();
			}
		}

		/// <summary>
		/// Parses the command line for tasks and adds them using the
		/// <see cref="RemoteExecutor"/> class.
		/// </summary>
		/// <param name="commandLine">The command line parameters passed to the program.</param>
		private static void CommandAddTask(Dictionary<string, string> arguments)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lists all registered erasure methods.
		/// </summary>
		/// <param name="commandLine">The command line parameters passed to the program.</param>
		private static void CommandQueryMethods(Dictionary<string, string> arguments)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Prints the help text for Eraser (with copyright)
		/// </summary>
		private static void CommandHelp()
		{
			Console.WriteLine(@"Eraser {0}
(c) 2008 The Eraser Project
Eraser is Open-Source Software: see http://eraser.heidi.ie/ for details.
",	Assembly.GetExecutingAssembly().GetName().Version);

			Console.Out.Flush();
			CommandUsage();
		}

		/// <summary>
		/// Prints the command line help for Eraser.
		/// </summary>
		private static void CommandUsage()
		{
			Console.WriteLine(@"usage: Eraser <action> <arguments>
where action is
    addtask                 Adds tasks to the current task list.
    querymethods            Lists all registered Erasure methods.

parameters for addtask:
    eraser addtask --method <methodGUID> (--recycled | --unused=<volume> | " +
@"--dir=<directory> | [file1 [file2 [...]]])

    --method, -m            The Erasure method to use.
    --recycled, -r          Erases files and folders in the recycle bin
    --unused, -u            Erases unused space in the volume.
    --dir, --directory, -d  Erases files and folders in the directory
        optional arguments: --dir=<directory>[,e=excludeMask][,i=includeMask]
            excludeMask     A wildcard expression for files and folders to exclude.
            includeMask     A wildcard expression for files and folders to include.
                            The include mask is applied before the exclude mask.
    file1 ... fileN         The list of files to erase.

parameters for querymethods:
    eraser querymethods");
			Console.Out.Flush();
		}

		/// <summary>
		/// Runs Eraser as a GUI application.
		/// </summary>
		/// <param name="isRestart">True if the program was passed the --restart
		/// switch.</param>
		private static void GUIMain(bool isRestart)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.SafeTopLevelCaptionFormat = S._("Eraser");

			using (ManagerLibrary library = new ManagerLibrary(new Settings()))
			using (eraserClient = new DirectExecutor())
			{
				//Set our UI language
				EraserSettings settings = new EraserSettings();
				System.Threading.Thread.CurrentThread.CurrentUICulture =
					new CultureInfo(settings.Language);

				//Load the task list
				if (settings.TaskList != null)
					using (MemoryStream stream = new MemoryStream(settings.TaskList))
						try
						{
							eraserClient.LoadTaskList(stream);
						}
						catch (Exception)
						{
							settings.TaskList = null;
							MessageBox.Show(S._("Could not load task list. All task entries have " +
								"been lost."), S._("Eraser"), MessageBoxButtons.OK,
								MessageBoxIcon.Error);
						}

				//Create the main form
				MainForm form = new MainForm();

				//Run tasks which are meant to be run on restart
				if (isRestart)
				{
					eraserClient.QueueRestartTasks();
				}

				//Run the program
				eraserClient.Run();
				Application.Run(form);

				//Save the task list
				using (MemoryStream stream = new MemoryStream())
				{
					eraserClient.SaveTaskList(stream);
					settings.TaskList = stream.ToArray();
				}
			}
		}

		/// <summary>
		/// The global Executor instance.
		/// </summary>
		public static Executor eraserClient;

		/// <summary>
		/// Handles commands passed to the program
		/// </summary>
		/// <param name="arguments">The arguments to the command</param>
		private delegate void CommandHandler(Dictionary<string, string> arguments);
	}

	class Settings : Manager.SettingsManager
	{
		/// <summary>
		/// Registry-based storage backing for the Settings class.
		/// </summary>
		private class RegistrySettings : Manager.Settings
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="key">The registry key to look for the settings in.</param>
			public RegistrySettings(Guid pluginID, RegistryKey key)
			{
				this.key = key;
			}

			public override object this[string setting]
			{
				get
				{
					byte[] currentSetting = (byte[])key.GetValue(setting, null);
					if (currentSetting != null && currentSetting.Length != 0)
						using (MemoryStream stream = new MemoryStream(currentSetting))
							try
							{
								return new BinaryFormatter().Deserialize(stream);
							}
							catch (Exception)
							{
								key.DeleteValue(setting);
								MessageBox.Show(S._("Could not load the setting {0} for plugin {1}. " +
									"The setting has been lost.", key, pluginID.ToString()),
									S._("Eraser"), MessageBoxButtons.OK, MessageBoxIcon.Error);
							}

					return null;
				}
				set
				{
					if (value == null)
					{
						key.DeleteValue(setting);
					}
					else
					{
						using (MemoryStream stream = new MemoryStream())
						{
							new BinaryFormatter().Serialize(stream, value);
							key.SetValue(setting, stream.ToArray(), RegistryValueKind.Binary);
						}
					}
				}
			}

			/// <summary>
			/// The GUID of the plugin whose settings this object is storing.
			/// </summary>
			private Guid pluginID;

			/// <summary>
			/// The registry key where the data is stored.
			/// </summary>
			private RegistryKey key;
		}

		public override void Save()
		{
		}

		protected override Manager.Settings GetSettings(Guid guid)
		{
			//Open the registry key containing the settings
			const string eraserKeyPath = @"SOFTWARE\Eraser\Eraser 6";
			RegistryKey eraserKey = Registry.CurrentUser.OpenSubKey(eraserKeyPath, true);
			if (eraserKey == null)
				eraserKey = Registry.CurrentUser.CreateSubKey(eraserKeyPath);

			RegistryKey pluginsKey = eraserKey.OpenSubKey(guid.ToString(), true);
			if (pluginsKey == null)
				pluginsKey = eraserKey.CreateSubKey(guid.ToString());

			//Return the Settings object.
			return new RegistrySettings(guid, pluginsKey);
		}
	}

	internal class EraserSettings
	{
		public EraserSettings()
		{
			settings = Manager.ManagerLibrary.Instance.SettingsManager.ModuleSettings;
		}

		/// <summary>
		/// Gets or sets the task list, serialised in binary form by the Manager assembly.
		/// </summary>
		public byte[] TaskList
		{
			get
			{
				return (byte[])settings["TaskList"];
			}
			set
			{
				settings["TaskList"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the LCID of the language which the UI should be displayed in.
		/// </summary>
		public string Language
		{
			get
			{
				return settings["Language"] == null ? 
					GetCurrentCulture().Name :
					(string)settings["Language"];
			}
			set
			{
				settings["Language"] = value;
			}
		}

		/// <summary>
		/// Gets or sets a value on whether the main frame should be minimised to the
		/// system notification area.
		/// </summary>
		public bool HideWhenMinimised
		{
			get
			{
				return settings["HideWhenMinimised"] == null ?
					true : (bool)settings["HideWhenMinimised"];
			}
			set
			{
				settings["HideWhenMinimised"] = value;
			}
		}

		/// <summary>
		/// Gets the current UI culture, correct to the top-level culture (i.e., English
		/// instead of English (United Kingdom))
		/// </summary>
		/// <returns>The CultureInfo of the current UI culture, correct to the top level.</returns>
		private static CultureInfo GetCurrentCulture()
		{
			CultureInfo culture = CultureInfo.CurrentUICulture;
			while (culture.Parent != CultureInfo.InvariantCulture)
				culture = culture.Parent;

			return culture;
		}

		private Manager.Settings settings;
	}
}
