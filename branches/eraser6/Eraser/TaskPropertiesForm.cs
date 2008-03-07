using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
					;
				}
			}
		}
	}
}