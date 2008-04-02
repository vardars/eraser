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
 * Foobar is distributed in the hope that it will be useful, but WITHOUT ANY
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

			if (DefaultPlugin.Settings.ContainsKey("EraseCustom"))
				customMethods = (Dictionary<Guid, CustomErasureMethod>)
					DefaultPlugin.Settings["EraseCustom"];
			else
				customMethods = new Dictionary<Guid, CustomErasureMethod>();
		}

		private void customMethodAdd_Click(object sender, EventArgs e)
		{
			CustomMethodEditorForm form = new CustomMethodEditorForm();
			if (form.ShowDialog() == DialogResult.OK)
			{
				CustomErasureMethod method = form.Method;
				addCustomMethods.Add(method);
				
				ListViewItem item = customMethod.Items.Add(method.Name);
				item.SubItems.Add(method.Passes.Length.ToString());
				item.Tag = method;
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

			//Save the list of custom erasure methods
			DefaultPlugin.Settings["EraseCustom"] = customMethods;

			//Update the Erasure method manager on the methods
			foreach (CustomErasureMethod method in addCustomMethods)
			{
				customMethods.Add(method.GUID, method);
				ErasureMethodManager.Register(new EraseCustom(method), new object[] { method });
			}
			
			//Remove the old methods.
			foreach (Guid guid in removeCustomMethods)
				ErasureMethodManager.Unregister(guid);

			//Close the dialog
			DialogResult = DialogResult.OK;
			Close();
		}

		private Dictionary<Guid, CustomErasureMethod> customMethods;
		private List<CustomErasureMethod> addCustomMethods = new List<CustomErasureMethod>();
		private List<Guid> removeCustomMethods = new List<Guid>();
	}
}