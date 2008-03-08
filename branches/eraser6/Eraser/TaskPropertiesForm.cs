using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Eraser.Manager;

namespace Eraser
{
	public partial class TaskPropertiesForm : Form
	{
		public TaskPropertiesForm()
		{
			InitializeComponent();
		}

		private void dataAdd_Click(object sender, EventArgs e)
		{
			using (TaskDataSelectionForm form = new TaskDataSelectionForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					Task.EraseTarget entry = form.GetTaskEntry();
					ListViewItem item = null;
					if (entry is Task.FilesystemObject)
						item = data.Items.Add((entry as Task.FilesystemObject).Path);
					else if (entry is Task.FreeSpace)
						data.Items.Add("Unused space on " + (entry as Task.FreeSpace).Drive);
					else
						throw new NotImplementedException("Unimplemented data erasure type.");
					item.SubItems[0].Text = entry.Method.Name;
					task.Entries.Add(entry);
				}
			}
		}

		private Task task = new Task();
	}
}