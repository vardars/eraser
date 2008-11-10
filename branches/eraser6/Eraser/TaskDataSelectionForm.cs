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
using Eraser.Util;
using System.IO;

namespace Eraser
{
	public partial class TaskDataSelectionForm : Form
	{
		private class DriveItem
		{
			public override string ToString()
			{
				return Label;
			}

			public string Drive;
			public string Label;
			public Icon Icon;
		}

		public TaskDataSelectionForm()
		{
			//Create the UI
			InitializeComponent();
			file.Checked = true;

			//Populate the drives list
			List<VolumeInfo> volumes = VolumeInfo.GetVolumes();
			foreach (VolumeInfo volume in volumes)
			{
				DriveType driveType = volume.VolumeType;
				if (driveType != DriveType.Unknown &&
					driveType != DriveType.NoRootDirectory &&
					driveType != DriveType.CDRom &&
					driveType != DriveType.Network)
				{
					DriveItem item = new DriveItem();
					string volumePath = volume.IsMounted ?
						volume.MountPoints[0] : volume.VolumeID;
					item.Drive = volumePath.Substring(0, volumePath.Length - 1);
					item.Label = Eraser.Util.File.GetFileDescription(volumePath);
					item.Icon = Eraser.Util.File.GetFileIcon(volumePath);
					unusedDisk.Items.Add(item);
				}
			}

			if (unusedDisk.Items.Count != 0)
				unusedDisk.SelectedIndex = 0;

			//And the methods list
			Dictionary<Guid, ErasureMethod> methods = ErasureMethodManager.GetAll();
			this.method.Items.Add(ErasureMethodManager.Default);
			foreach (ErasureMethod method in methods.Values)
				this.method.Items.Add(method);
			if (this.method.Items.Count != 0)
				this.method.SelectedIndex = 0;
		}

		/// <summary>
		/// Retrieves the settings on the property page as the Eraser Manager API equivalent.
		/// </summary>
		/// <returns>An Eraser.Manager.Task.Data or Eraser.Manager.Task.UnusedSpace object
		/// or any of its inherited classes, depending on the task selected</returns>
		public Task.ErasureTarget Target
		{
			get
			{
				Task.ErasureTarget result = null;
				if (file.Checked)
				{
					Manager.Task.File fileTask = new Task.File();
					result = fileTask;

					fileTask.Path = filePath.Text;
				}
				else if (folder.Checked)
				{
					Manager.Task.Folder folderTask = new Task.Folder();
					result = folderTask;

					folderTask.Path = folderPath.Text;
					folderTask.IncludeMask = folderInclude.Text;
					folderTask.ExcludeMask = folderExclude.Text;
					folderTask.DeleteIfEmpty = folderDelete.Checked;
				}
				else
				{
					Task.UnusedSpace unusedSpaceTask = new Task.UnusedSpace();
					result = unusedSpaceTask;

					unusedSpaceTask.Drive = ((DriveItem)unusedDisk.SelectedItem).Drive;
					unusedSpaceTask.EraseClusterTips = unusedClusterTips.Checked;
				}

				result.Method = (ErasureMethod)this.method.SelectedItem;
				return result;
			}
			set
			{
				//Set the erasure method.
				if (value.MethodDefined)
				{
					foreach (object item in method.Items)
						if (((ErasureMethod)item).GUID == value.Method.GUID)
							method.SelectedItem = item;
				}
				else
					method.SelectedIndex = 0;

				//Then the data to be erased.
				if (value is Task.File)
				{
					file.Checked = true;
					filePath.Text = ((Task.File)value).Path;
				}
				else if (value is Task.Folder)
				{
					folder.Checked = true;
					Manager.Task.Folder folderTask = (Task.Folder)value;

					folderPath.Text = folderTask.Path;
					folderInclude.Text = folderTask.IncludeMask;
					folderExclude.Text = folderTask.ExcludeMask;
					folderDelete.Checked = folderTask.DeleteIfEmpty;
				}
				else if (value is Task.UnusedSpace)
				{
					unused.Checked = true;
					Task.UnusedSpace unusedSpaceTask = new Task.UnusedSpace();
					foreach (object item in unusedDisk.Items)
						if (((DriveItem)item).Drive == ((Task.UnusedSpace)value).Drive)
							unusedDisk.SelectedItem = item;
					unusedClusterTips.Checked = unusedSpaceTask.EraseClusterTips;
				}
				else
					throw new NotImplementedException("Unknown erasure target.");
			}
		}

		private void method_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!(method.SelectedItem is UnusedSpaceErasureMethod) &&
				method.SelectedItem != ErasureMethodManager.Default)
			{
				if (unused.Checked)
				{
					file.Checked = true;
					errorProvider.SetError(unused, S._("The erasure method selected does " +
						"not support unused disk space erasures."));
				}
				unused.Enabled = false;
			}
			else if (!unused.Enabled)
			{
				unused.Enabled = true;
				errorProvider.Clear();
			}
		}

		private void data_CheckedChanged(object sender, EventArgs e)
		{
			filePath.Enabled = fileBrowse.Enabled = file.Checked;
			folderPath.Enabled = folderBrowse.Enabled = folderIncludeLbl.Enabled =
				folderInclude.Enabled = folderExcludeLbl.Enabled = folderExclude.Enabled =
				folderDelete.Enabled = folder.Checked;
			unusedDisk.Enabled = unusedClusterTips.Enabled = unused.Checked;
			errorProvider.Clear();
		}

		private void fileBrowse_Click(object sender, EventArgs e)
		{
			fileDialog.FileName = filePath.Text;
			if (fileDialog.ShowDialog() == DialogResult.OK)
				filePath.Text = fileDialog.FileName;
		}

		private void folderBrowse_Click(object sender, EventArgs e)
		{
			folderDialog.SelectedPath = folderPath.Text;
			if (folderDialog.ShowDialog() == DialogResult.OK)
				folderPath.Text = folderDialog.SelectedPath;
		}

		private void unusedDisk_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index == -1)
				return;

			Graphics g = e.Graphics;
			DriveItem item = (DriveItem)unusedDisk.Items[e.Index];
			Color textColour = e.ForeColor;
			PointF textPos = e.Bounds.Location;
			textPos.X += item.Icon.Width + 4;
			textPos.Y += 2;

			//Set the text colour and background colour if the control is disabled
			if ((e.State & DrawItemState.Disabled) == 0)
				e.DrawBackground();
			else
			{
				g.FillRectangle(new SolidBrush(SystemColors.ButtonFace), e.Bounds);
				textColour = SystemColors.GrayText;
			}

			g.DrawIcon(item.Icon, e.Bounds.X + 2, e.Bounds.Y);
			g.DrawString(item.Label, e.Font, new SolidBrush(textColour), textPos);
			if ((e.State & DrawItemState.Focus) != 0)
				e.DrawFocusRectangle();
		}

		private void ok_Click(object sender, EventArgs e)
		{
			if (file.Checked && filePath.Text.Length == 0)
				errorProvider.SetError(filePath, S._("Invalid file path"));
			else if (folder.Checked && folderPath.Text.Length == 0)
				errorProvider.SetError(folderPath, S._("Invalid folder path"));
			else
			{
				errorProvider.Clear();
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}