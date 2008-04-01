using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Eraser.Util;

namespace Eraser
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();
			S.TranslateControl(this);
		}

		private void AboutForm_Click(object sender, EventArgs e)
		{
			//This is the only component around, so when clicked, dismiss the
			//dialog.
			Close();
		}
	}
}