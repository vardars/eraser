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
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Erasure method providers", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Random number generators", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("History trace cleaners", System.Windows.Forms.HorizontalAlignment.Left);
			this.shell = new Eraser.LightGroup();
			this.shellContextMenu = new System.Windows.Forms.CheckBox();
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
			this.scheduler = new Eraser.LightGroup();
			this.schedulerMissed = new System.Windows.Forms.Label();
			this.schedulerMissedImmediate = new System.Windows.Forms.RadioButton();
			this.schedulerMissedIgnore = new System.Windows.Forms.RadioButton();
			this.saveSettings = new System.Windows.Forms.Button();
			this.erasePRNGLbl = new System.Windows.Forms.Label();
			this.erasePRNG = new System.Windows.Forms.ComboBox();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).BeginInit();
			this.content.SuspendLayout();
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
			this.content.Controls.Add(this.shellContextMenu);
			this.content.Controls.Add(this.shell);
			this.content.Size = new System.Drawing.Size(712, 578);
			// 
			// shell
			// 
			this.shell.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.shell.AutoSize = true;
			this.shell.Label = "Shell integration";
			this.shell.Location = new System.Drawing.Point(0, -6);
			this.shell.Name = "shell";
			this.shell.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
			this.shell.Size = new System.Drawing.Size(712, 32);
			this.shell.TabIndex = 1;
			// 
			// shellContextMenu
			// 
			this.shellContextMenu.AutoSize = true;
			this.shellContextMenu.Checked = true;
			this.shellContextMenu.CheckState = System.Windows.Forms.CheckState.Checked;
			this.shellContextMenu.Location = new System.Drawing.Point(20, 32);
			this.shellContextMenu.Name = "shellContextMenu";
			this.shellContextMenu.Size = new System.Drawing.Size(209, 17);
			this.shellContextMenu.TabIndex = 2;
			this.shellContextMenu.Text = "Integrate Eraser into Windows Explorer";
			this.shellContextMenu.UseVisualStyleBackColor = true;
			// 
			// lockedAllow
			// 
			this.lockedAllow.AutoSize = true;
			this.lockedAllow.Checked = true;
			this.lockedAllow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.lockedAllow.Location = new System.Drawing.Point(20, 166);
			this.lockedAllow.Name = "lockedAllow";
			this.lockedAllow.Size = new System.Drawing.Size(216, 17);
			this.lockedAllow.TabIndex = 10;
			this.lockedAllow.Text = "Allow files to be erased on system restart";
			this.lockedAllow.UseVisualStyleBackColor = true;
			// 
			// lockedConfirm
			// 
			this.lockedConfirm.AutoSize = true;
			this.lockedConfirm.Location = new System.Drawing.Point(36, 189);
			this.lockedConfirm.Name = "lockedConfirm";
			this.lockedConfirm.Size = new System.Drawing.Size(182, 17);
			this.lockedConfirm.TabIndex = 11;
			this.lockedConfirm.Text = "Confirm with user before doing so";
			this.lockedConfirm.UseVisualStyleBackColor = true;
			// 
			// erase
			// 
			this.erase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.erase.AutoSize = true;
			this.erase.Label = "Erase settings";
			this.erase.Location = new System.Drawing.Point(0, 49);
			this.erase.Name = "erase";
			this.erase.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
			this.erase.Size = new System.Drawing.Size(712, 32);
			this.erase.TabIndex = 3;
			// 
			// eraseFilesMethodLbl
			// 
			this.eraseFilesMethodLbl.AutoSize = true;
			this.eraseFilesMethodLbl.Location = new System.Drawing.Point(17, 88);
			this.eraseFilesMethodLbl.Name = "eraseFilesMethodLbl";
			this.eraseFilesMethodLbl.Size = new System.Drawing.Size(136, 13);
			this.eraseFilesMethodLbl.TabIndex = 4;
			this.eraseFilesMethodLbl.Text = "Default file erasure method:";
			// 
			// eraseUnusedMethodLbl
			// 
			this.eraseUnusedMethodLbl.AutoSize = true;
			this.eraseUnusedMethodLbl.Location = new System.Drawing.Point(17, 114);
			this.eraseUnusedMethodLbl.Name = "eraseUnusedMethodLbl";
			this.eraseUnusedMethodLbl.Size = new System.Drawing.Size(190, 13);
			this.eraseUnusedMethodLbl.TabIndex = 6;
			this.eraseUnusedMethodLbl.Text = "Default unused space erasure method:";
			// 
			// eraseFilesMethod
			// 
			this.eraseFilesMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.eraseFilesMethod.Location = new System.Drawing.Point(267, 84);
			this.eraseFilesMethod.Name = "eraseFilesMethod";
			this.eraseFilesMethod.Size = new System.Drawing.Size(190, 21);
			this.eraseFilesMethod.TabIndex = 5;
			// 
			// eraseUnusedMethod
			// 
			this.eraseUnusedMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.eraseUnusedMethod.Location = new System.Drawing.Point(267, 111);
			this.eraseUnusedMethod.Name = "eraseUnusedMethod";
			this.eraseUnusedMethod.Size = new System.Drawing.Size(190, 21);
			this.eraseUnusedMethod.TabIndex = 7;
			// 
			// plugins
			// 
			this.plugins.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.plugins.AutoSize = true;
			this.plugins.Label = "Plugins";
			this.plugins.Location = new System.Drawing.Point(0, 306);
			this.plugins.Name = "plugins";
			this.plugins.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
			this.plugins.Size = new System.Drawing.Size(712, 32);
			this.plugins.TabIndex = 16;
			// 
			// pluginsManager
			// 
			this.pluginsManager.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pluginsManager.CheckBoxes = true;
			this.pluginsManager.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginsManagerColName,
            this.pluginsManagerColAuthor,
            this.pluginsManagerColVersion,
            this.pluginsManagerColPath});
			this.pluginsManager.FullRowSelect = true;
			listViewGroup1.Header = "Erasure method providers";
			listViewGroup1.Name = "pluginsManagerGrpMethod";
			listViewGroup2.Header = "Random number generators";
			listViewGroup2.Name = "pluginsManagerGrpPrng";
			listViewGroup3.Header = "History trace cleaners";
			listViewGroup3.Name = "pluginsManagerGrpHistory";
			this.pluginsManager.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
			this.pluginsManager.Location = new System.Drawing.Point(20, 344);
			this.pluginsManager.Name = "pluginsManager";
			this.pluginsManager.Size = new System.Drawing.Size(689, 234);
			this.pluginsManager.TabIndex = 17;
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
			// scheduler
			// 
			this.scheduler.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scheduler.AutoSize = true;
			this.scheduler.Label = "Scheduler settings";
			this.scheduler.Location = new System.Drawing.Point(0, 212);
			this.scheduler.Name = "scheduler";
			this.scheduler.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
			this.scheduler.Size = new System.Drawing.Size(712, 32);
			this.scheduler.TabIndex = 12;
			// 
			// schedulerMissed
			// 
			this.schedulerMissed.AutoSize = true;
			this.schedulerMissed.Location = new System.Drawing.Point(17, 247);
			this.schedulerMissed.Name = "schedulerMissed";
			this.schedulerMissed.Size = new System.Drawing.Size(242, 13);
			this.schedulerMissed.TabIndex = 13;
			this.schedulerMissed.Text = "When a recurring task has missed its starting time,";
			// 
			// schedulerMissedImmediate
			// 
			this.schedulerMissedImmediate.AutoSize = true;
			this.schedulerMissedImmediate.Checked = true;
			this.schedulerMissedImmediate.Location = new System.Drawing.Point(36, 263);
			this.schedulerMissedImmediate.Name = "schedulerMissedImmediate";
			this.schedulerMissedImmediate.Size = new System.Drawing.Size(217, 17);
			this.schedulerMissedImmediate.TabIndex = 14;
			this.schedulerMissedImmediate.TabStop = true;
			this.schedulerMissedImmediate.Text = "execute the task when Eraser next starts";
			this.schedulerMissedImmediate.UseVisualStyleBackColor = true;
			// 
			// schedulerMissedIgnore
			// 
			this.schedulerMissedIgnore.AutoSize = true;
			this.schedulerMissedIgnore.Location = new System.Drawing.Point(36, 283);
			this.schedulerMissedIgnore.Name = "schedulerMissedIgnore";
			this.schedulerMissedIgnore.Size = new System.Drawing.Size(339, 17);
			this.schedulerMissedIgnore.TabIndex = 15;
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
			this.erasePRNGLbl.Location = new System.Drawing.Point(17, 142);
			this.erasePRNGLbl.Name = "erasePRNGLbl";
			this.erasePRNGLbl.Size = new System.Drawing.Size(131, 13);
			this.erasePRNGLbl.TabIndex = 8;
			this.erasePRNGLbl.Text = "Randomness data source:";
			// 
			// erasePRNG
			// 
			this.erasePRNG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.erasePRNG.FormattingEnabled = true;
			this.erasePRNG.Location = new System.Drawing.Point(267, 138);
			this.erasePRNG.Name = "erasePRNG";
			this.erasePRNG.Size = new System.Drawing.Size(190, 21);
			this.erasePRNG.TabIndex = 9;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// SettingsPanel
			// 
			this.Controls.Add(this.saveSettings);
			this.Name = "SettingsPanel";
			this.Size = new System.Drawing.Size(752, 650);
			this.Controls.SetChildIndex(this.saveSettings, 0);
			this.Controls.SetChildIndex(this.titleLbl, 0);
			this.Controls.SetChildIndex(this.titleIcon, 0);
			this.Controls.SetChildIndex(this.content, 0);
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).EndInit();
			this.content.ResumeLayout(false);
			this.content.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox lockedConfirm;
		private System.Windows.Forms.CheckBox lockedAllow;
		private System.Windows.Forms.CheckBox shellContextMenu;
		private LightGroup shell;
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
	}
}
