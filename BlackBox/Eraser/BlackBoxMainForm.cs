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

			ReportsLb.BeginUpdate();
			foreach (BlackBoxReport report in BlackBox.GetDumps())
			{
				ReportsLb.Items.Add(report);
				ReportsLb.SetItemChecked(ReportsLb.Items.Count - 1, true);
			}
			ReportsLb.EndUpdate();
		}

		private void SubmitBtn_Click(object sender, EventArgs e)
		{
			Visible = false;

			BlackBoxReport[] selectedReports = new BlackBoxReport[ReportsLb.CheckedItems.Count];
			ReportsLb.CheckedItems.CopyTo(selectedReports, 0);
			using (BlackBoxUploadForm form = new BlackBoxUploadForm(selectedReports))
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
