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
using System.Net;

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
				int progressPerReport = 100 / reports.Count;
				int stepsPerReport = 2;

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

				int lastRead = 0;
				byte[] buffer = new byte[524288];
				using (FileStream bzipFile = new FileStream(reportBaseName + ".tbz",
					FileMode.Create))
				using (FileStream tarStream = new FileStream(reportBaseName + ".tar",
					FileMode.Open, FileAccess.Read, FileShare.Read, 262144, FileOptions.DeleteOnClose))
				using (BZip2OutputStream bzipStream = new BZip2OutputStream(bzipFile, 262144))
				{
					//Compress the tar file
					while ((lastRead = tarStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						bzipStream.Write(buffer, 0, lastRead);
						UploadWorker.ReportProgress(baseProgress +
							(int)(tarStream.Position * progressPerReport / tarStream.Length /
								stepsPerReport));
					}
				}

				using (FileStream bzipFile = new FileStream(reportBaseName + ".tbz",
					FileMode.Open, FileAccess.Read, FileShare.Read, 131072, FileOptions.DeleteOnClose))
				{
					//Upload the file
					UploadWorker.ReportProgress(baseProgress + progressPerReport / 2,
						S._("Uploading Report {0}", reports[i].Name));
					MultipartFormDataBuilder builder = new MultipartFormDataBuilder();
					bzipFile.Position = 0;
					builder.AddPart("crashReport", "Report.tbz", bzipFile);

					WebRequest reportRequest = HttpWebRequest.Create("http://eraser.heidi.ie/BlackBox/upload.php");
					reportRequest.ContentType = "multipart/form-data; boundary=" + builder.Boundary;
					reportRequest.Method = "POST";
					using (Stream formStream = builder.Stream)
					using (Stream requestStream = reportRequest.GetRequestStream())
					{
						reportRequest.ContentLength = formStream.Length;
						while ((lastRead = formStream.Read(buffer, 0, buffer.Length)) != 0)
						{
							requestStream.Write(buffer, 0, lastRead);
							UploadWorker.ReportProgress(baseProgress + progressPerReport / stepsPerReport +
								(int)(formStream.Position * progressPerReport / formStream.Length / 2));
						}
					}

					HttpWebResponse response = reportRequest.GetResponse() as HttpWebResponse;
					if (response.StatusCode != HttpStatusCode.OK)
					{
						using (Stream responseStream = response.GetResponseStream())
						using (TextReader reader = new StreamReader(responseStream))
							throw new ArgumentException(reader.ReadToEnd());
					}
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
			if (e.Error != null)
				MessageBox.Show(e.Error.Message);
			CancelBtn.Text = S._("Close");
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			if (UploadWorker.IsBusy)
				UploadWorker.CancelAsync();
			else
				Close();
		}

		private ICollection<BlackBoxReport> Reports;
	}

	class MultipartFormDataBuilder
	{
		public MultipartFormDataBuilder()
		{
			FileName = Path.GetTempFileName();
		}

		public void AddPart(string fieldName, string fileName, Stream data)
		{
			//Generate a random part boundary
			if (Boundary == null)
			{
				Random rand = new Random();
				Boundary = "--";
				for (int i = 0; i < 10 + rand.Next(16); ++i)
					Boundary += ' ' + rand.Next(62);
			}

			using (FileStream stream = new FileStream(FileName, FileMode.Open, FileAccess.Write,
				FileShare.Read))
			{
				StringBuilder currentBoundary = new StringBuilder();
				currentBoundary.AppendFormat("{0}\r\n", Boundary);
				currentBoundary.AppendFormat("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"\r\n",
					fieldName, fileName);
				currentBoundary.AppendLine("Content-Type: binary");
				currentBoundary.AppendLine();
				byte[] boundary = Encoding.UTF8.GetBytes(currentBoundary.ToString());
				stream.Write(boundary, 0, boundary.Length);
				
				int lastRead = 0;
				byte[] buffer = new byte[524288];
				while ((lastRead = data.Read(buffer, 0, buffer.Length)) != 0)
					stream.Write(buffer, 0, lastRead);

				currentBoundary = new StringBuilder();
				currentBoundary.AppendFormat("{0}--\r\n", Boundary);
				boundary = Encoding.UTF8.GetBytes(currentBoundary.ToString());
				stream.Write(boundary, 0, boundary.Length);
			}
		}

		/// <summary>
		/// Gets a stream with which to read the data from.
		/// </summary>
		public Stream Stream
		{
			get
			{
				return new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
		}

		/// <summary>
		/// The Multipart/Form-Data boundary in use. If this is NULL, WritePostData will generate one
		/// and store it here.
		/// </summary>
		public string Boundary
		{
			get;
			set;
		}

		private string FileName;
	}
}
