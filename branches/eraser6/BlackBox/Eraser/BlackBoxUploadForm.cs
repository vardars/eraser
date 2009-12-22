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
			for (int i = 0; i < reports.Count; ++i)
			{
				//Calculate the base progress percentage for this job.
				int progressPerReport = 100 / reports.Count;
				int baseProgress = i * progressPerReport;
				int stepsPerReport = 2;

				BlackBoxReportUploader uploader = new BlackBoxReportUploader(reports[i]);

				//Check that a similar report has not yet been uploaded.
				UploadWorker.ReportProgress(baseProgress,
					S._("Checking for status of report {0}...", reports[i].Name));
				if (!uploader.ReportIsNew())
					continue;
				
				//No similar reports have been uploaded. Compress the report.
				UploadWorker.ReportProgress(baseProgress,
					S._("Compressing Report {0}: {1}%", reports[i].Name, 0));
				uploader.Compress(delegate(object from, ProgressChangedEventArgs progress)
					{
						UploadWorker.ReportProgress(baseProgress +
							progress.ProgressPercentage * progressPerReport / 100 / stepsPerReport,
							S._("Compressing Report {0}: {1}%",
								reports[i].Name, progress.ProgressPercentage));
					});

				//Upload the report.
				UploadWorker.ReportProgress(baseProgress + progressPerReport / 2,
					S._("Uploading Report {0}: {1}%", reports[i].Name));
				uploader.Upload(delegate(object from, ProgressChangedEventArgs progress)
					{
						UploadWorker.ReportProgress(baseProgress + progressPerReport / stepsPerReport +
							progress.ProgressPercentage * progressPerReport / 100 / stepsPerReport,
							S._("Uploading Report {0}: {1}%",
								reports[i].Name, progress.ProgressPercentage));
					});
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

		private IList<BlackBoxReport> Reports;
	}

	class BlackBoxReportUploader
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="report">The report to upload.</param>
		public BlackBoxReportUploader(BlackBoxReport report)
		{
			Report = report;
			if (!Directory.Exists(UploadTempDir))
				Directory.CreateDirectory(UploadTempDir);

			ReportBaseName =  Path.Combine(UploadTempDir, Report.Name);
		}

		/// <summary>
		/// Verifies the stack trace against the server to see if the report is new.
		/// </summary>
		/// <returns>True if the report is new; false otherwise</returns>
		public bool ReportIsNew()
		{
			throw new NotImplementedException();
		}

		public void Compress(ProgressChangedEventHandler progressChanged)
		{
			using (FileStream archiveStream = new FileStream(ReportBaseName + ".tar",
					FileMode.Create, FileAccess.Write))
			{
				//Add the report into a tar file
				TarArchive archive = TarArchive.CreateOutputTarArchive(archiveStream);
				foreach (FileInfo file in Report.Files)
					archive.WriteEntry(TarEntry.CreateEntryFromFile(file.FullName), false);
				archive.Close();
			}

			using (FileStream bzipFile = new FileStream(ReportBaseName + ".tbz",
				FileMode.Create))
			using (FileStream tarStream = new FileStream(ReportBaseName + ".tar",
				FileMode.Open, FileAccess.Read, FileShare.Read, 262144, FileOptions.DeleteOnClose))
			using (BZip2OutputStream bzipStream = new BZip2OutputStream(bzipFile, 262144))
			{
				//Compress the tar file
				int lastRead = 0;
				byte[] buffer = new byte[524288];
				while ((lastRead = tarStream.Read(buffer, 0, buffer.Length)) != 0)
				{
					bzipStream.Write(buffer, 0, lastRead);
					progressChanged(this, new ProgressChangedEventArgs(
						(int)(tarStream.Position * 100 / tarStream.Length), null));
				}
			}
		}

		public void Upload(ProgressChangedEventHandler progressChanged)
		{
			using (FileStream bzipFile = new FileStream(ReportBaseName + ".tbz",
				FileMode.Open, FileAccess.Read, FileShare.Read, 131072, FileOptions.DeleteOnClose))
			using (Stream logFile = Report.DebugLog)
			{
				//Build the POST request
				MultipartFormDataBuilder builder = new MultipartFormDataBuilder();
				builder.AddPart(new FormFileField("CrashReport", "Report.tbz", bzipFile));
				builder.AddPart(new FormFileField("DebugLog", "Debug.log", logFile));

				//Upload the POST request
				Uri blackBoxServer = new Uri("http://eraser.heidi.ie/BlackBox/upload.php");
				WebRequest reportRequest = HttpWebRequest.Create(blackBoxServer);
				reportRequest.ContentType = "multipart/form-data; boundary=" + builder.Boundary;
				reportRequest.Method = "POST";
				using (Stream formStream = builder.Stream)
				{
					reportRequest.ContentLength = formStream.Length;
					using (Stream requestStream = reportRequest.GetRequestStream())
					{
						int lastRead = 0;
						byte[] buffer = new byte[524288];
						while ((lastRead = formStream.Read(buffer, 0, buffer.Length)) != 0)
						{
							requestStream.Write(buffer, 0, lastRead);
							progressChanged(this, new ProgressChangedEventArgs(
								(int)(formStream.Position * 100 / formStream.Length), null));
						}
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

		/// <summary>
		/// The path to where the temporary files are stored before uploading.
		/// </summary>
		private static readonly string UploadTempDir =
			Path.Combine(Path.GetTempPath(), "Eraser Crash Reports");

		/// <summary>
		/// The report being uploaded.
		/// </summary>
		private BlackBoxReport Report;

		/// <summary>
		/// The base name of the report.
		/// </summary>
		private readonly string ReportBaseName;
	}

	class MultipartFormDataBuilder
	{
		public MultipartFormDataBuilder()
		{
			FileName = Path.GetTempFileName();
		}

		public void AddPart(FormField field)
		{
			//Generate a random part boundary
			if (Boundary == null)
			{
				Random rand = new Random();
				for (int i = 0; i < 10 + rand.Next(16); ++i)
					Boundary += ' ' + rand.Next(62);
			}

			using (FileStream stream = new FileStream(FileName, FileMode.Open, FileAccess.Write,
				FileShare.Read))
			{
				StringBuilder currentBoundary = new StringBuilder();
				currentBoundary.AppendFormat("--{0}\r\n", Boundary);
				if (field is FormFileField)
				{
					currentBoundary.AppendFormat(
						"Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"\r\n",
						field.FieldName, ((FormFileField)field).FileName);
					currentBoundary.AppendLine("Content-Type: application/octet-stream");
				}
				else
				{
					currentBoundary.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n",
						field.FieldName);
				}
				
				currentBoundary.AppendLine();
				byte[] boundary = Encoding.UTF8.GetBytes(currentBoundary.ToString());
				stream.Write(boundary, 0, boundary.Length);
				
				int lastRead = 0;
				byte[] buffer = new byte[524288];
				while ((lastRead = field.Stream.Read(buffer, 0, buffer.Length)) != 0)
					stream.Write(buffer, 0, lastRead);

				currentBoundary = new StringBuilder();
				currentBoundary.AppendFormat("--{0}--\r\n", Boundary);
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

	class FormField
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="stream">The stream containing the field data.</param>
		public FormField(string fieldName, Stream stream)
		{
			FieldName = fieldName;
			Stream = stream;
		}

		/// <summary>
		/// The name of the field.
		/// </summary>
		public string FieldName;

		/// <summary>
		/// The stream containing the data for this field.
		/// </summary>
		public Stream Stream;
	}

	class FormFileField : FormField
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fieldName">The name of the form field.</param>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="stream">The stream containing the field data.</param>
		public FormFileField(string fieldName, string fileName, Stream stream)
			: base(fieldName, stream)
		{
			FileName = fileName;
		}

		/// <summary>
		/// The name of the file.
		/// </summary>
		public string FileName;
	}
}
