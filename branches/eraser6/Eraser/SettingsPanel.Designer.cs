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
	partial class SettingsPanel
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsPanel));
			this.ui = new Eraser.LightGroup();
			this.uiContextMenu = new System.Windows.Forms.CheckBox();
			this.lockedAllow = new System.Windows.Forms.CheckBox();
			this.lockedConfirm = new System.Windows.Forms.CheckBox();
			this.erase = new Eraser.LightGroup();
			this.eraseFilesMethodLbl = new System.Windows.Forms.Label();
			this.eraseUnusedMethodLbl = new System.Windows.Forms.Label();
			this.eraseFilesMethod = new System.Windows.Forms.ComboBox();
			this.eraseUnusedMethod = new System.Windows.Forms.ComboBox();
			this.plugins = new Eraser.LightGroup();
			this.pluginsManager = new System.Windows.Forms.ListView();
			this.pluginsManagerColName = new System.Windows.Forms.ColumnHeader();
			this.pluginsManagerColAuthor = new System.Windows.Forms.ColumnHeader();
			this.pluginsManagerColVersion = new System.Windows.Forms.ColumnHeader();
			this.pluginsManagerColPath = new System.Windows.Forms.ColumnHeader();
			this.pluginsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scheduler = new Eraser.LightGroup();
			this.schedulerMissed = new System.Windows.Forms.Label();
			this.schedulerMissedImmediate = new System.Windows.Forms.RadioButton();
			this.schedulerMissedIgnore = new System.Windows.Forms.RadioButton();
			this.saveSettings = new System.Windows.Forms.Button();
			this.erasePRNGLbl = new System.Windows.Forms.Label();
			this.erasePRNG = new System.Windows.Forms.ComboBox();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.plausibleDeniability = new System.Windows.Forms.CheckBox();
			this.uiLanguageLbl = new System.Windows.Forms.Label();
			this.uiLanguage = new System.Windows.Forms.ComboBox();
			this.plausibleDeniabilityFiles = new System.Windows.Forms.ListBox();
			this.plausibleDeniabilityFilesAddFile = new System.Windows.Forms.Button();
			this.plausibleDeniabilityFilesRemove = new System.Windows.Forms.Button();
			this.plausibleDeniabilityFilesAddFolder = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).BeginInit();
			this.content.SuspendLayout();
			this.pluginsMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// titleLbl
			// 
			resources.ApplyResources(this.titleLbl, "titleLbl");
			// 
			// titleIcon
			// 
			this.titleIcon.Image = global::Eraser.Properties.Resources.ToolbarSettings;
			// 
			// content
			// 
			this.content.Controls.Add(this.plausibleDeniabilityFilesAddFolder);
			this.content.Controls.Add(this.plausibleDeniabilityFilesRemove);
			this.content.Controls.Add(this.plausibleDeniabilityFilesAddFile);
			this.content.Controls.Add(this.plausibleDeniabilityFiles);
			this.content.Controls.Add(this.uiLanguage);
			this.content.Controls.Add(this.uiLanguageLbl);
			this.content.Controls.Add(this.plausibleDeniability);
			this.content.Controls.Add(this.erasePRNG);
			this.content.Controls.Add(this.erasePRNGLbl);
			this.content.Controls.Add(this.schedulerMissedIgnore);
			this.content.Controls.Add(this.schedulerMissedImmediate);
			this.content.Controls.Add(this.schedulerMissed);
			this.content.Controls.Add(this.scheduler);
			this.content.Controls.Add(this.pluginsManager);
			this.content.Controls.Add(this.plugins);
			this.content.Controls.Add(this.eraseUnusedMethod);
			this.content.Controls.Add(this.eraseFilesMethod);
			this.content.Controls.Add(this.eraseUnusedMethodLbl);
			this.content.Controls.Add(this.eraseFilesMethodLbl);
			this.content.Controls.Add(this.erase);
			this.content.Controls.Add(this.lockedConfirm);
			this.content.Controls.Add(this.lockedAllow);
			this.content.Controls.Add(this.uiContextMenu);
			this.content.Controls.Add(this.ui);
			resources.ApplyResources(this.content, "content");
			// 
			// ui
			// 
			resources.ApplyResources(this.ui, "ui");
			this.ui.Label = "Shell integration";
			this.ui.Name = "ui";
			// 
			// uiContextMenu
			// 
			resources.ApplyResources(this.uiContextMenu, "uiContextMenu");
			this.uiContextMenu.Checked = true;
			this.uiContextMenu.CheckState = System.Windows.Forms.CheckState.Checked;
			this.uiContextMenu.Name = "uiContextMenu";
			this.uiContextMenu.UseVisualStyleBackColor = true;
			// 
			// lockedAllow
			// 
			resources.ApplyResources(this.lockedAllow, "lockedAllow");
			this.lockedAllow.Checked = true;
			this.lockedAllow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.lockedAllow.Name = "lockedAllow";
			this.lockedAllow.UseVisualStyleBackColor = true;
			this.lockedAllow.CheckedChanged += new System.EventHandler(this.lockedAllow_CheckedChanged);
			// 
			// lockedConfirm
			// 
			resources.ApplyResources(this.lockedConfirm, "lockedConfirm");
			this.lockedConfirm.Name = "lockedConfirm";
			this.lockedConfirm.UseVisualStyleBackColor = true;
			// 
			// erase
			// 
			resources.ApplyResources(this.erase, "erase");
			this.erase.Label = "Instellingen";
			this.erase.Name = "erase";
			// 
			// eraseFilesMethodLbl
			// 
			resources.ApplyResources(this.eraseFilesMethodLbl, "eraseFilesMethodLbl");
			this.eraseFilesMethodLbl.Name = "eraseFilesMethodLbl";
			// 
			// eraseUnusedMethodLbl
			// 
			resources.ApplyResources(this.eraseUnusedMethodLbl, "eraseUnusedMethodLbl");
			this.eraseUnusedMethodLbl.Name = "eraseUnusedMethodLbl";
			// 
			// eraseFilesMethod
			// 
			this.eraseFilesMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.eraseFilesMethod, "eraseFilesMethod");
			this.eraseFilesMethod.Name = "eraseFilesMethod";
			// 
			// eraseUnusedMethod
			// 
			this.eraseUnusedMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.eraseUnusedMethod, "eraseUnusedMethod");
			this.eraseUnusedMethod.Name = "eraseUnusedMethod";
			// 
			// plugins
			// 
			resources.ApplyResources(this.plugins, "plugins");
			this.plugins.Label = "Plugins";
			this.plugins.Name = "plugins";
			// 
			// pluginsManager
			// 
			resources.ApplyResources(this.pluginsManager, "pluginsManager");
			this.pluginsManager.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginsManagerColName,
            this.pluginsManagerColAuthor,
            this.pluginsManagerColVersion,
            this.pluginsManagerColPath});
			this.pluginsManager.ContextMenuStrip = this.pluginsMenu;
			this.pluginsManager.FullRowSelect = true;
			this.pluginsManager.Name = "pluginsManager";
			this.pluginsManager.UseCompatibleStateImageBehavior = false;
			this.pluginsManager.View = System.Windows.Forms.View.Details;
			// 
			// pluginsManagerColName
			// 
			resources.ApplyResources(this.pluginsManagerColName, "pluginsManagerColName");
			// 
			// pluginsManagerColAuthor
			// 
			resources.ApplyResources(this.pluginsManagerColAuthor, "pluginsManagerColAuthor");
			// 
			// pluginsManagerColVersion
			// 
			resources.ApplyResources(this.pluginsManagerColVersion, "pluginsManagerColVersion");
			// 
			// pluginsManagerColPath
			// 
			resources.ApplyResources(this.pluginsManagerColPath, "pluginsManagerColPath");
			// 
			// pluginsMenu
			// 
			this.pluginsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
			this.pluginsMenu.Name = "pluginsContextMenu";
			resources.ApplyResources(this.pluginsMenu, "pluginsMenu");
			this.pluginsMenu.Opening += new System.ComponentModel.CancelEventHandler(this.pluginsMenu_Opening);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// scheduler
			// 
			resources.ApplyResources(this.scheduler, "scheduler");
			this.scheduler.Label = "Planner instellingen";
			this.scheduler.Name = "scheduler";
			// 
			// schedulerMissed
			// 
			resources.ApplyResources(this.schedulerMissed, "schedulerMissed");
			this.schedulerMissed.Name = "schedulerMissed";
			// 
			// schedulerMissedImmediate
			// 
			resources.ApplyResources(this.schedulerMissedImmediate, "schedulerMissedImmediate");
			this.schedulerMissedImmediate.Checked = true;
			this.schedulerMissedImmediate.Name = "schedulerMissedImmediate";
			this.schedulerMissedImmediate.TabStop = true;
			this.schedulerMissedImmediate.UseVisualStyleBackColor = true;
			// 
			// schedulerMissedIgnore
			// 
			resources.ApplyResources(this.schedulerMissedIgnore, "schedulerMissedIgnore");
			this.schedulerMissedIgnore.Name = "schedulerMissedIgnore";
			this.schedulerMissedIgnore.TabStop = true;
			this.schedulerMissedIgnore.UseVisualStyleBackColor = true;
			// 
			// saveSettings
			// 
			resources.ApplyResources(this.saveSettings, "saveSettings");
			this.saveSettings.Name = "saveSettings";
			this.saveSettings.UseVisualStyleBackColor = true;
			this.saveSettings.Click += new System.EventHandler(this.saveSettings_Click);
			// 
			// erasePRNGLbl
			// 
			resources.ApplyResources(this.erasePRNGLbl, "erasePRNGLbl");
			this.erasePRNGLbl.Name = "erasePRNGLbl";
			// 
			// erasePRNG
			// 
			this.erasePRNG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.erasePRNG.FormattingEnabled = true;
			resources.ApplyResources(this.erasePRNG, "erasePRNG");
			this.erasePRNG.Name = "erasePRNG";
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// plausibleDeniability
			// 
			resources.ApplyResources(this.plausibleDeniability, "plausibleDeniability");
			this.plausibleDeniability.Name = "plausibleDeniability";
			this.plausibleDeniability.UseVisualStyleBackColor = true;
			this.plausibleDeniability.CheckedChanged += new System.EventHandler(this.plausibleDeniability_CheckedChanged);
			// 
			// uiLanguageLbl
			// 
			resources.ApplyResources(this.uiLanguageLbl, "uiLanguageLbl");
			this.uiLanguageLbl.Name = "uiLanguageLbl";
			// 
			// uiLanguage
			// 
			this.uiLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.uiLanguage.FormattingEnabled = true;
			resources.ApplyResources(this.uiLanguage, "uiLanguage");
			this.uiLanguage.Name = "uiLanguage";
			// 
			// plausibleDeniabilityFiles
			// 
			resources.ApplyResources(this.plausibleDeniabilityFiles, "plausibleDeniabilityFiles");
			this.plausibleDeniabilityFiles.FormattingEnabled = true;
			this.plausibleDeniabilityFiles.Name = "plausibleDeniabilityFiles";
			// 
			// plausibleDeniabilityFilesAddFile
			// 
			resources.ApplyResources(this.plausibleDeniabilityFilesAddFile, "plausibleDeniabilityFilesAddFile");
			this.plausibleDeniabilityFilesAddFile.Name = "plausibleDeniabilityFilesAddFile";
			this.plausibleDeniabilityFilesAddFile.UseVisualStyleBackColor = true;
			this.plausibleDeniabilityFilesAddFile.Click += new System.EventHandler(this.plausibleDeniabilityFilesAddFile_Click);
			// 
			// plausibleDeniabilityFilesRemove
			// 
			resources.ApplyResources(this.plausibleDeniabilityFilesRemove, "plausibleDeniabilityFilesRemove");
			this.plausibleDeniabilityFilesRemove.Name = "plausibleDeniabilityFilesRemove";
			this.plausibleDeniabilityFilesRemove.UseVisualStyleBackColor = true;
			this.plausibleDeniabilityFilesRemove.Click += new System.EventHandler(this.plausibleDeniabilityFilesRemove_Click);
			// 
			// plausibleDeniabilityFilesAddFolder
			// 
			resources.ApplyResources(this.plausibleDeniabilityFilesAddFolder, "plausibleDeniabilityFilesAddFolder");
			this.plausibleDeniabilityFilesAddFolder.Name = "plausibleDeniabilityFilesAddFolder";
			this.plausibleDeniabilityFilesAddFolder.UseVisualStyleBackColor = true;
			this.plausibleDeniabilityFilesAddFolder.Click += new System.EventHandler(this.plausibleDeniabilityFilesAddFolder_Click);
			// 
			// openFileDialog
			// 
			resources.ApplyResources(this.openFileDialog, "openFileDialog");
			this.openFileDialog.Multiselect = true;
			// 
			// SettingsPanel
			// 
			this.Controls.Add(this.saveSettings);
			this.Name = "SettingsPanel";
			resources.ApplyResources(this, "$this");
			this.Controls.SetChildIndex(this.saveSettings, 0);
			this.Controls.SetChildIndex(this.titleLbl, 0);
			this.Controls.SetChildIndex(this.titleIcon, 0);
			this.Controls.SetChildIndex(this.content, 0);
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).EndInit();
			this.content.ResumeLayout(false);
			this.content.PerformLayout();
			this.pluginsMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox lockedConfirm;
		private System.Windows.Forms.CheckBox lockedAllow;
		private System.Windows.Forms.CheckBox uiContextMenu;
		private LightGroup ui;
		private System.Windows.Forms.Label eraseUnusedMethodLbl;
		private System.Windows.Forms.Label eraseFilesMethodLbl;
		private LightGroup erase;
		private System.Windows.Forms.ComboBox eraseFilesMethod;
		private System.Windows.Forms.ComboBox eraseUnusedMethod;
		private System.Windows.Forms.ListView pluginsManager;
		private System.Windows.Forms.ColumnHeader pluginsManagerColName;
		private System.Windows.Forms.ColumnHeader pluginsManagerColAuthor;
		private System.Windows.Forms.ColumnHeader pluginsManagerColVersion;
		private System.Windows.Forms.ColumnHeader pluginsManagerColPath;
		private LightGroup plugins;
		private System.Windows.Forms.RadioButton schedulerMissedIgnore;
		private System.Windows.Forms.RadioButton schedulerMissedImmediate;
		private System.Windows.Forms.Label schedulerMissed;
		private LightGroup scheduler;
		private System.Windows.Forms.Button saveSettings;
		private System.Windows.Forms.ComboBox erasePRNG;
		private System.Windows.Forms.Label erasePRNGLbl;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.CheckBox plausibleDeniability;
		private System.Windows.Forms.ContextMenuStrip pluginsMenu;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ComboBox uiLanguage;
		private System.Windows.Forms.Label uiLanguageLbl;
		private System.Windows.Forms.Button plausibleDeniabilityFilesRemove;
		private System.Windows.Forms.Button plausibleDeniabilityFilesAddFile;
		private System.Windows.Forms.ListBox plausibleDeniabilityFiles;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.Button plausibleDeniabilityFilesAddFolder;
	}
}
