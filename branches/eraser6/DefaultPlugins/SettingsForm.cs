using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();

			//Populate the list of erasure passes, except the FL16KB.
			foreach (ErasureMethod method in ErasureMethodManager.GetAll().Values)
				if (method.GUID != new Guid("{0C2E07BF-0207-49a3-ADE8-46F9E1499C01}"))
					fl16MethodCmb.Items.Add(method);

			//Load the settings.
			Dictionary<string, object> settings = DefaultPlugin.Settings;
			if (settings.ContainsKey("FL16Method"))
			{
				Guid fl16Method = (Guid)settings["FL16Method"];
				foreach (object item in fl16MethodCmb.Items)
					if (((ErasureMethod)item).GUID == fl16Method)
					{
						fl16MethodCmb.SelectedItem = item;
						break;
					}
			}

			if (fl16MethodCmb.SelectedIndex == -1)
			{
				Guid defaultMethodGuid =
					ManagerLibrary.Instance.Settings.DefaultFileErasureMethod;
				foreach (object item in fl16MethodCmb.Items)
					if (((ErasureMethod)item).GUID == defaultMethodGuid)
					{
						fl16MethodCmb.SelectedItem = item;
						break;
					} 
			}
		}

		private void okBtn_Click(object sender, EventArgs e)
		{
			//Save the settings to the settings dictionary
			if (fl16MethodCmb.SelectedIndex == -1)
			{
				errorProvider.SetError(fl16MethodCmb, "An invalid erasure method was selected.");
				return;
			}

			DefaultPlugin.Settings["FL16Method"] = ((ErasureMethod)fl16MethodCmb.SelectedItem).GUID;

			//Close the dialog
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}