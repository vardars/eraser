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

namespace Eraser
{
	partial class TaskDataSelectionForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskDataSelectionForm));
			this.file = new System.Windows.Forms.RadioButton();
			this.folder = new System.Windows.Forms.RadioButton();
			this.unused = new System.Windows.Forms.RadioButton();
			this.filePath = new System.Windows.Forms.TextBox();
			this.fileBrowse = new System.Windows.Forms.Button();
			this.folderPath = new System.Windows.Forms.TextBox();
			this.folderBrowse = new System.Windows.Forms.Button();
			this.folderIncludeLbl = new System.Windows.Forms.Label();
			this.folderInclude = new System.Windows.Forms.TextBox();
			this.folderExcludeLbl = new System.Windows.Forms.Label();
			this.folderExclude = new System.Windows.Forms.TextBox();
			this.folderDelete = new System.Windows.Forms.CheckBox();
			this.unusedDisk = new System.Windows.Forms.ComboBox();
			this.methodLbl = new System.Windows.Forms.Label();
			this.method = new System.Windows.Forms.ComboBox();
			this.ok = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.fileDialog = new System.Windows.Forms.OpenFileDialog();
			this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.unusedClusterTips = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// file
			// 
			this.file.AccessibleDescription = null;
			this.file.AccessibleName = null;
			resources.ApplyResources(this.file, "file");
			this.file.BackgroundImage = null;
			this.errorProvider.SetError(this.file, resources.GetString("file.Error"));
			this.file.Font = null;
			this.errorProvider.SetIconAlignment(this.file, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("file.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.file, ((int)(resources.GetObject("file.IconPadding"))));
			this.file.Name = "file";
			this.file.TabStop = true;
			this.file.UseVisualStyleBackColor = true;
			this.file.CheckedChanged += new System.EventHandler(this.data_CheckedChanged);
			// 
			// folder
			// 
			this.folder.AccessibleDescription = null;
			this.folder.AccessibleName = null;
			resources.ApplyResources(this.folder, "folder");
			this.folder.BackgroundImage = null;
			this.errorProvider.SetError(this.folder, resources.GetString("folder.Error"));
			this.folder.Font = null;
			this.errorProvider.SetIconAlignment(this.folder, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("folder.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.folder, ((int)(resources.GetObject("folder.IconPadding"))));
			this.folder.Name = "folder";
			this.folder.TabStop = true;
			this.folder.UseVisualStyleBackColor = true;
			this.folder.CheckedChanged += new System.EventHandler(this.data_CheckedChanged);
			// 
			// unused
			// 
			this.unused.AccessibleDescription = null;
			this.unused.AccessibleName = null;
			resources.ApplyResources(this.unused, "unused");
			this.unused.BackgroundImage = null;
			this.errorProvider.SetError(this.unused, resources.GetString("unused.Error"));
			this.unused.Font = null;
			this.errorProvider.SetIconAlignment(this.unused, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("unused.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.unused, ((int)(resources.GetObject("unused.IconPadding"))));
			this.unused.Name = "unused";
			this.unused.TabStop = true;
			this.unused.UseVisualStyleBackColor = true;
			this.unused.CheckedChanged += new System.EventHandler(this.data_CheckedChanged);
			// 
			// filePath
			// 
			this.filePath.AccessibleDescription = null;
			this.filePath.AccessibleName = null;
			resources.ApplyResources(this.filePath, "filePath");
			this.filePath.BackgroundImage = null;
			this.errorProvider.SetError(this.filePath, resources.GetString("filePath.Error"));
			this.filePath.Font = null;
			this.errorProvider.SetIconAlignment(this.filePath, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("filePath.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.filePath, ((int)(resources.GetObject("filePath.IconPadding"))));
			this.filePath.Name = "filePath";
			// 
			// fileBrowse
			// 
			this.fileBrowse.AccessibleDescription = null;
			this.fileBrowse.AccessibleName = null;
			resources.ApplyResources(this.fileBrowse, "fileBrowse");
			this.fileBrowse.BackgroundImage = null;
			this.errorProvider.SetError(this.fileBrowse, resources.GetString("fileBrowse.Error"));
			this.fileBrowse.Font = null;
			this.errorProvider.SetIconAlignment(this.fileBrowse, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("fileBrowse.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.fileBrowse, ((int)(resources.GetObject("fileBrowse.IconPadding"))));
			this.fileBrowse.Name = "fileBrowse";
			this.fileBrowse.UseVisualStyleBackColor = true;
			this.fileBrowse.Click += new System.EventHandler(this.fileBrowse_Click);
			// 
			// folderPath
			// 
			this.folderPath.AccessibleDescription = null;
			this.folderPath.AccessibleName = null;
			resources.ApplyResources(this.folderPath, "folderPath");
			this.folderPath.BackgroundImage = null;
			this.errorProvider.SetError(this.folderPath, resources.GetString("folderPath.Error"));
			this.folderPath.Font = null;
			this.errorProvider.SetIconAlignment(this.folderPath, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("folderPath.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.folderPath, ((int)(resources.GetObject("folderPath.IconPadding"))));
			this.folderPath.Name = "folderPath";
			// 
			// folderBrowse
			// 
			this.folderBrowse.AccessibleDescription = null;
			this.folderBrowse.AccessibleName = null;
			resources.ApplyResources(this.folderBrowse, "folderBrowse");
			this.folderBrowse.BackgroundImage = null;
			this.errorProvider.SetError(this.folderBrowse, resources.GetString("folderBrowse.Error"));
			this.folderBrowse.Font = null;
			this.errorProvider.SetIconAlignment(this.folderBrowse, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("folderBrowse.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.folderBrowse, ((int)(resources.GetObject("folderBrowse.IconPadding"))));
			this.folderBrowse.Name = "folderBrowse";
			this.folderBrowse.UseVisualStyleBackColor = true;
			this.folderBrowse.Click += new System.EventHandler(this.folderBrowse_Click);
			// 
			// folderIncludeLbl
			// 
			this.folderIncludeLbl.AccessibleDescription = null;
			this.folderIncludeLbl.AccessibleName = null;
			resources.ApplyResources(this.folderIncludeLbl, "folderIncludeLbl");
			this.errorProvider.SetError(this.folderIncludeLbl, resources.GetString("folderIncludeLbl.Error"));
			this.folderIncludeLbl.Font = null;
			this.errorProvider.SetIconAlignment(this.folderIncludeLbl, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("folderIncludeLbl.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.folderIncludeLbl, ((int)(resources.GetObject("folderIncludeLbl.IconPadding"))));
			this.folderIncludeLbl.Name = "folderIncludeLbl";
			// 
			// folderInclude
			// 
			this.folderInclude.AccessibleDescription = null;
			this.folderInclude.AccessibleName = null;
			resources.ApplyResources(this.folderInclude, "folderInclude");
			this.folderInclude.BackgroundImage = null;
			this.errorProvider.SetError(this.folderInclude, resources.GetString("folderInclude.Error"));
			this.folderInclude.Font = null;
			this.errorProvider.SetIconAlignment(this.folderInclude, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("folderInclude.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.folderInclude, ((int)(resources.GetObject("folderInclude.IconPadding"))));
			this.folderInclude.Name = "folderInclude";
			// 
			// folderExcludeLbl
			// 
			this.folderExcludeLbl.AccessibleDescription = null;
			this.folderExcludeLbl.AccessibleName = null;
			resources.ApplyResources(this.folderExcludeLbl, "folderExcludeLbl");
			this.errorProvider.SetError(this.folderExcludeLbl, resources.GetString("folderExcludeLbl.Error"));
			this.folderExcludeLbl.Font = null;
			this.errorProvider.SetIconAlignment(this.folderExcludeLbl, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("folderExcludeLbl.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.folderExcludeLbl, ((int)(resources.GetObject("folderExcludeLbl.IconPadding"))));
			this.folderExcludeLbl.Name = "folderExcludeLbl";
			// 
			// folderExclude
			// 
			this.folderExclude.AccessibleDescription = null;
			this.folderExclude.AccessibleName = null;
			resources.ApplyResources(this.folderExclude, "folderExclude");
			this.folderExclude.BackgroundImage = null;
			this.errorProvider.SetError(this.folderExclude, resources.GetString("folderExclude.Error"));
			this.folderExclude.Font = null;
			this.errorProvider.SetIconAlignment(this.folderExclude, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("folderExclude.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.folderExclude, ((int)(resources.GetObject("folderExclude.IconPadding"))));
			this.folderExclude.Name = "folderExclude";
			// 
			// folderDelete
			// 
			this.folderDelete.AccessibleDescription = null;
			this.folderDelete.AccessibleName = null;
			resources.ApplyResources(this.folderDelete, "folderDelete");
			this.folderDelete.BackgroundImage = null;
			this.folderDelete.Checked = true;
			this.folderDelete.CheckState = System.Windows.Forms.CheckState.Checked;
			this.errorProvider.SetError(this.folderDelete, resources.GetString("folderDelete.Error"));
			this.folderDelete.Font = null;
			this.errorProvider.SetIconAlignment(this.folderDelete, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("folderDelete.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.folderDelete, ((int)(resources.GetObject("folderDelete.IconPadding"))));
			this.folderDelete.Name = "folderDelete";
			this.folderDelete.UseVisualStyleBackColor = true;
			// 
			// unusedDisk
			// 
			this.unusedDisk.AccessibleDescription = null;
			this.unusedDisk.AccessibleName = null;
			resources.ApplyResources(this.unusedDisk, "unusedDisk");
			this.unusedDisk.BackgroundImage = null;
			this.unusedDisk.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.unusedDisk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.errorProvider.SetError(this.unusedDisk, resources.GetString("unusedDisk.Error"));
			this.unusedDisk.Font = null;
			this.unusedDisk.FormattingEnabled = true;
			this.errorProvider.SetIconAlignment(this.unusedDisk, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("unusedDisk.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.unusedDisk, ((int)(resources.GetObject("unusedDisk.IconPadding"))));
			this.unusedDisk.Name = "unusedDisk";
			this.unusedDisk.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.unusedDisk_DrawItem);
			// 
			// methodLbl
			// 
			this.methodLbl.AccessibleDescription = null;
			this.methodLbl.AccessibleName = null;
			resources.ApplyResources(this.methodLbl, "methodLbl");
			this.errorProvider.SetError(this.methodLbl, resources.GetString("methodLbl.Error"));
			this.methodLbl.Font = null;
			this.errorProvider.SetIconAlignment(this.methodLbl, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("methodLbl.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.methodLbl, ((int)(resources.GetObject("methodLbl.IconPadding"))));
			this.methodLbl.Name = "methodLbl";
			// 
			// method
			// 
			this.method.AccessibleDescription = null;
			this.method.AccessibleName = null;
			resources.ApplyResources(this.method, "method");
			this.method.BackgroundImage = null;
			this.method.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.errorProvider.SetError(this.method, resources.GetString("method.Error"));
			this.method.Font = null;
			this.method.FormattingEnabled = true;
			this.errorProvider.SetIconAlignment(this.method, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("method.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.method, ((int)(resources.GetObject("method.IconPadding"))));
			this.method.Name = "method";
			this.method.SelectedIndexChanged += new System.EventHandler(this.method_SelectedIndexChanged);
			// 
			// ok
			// 
			this.ok.AccessibleDescription = null;
			this.ok.AccessibleName = null;
			resources.ApplyResources(this.ok, "ok");
			this.ok.BackgroundImage = null;
			this.errorProvider.SetError(this.ok, resources.GetString("ok.Error"));
			this.ok.Font = null;
			this.errorProvider.SetIconAlignment(this.ok, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("ok.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.ok, ((int)(resources.GetObject("ok.IconPadding"))));
			this.ok.Name = "ok";
			this.ok.UseVisualStyleBackColor = true;
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// cancel
			// 
			this.cancel.AccessibleDescription = null;
			this.cancel.AccessibleName = null;
			resources.ApplyResources(this.cancel, "cancel");
			this.cancel.BackgroundImage = null;
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.errorProvider.SetError(this.cancel, resources.GetString("cancel.Error"));
			this.cancel.Font = null;
			this.errorProvider.SetIconAlignment(this.cancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cancel.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.cancel, ((int)(resources.GetObject("cancel.IconPadding"))));
			this.cancel.Name = "cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// fileDialog
			// 
			resources.ApplyResources(this.fileDialog, "fileDialog");
			// 
			// folderDialog
			// 
			resources.ApplyResources(this.folderDialog, "folderDialog");
			this.folderDialog.ShowNewFolderButton = false;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			resources.ApplyResources(this.errorProvider, "errorProvider");
			// 
			// unusedClusterTips
			// 
			this.unusedClusterTips.AccessibleDescription = null;
			this.unusedClusterTips.AccessibleName = null;
			resources.ApplyResources(this.unusedClusterTips, "unusedClusterTips");
			this.unusedClusterTips.BackgroundImage = null;
			this.unusedClusterTips.Checked = true;
			this.unusedClusterTips.CheckState = System.Windows.Forms.CheckState.Checked;
			this.errorProvider.SetError(this.unusedClusterTips, resources.GetString("unusedClusterTips.Error"));
			this.unusedClusterTips.Font = null;
			this.errorProvider.SetIconAlignment(this.unusedClusterTips, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("unusedClusterTips.IconAlignment"))));
			this.errorProvider.SetIconPadding(this.unusedClusterTips, ((int)(resources.GetObject("unusedClusterTips.IconPadding"))));
			this.unusedClusterTips.Name = "unusedClusterTips";
			this.unusedClusterTips.UseVisualStyleBackColor = true;
			// 
			// TaskDataSelectionForm
			// 
			this.AcceptButton = this.ok;
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackgroundImage = null;
			this.CancelButton = this.cancel;
			this.Controls.Add(this.unusedClusterTips);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.method);
			this.Controls.Add(this.methodLbl);
			this.Controls.Add(this.unusedDisk);
			this.Controls.Add(this.folderDelete);
			this.Controls.Add(this.folderExclude);
			this.Controls.Add(this.folderExcludeLbl);
			this.Controls.Add(this.folderInclude);
			this.Controls.Add(this.folderIncludeLbl);
			this.Controls.Add(this.folderBrowse);
			this.Controls.Add(this.folderPath);
			this.Controls.Add(this.fileBrowse);
			this.Controls.Add(this.filePath);
			this.Controls.Add(this.unused);
			this.Controls.Add(this.folder);
			this.Controls.Add(this.file);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = null;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TaskDataSelectionForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton file;
		private System.Windows.Forms.RadioButton folder;
		private System.Windows.Forms.RadioButton unused;
		private System.Windows.Forms.TextBox filePath;
		private System.Windows.Forms.Button fileBrowse;
		private System.Windows.Forms.TextBox folderPath;
		private System.Windows.Forms.Button folderBrowse;
		private System.Windows.Forms.Label folderIncludeLbl;
		private System.Windows.Forms.TextBox folderInclude;
		private System.Windows.Forms.Label folderExcludeLbl;
		private System.Windows.Forms.TextBox folderExclude;
		private System.Windows.Forms.CheckBox folderDelete;
		private System.Windows.Forms.ComboBox unusedDisk;
		private System.Windows.Forms.Label methodLbl;
		private System.Windows.Forms.ComboBox method;
		private System.Windows.Forms.Button ok;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.OpenFileDialog fileDialog;
		private System.Windows.Forms.FolderBrowserDialog folderDialog;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.CheckBox unusedClusterTips;
	}
}