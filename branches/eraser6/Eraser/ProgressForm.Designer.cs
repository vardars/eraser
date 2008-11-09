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
	partial class ProgressForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
			this.overallProgressLbl = new System.Windows.Forms.Label();
			this.overallProgress = new System.Windows.Forms.ProgressBar();
			this.jobTitle = new System.Windows.Forms.Label();
			this.status = new System.Windows.Forms.Label();
			this.statusLbl = new System.Windows.Forms.Label();
			this.itemLbl = new System.Windows.Forms.Label();
			this.item = new System.Windows.Forms.Label();
			this.passLbl = new System.Windows.Forms.Label();
			this.pass = new System.Windows.Forms.Label();
			this.title = new System.Windows.Forms.PictureBox();
			this.titleLbl = new System.Windows.Forms.Label();
			this.itemProgressLbl = new System.Windows.Forms.Label();
			this.itemProgress = new System.Windows.Forms.ProgressBar();
			this.stop = new System.Windows.Forms.Button();
			this.bevelLine1 = new Trustbridge.Windows.Controls.BevelLine();
			this.bevelLine2 = new Trustbridge.Windows.Controls.BevelLine();
			this.timeLeftLbl = new System.Windows.Forms.Label();
			this.timeLeft = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.title)).BeginInit();
			this.SuspendLayout();
			// 
			// overallProgressLbl
			// 
			this.overallProgressLbl.AccessibleDescription = null;
			this.overallProgressLbl.AccessibleName = null;
			resources.ApplyResources(this.overallProgressLbl, "overallProgressLbl");
			this.overallProgressLbl.Font = null;
			this.overallProgressLbl.Name = "overallProgressLbl";
			// 
			// overallProgress
			// 
			this.overallProgress.AccessibleDescription = null;
			this.overallProgress.AccessibleName = null;
			resources.ApplyResources(this.overallProgress, "overallProgress");
			this.overallProgress.BackgroundImage = null;
			this.overallProgress.Font = null;
			this.overallProgress.Maximum = 1000;
			this.overallProgress.Name = "overallProgress";
			// 
			// jobTitle
			// 
			this.jobTitle.AccessibleDescription = null;
			this.jobTitle.AccessibleName = null;
			resources.ApplyResources(this.jobTitle, "jobTitle");
			this.jobTitle.Name = "jobTitle";
			// 
			// status
			// 
			this.status.AccessibleDescription = null;
			this.status.AccessibleName = null;
			resources.ApplyResources(this.status, "status");
			this.status.Name = "status";
			// 
			// statusLbl
			// 
			this.statusLbl.AccessibleDescription = null;
			this.statusLbl.AccessibleName = null;
			resources.ApplyResources(this.statusLbl, "statusLbl");
			this.statusLbl.Font = null;
			this.statusLbl.Name = "statusLbl";
			// 
			// itemLbl
			// 
			this.itemLbl.AccessibleDescription = null;
			this.itemLbl.AccessibleName = null;
			resources.ApplyResources(this.itemLbl, "itemLbl");
			this.itemLbl.Font = null;
			this.itemLbl.Name = "itemLbl";
			// 
			// item
			// 
			this.item.AccessibleDescription = null;
			this.item.AccessibleName = null;
			resources.ApplyResources(this.item, "item");
			this.item.Font = null;
			this.item.Name = "item";
			// 
			// passLbl
			// 
			this.passLbl.AccessibleDescription = null;
			this.passLbl.AccessibleName = null;
			resources.ApplyResources(this.passLbl, "passLbl");
			this.passLbl.Font = null;
			this.passLbl.Name = "passLbl";
			// 
			// pass
			// 
			this.pass.AccessibleDescription = null;
			this.pass.AccessibleName = null;
			resources.ApplyResources(this.pass, "pass");
			this.pass.Font = null;
			this.pass.Name = "pass";
			// 
			// title
			// 
			this.title.AccessibleDescription = null;
			this.title.AccessibleName = null;
			resources.ApplyResources(this.title, "title");
			this.title.BackgroundImage = null;
			this.title.Font = null;
			this.title.ImageLocation = null;
			this.title.Name = "title";
			this.title.TabStop = false;
			// 
			// titleLbl
			// 
			this.titleLbl.AccessibleDescription = null;
			this.titleLbl.AccessibleName = null;
			resources.ApplyResources(this.titleLbl, "titleLbl");
			this.titleLbl.Name = "titleLbl";
			// 
			// itemProgressLbl
			// 
			this.itemProgressLbl.AccessibleDescription = null;
			this.itemProgressLbl.AccessibleName = null;
			resources.ApplyResources(this.itemProgressLbl, "itemProgressLbl");
			this.itemProgressLbl.Font = null;
			this.itemProgressLbl.Name = "itemProgressLbl";
			// 
			// itemProgress
			// 
			this.itemProgress.AccessibleDescription = null;
			this.itemProgress.AccessibleName = null;
			resources.ApplyResources(this.itemProgress, "itemProgress");
			this.itemProgress.BackgroundImage = null;
			this.itemProgress.Font = null;
			this.itemProgress.Maximum = 1000;
			this.itemProgress.Name = "itemProgress";
			// 
			// stop
			// 
			this.stop.AccessibleDescription = null;
			this.stop.AccessibleName = null;
			resources.ApplyResources(this.stop, "stop");
			this.stop.BackgroundImage = null;
			this.stop.Font = null;
			this.stop.Name = "stop";
			this.stop.UseVisualStyleBackColor = true;
			this.stop.Click += new System.EventHandler(this.stop_Click);
			// 
			// bevelLine1
			// 
			this.bevelLine1.AccessibleDescription = null;
			this.bevelLine1.AccessibleName = null;
			resources.ApplyResources(this.bevelLine1, "bevelLine1");
			this.bevelLine1.Angle = 90;
			this.bevelLine1.BackgroundImage = null;
			this.bevelLine1.Font = null;
			this.bevelLine1.Name = "bevelLine1";
			// 
			// bevelLine2
			// 
			this.bevelLine2.AccessibleDescription = null;
			this.bevelLine2.AccessibleName = null;
			resources.ApplyResources(this.bevelLine2, "bevelLine2");
			this.bevelLine2.Angle = 0;
			this.bevelLine2.BackgroundImage = null;
			this.bevelLine2.Font = null;
			this.bevelLine2.Name = "bevelLine2";
			this.bevelLine2.Orientation = System.Windows.Forms.Orientation.Vertical;
			// 
			// timeLeftLbl
			// 
			this.timeLeftLbl.AccessibleDescription = null;
			this.timeLeftLbl.AccessibleName = null;
			resources.ApplyResources(this.timeLeftLbl, "timeLeftLbl");
			this.timeLeftLbl.Font = null;
			this.timeLeftLbl.Name = "timeLeftLbl";
			// 
			// timeLeft
			// 
			this.timeLeft.AccessibleDescription = null;
			this.timeLeft.AccessibleName = null;
			resources.ApplyResources(this.timeLeft, "timeLeft");
			this.timeLeft.Font = null;
			this.timeLeft.Name = "timeLeft";
			// 
			// ProgressForm
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackgroundImage = null;
			this.Controls.Add(this.timeLeft);
			this.Controls.Add(this.timeLeftLbl);
			this.Controls.Add(this.bevelLine2);
			this.Controls.Add(this.bevelLine1);
			this.Controls.Add(this.stop);
			this.Controls.Add(this.itemProgress);
			this.Controls.Add(this.itemProgressLbl);
			this.Controls.Add(this.titleLbl);
			this.Controls.Add(this.title);
			this.Controls.Add(this.pass);
			this.Controls.Add(this.passLbl);
			this.Controls.Add(this.item);
			this.Controls.Add(this.itemLbl);
			this.Controls.Add(this.statusLbl);
			this.Controls.Add(this.status);
			this.Controls.Add(this.jobTitle);
			this.Controls.Add(this.overallProgress);
			this.Controls.Add(this.overallProgressLbl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = null;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressForm";
			this.ShowInTaskbar = false;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProgressForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.title)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label overallProgressLbl;
		private System.Windows.Forms.ProgressBar overallProgress;
		private System.Windows.Forms.Label jobTitle;
		private System.Windows.Forms.Label status;
		private System.Windows.Forms.Label statusLbl;
		private System.Windows.Forms.Label itemLbl;
		private System.Windows.Forms.Label item;
		private System.Windows.Forms.Label passLbl;
		private System.Windows.Forms.Label pass;
		private System.Windows.Forms.PictureBox title;
		private System.Windows.Forms.Label titleLbl;
		private System.Windows.Forms.Label itemProgressLbl;
		private System.Windows.Forms.ProgressBar itemProgress;
		private System.Windows.Forms.Button stop;
		private Trustbridge.Windows.Controls.BevelLine bevelLine1;
		private Trustbridge.Windows.Controls.BevelLine bevelLine2;
		private System.Windows.Forms.Label timeLeftLbl;
		private System.Windows.Forms.Label timeLeft;
	}
}

