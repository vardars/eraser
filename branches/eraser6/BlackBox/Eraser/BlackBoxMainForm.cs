/* 
 * $Id$
 * Copyright 2008-2009 The Eraser Project
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
				if (report.Submitted)
					continue;

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
			BlackBoxUploadForm form = new BlackBoxUploadForm(selectedReports);
			form.Show();

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
