using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Eraser
{
	public partial class TaskDataSelectionForm : Form
	{
		public TaskDataSelectionForm()
		{
			InitializeComponent();
			file.Checked = true;
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
	}
}