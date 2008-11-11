/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By:	Kasra Nassiri <cjax@users.sourceforge.net> @10-11-2008 04:18:04
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
using Eraser.Util;

namespace Eraser.DefaultPlugins
{
	public partial class CustomMethodEditorForm : Form
	{
		public CustomMethodEditorForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Holds the CustomErasureMethod object which client code may set to allow
		/// method editing.
		/// </summary>
		private CustomErasureMethod method;

		/// <summary>
		/// Sets or retrieves the CustomErasureMethod object with all the values
		/// in the dialog.
		/// </summary>
		public CustomErasureMethod Method
		{
			get
			{
				if (method == null)
				{
					method = new CustomErasureMethod();
					method.GUID = Guid.NewGuid();
				}

				//The method name.
				method.Name = nameTxt.Text;

				//Whether passes can be randomized when executing.
				method.RandomizePasses = randomizeChk.Checked;

				//And all the passes.
				ErasureMethod.Pass[] passes = new ErasureMethod.Pass[passesLv.Items.Count];
				for (int i = 0; i < passesLv.Items.Count; ++i)
					passes[i] = (ErasureMethod.Pass)passesLv.Items[i].Tag;
				method.Passes = passes;

				return method;
			}
			set
			{
				method = value;

				//Method name.
				nameTxt.Text = method.Name;

				//Randomize passes
				randomizeChk.Checked = method.RandomizePasses;

				//Every pass.
				foreach (ErasureMethod.Pass pass in method.Passes)
					AddPass(pass);
			}
		}

		/// <summary>
		/// Adds the given pass to the displayed list of passes.
		/// </summary>
		/// <param name="pass">The pass to add.</param>
		/// <returns>The item added to the list view.</returns>
		private ListViewItem AddPass(ErasureMethod.Pass pass)
		{
			ListViewItem item = new ListViewItem((passesLv.Items.Count + 1).ToString());
			item.Tag = pass;
			if (pass.Function == ErasureMethod.Pass.WriteRandom)
				item.SubItems.Add("Random Data");
			else
				item.SubItems.Add(string.Format("Constant ({0} bytes)", ((byte[])pass.OpaqueValue).Length));

			passesLv.Items.Add(item);
			return item;
		}

		/// <summary>
		/// Saves the currently edited pass details to memory.
		/// </summary>
		private void SavePass(ListViewItem item)
		{
			ErasureMethod.Pass pass = (ErasureMethod.Pass)item.Tag;
			if (passTypeRandom.Checked)
			{
				pass.Function = ErasureMethod.Pass.WriteRandom;
				pass.OpaqueValue = null;
				item.SubItems[1].Text = "Random Data";
			}
			else
			{
				byte[] passConstant = ParseConstantStr(passTxt.Text, passTypeHex.Checked);
				pass.Function = ErasureMethod.Pass.WriteConstant;
				pass.OpaqueValue = passConstant;
				item.SubItems[1].Text = string.Format("Constant ({0} bytes)", passConstant.Length);
			}
		}

		/// <summary>
		/// Saves the pass constant currently in the pass constant text field.
		/// </summary>
		/// <param name="text">The text to parse.</param>
		/// <param name="parseHex">Parse the constant in the field as a string of
		/// hexadecimal numbers.</param>
		/// <returns>An array containing the byte-wise representation of the input
		/// string.</returns>
		private static byte[] ParseConstantStr(string text, bool parseHex)
		{
			if (parseHex)
			{
				List<byte> passConstantList = new List<byte>();
				string str = text.Replace(" ", "").ToUpper();

				for (int i = 0, j = str.Length - 2; i < j; i += 2)
					passConstantList.Add(Convert.ToByte(str.Substring(i, 2), 16));
				passConstantList.Add(Convert.ToByte(str.Substring(str.Length - 2), 16));

				byte[] result = new byte[passConstantList.Count];
				passConstantList.CopyTo(result);
				return result;
			}
			else
			{
				return Encoding.UTF8.GetBytes(text);
			}
		}

