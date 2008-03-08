using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Eraser.Manager;
using Eraser.Util;

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
			string[] drives = Environment.GetLogicalDrives();
			foreach (string drive in drives)
			{
				DriveTypes driveType = Drives.GetDriveType(drive);
				if (driveType != DriveTypes.DRIVE_UNKNOWN &&
					driveType != DriveTypes.DRIVE_NO_ROOT_DIR &&
					driveType != DriveTypes.DRIVE_CDROM &&
					driveType != DriveTypes.DRIVE_REMOTE)
				{
					DriveItem item = new DriveItem();
					item.Drive = drive.Substring(0, drive.Length - 1);
					item.Label = File.GetFileDescription(item.Drive);
					item.Icon = File.GetFileIcon(item.Drive);
					unusedDisk.Items.Add(item);
				}
			}

			if (unusedDisk.Items.Count != 0)
				unusedDisk.SelectedIndex = 0;

			//And the methods list
			List<EraseMethod> methods = EraseMethod.GetMethods();
			foreach (EraseMethod method in methods)
				this.method.Items.Add(method);
			if (this.method.Items.Count != 0)
				this.method.SelectedIndex = 0;
		}

		/// <summary>
		/// Retrieves the settings on the property page as the Eraser Manager API equivalent.
		/// </summary>
		/// <returns>An Eraser.Manager.Task.Data or Eraser.Manager.Task.FreeSpace object
		/// or any of its inherited classes, depending on the task selected</returns>
		public Task.EraseTarget GetTaskEntry()
		{
			Task.EraseTarget result = null;
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
				Task.FreeSpace freeSpaceTask = new Task.FreeSpace();
				result = freeSpaceTask;

				freeSpaceTask.Drive = (unusedDisk.SelectedItem as DriveItem).Drive;
			}

			result.Method = this.method.SelectedItem as EraseMethod;
			return result;
		}

		private void data_CheckedChanged(object sender, EventArgs e)
		{
			filePath.Enabled = fileBrowse.Enabled = file.Checked;
			folderPath.Enabled = folderBrowse.Enabled = folderIncludeLbl.Enabled =
				folderInclude.Enabled = folderExcludeLbl.Enabled = folderExclude.Enabled =
				folderDelete.Enabled = folder.Checked;
			unusedDisk.Enabled = unused.Checked;
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
				errorProvider.SetError(filePath, "Invalid file path");
			else if (folder.Checked && folderPath.Text.Length == 0)
				errorProvider.SetError(folderPath, "Invalid folder path");
			else
			{
				errorProvider.Clear();
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}