namespace Eraser.DefaultPlugins
{
	partial class SettingsForm
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
			this.fl16MethodLbl = new System.Windows.Forms.Label();
			this.fl16MethodCmb = new System.Windows.Forms.ComboBox();
			this.customPassGrp = new System.Windows.Forms.GroupBox();
			this.customPass = new System.Windows.Forms.ListView();
			this.okBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.customPassGrp.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// fl16MethodLbl
			// 
			this.fl16MethodLbl.AutoSize = true;
			this.fl16MethodLbl.Location = new System.Drawing.Point(12, 9);
			this.fl16MethodLbl.Name = "fl16MethodLbl";
			this.fl16MethodLbl.Size = new System.Drawing.Size(189, 15);
			this.fl16MethodLbl.TabIndex = 0;
			this.fl16MethodLbl.Text = "Erasure method for first/last 16 KB:";
			// 
			// fl16MethodCmb
			// 
			this.fl16MethodCmb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.fl16MethodCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.fl16MethodCmb.FormattingEnabled = true;
			this.fl16MethodCmb.Location = new System.Drawing.Point(31, 27);
			this.fl16MethodCmb.Name = "fl16MethodCmb";
			this.fl16MethodCmb.Size = new System.Drawing.Size(321, 23);
			this.fl16MethodCmb.TabIndex = 1;
			// 
			// customPassGrp
			// 
			this.customPassGrp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.customPassGrp.Controls.Add(this.customPass);
			this.customPassGrp.Location = new System.Drawing.Point(15, 56);
			this.customPassGrp.Name = "customPassGrp";
			this.customPassGrp.Size = new System.Drawing.Size(337, 315);
			this.customPassGrp.TabIndex = 3;
			this.customPassGrp.TabStop = false;
			this.customPassGrp.Text = "Custom Erasure Methods";
			// 
			// customPass
			// 
			this.customPass.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.customPass.Location = new System.Drawing.Point(6, 22);
			this.customPass.Name = "customPass";
			this.customPass.Size = new System.Drawing.Size(325, 287);
			this.customPass.TabIndex = 0;
			this.customPass.UseCompatibleStateImageBehavior = false;
			// 
			// okBtn
			// 
			this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okBtn.Location = new System.Drawing.Point(196, 377);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(75, 23);
			this.okBtn.TabIndex = 0;
			this.okBtn.Text = "OK";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// cancelBtn
			// 
			this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(277, 377);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(75, 23);
			this.cancelBtn.TabIndex = 1;
			this.cancelBtn.Text = "Cancel";
			this.cancelBtn.UseVisualStyleBackColor = true;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// SettingsForm
			// 
			this.AcceptButton = this.okBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancelBtn;
			this.ClientSize = new System.Drawing.Size(364, 412);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.customPassGrp);
			this.Controls.Add(this.fl16MethodCmb);
			this.Controls.Add(this.fl16MethodLbl);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Default Plugin - Settings";
			this.customPassGrp.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label fl16MethodLbl;
		private System.Windows.Forms.ComboBox fl16MethodCmb;
		private System.Windows.Forms.GroupBox customPassGrp;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.ListView customPass;
		private System.Windows.Forms.ErrorProvider errorProvider;
	}
}