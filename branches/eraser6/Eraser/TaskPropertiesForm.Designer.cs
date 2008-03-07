namespace Eraser
{
	partial class TaskPropertiesForm
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
			this.nameLbl = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.eraseLbl = new System.Windows.Forms.Label();
			this.typeLbl = new System.Windows.Forms.Label();
			this.typeOneTime = new System.Windows.Forms.RadioButton();
			this.typeRecurring = new System.Windows.Forms.RadioButton();
			this.data = new System.Windows.Forms.ListView();
			this.dataColData = new System.Windows.Forms.ColumnHeader();
			this.dataColMethod = new System.Windows.Forms.ColumnHeader();
			this.dataAdd = new System.Windows.Forms.Button();
			this.ok = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// nameLbl
			// 
			this.nameLbl.AutoSize = true;
			this.nameLbl.Location = new System.Drawing.Point(12, 9);
			this.nameLbl.Name = "nameLbl";
			this.nameLbl.Size = new System.Drawing.Size(122, 15);
			this.nameLbl.TabIndex = 0;
			this.nameLbl.Text = "Task name (optional):";
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(169, 6);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(237, 23);
			this.textBox1.TabIndex = 1;
			// 
			// eraseLbl
			// 
			this.eraseLbl.AutoSize = true;
			this.eraseLbl.Location = new System.Drawing.Point(12, 59);
			this.eraseLbl.Name = "eraseLbl";
			this.eraseLbl.Size = new System.Drawing.Size(78, 15);
			this.eraseLbl.TabIndex = 2;
			this.eraseLbl.Text = "Data to erase:";
			// 
			// typeLbl
			// 
			this.typeLbl.AutoSize = true;
			this.typeLbl.Location = new System.Drawing.Point(12, 34);
			this.typeLbl.Name = "typeLbl";
			this.typeLbl.Size = new System.Drawing.Size(63, 15);
			this.typeLbl.TabIndex = 4;
			this.typeLbl.Text = "Task Type:";
			// 
			// typeOneTime
			// 
			this.typeOneTime.AutoSize = true;
			this.typeOneTime.Checked = true;
			this.typeOneTime.Location = new System.Drawing.Point(171, 32);
			this.typeOneTime.Name = "typeOneTime";
			this.typeOneTime.Size = new System.Drawing.Size(76, 19);
			this.typeOneTime.TabIndex = 5;
			this.typeOneTime.TabStop = true;
			this.typeOneTime.Text = "One-time";
			this.typeOneTime.UseVisualStyleBackColor = true;
			// 
			// typeRecurring
			// 
			this.typeRecurring.AutoSize = true;
			this.typeRecurring.Location = new System.Drawing.Point(298, 32);
			this.typeRecurring.Name = "typeRecurring";
			this.typeRecurring.Size = new System.Drawing.Size(76, 19);
			this.typeRecurring.TabIndex = 6;
			this.typeRecurring.Text = "Recurring";
			this.typeRecurring.UseVisualStyleBackColor = true;
			// 
			// data
			// 
			this.data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.data.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dataColData,
            this.dataColMethod});
			this.data.Location = new System.Drawing.Point(15, 77);
			this.data.Name = "data";
			this.data.Size = new System.Drawing.Size(391, 196);
			this.data.TabIndex = 7;
			this.data.UseCompatibleStateImageBehavior = false;
			this.data.View = System.Windows.Forms.View.Details;
			// 
			// dataColData
			// 
			this.dataColData.Text = "Data Set";
			this.dataColData.Width = 240;
			// 
			// dataColMethod
			// 
			this.dataColMethod.Text = "Erasure Method";
			this.dataColMethod.Width = 120;
			// 
			// dataAdd
			// 
			this.dataAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.dataAdd.Location = new System.Drawing.Point(15, 279);
			this.dataAdd.Name = "dataAdd";
			this.dataAdd.Size = new System.Drawing.Size(75, 23);
			this.dataAdd.TabIndex = 8;
			this.dataAdd.Text = "Add Data";
			this.dataAdd.UseVisualStyleBackColor = true;
			this.dataAdd.Click += new System.EventHandler(this.dataAdd_Click);
			// 
			// ok
			// 
			this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ok.Location = new System.Drawing.Point(250, 317);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(75, 23);
			this.ok.TabIndex = 9;
			this.ok.Text = "OK";
			this.ok.UseVisualStyleBackColor = true;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(331, 317);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 10;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// TaskPropertiesForm
			// 
			this.AcceptButton = this.ok;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(418, 352);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.dataAdd);
			this.Controls.Add(this.data);
			this.Controls.Add(this.typeRecurring);
			this.Controls.Add(this.typeOneTime);
			this.Controls.Add(this.typeLbl);
			this.Controls.Add(this.eraseLbl);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.nameLbl);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TaskPropertiesForm";
			this.ShowInTaskbar = false;
			this.Text = "Task Properties";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label nameLbl;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label eraseLbl;
		private System.Windows.Forms.Label typeLbl;
		private System.Windows.Forms.RadioButton typeOneTime;
		private System.Windows.Forms.RadioButton typeRecurring;
		private System.Windows.Forms.ListView data;
		private System.Windows.Forms.ColumnHeader dataColData;
		private System.Windows.Forms.ColumnHeader dataColMethod;
		private System.Windows.Forms.Button dataAdd;
		private System.Windows.Forms.Button ok;
		private System.Windows.Forms.Button cancel;
	}
}