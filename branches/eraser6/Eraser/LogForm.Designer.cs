namespace Eraser
{
	partial class LogForm
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
			this.log = new System.Windows.Forms.ListView();
			this.clear = new System.Windows.Forms.Button();
			this.close = new System.Windows.Forms.Button();
			this.timestamp = new System.Windows.Forms.ColumnHeader();
			this.severity = new System.Windows.Forms.ColumnHeader();
			this.message = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// log
			// 
			this.log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.log.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.timestamp,
            this.severity,
            this.message});
			this.log.FullRowSelect = true;
			this.log.Location = new System.Drawing.Point(12, 12);
			this.log.MultiSelect = false;
			this.log.Name = "log";
			this.log.ShowGroups = false;
			this.log.Size = new System.Drawing.Size(600, 391);
			this.log.TabIndex = 0;
			this.log.UseCompatibleStateImageBehavior = false;
			this.log.View = System.Windows.Forms.View.Details;
			// 
			// clear
			// 
			this.clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.clear.Location = new System.Drawing.Point(456, 409);
			this.clear.Name = "clear";
			this.clear.Size = new System.Drawing.Size(75, 23);
			this.clear.TabIndex = 1;
			this.clear.Text = "Clear Log";
			this.clear.UseVisualStyleBackColor = true;
			this.clear.Click += new System.EventHandler(this.clear_Click);
			// 
			// close
			// 
			this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.close.Location = new System.Drawing.Point(537, 409);
			this.close.Name = "close";
			this.close.Size = new System.Drawing.Size(75, 23);
			this.close.TabIndex = 2;
			this.close.Text = "Close";
			this.close.UseVisualStyleBackColor = true;
			this.close.Click += new System.EventHandler(this.close_Click);
			// 
			// timestamp
			// 
			this.timestamp.Text = "Timestamp";
			this.timestamp.Width = 140;
			// 
			// severity
			// 
			this.severity.Text = "Severity";
			// 
			// message
			// 
			this.message.Text = "Message";
			this.message.Width = 375;
			// 
			// LogForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(624, 444);
			this.Controls.Add(this.close);
			this.Controls.Add(this.clear);
			this.Controls.Add(this.log);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LogForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Log Viewer";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView log;
		private System.Windows.Forms.Button clear;
		private System.Windows.Forms.Button close;
		private System.Windows.Forms.ColumnHeader timestamp;
		private System.Windows.Forms.ColumnHeader severity;
		private System.Windows.Forms.ColumnHeader message;
	}
}