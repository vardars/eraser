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
using System.IO;

using Eraser.Util;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.BZip2;

namespace Eraser
{
	public partial class BlackBoxUploadForm : Form
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reports">The list of reports to upload.</param>
		public BlackBoxUploadForm(IList<BlackBoxReport> reports)
		{
			InitializeComponent();
			UXThemeApi.UpdateControlTheme(this);

			Reports = reports;
			UploadWorker.RunWorkerAsync(reports);
		}

		private void UploadWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			IList<BlackBoxReport> reports = (IList<BlackBoxReport>)e.Argument;
			string uploadDir = Path.Combine(Path.GetTempPath(), "Eraser Crash Reports");
			if (!Directory.Exists(uploadDir))
				Directory.CreateDirectory(uploadDir);

			for (int i = 0; i < reports.Count; ++i)
			{
				//Generate the base name of the report.
				string reportBaseName = Path.Combine(uploadDir, reports[i].Name);

				//Calculate the base progress percentage for this job. Add values up to 10
				//to indicate report progress
				int baseProgress = i * 100 / reports.Count;
				int stepsPerReport = 100 / reports.Count;

				using (FileStream archiveStream = new FileStream(reportBaseName + ".tar",
					FileMode.Create, FileAccess.Write))
				{
					UploadWorker.ReportProgress(baseProgress,
						S._("Compressing Report {0}", reports[i].Name));

					//Add the report into a tar file
					TarArchive archive = TarArchive.CreateOutputTarArchive(archiveStream);
					foreach (FileInfo file in reports[i].Files)
						archive.WriteEntry(TarEntry.CreateEntryFromFile(file.FullName), false);
					archive.Close();
				}

				using (FileStream tarStream = new FileStream(reportBaseName + ".tar",
					FileMode.Open, FileAccess.Read))
				using (FileStream bzipFile = new FileStream(reportBaseName + ".tbz",
					FileMode.Create))
				using (BZip2OutputStream bzipStream = new BZip2OutputStream(bzipFile, 262144))
				{
					//Compress the tar file
					byte[] buffer = new byte[524288];
					int lastRead = 0;
					while ((lastRead = tarStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						bzipStream.Write(buffer, 0, lastRead);
						UploadWorker.ReportProgress(baseProgress +
							(int)(tarStream.Position * stepsPerReport / tarStream.Length / 2));
					}

					//Upload the file
					UploadWorker.ReportProgress(baseProgress,
						S._("Uploading Report {0}", reports[i].Name));
				}
			}
		}

		private void UploadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (e.UserState != null)
				ProgressLbl.Text = e.UserState as string;
			ProgressPb.Value = e.ProgressPercentage;
		}

		private void UploadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

		}

		private ICollection<BlackBoxReport> Reports;
	}
}
