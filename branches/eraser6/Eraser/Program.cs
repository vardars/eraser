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
