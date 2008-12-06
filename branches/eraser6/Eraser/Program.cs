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
using System.Diagnostics;

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
			//True if the user specified a quiet command.
			bool isQuiet = false;

			try
			{
				CommandLineProgram program = new CommandLineProgram(commandLine);
				isQuiet = program.Arguments.Quiet;

				using (ManagerLibrary library = new ManagerLibrary(new Settings()))
				using (Program.eraserClient = new RemoteExecutorClient())
				{
					if (!((RemoteExecutorClient)Program.eraserClient).Connect())
					{
						//The client cannot connect to the server. This probably means
						//that the server process isn't running. Start an instance.
						Process eraserInstance = Process.Start(
							Assembly.GetExecutingAssembly().Location);
						eraserInstance.WaitForInputIdle();

						if (!((RemoteExecutorClient)Program.eraserClient).Connect())
							throw new Exception("Eraser cannot connect to the running " +
								"instance for erasures.");
					}

					eraserClient.Run();
					program.Run();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				//Flush the buffered output to the console
				Console.Out.Flush();

				//Don't ask for a key to press if the user specified Quiet
				if (!isQuiet)
				{
					Console.Write("\nPress any key to continue . . . ");
					Console.Out.Flush();
					Console.ReadLine();
				}

				KernelAPI.FreeConsole();
			}
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
			using (eraserClient = new RemoteExecutorServer())
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

	class CommandLineProgram
	{
		#region Command Line parsing classes
		/// <summary>
		/// Manages a command line.
		/// </summary>
		public class CommandLine
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="cmdParams">The raw arguments passed to the program.</param>
			public CommandLine(string[] cmdParams)
			{
				//Get the action.
				if (cmdParams.Length < 1)
					throw new ArgumentException("An action must be specified.");
				Action = cmdParams[0];

				//Iterate over each argument, resolving them ourselves and letting
				//subclasses resolve them if we don't know how to.
				for (int i = 1; i != cmdParams.Length; ++i)
				{
					if (IsParam(cmdParams[i], "quiet", "q"))
						Quiet = true;
					else if (!ResolveParameter(cmdParams[i]))
						throw new ArgumentException("Unknown argument: " + cmdParams[i]);
				}
			}

			/// <summary>
			/// Called when a parameter is not used by the current CommandLine object
			/// for subclasses to handle their parameters.
			/// </summary>
			/// <param name="param">The parameter to resolve.</param>
			/// <returns>Return true if the parameter was resolved and accepted.</returns>
			virtual protected bool ResolveParameter(string param)
			{
				return false;
			}

			/// <summary>
			/// Checks if the given <paramref name="parameter"/> refers to the
			/// <paramref name="expectedParameter"/>, regardless of whether it is specified
			/// with -, --, or /
			/// </summary>
			/// <param name="parameter">The parameter on the command line.</param>
			/// <param name="expectedParameter">The parameter the program is looking for, without
			/// the - or / prefix.</param>
			/// <param name="shortParameter">The short parameter when used with a single hyphen,
			/// without the - or / prefix.</param>
			/// <returns>True if the parameter references the given expected parameter.</returns>
			protected static bool IsParam(string parameter, string expectedParameter,
				string shortParameter)
			{
				//Trivial case
				if (parameter.Length < 1)
					return false;

				//Extract the bits before the equal sign.
				{
					int equalPos = parameter.IndexOf('=');
					if (equalPos != -1)
						parameter = parameter.Substring(0, equalPos);
				}

				//Get the first letter.
				switch (parameter[0])
				{
					case '-':
						//Can be a - or a --. Check for the second parameter
						if (parameter.Length < 2)
							//Nothing specified at the end... it's invalid.
							return false;

						if (parameter[1] == '-')
							return parameter.Substring(2) == expectedParameter;
						else if (shortParameter == null || shortParameter == string.Empty)
							return parameter.Substring(1) == expectedParameter;
						else
							return parameter.Substring(1) == shortParameter;
						
					case '/':
						//The / can be used with both long and short parameters.
						parameter = parameter.Substring(1);
						return parameter == expectedParameter || (
							shortParameter != null && shortParameter != string.Empty &&
							parameter == shortParameter
						);

					default:
						return false;
				}
			}

			/// <summary>
			/// Gets the list of subparameters of the parameter. Subparameters are text
			/// after the first =, separated by commas.
			/// </summary>
			/// <param name="param">The subparameter text to parse.</param>
			/// <returns>The list of subparameters in the parameter.</returns>
			protected static List<KeyValuePair<string, string>> GetSubParameters(string param)
			{
				List<KeyValuePair<string, string>> result =
					new List<KeyValuePair<string, string>>();
				int lastPos = 0;
				int commaPos = (param += ',').IndexOf(',');

				while (commaPos != -1)
				{
					//Extract the current subparameter, and dissect the subparameter at
					//the first =.
					string subParam = param.Substring(lastPos, commaPos - lastPos);
					int equalPos = subParam.IndexOf('=');
					if (equalPos == -1)
						result.Add(new KeyValuePair<string, string>(subParam, null));
					else
						result.Add(new KeyValuePair<string, string>(subParam.Substring(0, equalPos),
							subParam.Substring(equalPos + 1)));

					//Find the next ,
					lastPos = ++commaPos;
					commaPos = param.IndexOf(',', commaPos);
				}

				return result;
			}

			/// <summary>
			/// The action that the command line specifies.
			/// </summary>
			public string Action
			{
				get
				{
					return action;
				}
				private set
				{
					action = value;
				}
			}

			/// <summary>
			/// True if no console window should be created.
			/// </summary>
			public bool Quiet
			{
				get
				{
					return quiet;
				}
				private set
				{
					quiet = value;
				}
			}

			private string action;
			private bool quiet;
		}

		/// <summary>
		/// Manages a command line for adding tasks to the global DirectExecutor
		/// </summary>
		class AddTaskCommandLine : CommandLine
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="cmdParams">The raw command line arguments passed to the program.</param>
			public AddTaskCommandLine(string[] cmdParams)
				: base(cmdParams)
			{
			}

			protected override bool ResolveParameter(string param)
			{
				int equalPos = param.IndexOf('=');
				if (IsParam(param, "method", "m"))
				{
					if (equalPos == -1)
						throw new ArgumentException("--method must be specified with an Erasure " +
							"method GUID.");
					ErasureMethod = new Guid(param.Substring(equalPos + 1));
				}
				else if (IsParam(param, "recycled", "r"))
				{
					targets.Add(new Task.RecycleBin());
				}
				else if (IsParam(param, "unused", "u"))
				{
					if (equalPos == -1)
						throw new ArgumentException("--unused must be specified with the Volume " +
							"to erase.");

					//Create the UnusedSpace target for inclusion into the task.
					Task.UnusedSpace target = new Task.UnusedSpace();

					//Determine if cluster tips should be erased.
					target.EraseClusterTips = false;
					List<KeyValuePair<string, string>> subParams =
						GetSubParameters(param.Substring(equalPos + 1));
					foreach (KeyValuePair<string, string> kvp in subParams)
						if (kvp.Value == null && target.Drive == null)
							target.Drive = kvp.Key;
						else if (kvp.Key == "clusterTips")
							target.EraseClusterTips = true;
						else
							throw new ArgumentException("Unknown subparameter: " + kvp.Key);
					targets.Add(target);
				}
				else if (IsParam(param, "dir", "d") || IsParam(param, "directory", null))
				{
					if (equalPos == -1)
						throw new ArgumentException("--directory must be specified with the " +
							"directory to erase.");

					//Create the base target
					Task.Folder target = new Task.Folder();

					//Parse the subparameters.
					List<KeyValuePair<string, string>> subParams =
						GetSubParameters(param.Substring(equalPos + 1));
					foreach (KeyValuePair<string, string> kvp in subParams)
						if (kvp.Value == null && target.Path == null)
							target.Path = kvp.Key;
						else if (kvp.Key == "excludeMask")
						{
							if (kvp.Value == null)
								throw new ArgumentException("The exclude mask must be specified " +
									"if the excludeMask subparameter is specified");
							target.ExcludeMask = kvp.Value;
						}
						else if (kvp.Key == "includeMask")
						{
							if (kvp.Value == null)
								throw new ArgumentException("The include mask must be specified " +
									"if the includeMask subparameter is specified");
							target.IncludeMask = kvp.Value;
						}
						else if (kvp.Key == "delete")
							target.DeleteIfEmpty = true;
						else
							throw new ArgumentException("Unknown subparameter: " + kvp.Key);

					//Add the target to the list of targets
					targets.Add(target);
				}
				else
				{
					//It's just a file!
					Task.File target = new Task.File();
					target.Path = param;
					targets.Add(target);
				}

				return true;
			}

			/// <summary>
			/// The erasure method which the user specified on the command line.
			/// </summary>
			public Guid ErasureMethod
			{
				get
				{
					return erasureMethod;
				}
				private set
				{
					erasureMethod = value;
				}
			}

			/// <summary>
			/// The list of targets which was specified on the command line.
			/// </summary>
			public List<Task.ErasureTarget> Targets
			{
				get
				{
					return new List<Task.ErasureTarget>(targets.ToArray());
				}
				set
				{
					targets = value;
				}
			}

			private Guid erasureMethod;
			private List<Task.ErasureTarget> targets = new List<Task.ErasureTarget>();
		}
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="cmdParams">The raw command line arguments passed to the program.</param>
		public CommandLineProgram(string[] cmdParams)
		{
			try
			{
				//Parse the command line arguments.
				if (cmdParams.Length < 1)
					throw new ArgumentException("An action must be specified.");

				switch (cmdParams[0])
				{
					case "addtask":
						Arguments = new AddTaskCommandLine(cmdParams);
						break;
					case "querymethods":
					case "help":
					default:
						Arguments = new CommandLine(cmdParams);
						break;
				}

				//If the user did not specify the quiet command line, then create the console.
				if (!Arguments.Quiet)
					CreateConsole();

				//Map actions to their handlers
				actionHandlers.Add("addtask", AddTask);
				actionHandlers.Add("querymethods", QueryMethods);
				actionHandlers.Add("help", Help);
			}
			finally
			{
				if (Arguments == null || !Arguments.Quiet)
					CreateConsole();
			}
		}

		/// <summary>
		/// Runs the program, analogous to System.Windows.Forms.Application.Run.
		/// </summary>
		public void Run()
		{
			//Call the function handling the current command line.
			actionHandlers[Arguments.Action]();
		}

		/// <summary>
		/// Creates a console for our application, setting the input/output streams to the
		/// defaults.
		/// </summary>
		private void CreateConsole()
		{
			if (KernelAPI.AllocConsole())
			{
				Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
				Console.SetIn(new StreamReader(Console.OpenStandardInput()));
			}
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

global parameters:
    --quiet, -q	            Do not create a Console window to display progress.

parameters for addtask:
    eraser addtask --method=<methodGUID> (--recycled | --unused=<volume> | " +
@"--dir=<directory> | [file1 [file2 [...]]])

    --method, -m            The Erasure method to use.
    --recycled, -r          Erases files and folders in the recycle bin
    --unused, -u            Erases unused space in the volume.
        optional arguments: --unused=<drive>[,clusterTips]
            clusterTips     If specified, the drive's files will have their cluster tips
                            erased.
    --dir, --directory, -d  Erases files and folders in the directory
        optional arguments: --dir=<directory>[,e=excludeMask][,i=includeMask][,delete]
            excludeMask     A wildcard expression for files and folders to exclude.
            includeMask     A wildcard expression for files and folders to include.
                            The include mask is applied before the exclude mask.
            delete          Deletes the folder at the end of the erasure if specified.
    file1 ... fileN         The list of files to erase.

parameters for querymethods:
    eraser querymethods

    no parameters to set.

All arguments are case sensitive.");
			Console.Out.Flush();
		}

		#region Action Handlers
		/// <summary>
		/// The command line arguments passed to the program.
		/// </summary>
		public CommandLine Arguments
		{
			get
			{
				return arguments;
			}
			private set
			{
				arguments = value;
			}
		}

		/// <summary>
		/// Prints the help text for Eraser (with copyright)
		/// </summary>
		private void Help()
		{
			Console.WriteLine(@"Eraser {0}
(c) 2008 The Eraser Project
Eraser is Open-Source Software: see http://eraser.heidi.ie/ for details.
", Assembly.GetExecutingAssembly().GetName().Version);

			Console.Out.Flush();
			CommandUsage();
		}

		/// <summary>
		/// Lists all registered erasure methods.
		/// </summary>
		/// <param name="commandLine">The command line parameters passed to the program.</param>
		private void QueryMethods()
		{
			//Output the header
			const string methodFormat = "{0,-2} {1,-39} {2}";
			Console.WriteLine(methodFormat, "", "Method", "GUID");
			Console.WriteLine(new string('-', 79));

			//Refresh the list of erasure methods
			Dictionary<Guid, ErasureMethod> methods = ErasureMethodManager.GetAll();
			foreach (ErasureMethod method in methods.Values)
			{
				Console.WriteLine(methodFormat, (method is UnusedSpaceErasureMethod) ?
					"U" : "", method.Name, method.GUID.ToString());
			}
		}

		/// <summary>
		/// Parses the command line for tasks and adds them using the
		/// <see cref="RemoteExecutor"/> class.
		/// </summary>
		/// <param name="commandLine">The command line parameters passed to the program.</param>
		private void AddTask()
		{
			AddTaskCommandLine arguments = (AddTaskCommandLine)Arguments;
			
			//Create the task, and set the method to use.
			Task task = new Task();
			ErasureMethod method = arguments.ErasureMethod == Guid.Empty ? 
				ErasureMethodManager.Default :
				ErasureMethodManager.GetInstance(arguments.ErasureMethod);
			foreach (Task.ErasureTarget target in arguments.Targets)
			{
				target.Method = method;
				task.Targets.Add(target);
			}

			//Send the task out.
			Program.eraserClient.AddTask(ref task);
		}
		#endregion

		/// <see cref="Arguments"/>
		private CommandLine arguments;

		/// <summary>
		/// The prototype of an action handler in the class which executes an
		/// action as specified in the command line.
		/// </summary>
		private delegate void ActionHandler();

		/// <summary>
		/// Matches an action handler to a function in the class.
		/// </summary>
		private Dictionary<string, ActionHandler> actionHandlers =
			new Dictionary<string, ActionHandler>();
	}

	internal class Settings : Manager.SettingsManager
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
		/// Gets or sets whether the Shell Extension should be loaded into Explorer.
		/// </summary>
		public bool IntegrateWithShell
		{
			get
			{
				return settings["IntegrateWithShell"] == null ?
					true : (bool)settings["IntegrateWithShell"];
			}
			set
			{
				settings["IntegrateWithShell"] = value;
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
