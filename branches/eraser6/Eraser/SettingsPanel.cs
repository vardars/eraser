using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Eraser.Manager;
using Eraser.Manager.Plugin;

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
			item.SubItems.Add(string.Empty);//item.SubItems.Add(i.Current.Version);
			item.SubItems.Add(instance.Path);
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
				if (((ErasureMethod)method).GUID == Globals.Settings.DefaultFileErasureMethod)
				{
					eraseFilesMethod.SelectedItem = method;
					break;
				}
			 
			foreach (Object method in eraseUnusedMethod.Items)
				if (((ErasureMethod)method).GUID == Globals.Settings.DefaultUnusedSpaceErasureMethod)
				{
					eraseUnusedMethod.SelectedItem = method;
					break;
				}

			foreach (Object prng in erasePRNG.Items)
				if (((PRNG)prng).GUID == Globals.Settings.ActivePRNG)
				{
					erasePRNG.SelectedItem = prng;
					break;
				}

			lockedAllow.Checked =
				Globals.Settings.AllowFilesToBeErasedOnRestart;
			lockedConfirm.Checked =
				Globals.Settings.ConfirmWithUserBeforeReschedulingErase;
			schedulerMissedImmediate.Checked =
				Globals.Settings.ExecuteMissedTasksImmediately;

			//After all the settings have been loaded, do a sanity check.
			//NotImplemented: This is a VERY crude way of getting the user to do things!
			//Select an intelligent default instead.
			if (eraseFilesMethod.SelectedIndex == -1)
				MessageBox.Show("The Default file erasure method is invalid, please set a valid default.");
			if (eraseUnusedMethod.SelectedIndex == -1)
				MessageBox.Show("The Default unused space erasure method is invalid, please set a valid default.");
			if (erasePRNG.SelectedIndex == -1)
				MessageBox.Show("The randomness data source is invalid, please set a valid source.");
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

			Globals.Settings.DefaultFileErasureMethod =
				((ErasureMethod)eraseFilesMethod.SelectedItem).GUID;
			Globals.Settings.DefaultUnusedSpaceErasureMethod =
				((ErasureMethod)eraseUnusedMethod.SelectedItem).GUID;

			PRNG newPRNG = (PRNG)erasePRNG.SelectedItem;
			if (newPRNG.GUID != Globals.Settings.ActivePRNG)
			{
				MessageBox.Show("The new randomness data source will only be used when " +
					"the next task is run.\nCurrently running tasks will use the old source.",
					"Eraser", MessageBoxButtons.OK, MessageBoxIcon.Information);
				Globals.Settings.ActivePRNG = newPRNG.GUID;
			}
			Globals.Settings.AllowFilesToBeErasedOnRestart =
				lockedAllow.Checked;
			Globals.Settings.ConfirmWithUserBeforeReschedulingErase =
				lockedConfirm.Checked;
			Globals.Settings.ExecuteMissedTasksImmediately =
				schedulerMissedImmediate.Checked;
		}
	}
}

