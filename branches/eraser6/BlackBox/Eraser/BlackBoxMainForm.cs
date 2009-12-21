using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Eraser.Util;

namespace Eraser
{
	public partial class BlackBoxMainForm : Form
	{
		public BlackBoxMainForm()
		{
			InitializeComponent();
			UXThemeApi.UpdateControlTheme(this);

			foreach (string id in BlackBox.GetDumps())
				ReportsLb.Items.Add(id);
		}

		private void SubmitBtn_Click(object sender, EventArgs e)
		{
			Visible = false;
			using (BlackBoxUploadForm form = new BlackBoxUploadForm())
				form.ShowDialog();
			Close();
		}

		private void PostponeBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// The global BlackBox instance.
		/// </summary>
		private BlackBox BlackBox = BlackBox.Get();
	}
}
