namespace Eraser
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.contentPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// contentPanel
			// 
			this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.contentPanel.AutoScroll = true;
			this.contentPanel.BackColor = System.Drawing.Color.White;
			this.contentPanel.Location = new System.Drawing.Point(14, 77);
			this.contentPanel.Name = "contentPanel";
			this.contentPanel.Size = new System.Drawing.Size(752, 449);
			this.contentPanel.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(766, 538);
			this.Controls.Add(this.contentPanel);
			this.MinimumSize = new System.Drawing.Size(760, 520);
			this.Name = "MainForm";
			this.Text = "Eraser";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel contentPanel;
	}
}