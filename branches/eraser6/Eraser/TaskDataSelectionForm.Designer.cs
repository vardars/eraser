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
			this.SuspendLayout();
			// 
			// file
			// 
			this.file.AutoSize = true;
			this.file.Location = new System.Drawing.Point(8, 44);
			this.file.Name = "file";
			this.file.Size = new System.Drawing.Size(43, 19);
			this.file.TabIndex = 3;
			this.file.TabStop = true;
			this.file.Text = "File";
			this.file.UseVisualStyleBackColor = true;
			this.file.CheckedChanged += new System.EventHandler(this.data_CheckedChanged);
			// 
			// folder
			// 
			this.folder.AutoSize = true;
			this.folder.Location = new System.Drawing.Point(8, 98);
			this.folder.Name = "folder";
			this.folder.Size = new System.Drawing.Size(95, 19);
			this.folder.TabIndex = 6;
			this.folder.TabStop = true;
			this.folder.Text = "Files in folder";
			this.folder.UseVisualStyleBackColor = true;
			this.folder.CheckedChanged += new System.EventHandler(this.data_CheckedChanged);
			// 
			// unused
			// 
			this.unused.AutoSize = true;
			this.unused.Location = new System.Drawing.Point(8, 235);
			this.unused.Name = "unused";
			this.unused.Size = new System.Drawing.Size(122, 19);
			this.unused.TabIndex = 14;
			this.unused.TabStop = true;
			this.unused.Text = "Unused disk space";
			this.unused.UseVisualStyleBackColor = true;
			this.unused.CheckedChanged += new System.EventHandler(this.data_CheckedChanged);
			// 
			// filePath
			// 
			this.filePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.filePath.Location = new System.Drawing.Point(31, 69);
			this.filePath.Name = "filePath";
			this.filePath.Size = new System.Drawing.Size(206, 23);
			this.filePath.TabIndex = 4;
			// 
			// fileBrowse
			// 
			this.fileBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.fileBrowse.Location = new System.Drawing.Point(243, 68);
			this.fileBrowse.Name = "fileBrowse";
			this.fileBrowse.Size = new System.Drawing.Size(75, 23);
			this.fileBrowse.TabIndex = 5;
			this.fileBrowse.Text = "Browse...";
			this.fileBrowse.UseVisualStyleBackColor = true;
			this.fileBrowse.Click += new System.EventHandler(this.fileBrowse_Click);
			// 
			// folderPath
			// 
			this.folderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.folderPath.Location = new System.Drawing.Point(31, 123);
			this.folderPath.Name = "folderPath";
			this.folderPath.Size = new System.Drawing.Size(206, 23);
			this.folderPath.TabIndex = 7;
			// 
			// folderBrowse
			// 
			this.folderBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.folderBrowse.Location = new System.Drawing.Point(243, 123);
			this.folderBrowse.Name = "folderBrowse";
			this.folderBrowse.Size = new System.Drawing.Size(75, 23);
			this.folderBrowse.TabIndex = 8;
			this.folderBrowse.Text = "Browse...";
			this.folderBrowse.UseVisualStyleBackColor = true;
			this.folderBrowse.Click += new System.EventHandler(this.folderBrowse_Click);
			// 
			// folderIncludeLbl
			// 
			this.folderIncludeLbl.AutoSize = true;
			this.folderIncludeLbl.Location = new System.Drawing.Point(28, 155);
			this.folderIncludeLbl.Name = "folderIncludeLbl";
			this.folderIncludeLbl.Size = new System.Drawing.Size(80, 15);
			this.folderIncludeLbl.TabIndex = 9;
			this.folderIncludeLbl.Text = "Include Mask:";
			// 
			// folderInclude
			// 
			this.folderInclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.folderInclude.Location = new System.Drawing.Point(129, 152);
			this.folderInclude.Name = "folderInclude";
			this.folderInclude.Size = new System.Drawing.Size(189, 23);
			this.folderInclude.TabIndex = 10;
			// 
			// folderExcludeLbl
			// 
			this.folderExcludeLbl.AutoSize = true;
			this.folderExcludeLbl.Location = new System.Drawing.Point(28, 184);
			this.folderExcludeLbl.Name = "folderExcludeLbl";
			this.folderExcludeLbl.Size = new System.Drawing.Size(81, 15);
			this.folderExcludeLbl.TabIndex = 11;
			this.folderExcludeLbl.Text = "Exclude Mask:";
			// 
			// folderExclude
			// 
			this.folderExclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.folderExclude.Location = new System.Drawing.Point(129, 181);
			this.folderExclude.Name = "folderExclude";
			this.folderExclude.Size = new System.Drawing.Size(189, 23);
			this.folderExclude.TabIndex = 12;
			// 
			// folderDelete
			// 
			this.folderDelete.AutoSize = true;
			this.folderDelete.Location = new System.Drawing.Point(31, 210);
			this.folderDelete.Name = "folderDelete";
			this.folderDelete.Size = new System.Drawing.Size(140, 19);
			this.folderDelete.TabIndex = 13;
			this.folderDelete.Text = "Delete folder if empty";
			this.folderDelete.UseVisualStyleBackColor = true;
			// 
			// unusedDisk
			// 
			this.unusedDisk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.unusedDisk.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.unusedDisk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.unusedDisk.FormattingEnabled = true;
			this.unusedDisk.Location = new System.Drawing.Point(31, 260);
			this.unusedDisk.Name = "unusedDisk";
			this.unusedDisk.Size = new System.Drawing.Size(287, 24);
			this.unusedDisk.TabIndex = 15;
			this.unusedDisk.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.unusedDisk_DrawItem);
			// 
			// methodLbl
			// 
			this.methodLbl.AutoSize = true;
			this.methodLbl.Location = new System.Drawing.Point(5, 9);
			this.methodLbl.Name = "methodLbl";
			this.methodLbl.Size = new System.Drawing.Size(93, 15);
			this.methodLbl.TabIndex = 1;
			this.methodLbl.Text = "Erasure method:";
			// 
			// method
			// 
			this.method.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.method.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.method.FormattingEnabled = true;
			this.method.Location = new System.Drawing.Point(129, 6);
			this.method.Name = "method";
			this.method.Size = new System.Drawing.Size(189, 23);
			this.method.TabIndex = 2;
			// 
			// ok
			// 
			this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ok.Location = new System.Drawing.Point(162, 312);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(75, 23);
			this.ok.TabIndex = 16;
			this.ok.Text = "OK";
			this.ok.UseVisualStyleBackColor = true;
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(243, 312);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 17;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// fileDialog
			// 
			this.fileDialog.Filter = "All files (*.*)|*.*";
			// 
			// folderDialog
			// 
			this.folderDialog.Description = "Select a folder to erase.";
			this.folderDialog.ShowNewFolderButton = false;
			// 
			// TaskDataSelectionForm
			// 
			this.AcceptButton = this.ok;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(330, 347);
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
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TaskDataSelectionForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Data to Erase";
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
	}
}