		/// <summary>
		/// Displays the pass constant stored by the SavePassConstant function.
		/// </summary>
		/// <param name="array">The array containing the constant to display.</param>
		/// <param name="asHex">Sets whether the array should be displayed as a
		/// hexadecimal string.</param>
		/// <returns>A string containing the user-visible representation of the
		/// input array.</returns>
		private static string DisplayConstantArray(byte[] array, bool asHex)
		{
			if (asHex)
			{
				StringBuilder displayText = new StringBuilder();
				foreach (byte b in array)
					displayText.Append(string.Format("{0,2} ", Convert.ToString(b, 16)));
				return displayText.ToString();
			}

			return Encoding.UTF8.GetString(array);
		}

		/// <summary>
		/// Renumbers all pass entries' pass number to be in sync with its position.
		/// </summary>
		private void RenumberPasses()
		{
			foreach (ListViewItem item in passesLv.Items)
				item.Text = (item.Index + 1).ToString();
		}

		/// <summary>
		/// Enables buttons relevant to the currently selected items.
		/// </summary>
		private void EnableButtons()
		{
			passesRemoveBtn.Enabled = passesDuplicateBtn.Enabled = passesMoveUpBtn.Enabled =
				passesMoveDownBtn.Enabled = passesLv.SelectedItems.Count >= 1;
			passGrp.Enabled = passTypeText.Enabled = passTypeHex.Enabled =
				passTypeRandom.Enabled = passTxt.Enabled =
				passesLv.SelectedItems.Count == 1;

			ListView.SelectedIndexCollection indices = passesLv.SelectedIndices;
			if (indices.Count > 0)
			{
				int index = indices[indices.Count - 1];
				passesMoveUpBtn.Enabled = passesMoveUpBtn.Enabled && index > 0;
				passesMoveDownBtn.Enabled = passesMoveDownBtn.Enabled && index < passesLv.Items.Count - 1;
			}
		}

		private void passesAddBtn_Click(object sender, EventArgs e)
		{
			ErasureMethod.Pass pass = new ErasureMethod.Pass(ErasureMethod.Pass.WriteRandom, null);
			ListViewItem item = AddPass(pass);

			if (passesLv.SelectedIndices.Count > 0)
			{
				item.Remove();
				passesLv.Items.Insert(passesLv.SelectedIndices[passesLv.SelectedIndices.Count - 1],
					item);
				RenumberPasses();
			}
		}

		private void passesRemoveBtn_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in passesLv.SelectedItems)
				passesLv.Items.Remove(item);

