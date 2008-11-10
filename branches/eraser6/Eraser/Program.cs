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
				//Load the task list
				RegistryKey key = Application.UserAppDataRegistry;
				byte[] savedTaskList = (byte[])key.GetValue("TaskList", new byte[0]);
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
						MessageBox.Show(S._("Could not load task list. All task entries have " +
							"been lost."), S._("Eraser"), MessageBoxButtons.OK,
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
				eraserClient.Run();
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
			Load();
		}

		protected override void Load()
		{
			RegistryKey key = Application.UserAppDataRegistry;
			ActivePRNG = new Guid((string)
				key.GetValue("PRNG", Guid.Empty.ToString()));
			EraseLockedFilesOnRestart =
				(int)key.GetValue("EraseOnRestart", (object)1) != 0;
			ConfirmEraseOnRestart =
				(int)key.GetValue("ConfirmEraseOnRestart", (object)0) != 0;
			DefaultFileErasureMethod = new Guid((string)
				key.GetValue("DefaultFileErasureMethod", Guid.Empty.ToString()));
			DefaultUnusedSpaceErasureMethod = new Guid((string)
				key.GetValue("DefaultUnusedSpaceErasureMethod", Guid.Empty.ToString()));
			ExecuteMissedTasksImmediately =
				(int)key.GetValue("ExecuteMissedTasksImmediately", (object)1) != 0;
			PlausibleDeniability =
				(int)key.GetValue("PlausibleDeniability", (object)0) != 0;

			UILanguage = (string)key.GetValue("UILanguage", CultureInfo.CurrentUICulture.Name);
			System.Threading.Thread.CurrentThread.CurrentUICulture =
				new CultureInfo(UILanguage);

			//Load the plausible deniability files
			byte[] plausibleDeniabilityFiles = (byte[])
				key.GetValue("PlausibleDeniabilityFiles", new byte[0]);
			if (plausibleDeniabilityFiles.Length != 0)
				using (MemoryStream stream = new MemoryStream(plausibleDeniabilityFiles))
				{
					try
					{
						this.PlausibleDeniabilityFiles = (List<string>)
							new BinaryFormatter().Deserialize(stream);
					}
					catch (Exception)
					{
						key.DeleteValue("PlausibleDeniabilityFiles");
						MessageBox.Show(S._("Could not load list of files used for plausible " +
							"deniability.\n\nThe list of files have been lost."),
							S._("Eraser"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
		}

		protected override void Save()
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
			key.SetValue("PlausibleDeniability", PlausibleDeniability,
				RegistryValueKind.DWord);
			key.SetValue("UILanguage", UILanguage);

			using (MemoryStream stream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(stream, PlausibleDeniabilityFiles);
				key.SetValue("PlausibleDeniabilityFiles", stream.ToArray(), RegistryValueKind.Binary);
			}
		}

		protected override Dictionary<string, object> GetSettings(Guid guid)
		{
			//Open the registry key containing the settings
			RegistryKey pluginsKey = Application.UserAppDataRegistry.OpenSubKey(
				"Plugins\\" + guid.ToString(), true);
			Dictionary<string, object> result = new Dictionary<string, object>();

			//Load every key/value pair into the dictionary
			if (pluginsKey != null)
				foreach (string key in pluginsKey.GetValueNames())
				{
					byte[] currentSetting = (byte[])pluginsKey.GetValue(key, null);
					if (currentSetting.Length != 0)
						using (MemoryStream stream = new MemoryStream(currentSetting))
						{
							try
							{
								result[key] = new BinaryFormatter().Deserialize(stream);
							}
							catch (Exception)
							{
								pluginsKey.DeleteValue(key);
								MessageBox.Show(string.Format(S._("Could not load the setting {0} for plugin {1}." +
									"The setting has been lost."), key, guid.ToString()),
									S._("Eraser"), MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}
					else
						result[key] = null;
				}

			//We're done!
			return result;
		}

		protected override void SetSettings(Guid guid, Dictionary<string, object> settings)
		{
			//Open the registry key containing the settings
			RegistryKey pluginKey = Application.UserAppDataRegistry.OpenSubKey(
				"Plugins\\" + guid.ToString(), true);
			if (pluginKey == null)
				pluginKey = Application.UserAppDataRegistry.CreateSubKey("Plugins\\" + guid.ToString());

			foreach (string key in settings.Keys)
			{
				using (MemoryStream stream = new MemoryStream())
				{
					new BinaryFormatter().Serialize(stream, settings[key]);
					pluginKey.SetValue(key, stream.ToArray(), RegistryValueKind.Binary);
				}
			}
		}
	}
}
