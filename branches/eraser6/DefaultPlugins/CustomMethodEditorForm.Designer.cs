namespace Eraser.DefaultPlugins
{
	partial class CustomMethodEditorForm
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
			this.nameLbl = new System.Windows.Forms.Label();
			this.nameTxt = new System.Windows.Forms.TextBox();
			this.passesLv = new System.Windows.Forms.ListView();
			this.passesColNumber = new System.Windows.Forms.ColumnHeader();
			this.passesColType = new System.Windows.Forms.ColumnHeader();
			this.passesAddBtn = new System.Windows.Forms.Button();
			this.passesRemoveBtn = new System.Windows.Forms.Button();
			this.passesDuplicateBtn = new System.Windows.Forms.Button();
			this.passesMoveUpBtn = new System.Windows.Forms.Button();
			this.passesMoveDownBtn = new System.Windows.Forms.Button();
			this.passGrp = new System.Windows.Forms.GroupBox();
			this.passTxt = new System.Windows.Forms.TextBox();
			this.passTypeGrp = new System.Windows.Forms.FlowLayoutPanel();
			this.passTypeText = new System.Windows.Forms.RadioButton();
			this.passTypeHex = new System.Windows.Forms.RadioButton();
			this.passTypeRandom = new System.Windows.Forms.RadioButton();
			this.randomizeChk = new System.Windows.Forms.CheckBox();
			this.okBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.passGrp.SuspendLayout();
			this.passTypeGrp.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// nameLbl
			// 
			this.nameLbl.AutoSize = true;
			this.nameLbl.Location = new System.Drawing.Point(12, 9);
			this.nameLbl.Name = "nameLbl";
			this.nameLbl.Size = new System.Drawing.Size(42, 15);
			this.nameLbl.TabIndex = 0;
			this.nameLbl.Text = "Name:";
			// 
			// nameTxt
			// 
			this.nameTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nameTxt.Location = new System.Drawing.Point(94, 6);
			this.nameTxt.Name = "nameTxt";
			this.nameTxt.Size = new System.Drawing.Size(268, 23);
			this.nameTxt.TabIndex = 1;
			// 
			// passesLv
			// 
			this.passesLv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.passesLv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.passesColNumber,
            this.passesColType});
			this.passesLv.FullRowSelect = true;
			this.passesLv.HideSelection = false;
			this.passesLv.Location = new System.Drawing.Point(12, 60);
			this.passesLv.MultiSelect = false;
			this.passesLv.Name = "passesLv";
			this.passesLv.Size = new System.Drawing.Size(254, 139);
			this.passesLv.TabIndex = 3;
			this.passesLv.UseCompatibleStateImageBehavior = false;
			this.passesLv.View = System.Windows.Forms.View.Details;
			this.passesLv.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.passesLv_ItemSelectionChanged);
			// 
			// passesColNumber
			// 
			this.passesColNumber.Text = "Pass Number";
			this.passesColNumber.Width = 90;
			// 
			// passesColType
			// 
			this.passesColType.Text = "Data";
			this.passesColType.Width = 135;
			// 
			// passesAddBtn
			// 
			this.passesAddBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.passesAddBtn.Location = new System.Drawing.Point(272, 60);
			this.passesAddBtn.Name = "passesAddBtn";
			this.passesAddBtn.Size = new System.Drawing.Size(90, 23);
			this.passesAddBtn.TabIndex = 4;
			this.passesAddBtn.Text = "Add";
			this.passesAddBtn.UseVisualStyleBackColor = true;
			this.passesAddBtn.Click += new System.EventHandler(this.passesAddBtn_Click);
			// 
			// passesRemoveBtn
			// 
			this.passesRemoveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.passesRemoveBtn.Enabled = false;
			this.passesRemoveBtn.Location = new System.Drawing.Point(272, 89);
			this.passesRemoveBtn.Name = "passesRemoveBtn";
			this.passesRemoveBtn.Size = new System.Drawing.Size(90, 23);
			this.passesRemoveBtn.TabIndex = 5;
			this.passesRemoveBtn.Text = "Remove";
			this.passesRemoveBtn.UseVisualStyleBackColor = true;
			this.passesRemoveBtn.Click += new System.EventHandler(this.passesRemoveBtn_Click);
			// 
			// passesDuplicateBtn
			// 
			this.passesDuplicateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.passesDuplicateBtn.Enabled = false;
			this.passesDuplicateBtn.Location = new System.Drawing.Point(272, 118);
			this.passesDuplicateBtn.Name = "passesDuplicateBtn";
			this.passesDuplicateBtn.Size = new System.Drawing.Size(90, 23);
			this.passesDuplicateBtn.TabIndex = 6;
			this.passesDuplicateBtn.Text = "Duplicate";
			this.passesDuplicateBtn.UseVisualStyleBackColor = true;
			this.passesDuplicateBtn.Click += new System.EventHandler(this.passesDuplicateBtn_Click);
			// 
			// passesMoveUpBtn
			// 
			this.passesMoveUpBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.passesMoveUpBtn.Enabled = false;
			this.passesMoveUpBtn.Location = new System.Drawing.Point(272, 147);
			this.passesMoveUpBtn.Name = "passesMoveUpBtn";
			this.passesMoveUpBtn.Size = new System.Drawing.Size(90, 23);
			this.passesMoveUpBtn.TabIndex = 7;
			this.passesMoveUpBtn.Text = "Move Up";
			this.passesMoveUpBtn.UseVisualStyleBackColor = true;
			this.passesMoveUpBtn.Click += new System.EventHandler(this.passesMoveUpBtn_Click);
			// 
			// passesMoveDownBtn
			// 
			this.passesMoveDownBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.passesMoveDownBtn.Enabled = false;
			this.passesMoveDownBtn.Location = new System.Drawing.Point(272, 176);
			this.passesMoveDownBtn.Name = "passesMoveDownBtn";
			this.passesMoveDownBtn.Size = new System.Drawing.Size(90, 23);
			this.passesMoveDownBtn.TabIndex = 8;
			this.passesMoveDownBtn.Text = "Move Down";
			this.passesMoveDownBtn.UseVisualStyleBackColor = true;
			this.passesMoveDownBtn.Click += new System.EventHandler(this.passesMoveDownBtn_Click);
			// 
			// passGrp
			// 
			this.passGrp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.passGrp.Controls.Add(this.passTxt);
			this.passGrp.Controls.Add(this.passTypeGrp);
			this.passGrp.Enabled = false;
			this.passGrp.Location = new System.Drawing.Point(15, 205);
			this.passGrp.Name = "passGrp";
			this.passGrp.Size = new System.Drawing.Size(347, 142);
			this.passGrp.TabIndex = 9;
			this.passGrp.TabStop = false;
			this.passGrp.Text = "Pass Data";
			// 
			// passTxt
			// 
			this.passTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.passTxt.Enabled = false;
			this.passTxt.Location = new System.Drawing.Point(9, 50);
			this.passTxt.Multiline = true;
			this.passTxt.Name = "passTxt";
			this.passTxt.Size = new System.Drawing.Size(332, 86);
			this.passTxt.TabIndex = 4;
			// 
			// passTypeGrp
			// 
			this.passTypeGrp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.passTypeGrp.Controls.Add(this.passTypeText);
			this.passTypeGrp.Controls.Add(this.passTypeHex);
			this.passTypeGrp.Controls.Add(this.passTypeRandom);
			this.passTypeGrp.Location = new System.Drawing.Point(6, 17);
			this.passTypeGrp.Name = "passTypeGrp";
			this.passTypeGrp.Size = new System.Drawing.Size(335, 27);
			this.passTypeGrp.TabIndex = 3;
			// 
			// passTypeText
			// 
			this.passTypeText.AutoSize = true;
			this.passTypeText.Checked = true;
			this.passTypeText.Enabled = false;
			this.passTypeText.Location = new System.Drawing.Point(3, 3);
			this.passTypeText.Name = "passTypeText";
			this.passTypeText.Size = new System.Drawing.Size(47, 19);
			this.passTypeText.TabIndex = 0;
			this.passTypeText.TabStop = true;
			this.passTypeText.Text = "Text";
			this.passTypeText.UseVisualStyleBackColor = true;
			this.passTypeText.CheckedChanged += new System.EventHandler(this.passType_CheckedChanged);
			// 
			// passTypeHex
			// 
			this.passTypeHex.AutoSize = true;
			this.passTypeHex.Enabled = false;
			this.passTypeHex.Location = new System.Drawing.Point(56, 3);
			this.passTypeHex.Name = "passTypeHex";
			this.passTypeHex.Size = new System.Drawing.Size(93, 19);
			this.passTypeHex.TabIndex = 1;
			this.passTypeHex.Text = "Hexadecimal";
			this.passTypeHex.UseVisualStyleBackColor = true;
			this.passTypeHex.CheckedChanged += new System.EventHandler(this.passType_CheckedChanged);
			// 
			// passTypeRandom
			// 
			this.passTypeRandom.AutoSize = true;
			this.passTypeRandom.Enabled = false;
			this.passTypeRandom.Location = new System.Drawing.Point(155, 3);
			this.passTypeRandom.Name = "passTypeRandom";
			this.passTypeRandom.Size = new System.Drawing.Size(70, 19);
			this.passTypeRandom.TabIndex = 2;
			this.passTypeRandom.Text = "Random";
			this.passTypeRandom.UseVisualStyleBackColor = true;
			this.passTypeRandom.CheckedChanged += new System.EventHandler(this.passType_CheckedChanged);
			// 
			// randomizeChk
			// 
			this.randomizeChk.AutoSize = true;
			this.randomizeChk.Location = new System.Drawing.Point(15, 35);
			this.randomizeChk.Name = "randomizeChk";
			this.randomizeChk.Size = new System.Drawing.Size(142, 19);
			this.randomizeChk.TabIndex = 3;
			this.randomizeChk.Text = "Randomize pass order";
			this.randomizeChk.UseVisualStyleBackColor = true;
			// 
			// okBtn
			// 
			this.okBtn.Location = new System.Drawing.Point(206, 353);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(75, 23);
			this.okBtn.TabIndex = 10;
			this.okBtn.Text = "OK";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// cancelBtn
			// 
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(287, 353);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(75, 23);
			this.cancelBtn.TabIndex = 11;
			this.cancelBtn.Text = "Cancel";
			this.cancelBtn.UseVisualStyleBackColor = true;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// CustomMethodEditorForm
			// 
			this.AcceptButton = this.okBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancelBtn;
			this.ClientSize = new System.Drawing.Size(374, 388);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.randomizeChk);
			this.Controls.Add(this.passGrp);
			this.Controls.Add(this.passesMoveDownBtn);
			this.Controls.Add(this.passesMoveUpBtn);
			this.Controls.Add(this.passesDuplicateBtn);
			this.Controls.Add(this.passesRemoveBtn);
			this.Controls.Add(this.passesAddBtn);
			this.Controls.Add(this.passesLv);
			this.Controls.Add(this.nameTxt);
			this.Controls.Add(this.nameLbl);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CustomMethodEditorForm";
			this.ShowInTaskbar = false;
			this.Text = "Custom Erasure Method Editor";
			this.passGrp.ResumeLayout(false);
			this.passGrp.PerformLayout();
			this.passTypeGrp.ResumeLayout(false);
			this.passTypeGrp.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label nameLbl;
		private System.Windows.Forms.TextBox nameTxt;
		private System.Windows.Forms.ListView passesLv;
		private System.Windows.Forms.Button passesAddBtn;
		private System.Windows.Forms.Button passesRemoveBtn;
		private System.Windows.Forms.Button passesDuplicateBtn;
		private System.Windows.Forms.Button passesMoveUpBtn;
		private System.Windows.Forms.Button passesMoveDownBtn;
		private System.Windows.Forms.GroupBox passGrp;
		private System.Windows.Forms.FlowLayoutPanel passTypeGrp;
		private System.Windows.Forms.RadioButton passTypeText;
		private System.Windows.Forms.RadioButton passTypeHex;
		private System.Windows.Forms.RadioButton passTypeRandom;
		private System.Windows.Forms.CheckBox randomizeChk;
		private System.Windows.Forms.ColumnHeader passesColNumber;
		private System.Windows.Forms.ColumnHeader passesColType;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.TextBox passTxt;
		private System.Windows.Forms.ErrorProvider errorProvider;
	}
}