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
using System.Text;

using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

using Eraser.Manager;

namespace Eraser
{
	internal static partial class Program
	{
		/// <summary>
		/// Manages and runs a console program. This allows the program to switch
		/// between console and GUI subsystems at runtime. This class will manage
		/// the creation of a console window and destruction of the window upon
		/// exit of the program.
		/// </summary>
		class ConsoleProgram
		{
			#region Command Line parsing classes
			/// <summary>
			/// Manages a command line.
			/// </summary>
			public abstract class CommandLine
			{
				/// <summary>
				/// Gets the CommandLine-derived object for the given command line.
				/// </summary>
				/// <param name="cmdParams">The raw arguments passed to the program.</param>
				/// <returns>A processed CommandLine Object.</returns>
				public static CommandLine Get(string[] cmdParams)
				{
					//Get the action.
					if (cmdParams.Length < 1)
						throw new ArgumentException("An action must be specified.");
					string action = cmdParams[0];

					CommandLine result = null;
					switch (action)
					{
						case "help":
							result = new HelpCommandLine();
							break;
						case "querymethods":
							result = new QueryMethodsCommandLine();
							break;
						case "importtasklist":
							result = new ImportTaskListCommandLine();
							break;
						case "addtask":
							result = new AddTaskCommandLine();
							break;
					}

					if (result != null)
					{
						result.Parse(cmdParams);
						return result;
					}
					else
						throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
							"Unknown action: {0}", action));
				}

				/// <summary>
				/// Constructor.
				/// </summary>
				protected CommandLine()
				{
				}

				/// <summary>
				/// Parses the given command line arguments to the respective properties of
				/// the class.
				/// </summary>
				/// <param name="cmdParams">The raw arguments passed to the program.</param>
				/// <returns></returns>
				public bool Parse(string[] cmdParams)
				{
					//Iterate over each argument, resolving them ourselves and letting
					//subclasses resolve them if we don't know how to.
					for (int i = 1; i != cmdParams.Length; ++i)
					{
						if (IsParam(cmdParams[i], "quiet", "q"))
							Quiet = true;
						else if (!ResolveParameter(cmdParams[i]))
							throw new ArgumentException("Unknown argument: " + cmdParams[i]);
					}

					return true;
				}

				/// <summary>
				/// Called when a parameter is not used by the current CommandLine object
				/// for subclasses to handle their parameters.
				/// </summary>
				/// <param name="param">The parameter to resolve.</param>
				/// <returns>Return true if the parameter was resolved and accepted.</returns>
				protected virtual bool ResolveParameter(string param)
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
							else if (string.IsNullOrEmpty(shortParameter))
								return parameter.Substring(1) == expectedParameter;
							else
								return parameter.Substring(1) == shortParameter;

						case '/':
							//The / can be used with both long and short parameters.
							parameter = parameter.Substring(1);
							return parameter == expectedParameter || (
								!string.IsNullOrEmpty(shortParameter) && parameter == shortParameter
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
						//Check that the first parameter is not a \ otherwise this comma
						//is escaped
						if (commaPos == 0 ||									//No possibility of escaping
							(commaPos >= 1 && param[commaPos - 1] != '\\') ||	//Second character
							(commaPos >= 2 && param[commaPos - 2] == '\\'))		//Cannot be a \\ which is an escape
						{
							//Extract the current subparameter, and dissect the subparameter
							//at the first =.
							string subParam = param.Substring(lastPos, commaPos - lastPos);
							int equalPos = -1;

							do
							{
								equalPos = subParam.IndexOf('=', equalPos + 1);
								if (equalPos == -1)
								{
									result.Add(new KeyValuePair<string, string>(
										UnescapeCommandLine(subParam), null));
								}
								else if (equalPos == 0 ||								//No possibility of escaping
									(equalPos >= 1 && subParam[equalPos - 1] != '\\') ||//Second character
									(equalPos >= 2 && subParam[equalPos - 2] == '\\'))	//Double \\ which is an escape
								{
									result.Add(new KeyValuePair<string, string>(
										UnescapeCommandLine(subParam.Substring(0, equalPos)),
										UnescapeCommandLine(subParam.Substring(equalPos + 1))));
									break;
								}
							}
							while (equalPos != -1);
							lastPos = ++commaPos;
						}
						else
							++commaPos;

						//Find the next ,
						commaPos = param.IndexOf(',', commaPos);
					}

