partial class DragDropSample
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
        this.panel1 = new System.Windows.Forms.Panel();
        this.panel4 = new System.Windows.Forms.Panel();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.panel3 = new System.Windows.Forms.Panel();
        this.button1 = new System.Windows.Forms.Button();
        this.panel2 = new System.Windows.Forms.Panel();
        this.panel6 = new System.Windows.Forms.Panel();
        this.textBox3 = new System.Windows.Forms.TextBox();
        this.panel5 = new System.Windows.Forms.Panel();
        this.textBox2 = new System.Windows.Forms.TextBox();
        this.panel1.SuspendLayout();
        this.panel4.SuspendLayout();
        this.panel3.SuspendLayout();
        this.panel2.SuspendLayout();
        this.panel6.SuspendLayout();
        this.panel5.SuspendLayout();
        this.SuspendLayout();
        // 
        // panel1
        // 
        this.panel1.Controls.Add(this.panel4);
        this.panel1.Controls.Add(this.panel3);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
        this.panel1.Location = new System.Drawing.Point(0, 0);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(200, 280);
        this.panel1.TabIndex = 0;
        // 
        // panel4
        // 
        this.panel4.Controls.Add(this.textBox1);
        this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel4.Location = new System.Drawing.Point(0, 100);
        this.panel4.Name = "panel4";
        this.panel4.Padding = new System.Windows.Forms.Padding(10);
        this.panel4.Size = new System.Drawing.Size(200, 180);
        this.panel4.TabIndex = 1;
        // 
        // textBox1
        // 
        this.textBox1.AllowDrop = true;
        this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.textBox1.Location = new System.Drawing.Point(10, 10);
        this.textBox1.Multiline = true;
        this.textBox1.Name = "textBox1";
        this.textBox1.ReadOnly = true;
        this.textBox1.Size = new System.Drawing.Size(180, 160);
        this.textBox1.TabIndex = 0;
        this.textBox1.Text = "Drop Target [Copy]\r\n(Does not set DropDescription)\r\nAccepts: Text\r\n";
        this.textBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox1_DragDrop);
        this.textBox1.DragLeave += new System.EventHandler(this.textBox1_DragLeave);
        this.textBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBox1_DragEnter);
        this.textBox1.DragOver += new System.Windows.Forms.DragEventHandler(this.textBox1_DragOver);
        // 
        // panel3
        // 
        this.panel3.Controls.Add(this.button1);
        this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel3.Location = new System.Drawing.Point(0, 0);
        this.panel3.Name = "panel3";
        this.panel3.Padding = new System.Windows.Forms.Padding(10);
        this.panel3.Size = new System.Drawing.Size(200, 100);
        this.panel3.TabIndex = 0;
        // 
        // button1
        // 
        this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.button1.Location = new System.Drawing.Point(10, 10);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(180, 80);
        this.button1.TabIndex = 0;
        this.button1.Text = "Drag Source";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button1_MouseDown);
        // 
        // panel2
        // 
        this.panel2.Controls.Add(this.panel6);
        this.panel2.Controls.Add(this.panel5);
        this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel2.Location = new System.Drawing.Point(200, 0);
        this.panel2.Name = "panel2";
        this.panel2.Size = new System.Drawing.Size(250, 280);
        this.panel2.TabIndex = 1;
        // 
        // panel6
        // 
        this.panel6.Controls.Add(this.textBox3);
        this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel6.Location = new System.Drawing.Point(0, 100);
        this.panel6.Name = "panel6";
        this.panel6.Padding = new System.Windows.Forms.Padding(10);
        this.panel6.Size = new System.Drawing.Size(250, 180);
        this.panel6.TabIndex = 1;
        // 
        // textBox3
        // 
        this.textBox3.AllowDrop = true;
        this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
        this.textBox3.Location = new System.Drawing.Point(10, 10);
        this.textBox3.Multiline = true;
        this.textBox3.Name = "textBox3";
        this.textBox3.ReadOnly = true;
        this.textBox3.Size = new System.Drawing.Size(230, 160);
        this.textBox3.TabIndex = 1;
        this.textBox3.Text = "Drop Target [Link]\r\n(Does not set DropDescription)\r\n(Does not call IDropTargetHel" +
            "per)\r\nAccepts: HTML\r\n";
        this.textBox3.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox3_DragDrop);
        this.textBox3.DragLeave += new System.EventHandler(this.textBox3_DragLeave);
        this.textBox3.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBox3_DragEnter);
        this.textBox3.DragOver += new System.Windows.Forms.DragEventHandler(this.textBox3_DragOver);
        // 
        // panel5
        // 
        this.panel5.Controls.Add(this.textBox2);
        this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel5.Location = new System.Drawing.Point(0, 0);
        this.panel5.Name = "panel5";
        this.panel5.Padding = new System.Windows.Forms.Padding(10);
        this.panel5.Size = new System.Drawing.Size(250, 100);
        this.panel5.TabIndex = 0;
        // 
        // textBox2
        // 
        this.textBox2.AllowDrop = true;
        this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
        this.textBox2.Location = new System.Drawing.Point(10, 10);
        this.textBox2.Multiline = true;
        this.textBox2.Name = "textBox2";
        this.textBox2.ReadOnly = true;
        this.textBox2.Size = new System.Drawing.Size(230, 80);
        this.textBox2.TabIndex = 1;
        this.textBox2.Text = "Drop Target [Move]\r\n(Sets DropDescription)\r\nAccepts: FileDrop\r\n";
        this.textBox2.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox2_DragDrop);
        this.textBox2.DragLeave += new System.EventHandler(this.textBox2_DragLeave);
        this.textBox2.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBox2_DragEnter);
        this.textBox2.DragOver += new System.Windows.Forms.DragEventHandler(this.textBox2_DragOver);
        // 
        // DragDropSample
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(450, 280);
        this.Controls.Add(this.panel2);
        this.Controls.Add(this.panel1);
        this.Name = "DragDropSample";
        this.Text = "SWF DragDropSample Application";
        this.panel1.ResumeLayout(false);
        this.panel4.ResumeLayout(false);
        this.panel4.PerformLayout();
        this.panel3.ResumeLayout(false);
        this.panel2.ResumeLayout(false);
        this.panel6.ResumeLayout(false);
        this.panel6.PerformLayout();
        this.panel5.ResumeLayout(false);
        this.panel5.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel4;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel panel6;
    private System.Windows.Forms.Panel panel5;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.TextBox textBox2;
}
