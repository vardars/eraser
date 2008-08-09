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
	partial class BasePanel
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasePanel));
			this.titleLbl = new System.Windows.Forms.Label();
			this.content = new System.Windows.Forms.Panel();
			this.titleIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// titleLbl
			// 
			this.titleLbl.AccessibleDescription = null;
			this.titleLbl.AccessibleName = null;
			resources.ApplyResources(this.titleLbl, "titleLbl");
			this.titleLbl.Name = "titleLbl";
			// 
			// content
			// 
			this.content.AccessibleDescription = null;
			this.content.AccessibleName = null;
			resources.ApplyResources(this.content, "content");
			this.content.BackgroundImage = null;
			this.content.Font = null;
			this.content.Name = "content";
			// 
			// titleIcon
			// 
			this.titleIcon.AccessibleDescription = null;
			this.titleIcon.AccessibleName = null;
			resources.ApplyResources(this.titleIcon, "titleIcon");
			this.titleIcon.BackgroundImage = null;
			this.titleIcon.Font = null;
			this.titleIcon.ImageLocation = null;
			this.titleIcon.Name = "titleIcon";
			this.titleIcon.TabStop = false;
			// 
			// BasePanel
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			resources.ApplyResources(this, "$this");
			this.BackgroundImage = null;
			this.Controls.Add(this.content);
			this.Controls.Add(this.titleIcon);
			this.Controls.Add(this.titleLbl);
			this.Font = null;
			this.Name = "BasePanel";
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected System.Windows.Forms.Label titleLbl;
		protected System.Windows.Forms.PictureBox titleIcon;
		protected System.Windows.Forms.Panel content;
	}
}
