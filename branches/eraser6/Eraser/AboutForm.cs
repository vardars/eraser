using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Eraser
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();
		}

		private void AboutForm_Click(object sender, EventArgs e)
		{
			//This is the only component around, so when clicked, dismiss the
			//dialog.
			Close();
		}
	}
}