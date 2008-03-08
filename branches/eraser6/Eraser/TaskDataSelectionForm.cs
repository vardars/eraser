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
					unusedDisk.Items.Add(File.GetFileDescription(drive));
				}
			}
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

				freeSpaceTask.Drive = (string)unusedDisk.SelectedValue;
			}

			return result;
		}

		private void data_CheckedChanged(object sender, EventArgs e)
		{
			filePath.Enabled = fileBrowse.Enabled = file.Checked;
			folderPath.Enabled = folderBrowse.Enabled = folderIncludeLbl.Enabled =
				folderInclude.Enabled = folderExcludeLbl.Enabled = folderExclude.Enabled =
				folderDelete.Enabled = folder.Checked;
			unusedDisk.Enabled = unused.Checked;
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

		private void ok_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}