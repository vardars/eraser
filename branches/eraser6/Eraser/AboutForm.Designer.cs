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
	partial class AboutForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
			this.aboutPicBox = new System.Windows.Forms.PictureBox();
			this.fadeTimer = new System.Windows.Forms.Timer(this.components);
			this.scrollTimer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.aboutPicBox)).BeginInit();
			this.SuspendLayout();
			// 
			// aboutPicBox
			// 
			resources.ApplyResources(this.aboutPicBox, "aboutPicBox");
			this.aboutPicBox.Image = global::Eraser.Properties.Resources.AboutDialog;
			this.aboutPicBox.Name = "aboutPicBox";
			this.aboutPicBox.TabStop = false;
			this.aboutPicBox.Click += new System.EventHandler(this.AboutForm_Click);
			// 
			// fadeTimer
			// 
			this.fadeTimer.Enabled = true;
			this.fadeTimer.Interval = 10;
			this.fadeTimer.Tick += new System.EventHandler(this.fadeTimer_Tick);
			// 
			// scrollTimer
			// 
			this.scrollTimer.Interval = 50;
			this.scrollTimer.Tick += new System.EventHandler(this.scrollTimer_Tick);
			// 
			// AboutForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.aboutPicBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.aboutPicBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox aboutPicBox;
		private System.Windows.Forms.Timer fadeTimer;
		private System.Windows.Forms.Timer scrollTimer;
	}
}