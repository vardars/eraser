/* 
 * $Id$
 * Copyright 2008 The Eraser Project
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
using System.Net;
using System.Reflection;
using System.IO;
using System.Xml;
using Eraser.Util;
using System.Net.Cache;

namespace Eraser
{
	public partial class UpdateForm : Form
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public UpdateForm()
		{
			InitializeComponent();

			updates = new UpdateManager();
			updateListDownloader.RunWorkerAsync();
		}

		#region Update List retrieval
		/// <summary>
		/// Downloads and parses the list of updates available for this client.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void updateListDownloader_DoWork(object sender, DoWorkEventArgs e)
		{
			UpdateManager.ProgressEventFunction progressFunc = null;
			progressFunc = delegate(float itemProgress, float overallProgress, string status)
			{
				if (InvokeRequired)
				{
					Invoke(progressFunc, itemProgress, overallProgress, status);
					return;
				}

				updateListDownloader.ReportProgress((int)(overallProgress * 100));
				progressLbl.Text = status;
			};

			try
			{
				updates.OnProgressEvent += progressFunc;
				updates.GetUpdates();
			}
			catch (Exception ex)
			{
				e.Result = ex;
			}
			finally
			{
				updates.OnProgressEvent -= progressFunc;
			}
		}

		/// <summary>
		/// Called when progress has been made in the update list download.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void updateListDownloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressPb.Style = ProgressBarStyle.Continuous;
			progressPb.Value = e.ProgressPercentage;

			if (progressPb.Value == 100)
				progressProgressLbl.Text = S._("Processing update list...");
		}

		/// <summary>
		/// Displays the parsed updates on the updates list view, filtering and displaying
		/// only those relevant to the current system's architecture.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void updateListDownloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//The return result will normally be null unless there are errors during the download.
			if (e.Result != null)
			{
				MessageBox.Show(this, ((Exception)e.Result).Message, S._("Eraser"),
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			progressPanel.Visible = false;

			updatesPanel.Show();
			updatesPanel.Visible = true;
			updatesPanel.ResumeLayout();

			//Get a list of translatable categories (this will change as more categories
			//are added)
			Dictionary<string, string> updateCategories = new Dictionary<string, string>();
			updateCategories.Add("updates", S._("Updates"));
			updateCategories.Add("plugins", S._("Plugins"));

			//Only include those whose architecture is compatible with ours.
			List<string> compatibleArchs = new List<string>();
			{
				//any is always compatible.
				compatibleArchs.Add("any");

				KernelAPI.SYSTEM_INFO info = new KernelAPI.SYSTEM_INFO();
				KernelAPI.GetSystemInfo(out info);
				switch (info.processorArchitecture)
				{
					case KernelAPI.SYSTEM_INFO.ProcessorArchitecture.PROCESSOR_ARCHITECTURE_AMD64:
						compatibleArchs.Add("x64");
						break;

					case KernelAPI.SYSTEM_INFO.ProcessorArchitecture.PROCESSOR_ARCHITECTURE_IA64:
						compatibleArchs.Add("ia64");
						break;

					case KernelAPI.SYSTEM_INFO.ProcessorArchitecture.PROCESSOR_ARCHITECTURE_INTEL:
						compatibleArchs.Add("x86");
						break;
				}
			}

			foreach (string key in updates.Categories)
			{
				ListViewGroup group = new ListViewGroup(updateCategories.ContainsKey(key) ?
					updateCategories[key] : key);
				updatesLv.Groups.Add(group);

				foreach (UpdateManager.Update update in updates[key])
				{
					//Skip if this update won't work on our current architecture.
					if (compatibleArchs.IndexOf(update.Architecture) == -1)
						continue;

					ListViewItem item = new ListViewItem(update.Name);
					item.SubItems.Add(update.Version.ToString());
					item.SubItems.Add(update.Publisher);
					item.SubItems.Add(update.FileSize.ToString());

					item.Tag = update;
					item.Group = group;
					item.Checked = true;

					updatesLv.Items.Add(item);
				}
			}
		}
		#endregion

		#region Update downloader
		private void updatesBtn_Click(object sender, EventArgs e)
		{
			updatesPanel.Visible = false;
			downloadingPnl.Show();
			List<UpdateManager.Update> updatesToInstall =
				new List<UpdateManager.Update>();

			//Collect the items that need to be installed
			foreach (ListViewItem item in updatesLv.Items)
				if (item.Checked)
				{
					UpdateManager.Update upd = (UpdateManager.Update)item.Tag;
					ListViewItem lvItem = downloadingLv.Items.Add(item.Text);
					lvItem.SubItems.Add(upd.FileSize.ToString());
					lvItem.Tag = upd;

					updatesToInstall.Add((UpdateManager.Update)item.Tag);
				}

			//Then run the thread.
			downloader.RunWorkerAsync(updatesToInstall);
		}

		private void downloader_DoWork(object sender, DoWorkEventArgs e)
		{
			UpdateManager.ProgressEventFunction progressFunc = null;
			progressFunc = delegate(float itemProgress, float overallProgress, string status)
				{
					if (InvokeRequired)
					{
						Invoke(progressFunc, itemProgress, overallProgress, status);
						return;
					}

					downloadingItemLbl.Text = status;
					downloadingItemPb.Value = (int)(itemProgress * 100);
					downloadingOverallPb.Value = (int)(overallProgress * 100);

					long amountToDownload = 0;
					foreach (ListViewItem item in downloadingLv.Items)
						amountToDownload +=
							Convert.ToInt32(item.SubItems[1].Text);
				};

			try
			{
				updates.OnProgressEvent += progressFunc;
				updates.InstallUpdates((List<UpdateManager.Update>)e.Argument);
			}
			finally
			{
				updates.OnProgressEvent -= progressFunc;
			}
		}

		private void downloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{

		}

		private void downloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

		}
		#endregion

		/// <summary>
		/// The Update manager instance used by this form.
		/// </summary>
		UpdateManager updates;
	}

	public class UpdateManager
	{
		/// <summary>
		/// Represents an update available on the server.
		/// </summary>
		public struct Update
		{
			public string Name;
			public Version Version;
			public string Publisher;
			public string Architecture;
			public long FileSize;
			public string Link;
		}

		/// <summary>
		/// Retrieves the update list from the server.
		/// </summary>
		public void GetUpdates()
		{
			WebRequest.DefaultCachePolicy = new HttpRequestCachePolicy(
				HttpRequestCacheLevel.Refresh);
			HttpWebRequest req = (HttpWebRequest)
				WebRequest.Create("http://eraser.sourceforge.net/updates?action=listupdates&" +
					"version=" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
			
			using (WebResponse resp = req.GetResponse())
			using (Stream strm = resp.GetResponseStream())
			{
				//Download the response
				int bytesRead = 0;
				byte[] buffer = new byte[16384];
				List<byte> responseBuffer = new List<byte>();
				while ((bytesRead = strm.Read(buffer, 0, buffer.Length)) != 0)
				{
					byte[] tmpDest = new byte[bytesRead];
					Buffer.BlockCopy(buffer, 0, tmpDest, 0, bytesRead);
					responseBuffer.AddRange(tmpDest);

					float progress = responseBuffer.Count / (float)resp.ContentLength;
					OnProgress(progress, progress, string.Format(
						S._("{0} of {1} bytes downloaded"),
						responseBuffer.Count, resp.ContentLength));
				}

				//Parse it.
				using (MemoryStream mStrm = new MemoryStream(responseBuffer.ToArray()))
					ParseUpdateList(mStrm);
			}
		}

		/// <summary>
		/// Parses the list of updates provided by the server
		/// </summary>
		/// <param name="strm">The stream containing the XML data.</param>
		private void ParseUpdateList(Stream strm)
		{
			//Move the XmlReader to the root node
			updates.Clear();
			mirrors.Clear();
			XmlReader rdr = XmlReader.Create(strm);
			rdr.ReadToFollowing("updateList");

			//Read the descendants of the updateList node (which are categories,
			//except for the <mirrors> element)
			XmlReader categories = rdr.ReadSubtree();
			bool cont = categories.ReadToDescendant("updates");
			while (cont)
			{
				if (categories.NodeType == XmlNodeType.Element)
				{
					if (categories.Name == "mirrors")
					{
						Dictionary<string, string> mirrorsList =
							ParseMirror(categories.ReadSubtree());
						Dictionary<string, string>.Enumerator e = mirrorsList.GetEnumerator();
						while (e.MoveNext())
							this.mirrors.Add(e.Current.Key, e.Current.Value);
					}
					else
						updates.Add(categories.Name, ParseUpdateCategory(categories.ReadSubtree()));
				}

				cont = categories.Read();
			}
		}

		/// <summary>
		/// Parses a list of mirrors.
		/// </summary>
		/// <param name="rdr">The XML reader object representing the &lt;mirrors&gt; node</param>
		/// <returns>The list of mirrors defined by the element.</returns>
		private static Dictionary<string, string> ParseMirror(XmlReader rdr)
		{
			Dictionary<string, string> result = new Dictionary<string,string>();
			if (!rdr.ReadToDescendant("mirror"))
				return result;

			//Load every element.
			do
			{
				if (rdr.Name != "mirror")
					continue;

				result.Add(rdr.ReadElementContentAsString(), rdr.GetAttribute("location"));
			}
			while (rdr.ReadToNextSibling("mirror"));

			return result;
		}

		/// <summary>
		/// Parses a specific category and its assocaited updates.
		/// </summary>
		/// <param name="rdr">The XML reader object representing the element and its children.</param>
		/// <returns>A list of updates in the category.</returns>
		private static List<Update> ParseUpdateCategory(XmlReader rdr)
		{
			List<Update> result = new List<Update>();
			if (!rdr.ReadToDescendant("item"))
				return result;

			//Load every element.
			do
			{
				if (rdr.Name != "item")
					continue;

				Update update = new Update();
				update.Name = rdr.GetAttribute("name");
				update.Version = new Version(rdr.GetAttribute("version"));
				update.Publisher = rdr.GetAttribute("publisher");
				update.Architecture = rdr.GetAttribute("architecture");
				update.FileSize = Convert.ToInt64(rdr.GetAttribute("filesize"));
				update.Link = rdr.ReadElementContentAsString();

				result.Add(update);
			}
			while (rdr.ReadToNextSibling("item"));

			return result;
		}

		/// <summary>
		/// Downloads and installs the list of updates.
		/// </summary>
		/// <param name="updates">The updates to retrieve and install.</param>
		public void InstallUpdates(List<Update> updates)
		{
			string mirror = "http://eraser.sourceforge.net";
			//Create a folder to hold all our updates.
			DirectoryInfo tempDir = new DirectoryInfo(Path.GetTempPath());
			tempDir = tempDir.CreateSubdirectory("eraser" + Environment.TickCount.ToString());

			try
			{
				//Step 1: download those updates!
				int currUpdate = 0;
				Dictionary<string, Update> tempFilesMap = new Dictionary<string, Update>();
				foreach (Update update in updates)
				{
					try
					{
						//Decide on the URL to connect to. The Link of the update may
						//be a relative path (relative to the selected mirror) or an
						//absolute path (which we have no choice)
						Uri reqUri = new Uri(update.Link);
						if (!reqUri.IsAbsoluteUri)
							reqUri = new Uri(new Uri(mirror), update.Link);

						//Then grab the download.
						HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqUri);
						using (WebResponse resp = req.GetResponse())
						{
							byte[] tempBuffer = new byte[16384];
							string tempFilePath = Path.Combine(
								tempDir.FullName, string.Format("{0}-{1}", ++currUpdate,
								reqUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)));

							using (Stream strm = resp.GetResponseStream())
							using (FileStream tempStrm = new FileStream(tempFilePath, FileMode.CreateNew))
							using (BufferedStream bufStrm = new BufferedStream(tempStrm))
							{
								//Copy the information into the file stream
								int readBytes = 0;
								while ((readBytes = strm.Read(tempBuffer, 0, tempBuffer.Length)) != 0)
								{
									bufStrm.Write(tempBuffer, 0, readBytes);

									//Compute progress
									float itemProgress = tempStrm.Position / (float)resp.ContentLength;
									float overallProgress = (currUpdate - 1 + itemProgress) / updates.Count;
									OnProgress(itemProgress, overallProgress / 2,
										S._(string.Format("Downloading {0}", update.Name)));
								}
							}

							//Store the filename-to-update mapping
							tempFilesMap.Add(tempFilePath, update);
						}
					}
					catch (WebException e)
					{
						OnProgress(1.0f, (float)currUpdate / 2,
							S._(string.Format("Error downloading {0}: {1}", update.Name,
								e.Message)));
					}
				}

				//Step 2: Install them.
				Dictionary<string, Update>.KeyCollection files = tempFilesMap.Keys;
				int currItem = 0;
				foreach (string path in files)
				{
					OnProgress(0.0f, (float)currItem++ / 2 + 0.5f,
						S._(string.Format("Installing {0}", tempFilesMap[path].Name)));
					System.Diagnostics.Process.Start(path);
				}

				OnProgress(0.0f, 1.0f, S._("Complete."));
			}
			finally
			{
				//Clean up after ourselves
				tempDir.Delete(true);
			}
		}

		/// <summary>
		/// Prototype of the callback functions from this object.
		/// </summary>
		/// <param name="progress">A number, from 0 to 1, indcating the progress
		/// of the previous operation.</param>
		/// <param name="overallProgress">The progress of the operation, from 0 to 1</param>
		/// <param name="status">The status of the current operation.</param>
		public delegate void ProgressEventFunction(float progress, float overallProgress, string status);

		/// <summary>
		/// Called when the progress of the operation changes.
		/// </summary>
		public ProgressEventFunction OnProgressEvent;

		/// <summary>
		/// Helper function: invokes the OnProgressEvent delegate.
		/// </summary>
		/// <param name="itemProgress">The progress of the current item, from 0 to 1</param>
		/// <param name="overallProgress">The progress of the operation, from 0 to 1</param>
		/// <param name="status">The status of the current operation.</param>
		private void OnProgress(float itemProgress, float overallProgress, string status)
		{
			if (OnProgressEvent != null)
				OnProgressEvent(itemProgress, overallProgress, status);
		}

		/// <summary>
		/// Retrieves the categories available.
		/// </summary>
		public Dictionary<string, List<Update>>.KeyCollection Categories
		{
			get
			{
				return updates.Keys;
			}
		}

		/// <summary>
		/// Retrieves all updates available.
		/// </summary>
		public Dictionary<string, List<Update>> Updates
		{
			get
			{
				return updates;
			}
		}

		/// <summary>
		/// Retrieves the updates in the given category.
		/// </summary>
		/// <param name="key">The category to retrieve.</param>
		/// <returns>All updates in the given category.</returns>
		public List<Update> this[string key]
		{
			get
			{
				return updates[key];
			}
		}

		/// <summary>
		/// The list of mirrors to download updates from.
		/// </summary>
		private Dictionary<string, string> mirrors = new Dictionary<string, string>();

		/// <summary>
		/// The list of updates downloaded.
		/// </summary>
		private Dictionary<string, List<Update>> updates =
			new Dictionary<string, List<Update>>();
	}
}
