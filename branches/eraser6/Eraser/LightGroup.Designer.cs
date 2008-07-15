/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By:
 * 
 * This file is part of Eraser.
 * 
 * Eraser is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 * 
 * Eraser is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

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
			this.separator = new Trustbridge.Windows.Controls.BevelLine();
			this.label = new System.Windows.Forms.Label();
			this.container.SuspendLayout();
			this.SuspendLayout();
			// 
			// container
			// 
			this.container.AutoSize = true;
			this.container.ColumnCount = 2;
			this.container.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.container.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.container.Controls.Add(this.separator, 1, 0);
			this.container.Controls.Add(this.label, 0, 0);
			this.container.Dock = System.Windows.Forms.DockStyle.Fill;
			this.container.Location = new System.Drawing.Point(0, 10);
			this.container.Name = "container";
			this.container.RowCount = 1;
			this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.container.Size = new System.Drawing.Size(274, 23);
			this.container.TabIndex = 3;
			// 
			// separator
			// 
			this.separator.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.separator.Angle = 90;
			this.separator.Location = new System.Drawing.Point(62, 11);
			this.separator.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
			this.separator.Name = "separator";
			this.separator.Size = new System.Drawing.Size(212, 2);
			this.separator.TabIndex = 6;
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label.Location = new System.Drawing.Point(3, 4);
			this.label.Margin = new System.Windows.Forms.Padding(3, 4, 0, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(59, 19);
			this.label.TabIndex = 1;
			this.label.Text = "Group Title";
			// 
			// LightGroup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.AutoSize = true;
			this.Controls.Add(this.container);
			this.Name = "LightGroup";
			this.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
			this.Size = new System.Drawing.Size(274, 38);
			this.container.ResumeLayout(false);
			this.container.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel container;
        private Trustbridge.Windows.Controls.BevelLine separator;
		private System.Windows.Forms.Label label;

	}
}