			RenumberPasses();
		}

		private void passesDuplicateBtn_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem index in passesLv.SelectedItems)
			{
				ListViewItem item = (ListViewItem)index.Clone();
				ErasureMethod.Pass pass = (ErasureMethod.Pass)item.Tag;
				item.Text = (passesLv.Items.Count + 1).ToString();
				item.Tag = new ErasureMethod.Pass(pass.Function, pass.OpaqueValue);
				passesLv.Items.Add(item);
			}
		}

		private void passesMoveUpBtn_Click(object sender, EventArgs e)
		{
			//Get the selection index
			int selectedIndex = passesLv.SelectedIndices[0];
			if (selectedIndex == 0)
				return;

			//Insert the current item into the index before.
			ListViewItem item = passesLv.Items[selectedIndex];
			passesLv.Items.RemoveAt(selectedIndex);
			passesLv.Items.Insert(selectedIndex - 1, item);
			RenumberPasses();
			EnableButtons();
		}

		private void passesMoveDownBtn_Click(object sender, EventArgs e)
		{
			//Get the selection index
			int selectedIndex = passesLv.SelectedIndices[0];
			if (selectedIndex == passesLv.Items.Count - 1)
				return;

			//Insert the current item into the index after.
			ListViewItem item = passesLv.Items[selectedIndex];
			passesLv.Items.RemoveAt(selectedIndex);
			passesLv.Items.Insert(selectedIndex + 1, item);
			RenumberPasses();
			EnableButtons();
		}

		private void passesLv_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			ErasureMethod.Pass pass = (ErasureMethod.Pass)e.Item.Tag;
			EnableButtons();

			//Determine if we should load or save the pass information
			if (!e.Item.Selected)
			{
				if (passesLv.SelectedIndices.Count == 0 && passTxt.Text.Length > 0)
					SavePass(e.Item);
			}
			else
			{
				try
				{
					//Disable the pass display events
					passTypeText.CheckedChanged -= new EventHandler(passType_CheckedChanged);
					passTypeHex.CheckedChanged -= new EventHandler(passType_CheckedChanged);
					passTypeRandom.CheckedChanged -= new EventHandler(passType_CheckedChanged);

					//Get the pass data from the method structure.
					passTxt.Text = string.Empty;

					//Set the pass type to be undefined, to be set later.
					passTypeText.Checked = passTypeHex.Checked = passTypeRandom.Checked = false;
				}
				finally
				{
					//Reenable the pass display events
					passTypeText.CheckedChanged += new EventHandler(passType_CheckedChanged);
					passTypeHex.CheckedChanged += new EventHandler(passType_CheckedChanged);
					passTypeRandom.CheckedChanged += new EventHandler(passType_CheckedChanged);
				}

				//Set the pass type
				if (pass.Function == ErasureMethod.Pass.WriteRandom)
					passTypeRandom.Checked = true;
				else if (pass.Function == ErasureMethod.Pass.WriteConstant)
				{
					passTypeHex.Checked = true;
					passTxt.Text = DisplayConstantArray((byte[])pass.OpaqueValue, true);
				}
				else
					throw new ArgumentException("Unknown pass data.");
			}

			//Blank the pass text if it is not editable
			if (!passTxt.Enabled)
				passTxt.Text = string.Empty;
		}

		private void passType_CheckedChanged(object sender, EventArgs e)
		{
			//Enable the text field if the selected pass type is not random data
			passTxt.Enabled = !passTypeRandom.Checked;

			//Copy or load the constant into the text field
			if (sender != passTypeRandom && !passTypeRandom.Checked)
			{
				byte[] constant = null;
				if (passTxt.Text.Length != 0)
					constant = ParseConstantStr(passTxt.Text, sender == passTypeHex);

				if (constant != null && constant.Length != 0)
					passTxt.Text = DisplayConstantArray(constant, passTypeHex.Checked);
			}
		}

		private void okBtn_Click(object sender, EventArgs e)
		{
			//Clear the errorProvider status
			errorProvider.Clear();
			bool hasError = false;

			//Save the currently edited pass.
			if (passesLv.SelectedItems.Count == 1)
				SavePass(passesLv.SelectedItems[0]);

			//Validate the information
			if (nameTxt.Text.Length == 0)
			{
				errorProvider.SetError(nameTxt, S._("The name of the custom method cannot be empty."));
				errorProvider.SetIconPadding(nameTxt, -16);
				hasError = true;
			}

			//Validate all passes
			if (passesLv.Items.Count == 0)
			{
				errorProvider.SetError(passesLv, S._("The method needs to have at least one pass " +
					"defined."));
				errorProvider.SetIconPadding(passesLv, -16);
				hasError = true;
			}

			foreach (ListViewItem item in passesLv.Items)
			{
				ErasureMethod.Pass pass = (ErasureMethod.Pass)item.Tag;
				if (pass.Function == ErasureMethod.Pass.WriteConstant &&
					(pass.OpaqueValue == null || ((byte[])pass.OpaqueValue).Length == 0))
				{
					//Select the pass causing the error.
					passesLv.SelectedIndices.Clear();
					passesLv.SelectedIndices.Add(passesLv.Items.IndexOf(item));

					//Highlight the error
					errorProvider.SetError(passTxt, S._("The pass is supposed to write a " +
						"constant to the files to be erased, but no constant was defined."));
					errorProvider.SetIconPadding(passTxt, -16);
					hasError = true;
					break;
				}
			}

			//If there are errors, don't close the dialog.
			if (!hasError)
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
