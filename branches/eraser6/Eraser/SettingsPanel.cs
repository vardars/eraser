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

namespace Eraser
{
	public partial class SettingsPanel : Eraser.BasePanel
	{
		public SettingsPanel()
		{
			InitializeComponent();
			Dock = DockStyle.None;

			//For new plugins, register the callback.
			Host.Instance.PluginLoad += new Host.OnPluginLoadEventHandler(OnNewPluginLoaded);

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

		private void LoadPluginDependantValues()
		{
			//Load the list of plugins
			Host instance = Host.Instance;
			List<PluginInstance>.Enumerator i = instance.Plugins.GetEnumerator();
			while (i.MoveNext())
				OnNewPluginLoaded(i.Current);

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

			lockedAllow.Checked =
				ManagerLibrary.Instance.Settings.EraseLockedFilesOnRestart;
			lockedConfirm.Checked =
				ManagerLibrary.Instance.Settings.ConfirmEraseOnRestart;
			schedulerMissedImmediate.Checked =
				ManagerLibrary.Instance.Settings.ExecuteMissedTasksImmediately;
			schedulerMissedIgnore.Checked =
				!ManagerLibrary.Instance.Settings.ExecuteMissedTasksImmediately;
			plausibleDeniability.Checked =
				ManagerLibrary.Instance.Settings.PlausibleDeniability;

			//Select an intelligent default if the settings are invalid.
			string defaults = string.Empty;
			if (eraseFilesMethod.SelectedIndex == -1)
			{
				if (eraseFilesMethod.Items.Count > 0)
				{
					eraseFilesMethod.SelectedIndex = 0;
					ManagerLibrary.Instance.Settings.DefaultFileErasureMethod =
						((ErasureMethod)eraseFilesMethod.SelectedItem).GUID;
				}
				defaults += "\tDefault file erasure method\n";
			}
			if (eraseUnusedMethod.SelectedIndex == -1)
			{
				if (eraseUnusedMethod.Items.Count > 0)
				{
					eraseUnusedMethod.SelectedIndex = 0;
					ManagerLibrary.Instance.Settings.DefaultUnusedSpaceErasureMethod =
						((ErasureMethod)eraseUnusedMethod.SelectedItem).GUID;
				}
				defaults += "\tDefault unused space erasure method\n";
			}
			if (erasePRNG.SelectedIndex == -1)
			{
				if (erasePRNG.Items.Count > 0)
				{
					erasePRNG.SelectedIndex = 0;
					ManagerLibrary.Instance.Settings.ActivePRNG =
						((PRNG)erasePRNG.SelectedItem).GUID;
				}
				defaults += "\tRandomness data source\n";
			}

			//Remind the user.
			if (defaults.Length != 0)
			{
				MessageBox.Show(string.Format("The following settings held invalid values:\n\n" +
					"{0}\nThese settings have now been set to naive defaults.\n\n" +
					"Please check that the new settings suit your required level of security.",
					defaults), "Eraser", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				saveSettings_Click(null, null);
			}
		}

		private void lockedAllow_CheckedChanged(object sender, EventArgs e)
		{
			lockedConfirm.Enabled = lockedAllow.Checked;
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
			if (eraseFilesMethod.SelectedIndex == -1)
			{
				errorProvider.SetError(eraseFilesMethod, "An invalid file erasure method " +
					"was selected.");
				return;
			}
			else if (eraseUnusedMethod.SelectedIndex == -1)
			{
				errorProvider.SetError(eraseUnusedMethod, "An invalid unused disk space " +
					"erasure method was selected.");
				return;
			}
			else if (erasePRNG.SelectedIndex == -1)
			{
				errorProvider.SetError(erasePRNG, "An invalid randomness data " +
					"source was selected.");
				return;
			}

			ManagerLibrary.Instance.Settings.DefaultFileErasureMethod =
				((ErasureMethod)eraseFilesMethod.SelectedItem).GUID;
			ManagerLibrary.Instance.Settings.DefaultUnusedSpaceErasureMethod =
				((ErasureMethod)eraseUnusedMethod.SelectedItem).GUID;

			PRNG newPRNG = (PRNG)erasePRNG.SelectedItem;
			if (newPRNG.GUID != ManagerLibrary.Instance.Settings.ActivePRNG)
			{
				MessageBox.Show("The new randomness data source will only be used when " +
					"the next task is run.\nCurrently running tasks will use the old source.",
					"Eraser", MessageBoxButtons.OK, MessageBoxIcon.Information);
				ManagerLibrary.Instance.Settings.ActivePRNG = newPRNG.GUID;
			}
			ManagerLibrary.Instance.Settings.EraseLockedFilesOnRestart =
				lockedAllow.Checked;
			ManagerLibrary.Instance.Settings.ConfirmEraseOnRestart =
				lockedConfirm.Checked;
			ManagerLibrary.Instance.Settings.ExecuteMissedTasksImmediately =
				schedulerMissedImmediate.Checked;
			ManagerLibrary.Instance.Settings.PlausibleDeniability =
				plausibleDeniability.Checked;
		}
	}
}

