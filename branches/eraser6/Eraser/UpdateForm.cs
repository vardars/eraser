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
			ControlBox = false;

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
			try
			{
				updates.OnProgressEvent += updateListDownloader_ProgressChanged;
				updates.GetUpdates();
			}
			finally
			{
				updates.OnProgressEvent -= updateListDownloader_ProgressChanged;
			}
		}

		/// <summary>
		/// Called when progress has been made in the update list download.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void updateListDownloader_ProgressChanged(object sender, UpdateManager.ProgressEventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new UpdateManager.ProgressEventFunction(updateListDownloader_ProgressChanged),
					sender, e);
				return;
			}

			progressPb.Style = ProgressBarStyle.Continuous;
			progressPb.Value = (int)(e.OverallProgressPercentage * 100);
			progressLbl.Text = e.Message;

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
			//The Error property will normally be null unless there are errors during the download.
			if (e.Error != null)
			{
				MessageBox.Show(this, e.Error.Message, S._("Eraser"),
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
				return;
			}

			ControlBox = true;
			progressPanel.Visible = false;
			updatesPanel.Show();

			//First list all available mirrors
			Dictionary<string, UpdateManager.Mirror>.Enumerator iter =
				updates.Mirrors.GetEnumerator();
			while (iter.MoveNext())
				updatesMirrorCmb.Items.Add(iter.Current.Value);
			updatesMirrorCmb.SelectedIndex = 0;

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
					uiUpdates.Add(update, new UpdateData(update, item));
				}
			}
		}
		#endregion

		#region Update downloader
		/// <summary>
		/// Handles the update checked event.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void updatesLv_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (selectedUpdates == -1 || updatesCount != updatesLv.Items.Count)
			{
				updatesCount = updatesLv.Items.Count;
				selectedUpdates = 0;
				foreach (ListViewItem item in updatesLv.Items)
					if (item.Checked)
						++selectedUpdates;
			}
			else
				selectedUpdates += e.Item.Checked ? 1 : -1;
			updatesBtn.Text = selectedUpdates == 0 ? S._("Close") : S._("Install");
		}

		/// <summary>
		/// Handles the Install button click; fetches and installs the updates selected.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void updatesBtn_Click(object sender, EventArgs e)
		{
			updatesPanel.Visible = false;
			downloadingPnl.Show();
			ControlBox = false;
			List<UpdateManager.Update> updatesToInstall =
				new List<UpdateManager.Update>();

			//Set the mirror
			updates.SelectedMirror = (UpdateManager.Mirror)
				updatesMirrorCmb.SelectedItem;

			//Collect the items that need to be installed
			foreach (ListViewItem item in updatesLv.Items)
				if (item.Checked)
				{
					item.Remove();
					item.SubItems.RemoveAt(1);
					item.SubItems.RemoveAt(1);
					downloadingLv.Items.Add(item);

					updatesToInstall.Add((UpdateManager.Update)item.Tag);
				}
				else
					uiUpdates.Remove((UpdateManager.Update)item.Tag);

			//Then run the thread if there are updates.
			if (updatesToInstall.Count > 0)
				downloader.RunWorkerAsync(updatesToInstall);
			else
				Close();
		}

		/// <summary>
		/// Background thread to do the downloading and installing of updates.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void downloader_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				updates.OnProgressEvent += downloader_ProgressChanged;
				object downloadedUpdates = updates.DownloadUpdates((List<UpdateManager.Update>)e.Argument);
				e.Result = downloadedUpdates;
			}
			finally
			{
				updates.OnProgressEvent -= downloader_ProgressChanged;
			}
		}

		/// <summary>
		/// Handles the download progress changed event.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void downloader_ProgressChanged(object sender, UpdateManager.ProgressEventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new UpdateManager.ProgressEventFunction(downloader_ProgressChanged),
					sender, e);
				return;
			}

			UpdateData update = uiUpdates[(UpdateManager.Update)e.UserState];
			int amountLeft = (int)((1 - e.ProgressPercentage) * update.Update.FileSize);

			if (e is UpdateManager.ProgressErrorEventArgs)
			{
				update.Error = ((UpdateManager.ProgressErrorEventArgs)e).Exception;
				update.LVItem.ImageIndex = 3;
				update.LVItem.SubItems[1].Text = S._("Error");
				update.LVItem.ToolTipText = update.Error.Message;
			}
			else
			{
				if (amountLeft == 0)
				{
					update.LVItem.ImageIndex = -1;
					update.LVItem.SubItems[1].Text = S._("Downloaded");
				}
				else
				{
					update.LVItem.ImageIndex = 0;
					update.LVItem.SubItems[1].Text = amountLeft.ToString();
				}
			}

			downloadingItemLbl.Text = e.Message;
			downloadingItemPb.Value = (int)(e.ProgressPercentage * 100);
			downloadingOverallPb.Value = (int)(e.OverallProgressPercentage * 100);

			long amountToDownload = 0;
			foreach (ListViewItem lvItem in downloadingLv.Items)
				try
				{
					amountToDownload +=
						Convert.ToInt32(lvItem.SubItems[1].Text);
				}
				catch (FormatException)
				{
				}

			downloadingOverallLbl.Text = string.Format(S._("Overall progress: {0} bytes left"),
				amountToDownload);
		}

		/// <summary>
		/// Handles the completion of updating event
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void downloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(this, e.Error.Message, S._("Eraser"), MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				Close();
				return;
			}

			downloadingPnl.Visible = false;
			installingPnl.Show();

			foreach (ListViewItem item in downloadingLv.Items)
			{
				item.Remove();
				installingLv.Items.Add(item);

				UpdateData update = uiUpdates[(UpdateManager.Update)item.Tag];
				if (update.Error == null)
					item.SubItems[1].Text = string.Empty;
				else
					item.SubItems[1].Text = string.Format(S._("Error: {0}"),
						update.Error.Message);
			}

			installer.RunWorkerAsync(e.Result);
		}
		#endregion

		#region Update installer
		/// <summary>
		/// Background thread to install downloaded updates
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void installer_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				updates.OnProgressEvent += installer_ProgressChanged;
				updates.InstallUpdates(e.Argument);
			}
			finally
			{
				updates.OnProgressEvent -= installer_ProgressChanged;
			}
		}

		/// <summary>
		/// Handles the progress events generated during update installation.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void installer_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new UpdateManager.ProgressEventFunction(installer_ProgressChanged),
					sender, e);
				return;
			}

			UpdateData update = uiUpdates[(UpdateManager.Update)e.UserState];
			if (e is UpdateManager.ProgressErrorEventArgs)
			{
				update.Error = ((UpdateManager.ProgressErrorEventArgs)e).Exception;
				update.LVItem.ImageIndex = 3;
				update.LVItem.SubItems[1].Text = string.Format(S._("Error: {0}"),
					update.Error.Message);
			}
			else
				switch (update.LVItem.ImageIndex)
				{
					case -1:
						update.LVItem.ImageIndex = 1;
						break;
					case 1:
						update.LVItem.ImageIndex = 2;
						break;
				}
		}

		/// <summary>
		/// Re-enables the close dialog button.
		/// </summary>
		/// <param name="sender">The object triggering this event/</param>
		/// <param name="e">Event argument.</param>
		private void installer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			ControlBox = true;
		}
		#endregion

		/// <summary>
		/// The Update manager instance used by this form.
		/// </summary>
		UpdateManager updates;

		/// <summary>
		/// Maps listview items to the UpdateManager.Update object.
		/// </summary>
		Dictionary<UpdateManager.Update, UpdateData> uiUpdates =
			new Dictionary<UpdateManager.Update, UpdateData>();

		/// <summary>
		/// Manages information associated with the update.
		/// </summary>
		private class UpdateData
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="update">The UpdateManager.Update object containing the
			/// internal representation of the update.</param>
			/// <param name="item">The ListViewItem used for the display of the
			/// update.</param>
			public UpdateData(UpdateManager.Update update, ListViewItem item)
			{
				Update = update;
				LVItem = item;
				Error = null;
			}

			/// <summary>
			/// The UpdateManager.Update object containing the internal representation
			/// of the update.
			/// </summary>
			public UpdateManager.Update Update;

			/// <summary>
			/// The ListViewItem used for the display of the update.
			/// </summary>
			public ListViewItem LVItem;

			/// <summary>
			/// The error raised when downloading/installing the update, if any. Null
			/// otherwise.
			/// </summary>
			public Exception Error;
		}

		/// <summary>
		/// The number of updates selected for download.
		/// </summary>
		private int selectedUpdates = -1;

		/// <summary>
		/// The number of updates present in the previous count, so the Selected
		/// Updates number can be deemed invalid.
		/// </summary>
		private int updatesCount = -1;
	}

	public class UpdateManager
	{
		/// <summary>
		/// Represents a download mirror.
		/// </summary>
		public struct Mirror
		{
			public Mirror(string location, string link)
			{
				Location = location;
				Link = link;
			}

			/// <summary>
			/// The location where the mirror is at.
			/// </summary>
			public string Location;

			/// <summary>
			/// The URL prefix to utilise the mirror.
			/// </summary>
			public string Link;

			public override string ToString()
			{
				return Location;
			}
		}

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
		/// Specialised progress event argument, containing message describing
		/// current action, and overall progress percentage.
		/// </summary>
		public class ProgressEventArgs : ProgressChangedEventArgs
		{
			public ProgressEventArgs(float progressPercentage, float overallPercentage,
				object userState, string message)
				: base((int)(progressPercentage * 100), userState)
			{
				this.progressPercentage = progressPercentage;
				this.overallProgressPercentage = overallPercentage;
				this.message = message;
			}

			/// <summary>
			/// Gets the asynchronous task progress percentage.
			/// </summary>
			public new float ProgressPercentage
			{
				get
				{
					return progressPercentage;
				}
			}

			/// <summary>
			/// Gets the asynchronous task overall progress percentage.
			/// </summary>
			public float OverallProgressPercentage
			{
				get
				{
					return overallProgressPercentage;
				}
			}

			/// <summary>
			/// Gets the message associated with the current task.
			/// </summary>
			public string Message
			{
				get
				{
					return message;
				}
			}

			float progressPercentage;
			float overallProgressPercentage;
			string message;
		}

		/// <summary>
		/// Extends the ProgressEventArgs further by allowing for the inclusion of
		/// an exception.
		/// </summary>
		public class ProgressErrorEventArgs : ProgressEventArgs
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="e">The base ProgressEventArgs object.</param>
			/// <param name="ex">The exception</param>
			public ProgressErrorEventArgs(ProgressEventArgs e, Exception ex)
				: base(e.ProgressPercentage, e.OverallProgressPercentage, e.UserState, e.Message)
			{
				this.exception = ex;
			}

			/// <summary>
			/// The exception associated with the progress event.
			/// </summary>
			public Exception Exception
			{
				get
				{
					return exception;
				}
			}

			private Exception exception;
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
					OnProgress(new ProgressEventArgs(progress, progress, null,
						string.Format(S._("{0} of {1} bytes downloaded"),
							responseBuffer.Count, resp.ContentLength)));
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
			bool cont = categories.ReadToDescendant("mirrors");
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
							this.mirrors.Add(e.Current.Key,
								new Mirror(e.Current.Value, e.Current.Key));
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
				if (rdr.NodeType != XmlNodeType.Element || rdr.Name != "mirror")
					continue;

				string location = rdr.GetAttribute("location");
				result.Add(rdr.ReadElementContentAsString(), location);
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
		/// Downloads the list of updates.
		/// </summary>
		/// <param name="updates">The updates to retrieve and install.</param>
		/// <returns>An opaque object for use with InstallUpdates.</returns>
		public object DownloadUpdates(List<Update> updates)
		{
			//Create a folder to hold all our updates.
			DirectoryInfo tempDir = new DirectoryInfo(Path.GetTempPath());
			tempDir = tempDir.CreateSubdirectory("eraser" + Environment.TickCount.ToString());

			int currUpdate = 0;
			Dictionary<string, Update> tempFilesMap = new Dictionary<string, Update>();
			foreach (Update update in updates)
			{
				try
				{
					//Decide on the URL to connect to. The Link of the update may
					//be a relative path (relative to the selected mirror) or an
					//absolute path (which we have no choice)
					Uri reqUri = null;
					if (Uri.IsWellFormedUriString(update.Link, UriKind.Absolute))
						reqUri = new Uri(update.Link);
					else
						reqUri = new Uri(new Uri(SelectedMirror.Link), update.Link);
					
					//Then grab the download.
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqUri);
					using (WebResponse resp = req.GetResponse())
					{
						byte[] tempBuffer = new byte[16384];
						string tempFilePath = Path.Combine(
							tempDir.FullName, string.Format("{0}-{1}", ++currUpdate,
							Path.GetFileName(reqUri.GetComponents(UriComponents.Path, UriFormat.Unescaped))));

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
								OnProgress(new ProgressEventArgs(itemProgress, overallProgress,
									update, string.Format(S._("Downloading: {0}"), update.Name)));
							}
						}

						//Store the filename-to-update mapping
						tempFilesMap.Add(tempFilePath, update);

						//Let the event handler know the download is complete.
						OnProgress(new ProgressEventArgs(1.0f, (float)currUpdate / updates.Count,
							update, string.Format(S._("Downloaded: {0}"), update.Name)));
					}
				}
				catch (Exception e)
				{
					OnProgress(new ProgressErrorEventArgs(new ProgressEventArgs(1.0f,
						(float)currUpdate / updates.Count, update,
							string.Format(S._("Error downloading {0}: {1}"), update.Name, e.Message)),
						e));
				}
			}

			return tempFilesMap;
		}

		public void InstallUpdates(object downloadObject)
		{
			Dictionary<string, Update> tempFiles = (Dictionary<string, Update>)downloadObject;
			Dictionary<string, Update>.KeyCollection files = tempFiles.Keys;
			int currItem = 0;

			try
			{
				foreach (string path in files)
				{
					Update item = tempFiles[path];
					float progress = (float)currItem++ / files.Count;
					OnProgress(new ProgressEventArgs(0.0f, progress,
						item, string.Format(S._("Installing {0}"), item.Name)));

					System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
					info.FileName = path;
					info.UseShellExecute = true;

					System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
					process.WaitForExit(Int32.MaxValue);
					if (process.ExitCode == 0)
						OnProgress(new ProgressEventArgs(1.0f, progress,
							item, string.Format(S._("Installed {0}"), item.Name)));
					else
						OnProgress(new ProgressErrorEventArgs(new ProgressEventArgs(1.0f,
							progress, item, string.Format(S._("Error installing {0}"), item.Name)),
							new Exception(string.Format(S._("The installer exited with an error code {0}"),
								process.ExitCode))));
				}
			}
			finally
			{
				//Clean up after ourselves
				foreach (string file in files)
				{
					DirectoryInfo tempDir = null;
					{
						FileInfo info = new FileInfo(file);
						tempDir = info.Directory;
					}

					tempDir.Delete(true);
				}
			}
		}

		/// <summary>
		/// Prototype of the callback functions from this object.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="arg">The ProgressEventArgs object holding information
		/// about the progress of the current operation.</param>
		public delegate void ProgressEventFunction(object sender, ProgressEventArgs arg);

		/// <summary>
		/// Called when the progress of the operation changes.
		/// </summary>
		public ProgressEventFunction OnProgressEvent;

		/// <summary>
		/// Helper function: invokes the OnProgressEvent delegate.
		/// </summary>
		/// <param name="arg">The ProgressEventArgs object holding information
		/// about the progress of the current operation.</param>
		private void OnProgress(ProgressEventArgs arg)
		{
			if (OnProgressEvent != null)
				OnProgressEvent(this, arg);
		}

		/// <summary>
		/// Retrieves the list of mirrors which the server has indicated to exist.
		/// </summary>
		public Dictionary<string, Mirror> Mirrors
		{
			get
			{
				return mirrors;
			}
		}

		/// <summary>
		/// Gets or sets the active mirror to use to download mirrored updates.
		/// </summary>
		public Mirror SelectedMirror
		{
			get
			{
				if (selectedMirror.Link.Length == 0)
				{
					Dictionary<string, Mirror>.Enumerator iter = mirrors.GetEnumerator();
					if (iter.MoveNext())
						return iter.Current.Value;
				}
				return selectedMirror;
			}
			set
			{
				foreach (Mirror mirror in Mirrors.Values)
					if (mirror.Equals(value))
					{
						selectedMirror = value;
						return;
					}

				throw new IndexOutOfRangeException();
			}
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
		private Dictionary<string, Mirror> mirrors =
			new Dictionary<string, Mirror>();

		/// <summary>
		/// The currently selected mirror.
		/// </summary>
		private Mirror selectedMirror;

		/// <summary>
		/// The list of updates downloaded.
		/// </summary>
		private Dictionary<string, List<Update>> updates =
			new Dictionary<string, List<Update>>();
	}
}
