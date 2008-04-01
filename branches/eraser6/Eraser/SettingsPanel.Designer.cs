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
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.plausibleDeniabilityFilesAddFolder = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).BeginInit();
			this.content.SuspendLayout();
			this.pluginsMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// titleLbl
			// 
			this.titleLbl.Size = new System.Drawing.Size(101, 32);
			this.titleLbl.Text = "Settings";
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
			this.content.Size = new System.Drawing.Size(712, 780);
			// 
			// ui
			// 
			this.ui.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ui.AutoSize = true;
			this.ui.Label = "Shell integration";
			this.ui.Location = new System.Drawing.Point(0, -6);
			this.ui.Name = "ui";
			this.ui.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
			this.ui.Size = new System.Drawing.Size(712, 32);
			this.ui.TabIndex = 1;
			// 
			// uiContextMenu
			// 
			this.uiContextMenu.AutoSize = true;
			this.uiContextMenu.Checked = true;
			this.uiContextMenu.CheckState = System.Windows.Forms.CheckState.Checked;
			this.uiContextMenu.Location = new System.Drawing.Point(20, 53);
			this.uiContextMenu.Name = "uiContextMenu";
			this.uiContextMenu.Size = new System.Drawing.Size(209, 17);
			this.uiContextMenu.TabIndex = 4;
			this.uiContextMenu.Text = "Integrate Eraser into Windows Explorer";
			this.uiContextMenu.UseVisualStyleBackColor = true;
			// 
			// lockedAllow
			// 
			this.lockedAllow.AutoSize = true;
			this.lockedAllow.Checked = true;
			this.lockedAllow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.lockedAllow.Location = new System.Drawing.Point(20, 193);
			this.lockedAllow.Name = "lockedAllow";
			this.lockedAllow.Size = new System.Drawing.Size(251, 17);
			this.lockedAllow.TabIndex = 12;
			this.lockedAllow.Text = "Allow locked files to be erased on system restart";
			this.lockedAllow.UseVisualStyleBackColor = true;
			this.lockedAllow.CheckedChanged += new System.EventHandler(this.lockedAllow_CheckedChanged);
			// 
			// lockedConfirm
			// 
			this.lockedConfirm.AutoSize = true;
			this.lockedConfirm.Location = new System.Drawing.Point(36, 216);
			this.lockedConfirm.Name = "lockedConfirm";
			this.lockedConfirm.Size = new System.Drawing.Size(182, 17);
			this.lockedConfirm.TabIndex = 13;
			this.lockedConfirm.Text = "Confirm with user before doing so";
			this.lockedConfirm.UseVisualStyleBackColor = true;
			// 
			// erase
			// 
			this.erase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.erase.AutoSize = true;
			this.erase.Label = "Erase settings";
			this.erase.Location = new System.Drawing.Point(0, 76);
			this.erase.Name = "erase";
			this.erase.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
			this.erase.Size = new System.Drawing.Size(712, 32);
			this.erase.TabIndex = 5;
			// 
			// eraseFilesMethodLbl
			// 
			this.eraseFilesMethodLbl.AutoSize = true;
			this.eraseFilesMethodLbl.Location = new System.Drawing.Point(17, 115);
			this.eraseFilesMethodLbl.Name = "eraseFilesMethodLbl";
			this.eraseFilesMethodLbl.Size = new System.Drawing.Size(136, 13);
			this.eraseFilesMethodLbl.TabIndex = 6;
			this.eraseFilesMethodLbl.Text = "Default file erasure method:";
			// 
			// eraseUnusedMethodLbl
			// 
			this.eraseUnusedMethodLbl.AutoSize = true;
			this.eraseUnusedMethodLbl.Location = new System.Drawing.Point(17, 141);
			this.eraseUnusedMethodLbl.Name = "eraseUnusedMethodLbl";
			this.eraseUnusedMethodLbl.Size = new System.Drawing.Size(190, 13);
			this.eraseUnusedMethodLbl.TabIndex = 8;
			this.eraseUnusedMethodLbl.Text = "Default unused space erasure method:";
			// 
			// eraseFilesMethod
			// 
			this.eraseFilesMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.eraseFilesMethod.Location = new System.Drawing.Point(267, 111);
			this.eraseFilesMethod.Name = "eraseFilesMethod";
			this.eraseFilesMethod.Size = new System.Drawing.Size(290, 21);
			this.eraseFilesMethod.TabIndex = 7;
			// 
			// eraseUnusedMethod
			// 
			this.eraseUnusedMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.eraseUnusedMethod.Location = new System.Drawing.Point(267, 138);
			this.eraseUnusedMethod.Name = "eraseUnusedMethod";
			this.eraseUnusedMethod.Size = new System.Drawing.Size(290, 21);
			this.eraseUnusedMethod.TabIndex = 9;
			// 
			// plugins
			// 
			this.plugins.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.plugins.AutoSize = true;
			this.plugins.Label = "Plugins";
			this.plugins.Location = new System.Drawing.Point(0, 507);
			this.plugins.Name = "plugins";
			this.plugins.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
			this.plugins.Size = new System.Drawing.Size(712, 32);
			this.plugins.TabIndex = 19;
			// 
			// pluginsManager
			// 
			this.pluginsManager.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pluginsManager.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginsManagerColName,
            this.pluginsManagerColAuthor,
            this.pluginsManagerColVersion,
            this.pluginsManagerColPath});
			this.pluginsManager.ContextMenuStrip = this.pluginsMenu;
			this.pluginsManager.FullRowSelect = true;
			this.pluginsManager.Location = new System.Drawing.Point(20, 545);
			this.pluginsManager.Name = "pluginsManager";
			this.pluginsManager.Size = new System.Drawing.Size(689, 234);
			this.pluginsManager.TabIndex = 20;
			this.pluginsManager.UseCompatibleStateImageBehavior = false;
			this.pluginsManager.View = System.Windows.Forms.View.Details;
			// 
			// pluginsManagerColName
			// 
			this.pluginsManagerColName.Text = "Name";
			this.pluginsManagerColName.Width = 250;
			// 
			// pluginsManagerColAuthor
			// 
			this.pluginsManagerColAuthor.Text = "Author";
			this.pluginsManagerColAuthor.Width = 140;
			// 
			// pluginsManagerColVersion
			// 
			this.pluginsManagerColVersion.Text = "Version";
			this.pluginsManagerColVersion.Width = 80;
			// 
			// pluginsManagerColPath
			// 
			this.pluginsManagerColPath.Text = "File Path";
			this.pluginsManagerColPath.Width = 180;
			// 
			// pluginsMenu
			// 
			this.pluginsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
			this.pluginsMenu.Name = "pluginsContextMenu";
			this.pluginsMenu.Size = new System.Drawing.Size(126, 26);
			this.pluginsMenu.Opening += new System.ComponentModel.CancelEventHandler(this.pluginsMenu_Opening);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.settingsToolStripMenuItem.Text = "Settings...";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// scheduler
			// 
			this.scheduler.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scheduler.AutoSize = true;
			this.scheduler.Label = "Scheduler settings";
			this.scheduler.Location = new System.Drawing.Point(0, 413);
			this.scheduler.Name = "scheduler";
			this.scheduler.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
			this.scheduler.Size = new System.Drawing.Size(712, 32);
			this.scheduler.TabIndex = 15;
			// 
			// schedulerMissed
			// 
			this.schedulerMissed.AutoSize = true;
			this.schedulerMissed.Location = new System.Drawing.Point(17, 448);
			this.schedulerMissed.Name = "schedulerMissed";
			this.schedulerMissed.Size = new System.Drawing.Size(242, 13);
			this.schedulerMissed.TabIndex = 16;
			this.schedulerMissed.Text = "When a recurring task has missed its starting time,";
			// 
			// schedulerMissedImmediate
			// 
			this.schedulerMissedImmediate.AutoSize = true;
			this.schedulerMissedImmediate.Checked = true;
			this.schedulerMissedImmediate.Location = new System.Drawing.Point(36, 464);
			this.schedulerMissedImmediate.Name = "schedulerMissedImmediate";
			this.schedulerMissedImmediate.Size = new System.Drawing.Size(217, 17);
			this.schedulerMissedImmediate.TabIndex = 17;
			this.schedulerMissedImmediate.TabStop = true;
			this.schedulerMissedImmediate.Text = "execute the task when Eraser next starts";
			this.schedulerMissedImmediate.UseVisualStyleBackColor = true;
			// 
			// schedulerMissedIgnore
			// 
			this.schedulerMissedIgnore.AutoSize = true;
			this.schedulerMissedIgnore.Location = new System.Drawing.Point(36, 484);
			this.schedulerMissedIgnore.Name = "schedulerMissedIgnore";
			this.schedulerMissedIgnore.Size = new System.Drawing.Size(339, 17);
			this.schedulerMissedIgnore.TabIndex = 18;
			this.schedulerMissedIgnore.TabStop = true;
			this.schedulerMissedIgnore.Text = "ignore the missed schedule and run only at the next appointed time";
			this.schedulerMissedIgnore.UseVisualStyleBackColor = true;
			// 
			// saveSettings
			// 
			this.saveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.saveSettings.Location = new System.Drawing.Point(639, 27);
			this.saveSettings.Name = "saveSettings";
			this.saveSettings.Size = new System.Drawing.Size(90, 23);
			this.saveSettings.TabIndex = 18;
			this.saveSettings.Text = "Save Settings";
			this.saveSettings.UseVisualStyleBackColor = true;
			this.saveSettings.Click += new System.EventHandler(this.saveSettings_Click);
			// 
			// erasePRNGLbl
			// 
			this.erasePRNGLbl.AutoSize = true;
			this.erasePRNGLbl.Location = new System.Drawing.Point(17, 169);
			this.erasePRNGLbl.Name = "erasePRNGLbl";
			this.erasePRNGLbl.Size = new System.Drawing.Size(131, 13);
			this.erasePRNGLbl.TabIndex = 10;
			this.erasePRNGLbl.Text = "Randomness data source:";
			// 
			// erasePRNG
			// 
			this.erasePRNG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.erasePRNG.FormattingEnabled = true;
			this.erasePRNG.Location = new System.Drawing.Point(267, 165);
			this.erasePRNG.Name = "erasePRNG";
			this.erasePRNG.Size = new System.Drawing.Size(290, 21);
			this.erasePRNG.TabIndex = 11;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// plausibleDeniability
			// 
			this.plausibleDeniability.AutoSize = true;
			this.plausibleDeniability.Checked = true;
			this.plausibleDeniability.CheckState = System.Windows.Forms.CheckState.Checked;
			this.plausibleDeniability.Location = new System.Drawing.Point(20, 240);
			this.plausibleDeniability.Name = "plausibleDeniability";
			this.plausibleDeniability.Size = new System.Drawing.Size(359, 17);
			this.plausibleDeniability.TabIndex = 14;
			this.plausibleDeniability.Text = "Replace erased files with the following files to allow plausible deniability";
			this.plausibleDeniability.UseVisualStyleBackColor = true;
			this.plausibleDeniability.CheckedChanged += new System.EventHandler(this.plausibleDeniability_CheckedChanged);
			// 
			// uiLanguageLbl
			// 
			this.uiLanguageLbl.AutoSize = true;
			this.uiLanguageLbl.Location = new System.Drawing.Point(17, 29);
			this.uiLanguageLbl.Name = "uiLanguageLbl";
			this.uiLanguageLbl.Size = new System.Drawing.Size(123, 13);
			this.uiLanguageLbl.TabIndex = 2;
			this.uiLanguageLbl.Text = "User interface language:";
			// 
			// uiLanguage
			// 
			this.uiLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.uiLanguage.FormattingEnabled = true;
			this.uiLanguage.Location = new System.Drawing.Point(267, 26);
			this.uiLanguage.Name = "uiLanguage";
			this.uiLanguage.Size = new System.Drawing.Size(290, 21);
			this.uiLanguage.TabIndex = 3;
			// 
			// plausibleDeniabilityFiles
			// 
			this.plausibleDeniabilityFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.plausibleDeniabilityFiles.FormattingEnabled = true;
			this.plausibleDeniabilityFiles.Location = new System.Drawing.Point(36, 263);
			this.plausibleDeniabilityFiles.Name = "plausibleDeniabilityFiles";
			this.plausibleDeniabilityFiles.Size = new System.Drawing.Size(673, 147);
			this.plausibleDeniabilityFiles.TabIndex = 21;
			// 
			// plausibleDeniabilityFilesAddFile
			// 
			this.plausibleDeniabilityFilesAddFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.plausibleDeniabilityFilesAddFile.Location = new System.Drawing.Point(462, 236);
			this.plausibleDeniabilityFilesAddFile.Name = "plausibleDeniabilityFilesAddFile";
			this.plausibleDeniabilityFilesAddFile.Size = new System.Drawing.Size(75, 23);
			this.plausibleDeniabilityFilesAddFile.TabIndex = 22;
			this.plausibleDeniabilityFilesAddFile.Text = "Add File...";
			this.plausibleDeniabilityFilesAddFile.UseVisualStyleBackColor = true;
			this.plausibleDeniabilityFilesAddFile.Click += new System.EventHandler(this.plausibleDeniabilityFilesAddFile_Click);
			// 
			// plausibleDeniabilityFilesRemove
			// 
			this.plausibleDeniabilityFilesRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.plausibleDeniabilityFilesRemove.Location = new System.Drawing.Point(634, 236);
			this.plausibleDeniabilityFilesRemove.Name = "plausibleDeniabilityFilesRemove";
			this.plausibleDeniabilityFilesRemove.Size = new System.Drawing.Size(75, 23);
			this.plausibleDeniabilityFilesRemove.TabIndex = 23;
			this.plausibleDeniabilityFilesRemove.Text = "Remove";
			this.plausibleDeniabilityFilesRemove.UseVisualStyleBackColor = true;
			this.plausibleDeniabilityFilesRemove.Click += new System.EventHandler(this.plausibleDeniabilityFilesRemove_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "All Files (*.*)|*.*";
			this.openFileDialog.Multiselect = true;
			// 
			// plausibleDeniabilityFilesAddFolder
			// 
			this.plausibleDeniabilityFilesAddFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.plausibleDeniabilityFilesAddFolder.Location = new System.Drawing.Point(543, 236);
			this.plausibleDeniabilityFilesAddFolder.Name = "plausibleDeniabilityFilesAddFolder";
			this.plausibleDeniabilityFilesAddFolder.Size = new System.Drawing.Size(85, 23);
			this.plausibleDeniabilityFilesAddFolder.TabIndex = 24;
			this.plausibleDeniabilityFilesAddFolder.Text = "Add Folder...";
			this.plausibleDeniabilityFilesAddFolder.UseVisualStyleBackColor = true;
			this.plausibleDeniabilityFilesAddFolder.Click += new System.EventHandler(this.plausibleDeniabilityFilesAddFolder_Click);
			// 
			// SettingsPanel
			// 
			this.Controls.Add(this.saveSettings);
			this.Name = "SettingsPanel";
			this.Size = new System.Drawing.Size(752, 852);
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
