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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Eraser.Manager;
using Eraser.Manager.Plugin;
using Microsoft.Win32;
using System.Globalization;
using Eraser.Util;
using System.Threading;

namespace Eraser
{
	public partial class SettingsPanel : Eraser.BasePanel
	{
		public SettingsPanel()
		{
			InitializeComponent();
			Dock = DockStyle.None;

			//For new plugins, register the callback.
			Host.Instance.PluginLoaded += new Host.PluginLoadedFunction(OnNewPluginLoaded);
			ErasureMethodManager.MethodRegistered +=
				new ErasureMethodManager.MethodRegisteredFunction(OnMethodRegistered);

			//Load the values
			LoadPluginDependantValues();
			LoadSettings();
		}

		private void OnNewPluginLoaded(PluginInstance instance)
		{
			ListViewItem item = pluginsManager.Items.Add(instance.Plugin.Name);
			item.SubItems.Add(instance.Plugin.Author);
			item.SubItems.Add(instance.Assembly.GetName().Version.ToString());
			item.SubItems.Add(instance.Path);
			item.Tag = instance;
		}

		private void OnMethodRegistered(Guid guid)
		{
			ErasureMethod method = ErasureMethodManager.GetInstance(guid);
			eraseFilesMethod.Items.Add(method);
			if (method is UnusedSpaceErasureMethod)
				eraseUnusedMethod.Items.Add(method);
		}

		private void LoadPluginDependantValues()
		{
			//Load the list of plugins
			Host instance = Host.Instance;
			List<PluginInstance>.Enumerator i = instance.Plugins.GetEnumerator();
			while (i.MoveNext())
				OnNewPluginLoaded(i.Current);

			//Refresh the list of languages
			List<Language> languages = LanguageManager.GetAll();
			foreach (Language culture in languages)
				uiLanguage.Items.Add(culture);

			//Refresh the list of erasure methods
			Dictionary<Guid, ErasureMethod> methods = ErasureMethodManager.GetAll();
			foreach (ErasureMethod method in methods.Values)
			{
				eraseFilesMethod.Items.Add(method);
				if (method is UnusedSpaceErasureMethod)
					eraseUnusedMethod.Items.Add(method);
			}

			//Refresh the list of PRNGs
			Dictionary<Guid, PRNG> prngs = PRNGManager.GetAll();
			foreach (PRNG prng in prngs.Values)
				erasePRNG.Items.Add(prng);
		}

		private void LoadSettings()
		{
			foreach (Object lang in uiLanguage.Items)
				if (((Language)lang).Name == ManagerLibrary.Instance.Settings.UILanguage)
				{
					uiLanguage.SelectedItem = lang;
					break;
				}

			foreach (Object method in eraseFilesMethod.Items)
				if (((ErasureMethod)method).GUID == ManagerLibrary.Instance.Settings.DefaultFileErasureMethod)
				{
					eraseFilesMethod.SelectedItem = method;
					break;
				}
			 
			foreach (Object method in eraseUnusedMethod.Items)
				if (((ErasureMethod)method).GUID == ManagerLibrary.Instance.Settings.DefaultUnusedSpaceErasureMethod)
				{
					eraseUnusedMethod.SelectedItem = method;
					break;
				}

			foreach (Object prng in erasePRNG.Items)
				if (((PRNG)prng).GUID == ManagerLibrary.Instance.Settings.ActivePRNG)
				{
					erasePRNG.SelectedItem = prng;
					break;
				}

			foreach (string path in ManagerLibrary.Instance.Settings.PlausibleDeniabilityFiles)
				plausibleDeniabilityFiles.Items.Add(path);

			lockedAllow.Checked =
				ManagerLibrary.Instance.Settings.EraseLockedFilesOnRestart;
			lockedConfirm.Checked =
				ManagerLibrary.Instance.Settings.ConfirmEraseOnRestart;
			lockedAllow_CheckedChanged(lockedAllow, new EventArgs());
			schedulerMissedImmediate.Checked =
				ManagerLibrary.Instance.Settings.ExecuteMissedTasksImmediately;
			schedulerMissedIgnore.Checked =
				!ManagerLibrary.Instance.Settings.ExecuteMissedTasksImmediately;
			plausibleDeniability.Checked =
				ManagerLibrary.Instance.Settings.PlausibleDeniability;
			plausibleDeniability_CheckedChanged(plausibleDeniability, new EventArgs());
			List<string> defaultsList = new List<string>();

			//Select an intelligent default if the settings are invalid.
			if (uiLanguage.SelectedIndex == -1)
			{
				defaultsList.Add(S._("User interface language"));
				foreach (Language lang in uiLanguage.Items)
					//TODO: This is an approximation - but that's the best I can think of at the moment
					if (((CultureInfo)lang).ThreeLetterISOLanguageName ==
						Thread.CurrentThread.CurrentUICulture.ThreeLetterISOLanguageName)
						uiLanguage.SelectedItem = lang;
			}
			if (eraseFilesMethod.SelectedIndex == -1)
			{
				if (eraseFilesMethod.Items.Count > 0)
				{
					eraseFilesMethod.SelectedIndex = 0;
					ManagerLibrary.Instance.Settings.DefaultFileErasureMethod =
						((ErasureMethod)eraseFilesMethod.SelectedItem).GUID;
				}
				defaultsList.Add(S._("Default file erasure method"));
			}
			if (eraseUnusedMethod.SelectedIndex == -1)
			{
				if (eraseUnusedMethod.Items.Count > 0)
				{
					eraseUnusedMethod.SelectedIndex = 0;
					ManagerLibrary.Instance.Settings.DefaultUnusedSpaceErasureMethod =
						((ErasureMethod)eraseUnusedMethod.SelectedItem).GUID;
				}
				defaultsList.Add(S._("Default unused space erasure method"));
			}
			if (erasePRNG.SelectedIndex == -1)
			{
				if (erasePRNG.Items.Count > 0)
				{
					erasePRNG.SelectedIndex = 0;
					ManagerLibrary.Instance.Settings.ActivePRNG =
						((PRNG)erasePRNG.SelectedItem).GUID;
				}
				defaultsList.Add(S._("Randomness data source"));
			}

			//Remind the user.
			if (defaultsList.Count != 0)
			{
				string defaults = string.Empty;
				foreach (string item in defaultsList)
					defaults += "\t" + item +"\n";
				MessageBox.Show(string.Format(S._("The following settings held invalid values:\n\n" +
					"{0}\nDefault settings are loaded.\n\n" +
					"Please, check that the new settings suit your required level of security."),
					defaults), S._("Eraser"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				saveSettings_Click(null, null);
			}
		}

		private void lockedAllow_CheckedChanged(object sender, EventArgs e)
		{
			lockedConfirm.Enabled = lockedAllow.Checked;
		}

		private void plausibleDeniability_CheckedChanged(object sender, EventArgs e)
		{
			plausibleDeniabilityFiles.Enabled = plausibleDeniabilityFilesAddFile.Enabled =
				plausibleDeniabilityFilesAddFolder.Enabled = plausibleDeniabilityFilesRemove.Enabled =
				plausibleDeniability.Checked;
		}

		private void plausibleDeniabilityFilesAddFile_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
				plausibleDeniabilityFiles.Items.AddRange(openFileDialog.FileNames);
		}

		private void plausibleDeniabilityFilesAddFolder_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				plausibleDeniabilityFiles.Items.Add(folderBrowserDialog.SelectedPath);
		}

