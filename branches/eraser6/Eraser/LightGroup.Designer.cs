namespace Eraser
{
	partial class LightGroup
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.container = new System.Windows.Forms.TableLayoutPanel();
			this.label = new System.Windows.Forms.Label();
			this.separator = new Trustbridge.Windows.Controls.BevelLine();
			this.container.SuspendLayout();
			this.SuspendLayout();
			// 
			// container
			// 
			this.container.ColumnCount = 2;
			this.container.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.container.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.container.Controls.Add(this.separator, 0, 2);
			this.container.Controls.Add(this.label, 0, 0);
			this.container.Dock = System.Windows.Forms.DockStyle.Fill;
			this.container.Location = new System.Drawing.Point(0, 0);
			this.container.Name = "container";
			this.container.RowCount = 3;
			this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
			this.container.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75.00188F));
			this.container.Size = new System.Drawing.Size(274, 21);
			this.container.TabIndex = 3;
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(3, 4);
			this.label.Margin = new System.Windows.Forms.Padding(3, 4, 0, 0);
			this.label.Name = "label";
			this.container.SetRowSpan(this.label, 3);
			this.label.Size = new System.Drawing.Size(59, 13);
			this.label.TabIndex = 1;
			this.label.Text = "Group Title";
			// 
			// separator
			// 
			this.separator.Angle = 90;
			this.separator.Dock = System.Windows.Forms.DockStyle.Fill;
			this.separator.Location = new System.Drawing.Point(62, 13);
			this.separator.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this.separator.Name = "separator";
			this.separator.Size = new System.Drawing.Size(212, 2);
			this.separator.TabIndex = 3;
			// 
			// LightGroup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.container);
			this.Name = "LightGroup";
			this.Size = new System.Drawing.Size(274, 21);
			this.container.ResumeLayout(false);
			this.container.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel container;
		protected System.Windows.Forms.Label label;
		private Trustbridge.Windows.Controls.BevelLine separator;

	}
}