					return result;
				}

				/// <summary>
				/// Unescapes a subparameter command line, removing the extra 
				/// </summary>
				/// <param name="param"></param>
				/// <returns></returns>
				private static string UnescapeCommandLine(string param)
				{
					StringBuilder result = new StringBuilder(param.Length);
					for (int i = 0; i < param.Length; ++i)
						if (param[i] == '\\' && i < param.Length - 1)
							result.Append(param[++i]);
						else
							result.Append(param[i]);
					return result.ToString();
				}

				/// <summary>
				/// True if no console window should be created.
				/// </summary>
				public bool Quiet { get; private set; }
			}

			/// <summary>
			/// Manages a help query command line.
			/// </summary>
			class HelpCommandLine : CommandLine
			{
				public HelpCommandLine()
				{
				}
			}

			class QueryMethodsCommandLine : CommandLine
			{
				public QueryMethodsCommandLine()
				{
				}
			}

			/// <summary>
			/// Manages a command line for adding tasks to the global DirectExecutor
			/// </summary>
			class AddTaskCommandLine : CommandLine
			{
				/// <summary>
				/// Constructor.
				/// </summary>
				public AddTaskCommandLine()
				{
					Schedule = Schedule.RunNow;
					Targets = new List<ErasureTarget>();
				}

				protected override bool ResolveParameter(string param)
				{
					int equalPos = param.IndexOf('=');
					if (IsParam(param, "method", "m"))
					{
						if (equalPos == -1)
							throw new ArgumentException("--method must be specified with an Erasure " +
								"method GUID.");

						List<KeyValuePair<string, string>> subParams =
							GetSubParameters(param.Substring(equalPos + 1));
						ErasureMethod = new Guid(subParams[0].Key);
					}
					else if (IsParam(param, "schedule", "s"))
					{
						if (equalPos == -1)
							throw new ArgumentException("--schedule must be specified with a Schedule " +
								"type.");

						List<KeyValuePair<string, string>> subParams =
							GetSubParameters(param.Substring(equalPos + 1));
						switch (subParams[0].Key)
						{
							case "now":
								Schedule = Schedule.RunNow;
								break;
							case "manually":
								Schedule = Schedule.RunManually;
								break;
							case "restart":
								Schedule = Schedule.RunOnRestart;
								break;
							default:
								throw new ArgumentException("Unknown schedule type: " + subParams[0].Key);
						}
					}
					else if (IsParam(param, "recycled", "r"))
					{
						Targets.Add(new RecycleBinTarget());
					}
					else if (IsParam(param, "unused", "u"))
					{
						if (equalPos == -1)
							throw new ArgumentException("--unused must be specified with the Volume " +
								"to erase.");

						//Create the UnusedSpace target for inclusion into the task.
						UnusedSpaceTarget target = new UnusedSpaceTarget();

						//Determine if cluster tips should be erased.
						target.EraseClusterTips = false;
						List<KeyValuePair<string, string>> subParams =
							GetSubParameters(param.Substring(equalPos + 1));
						foreach (KeyValuePair<string, string> kvp in subParams)
							if (kvp.Value == null && target.Drive == null)
								target.Drive = Path.GetFullPath(kvp.Key);
							else if (kvp.Key == "clusterTips")
								target.EraseClusterTips = true;
							else
								throw new ArgumentException("Unknown subparameter: " + kvp.Key);
						Targets.Add(target);
					}
					else if (IsParam(param, "dir", "d") || IsParam(param, "directory", null))
					{
						if (equalPos == -1)
							throw new ArgumentException("--directory must be specified with the " +
								"directory to erase.");

						//Create the base target
						FolderTarget target = new FolderTarget();

						//Parse the subparameters.
						List<KeyValuePair<string, string>> subParams =
							GetSubParameters(param.Substring(equalPos + 1));
						foreach (KeyValuePair<string, string> kvp in subParams)
							if (kvp.Value == null && target.Path == null)
								target.Path = Path.GetFullPath(kvp.Key);
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
						Targets.Add(target);
					}
					else if (IsParam(param, "file", "f"))
					{
						if (equalPos == -1)
							throw new ArgumentException("--file must be specified with the " +
								"file to erase.");

						//It's just a file!
						FileTarget target = new FileTarget();

						//Parse the subparameters.
						List<KeyValuePair<string, string>> subParams =
							GetSubParameters(param.Substring(equalPos + 1));
						foreach (KeyValuePair<string, string> kvp in subParams)
							if (kvp.Value == null && target.Path == null)
								target.Path = Path.GetFullPath(kvp.Key);
							else
								throw new ArgumentException("Unknown subparameter: " + kvp.Key);

						Targets.Add(target);
					}
					else
						return false;

					return true;
				}

				/// <summary>
				/// The erasure method which the user specified on the command line.
				/// </summary>
				public Guid ErasureMethod { get; private set; }

				/// <summary>
				/// The schedule for the current set of targets.
				/// </summary>
				public Schedule Schedule { get; private set; }

				/// <summary>
				/// The list of targets which was specified on the command line.
				/// </summary>
				public List<ErasureTarget> Targets { get; private set; }
			}

			/// <summary>
			/// Manages a command line for importing a task list into the global
			/// DirectExecutor.
			/// </summary>
			class ImportTaskListCommandLine : CommandLine
			{
				/// <summary>
				/// Constructor.
				/// </summary>
				public ImportTaskListCommandLine()
				{
				}

				protected override bool ResolveParameter(string param)
				{
					if (!System.IO.File.Exists(param))
						throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
							"The file {0} does not exist.", param));

					files.Add(param);
					return true;
				}

				public ICollection<string> Files
				{
					get
					{
						return files.AsReadOnly();
					}
				}

				private List<string> files = new List<string>();
			}
			#endregion

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="cmdParams">The raw command line arguments passed to the program.</param>
			public ConsoleProgram(string[] cmdParams)
			{
				try
				{
					Arguments = CommandLine.Get(cmdParams);

					//If the user did not specify the quiet command line, then create the console.
					if (!Arguments.Quiet)
						CreateConsole();

					//Map actions to their handlers
					actionHandlers.Add(typeof(AddTaskCommandLine), AddTask);
					actionHandlers.Add(typeof(ImportTaskListCommandLine), ImportTaskList);
					actionHandlers.Add(typeof(QueryMethodsCommandLine), QueryMethods);
					actionHandlers.Add(typeof(HelpCommandLine), Help);
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
				actionHandlers[Arguments.GetType()]();
			}

			/// <summary>
			/// Creates a console for our application, setting the input/output streams to the
			/// defaults.
			/// </summary>
			private static void CreateConsole()
			{
				if (Util.KernelApi.AllocConsole())
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
    help                    Show this help message.
    addtask                 Adds tasks to the current task list.
    querymethods            Lists all registered Erasure methods.

global parameters:
    --quiet, -q	            Do not create a Console window to display progress.

parameters for help:
    eraser help

    no parameters to set.

parameters for addtask:
    eraser addtask [--method=<methodGUID>] [--schedule=(now|manually|restart)] (--recycled " +
	@"| --unused=<volume> | --dir=<directory> | --file=<file>)[...]

    --method, -m            The Erasure method to use.
    --schedule, -s          The schedule the task will follow. The value must
                            be one of:
            now             The task will be queued for immediate execution.
            manually        The task will be created but not queued for execution.
            restart         The task will be queued for execution when the
                            computer is next restarted.
                            This parameter defaults to now.
    --recycled, -r          Erases files and folders in the recycle bin
    --unused, -u            Erases unused space in the volume.
        optional arguments: --unused=<drive>[,clusterTips]
            clusterTips     If specified, the drive's files will have their
                            cluster tips erased.
    --dir, --directory, -d  Erases files and folders in the directory
        optional arguments: --dir=<directory>[,e=excludeMask][,i=includeMask][,delete]
            excludeMask     A wildcard expression for files and folders to
                            exclude.
            includeMask     A wildcard expression for files and folders to
                            include.
                            The include mask is applied before the exclude
                            mask.
            delete          Deletes the folder at the end of the erasure if
                            specified.
    --file, -f              Erases the specified file

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
			public CommandLine Arguments { get; private set; }

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
				Dictionary<Guid, ErasureMethod> methods = ErasureMethodManager.Items;
				foreach (ErasureMethod method in methods.Values)
				{
					Console.WriteLine(methodFormat, (method is UnusedSpaceErasureMethod) ?
						"U" : "", method.Name, method.Guid.ToString());
				}
			}

			/// <summary>
			/// Parses the command line for tasks and adds them using the
			/// <see cref="RemoteExecutor"/> class.
			/// </summary>
			/// <param name="commandLine">The command line parameters passed to the program.</param>
			private void AddTask()
			{
				AddTaskCommandLine taskArgs = (AddTaskCommandLine)Arguments;

				//Create the task, and set the method to use.
				Task task = new Task();
				ErasureMethod method = taskArgs.ErasureMethod == Guid.Empty ?
					ErasureMethodManager.Default :
					ErasureMethodManager.GetInstance(taskArgs.ErasureMethod);

				foreach (ErasureTarget target in taskArgs.Targets)
				{
					target.Method = method;
					task.Targets.Add(target);
				}

				//Check the number of tasks in the task.
				if (task.Targets.Count == 0)
					throw new ArgumentException("Tasks must contain at least one erasure target.");

				//Set the schedule for the task.
				task.Schedule = taskArgs.Schedule;

				//Send the task out.
				try
				{
					using (RemoteExecutorClient client = new RemoteExecutorClient())
					{
						client.Run();
						if (!client.IsConnected)
						{
							//The client cannot connect to the server. This probably means
							//that the server process isn't running. Start an instance.
							Process eraserInstance = Process.Start(
								Assembly.GetExecutingAssembly().Location, "--quiet");
							eraserInstance.WaitForInputIdle();

							client.Run();
							if (!client.IsConnected)
								throw new IOException("Eraser cannot connect to the running " +
									"instance for erasures.");
						}

						client.Tasks.Add(task);
					}
				}
				catch (UnauthorizedAccessException e)
				{
					//We can't connect to the pipe because the other instance of Eraser
					//is running with higher privileges than this instance.
					throw new UnauthorizedAccessException("Another instance of Eraser " +
						"is already running but it is running with higher privileges than " +
						"this instance of Eraser. Tasks cannot be added in this manner.\n\n" +
						"Close the running instance of Eraser and start it again without " +
						"administrator privileges, or run the command again as an " +
						"administrator.", e);
				}
			}

			/// <summary>
			/// Imports the given tasklists and adds them to the global Eraser instance.
			/// </summary>
			private void ImportTaskList()
			{
				ImportTaskListCommandLine cmdLine = (ImportTaskListCommandLine)Arguments;

				//Import the task list
				try
				{
					using (RemoteExecutorClient client = new RemoteExecutorClient())
					{
						client.Run();
						if (!client.IsConnected)
						{
							//The client cannot connect to the server. This probably means
							//that the server process isn't running. Start an instance.
							Process eraserInstance = Process.Start(
								Assembly.GetExecutingAssembly().Location, "--quiet");
							eraserInstance.WaitForInputIdle();

							client.Run();
							if (!client.IsConnected)
								throw new IOException("Eraser cannot connect to the running " +
									"instance for erasures.");
						}

						foreach (string path in cmdLine.Files)
							using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
								client.Tasks.LoadFromStream(stream);
					}
				}
				catch (UnauthorizedAccessException e)
				{
					//We can't connect to the pipe because the other instance of Eraser
					//is running with higher privileges than this instance.
					throw new UnauthorizedAccessException("Another instance of Eraser " +
						"is already running but it is running with higher privileges than " +
						"this instance of Eraser. Tasks cannot be added in this manner.\n\n" +
						"Close the running instance of Eraser and start it again without " +
						"administrator privileges, or run the command again as an " +
						"administrator.", e);
				}
			}
			#endregion

			/// <summary>
			/// The prototype of an action handler in the class which executes an
			/// action as specified in the command line.
			/// </summary>
			private delegate void ActionHandler();

			/// <summary>
			/// Matches an action handler to a function in the class.
			/// </summary>
			private Dictionary<Type, ActionHandler> actionHandlers =
				new Dictionary<Type, ActionHandler>();
		}
	}
}
