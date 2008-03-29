namespace BevelLineTest
{
	partial class Form1
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
			this.bevelLine3 = new Trustbridge.Windows.Controls.BevelLine();
			this.bevelLine2 = new Trustbridge.Windows.Controls.BevelLine();
			this.bevelLine1 = new Trustbridge.Windows.Controls.BevelLine();
			this.bevelLine4 = new Trustbridge.Windows.Controls.BevelLine();
			this.bevelLine5 = new Trustbridge.Windows.Controls.BevelLine();
			this.SuspendLayout();
			// 
			// bevelLine3
			// 
			this.bevelLine3.Angle = 90;
			this.bevelLine3.BevelLineWidth = 5;
			this.bevelLine3.Blend = true;
			this.bevelLine3.Location = new System.Drawing.Point(31, 71);
			this.bevelLine3.Name = "bevelLine3";
			this.bevelLine3.Size = new System.Drawing.Size(149, 10);
			this.bevelLine3.TabIndex = 4;
			this.bevelLine3.Text = "bevelLine3";
			// 
			// bevelLine2
			// 
			this.bevelLine2.Angle = 0;
			this.bevelLine2.Location = new System.Drawing.Point(209, 16);
			this.bevelLine2.Name = "bevelLine2";
			this.bevelLine2.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.bevelLine2.Size = new System.Drawing.Size(2, 149);
			this.bevelLine2.TabIndex = 3;
			this.bevelLine2.Text = "bevelLine2";
			// 
			// bevelLine1
			// 
			this.bevelLine1.Angle = 90;
			this.bevelLine1.Location = new System.Drawing.Point(31, 38);
			this.bevelLine1.Name = "bevelLine1";
			this.bevelLine1.Size = new System.Drawing.Size(149, 2);
			this.bevelLine1.TabIndex = 2;
			this.bevelLine1.Text = "bevelLine1";
			// 
			// bevelLine4
			// 
			this.bevelLine4.Angle = 0;
			this.bevelLine4.BevelLineWidth = 5;
			this.bevelLine4.Blend = true;
			this.bevelLine4.Location = new System.Drawing.Point(244, 16);
			this.bevelLine4.Name = "bevelLine4";
			this.bevelLine4.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.bevelLine4.Size = new System.Drawing.Size(10, 149);
			this.bevelLine4.TabIndex = 5;
			this.bevelLine4.Text = "bevelLine4";
			// 
			// bevelLine5
			// 
			this.bevelLine5.Angle = 90;
			this.bevelLine5.BevelLineWidth = 5;
			this.bevelLine5.Blend = true;
			this.bevelLine5.Location = new System.Drawing.Point(31, 113);
			this.bevelLine5.Name = "bevelLine5";
			this.bevelLine5.Size = new System.Drawing.Size(149, 10);
			this.bevelLine5.TabIndex = 6;
			this.bevelLine5.Text = "bevelLine5";
			this.bevelLine5.TopLineColor = System.Drawing.Color.Green;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 177);
			this.Controls.Add(this.bevelLine5);
			this.Controls.Add(this.bevelLine4);
			this.Controls.Add(this.bevelLine3);
			this.Controls.Add(this.bevelLine2);
			this.Controls.Add(this.bevelLine1);
			this.Name = "Form1";
			this.Text = "Bevel Line Control Test";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private Trustbridge.Windows.Controls.BevelLine bevelLine3;
		private Trustbridge.Windows.Controls.BevelLine bevelLine2;
		private Trustbridge.Windows.Controls.BevelLine bevelLine1;
		private Trustbridge.Windows.Controls.BevelLine bevelLine4;
		private Trustbridge.Windows.Controls.BevelLine bevelLine5;



	}
}

