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
			this.components = new System.ComponentModel.Container();
			this.contentPanel = new System.Windows.Forms.Panel();
			this.toolbarScheduleMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolbarScheduleMenuQueue = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbarScheduleMenu.SuspendLayout();
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
			// toolbarScheduleMenu
			// 
			this.toolbarScheduleMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbarScheduleMenuQueue});
			this.toolbarScheduleMenu.Name = "toolbarScheduleMenu";
			this.toolbarScheduleMenu.Size = new System.Drawing.Size(153, 48);
			// 
			// toolbarScheduleMenuQueue
			// 
			this.toolbarScheduleMenuQueue.Name = "toolbarScheduleMenuQueue";
			this.toolbarScheduleMenuQueue.Size = new System.Drawing.Size(152, 22);
			this.toolbarScheduleMenuQueue.Text = "Queue Up";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(766, 538);
			this.Controls.Add(this.contentPanel);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new System.Drawing.Size(760, 520);
			this.Name = "MainForm";
			this.Text = "Eraser";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.toolbarScheduleMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel contentPanel;
		private System.Windows.Forms.ContextMenuStrip toolbarScheduleMenu;
		private System.Windows.Forms.ToolStripMenuItem toolbarScheduleMenuQueue;

	}
}