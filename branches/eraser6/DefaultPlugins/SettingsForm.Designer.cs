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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
			this.fl16MethodLbl = new System.Windows.Forms.Label();
			this.fl16MethodCmb = new System.Windows.Forms.ComboBox();
			this.customPassGrp = new System.Windows.Forms.GroupBox();
			this.customMethodAdd = new System.Windows.Forms.Button();
			this.customMethod = new System.Windows.Forms.ListView();
			this.customPassName = new System.Windows.Forms.ColumnHeader();
			this.customPassPassCount = new System.Windows.Forms.ColumnHeader();
			this.okBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.customMethodContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteMethodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.customPassGrp.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.customMethodContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// fl16MethodLbl
			// 
			resources.ApplyResources(this.fl16MethodLbl, "fl16MethodLbl");
			this.fl16MethodLbl.Name = "fl16MethodLbl";
			// 
			// fl16MethodCmb
			// 
			resources.ApplyResources(this.fl16MethodCmb, "fl16MethodCmb");
			this.fl16MethodCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.fl16MethodCmb.FormattingEnabled = true;
			this.fl16MethodCmb.Name = "fl16MethodCmb";
			// 
			// customPassGrp
			// 
			resources.ApplyResources(this.customPassGrp, "customPassGrp");
			this.customPassGrp.Controls.Add(this.customMethodAdd);
			this.customPassGrp.Controls.Add(this.customMethod);
			this.customPassGrp.Name = "customPassGrp";
			this.customPassGrp.TabStop = false;
			// 
			// customMethodAdd
			// 
			resources.ApplyResources(this.customMethodAdd, "customMethodAdd");
			this.customMethodAdd.Name = "customMethodAdd";
			this.customMethodAdd.UseVisualStyleBackColor = true;
			this.customMethodAdd.Click += new System.EventHandler(this.customMethodAdd_Click);
			// 
			// customMethod
			// 
			resources.ApplyResources(this.customMethod, "customMethod");
			this.customMethod.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.customPassName,
            this.customPassPassCount});
			this.customMethod.ContextMenuStrip = this.customMethodContextMenuStrip;
			this.customMethod.FullRowSelect = true;
			this.customMethod.MultiSelect = false;
			this.customMethod.Name = "customMethod";
			this.customMethod.UseCompatibleStateImageBehavior = false;
			this.customMethod.View = System.Windows.Forms.View.Details;
			this.customMethod.ItemActivate += new System.EventHandler(this.customMethod_ItemActivate);
			// 
			// customPassName
			// 
			resources.ApplyResources(this.customPassName, "customPassName");
			// 
			// customPassPassCount
			// 
			resources.ApplyResources(this.customPassPassCount, "customPassPassCount");
			// 
			// okBtn
			// 
			resources.ApplyResources(this.okBtn, "okBtn");
			this.okBtn.Name = "okBtn";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// cancelBtn
			// 
			resources.ApplyResources(this.cancelBtn, "cancelBtn");
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.UseVisualStyleBackColor = true;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// customMethodContextMenuStrip
			// 
			this.customMethodContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteMethodToolStripMenuItem});
			this.customMethodContextMenuStrip.Name = "customMethodContextMenuStrip";
			resources.ApplyResources(this.customMethodContextMenuStrip, "customMethodContextMenuStrip");
			this.customMethodContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.customMethodContextMenuStrip_Opening);
			// 
			// deleteMethodToolStripMenuItem
			// 
			this.deleteMethodToolStripMenuItem.Name = "deleteMethodToolStripMenuItem";
			resources.ApplyResources(this.deleteMethodToolStripMenuItem, "deleteMethodToolStripMenuItem");
			this.deleteMethodToolStripMenuItem.Click += new System.EventHandler(this.deleteMethodToolStripMenuItem_Click);
			// 
			// SettingsForm
			// 
			this.AcceptButton = this.okBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancelBtn;
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.customPassGrp);
			this.Controls.Add(this.fl16MethodCmb);
			this.Controls.Add(this.fl16MethodLbl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.ShowInTaskbar = false;
			this.customPassGrp.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.customMethodContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label fl16MethodLbl;
		private System.Windows.Forms.ComboBox fl16MethodCmb;
		private System.Windows.Forms.GroupBox customPassGrp;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.ListView customMethod;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ColumnHeader customPassName;
		private System.Windows.Forms.ColumnHeader customPassPassCount;
		private System.Windows.Forms.Button customMethodAdd;
		private System.Windows.Forms.ContextMenuStrip customMethodContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem deleteMethodToolStripMenuItem;
	}
}