		private void plausibleDeniabilityFilesRemove_Click(object sender, EventArgs e)
		{
			if (plausibleDeniabilityFiles.SelectedIndex != -1)
				plausibleDeniabilityFiles.Items.RemoveAt(plausibleDeniabilityFiles.SelectedIndex);
		}

		private void pluginsMenu_Opening(object sender, CancelEventArgs e)
		{
			if (pluginsManager.SelectedItems.Count == 1)
			{
				PluginInstance instance = (PluginInstance)pluginsManager.SelectedItems[0].Tag;
				e.Cancel = !instance.Plugin.Configurable;
			}
			else
				e.Cancel = true;
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pluginsManager.SelectedItems.Count != 1)
				return;
			
			PluginInstance instance = (PluginInstance)pluginsManager.SelectedItems[0].Tag;
			instance.Plugin.DisplaySettings(this);
		}

		private void saveSettings_Click(object sender, EventArgs e)
		{
			//Error checks first.
			errorProvider.Clear();
			if (uiLanguage.SelectedIndex == -1)
			{
				errorProvider.SetError(uiLanguage, S._("An invalid language was selected."));
				return;
			}
			else if (eraseFilesMethod.SelectedIndex == -1)
			{
				errorProvider.SetError(eraseFilesMethod, S._("An invalid file erasure method " +
					"was selected."));
				return;
			}
			else if (eraseUnusedMethod.SelectedIndex == -1)
			{
				errorProvider.SetError(eraseUnusedMethod, S._("An invalid unused disk space " +
					"erasure method was selected."));
				return;
			}
			else if (erasePRNG.SelectedIndex == -1)
			{
				errorProvider.SetError(erasePRNG, S._("An invalid randomness data " +
					"source was selected."));
				return;
			}
			else if (plausibleDeniability.Checked && plausibleDeniabilityFiles.Items.Count == 0)
			{
				errorProvider.SetError(plausibleDeniabilityFiles, S._("Erasures with plausible deniability " +
					"was selected, but no files were selected to be used as decoys."));
				errorProvider.SetIconPadding(plausibleDeniabilityFiles, -16);
				return;
			}

			ManagerLibrary.Instance.Settings.UILanguage =
				((Language)uiLanguage.SelectedItem).Name;
			ManagerLibrary.Instance.Settings.DefaultFileErasureMethod =
				((ErasureMethod)eraseFilesMethod.SelectedItem).GUID;
			ManagerLibrary.Instance.Settings.DefaultUnusedSpaceErasureMethod =
				((ErasureMethod)eraseUnusedMethod.SelectedItem).GUID;

			PRNG newPRNG = (PRNG)erasePRNG.SelectedItem;
			if (newPRNG.GUID != ManagerLibrary.Instance.Settings.ActivePRNG)
			{
				MessageBox.Show(S._("The new randomness data source will only be used when " +
					"the next task is run.\nCurrently running tasks will use the old source."),
					S._("Eraser"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				ManagerLibrary.Instance.Settings.ActivePRNG = newPRNG.GUID;
			}
			ManagerLibrary.Instance.Settings.EraseLockedFilesOnRestart =
				lockedAllow.Checked;
			ManagerLibrary.Instance.Settings.ConfirmEraseOnRestart =
				lockedConfirm.Checked;

			List<string> plausibleDeniabilityFilesList = new List<string>();
			foreach (string str in this.plausibleDeniabilityFiles.Items)
				plausibleDeniabilityFilesList.Add(str);
			ManagerLibrary.Instance.Settings.PlausibleDeniabilityFiles = plausibleDeniabilityFilesList;

			ManagerLibrary.Instance.Settings.ExecuteMissedTasksImmediately =
				schedulerMissedImmediate.Checked;
			ManagerLibrary.Instance.Settings.PlausibleDeniability =
				plausibleDeniability.Checked;
		}
	}
}

