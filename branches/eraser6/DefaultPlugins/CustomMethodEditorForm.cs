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
		}

		/// <summary>
		/// Saves the currently edited pass details to memory.
		/// </summary>
		private void SavePass()
		{
			if (passesLv.SelectedIndices.Count != 1)
				return;

			ListViewItem item = passesLv.SelectedItems[0];
			ErasureMethod.Pass pass = (ErasureMethod.Pass)item.Tag;
			if (passTypeRandom.Checked)
			{
				pass.Function = ErasureMethod.Pass.WriteRandom;
				pass.OpaqueValue = null;
				item.SubItems[1].Text = "Random Data";
			}
			else
			{
				SavePassConstant(passTypeHex.Checked);
				pass.Function = ErasureMethod.Pass.WriteConstant;
				pass.OpaqueValue = passConstant;
				item.SubItems[1].Text = string.Format("Constant ({0} bytes)", passConstant.Length);
			}
		}

		/// <summary>
		/// Holds the constant that will be written in the currently selected pass.
		/// </summary>
		private byte[] passConstant = null;

		/// <summary>
		/// Saves the pass constant currently in the pass constant text field.
		/// </summary>
		/// <param name="parseHex">Parse the constant in the field as a string of
		/// hexadecimal numbers.</param>
		private void SavePassConstant(bool parseHex)
		{
			if (parseHex)
			{
				List<byte> passConstantList = new List<byte>();
				string str = passTxt.Text.Replace(" ", "").ToUpper();
				for (int i = 0, j = str.Length; i < j; i += 2)
					passConstantList.Add(Convert.ToByte(str.Substring(i, 2), 16));
				if (str.Length % 2 == 1)
					passConstantList.Add(Convert.ToByte(str.Substring(str.Length - 1), 16));

				passConstant = new byte[passConstantList.Count];
				passConstantList.CopyTo(passConstant);
			}
			else
			{
				passConstant = Encoding.UTF8.GetBytes(passTxt.Text);
			}
		}

		/// <summary>
		/// Displays the pass constant stored by the SavePassConstant function.
		/// </summary>
		/// <param name="displayHex">Displays the buffer as a list of bytes in
		/// hexadecimal if true, parses as UTF8 string otherwise.</param>
		private void DisplayPassConstant(bool displayHex)
		{
			if (displayHex)
			{
				StringBuilder displayText = new StringBuilder();
				foreach (byte b in passConstant)
					displayText.Append(string.Format("{0,2} ", Convert.ToString(b, 16)));

				passTxt.Text = displayText.ToString();
			}
			else
			{
				passTxt.Text = Encoding.UTF8.GetString(passConstant);
			}
		}

		/// <summary>
		/// Renumbers all pass entries' pass number to be in sync with its position.
		/// </summary>
		private void RenumberPasses()
		{
			foreach (ListViewItem item in passesLv.Items)
				item.Text = (item.Index + 1).ToString();
		}

		private void passesAddBtn_Click(object sender, EventArgs e)
		{
			ListViewItem item = new ListViewItem((passesLv.Items.Count + 1).ToString());
			item.SubItems.Add("Random Data");

			ErasureMethod.Pass pass = new ErasureMethod.Pass(ErasureMethod.Pass.WriteRandom, null);
			item.Tag = pass;

			passesLv.Items.Add(item);
		}

		private void passesRemoveBtn_Click(object sender, EventArgs e)
		{
			foreach (int index in passesLv.SelectedIndices)
				passesLv.Items.RemoveAt(index);

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
		}

		private void passesLv_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			ErasureMethod.Pass pass = (ErasureMethod.Pass)e.Item.Tag;

			//Enable or disable the pass edit controls and the pass list modifiers
			passesRemoveBtn.Enabled = passesDuplicateBtn.Enabled = passesMoveUpBtn.Enabled =
				passesMoveDownBtn.Enabled = passGrp.Enabled = passTypeText.Enabled =
				passTypeHex.Enabled = passTypeRandom.Enabled = passTxt.Enabled =
				passesLv.SelectedItems.Count == 1;

			//Determine if we should load or save the pass information
			if (!e.Item.Selected)
			{
				SavePass();
			}
			else
			{
				try
				{
					//Disable the pass display events
					passTypeText.CheckedChanged -= new System.EventHandler(passType_CheckedChanged);
					passTypeHex.CheckedChanged -= new System.EventHandler(passType_CheckedChanged);
					passTypeRandom.CheckedChanged -= new System.EventHandler(passType_CheckedChanged);

					//Get the pass data from the method structure.
					passTxt.Text = string.Empty;
					passConstant = (byte[])pass.OpaqueValue;

					//Set the pass type to be undefined, to be set later.
					passTypeText.Checked = passTypeHex.Checked = passTypeRandom.Checked = false;
				}
				finally
				{
					//Reenable the pass display events
					passTypeText.CheckedChanged += new System.EventHandler(passType_CheckedChanged);
					passTypeHex.CheckedChanged += new System.EventHandler(passType_CheckedChanged);
					passTypeRandom.CheckedChanged += new System.EventHandler(passType_CheckedChanged);
				}

				//Set the pass type
				if (pass.Function == ErasureMethod.Pass.WriteRandom)
					passTypeRandom.Checked = true;
				else if (pass.Function == ErasureMethod.Pass.WriteConstant)
				{
					Encoding coder = (Encoding)Encoding.UTF8.Clone();
					coder.DecoderFallback = new DecoderExceptionFallback();
					try
					{
						coder.GetString(passConstant);
						passTypeText.Checked = true;
					}
					catch (DecoderFallbackException)
					{
						passTypeHex.Checked = true;
					}
				}
				else
					throw new ArgumentException("Unknown pass data.");
			}
		}

		private void passType_CheckedChanged(object sender, EventArgs e)
		{
			//Enable the text field if the selected pass type is not random data
			passTxt.Enabled = !passTypeRandom.Checked;

			//Copy or load the constant into the text field
			if (sender != passTypeRandom)
				if (!((RadioButton)sender).Checked)
					SavePassConstant(sender == passTypeHex);
				else if (passConstant != null && passConstant.Length != 0)
					DisplayPassConstant(sender == passTypeHex);
		}

		private void okBtn_Click(object sender, EventArgs e)
		{
			//Save the currently edited pass.
			SavePass();

			//Clear the errorProvider status
			errorProvider.Clear();
			bool hasError = false;

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