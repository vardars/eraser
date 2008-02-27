using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Eraser
{
	public partial class MainForm : Form
	{
		private ToolBar toolBar;
		public MainForm()
		{
			InitializeComponent();

			//Create the toolbar control
			toolBar = new ToolBar();
			toolBar.Name = "toolBar";
			toolBar.Location = new Point(14, 27);
			toolBar.Size = new Size(500, 26);
			toolBar.TabIndex = 1;
			this.Controls.Add(this.toolBar);

			ToolBarItem schedule = new ToolBarItem();
			schedule.Bitmap = Properties.Resources.ToolbarSchedule;
			schedule.Text = "Erasing Schedule";
			schedule.Menu = new ContextMenu();
			schedule.Menu.MenuItems.Add("Queue Task");
			toolBar.Items.Add(schedule);

			ToolBarItem settings = new ToolBarItem();
			settings.Bitmap = Properties.Resources.ToolbarSettings;
			settings.Text = "Settings";
			toolBar.Items.Add(settings);

			ToolBarItem help = new ToolBarItem();
			help.Bitmap = Properties.Resources.ToolbarHelp;
			help.Text = "Help";
			toolBar.Items.Add(help);
		}

		private static GraphicsPath CreateRoundRect(float X, float Y, float width,
			float height, float radius)
		{
			GraphicsPath result = new GraphicsPath();

			//Top line.
			result.AddLine(X + radius, Y, X + width - 2 * radius, Y);

			//Top-right corner
			result.AddArc(X + width - 2 * radius, Y, 2 * radius, 2 * radius, 270, 90);

			//Right line.
			result.AddLine(X + width, Y + radius, X + width, Y + height - 2 * radius);

			//Bottom-right corner
			result.AddArc(X + width - 2 * radius, Y + height - 2 * radius, 2 * radius, 2 * radius, 0, 90);

			//Bottom line.
			result.AddLine(X + width - 2 * radius, Y + height, X + radius, Y + height);

			//Bottom-left corner
			result.AddArc(X, Y + height - 2 *radius, 2 * radius, 2 * radius, 90, 90);

			//Left line
			result.AddLine(X, Y + height - 2 * radius, X, Y + radius);

			//Top-left corner
			result.AddArc(X, Y, 2 * radius, 2 * radius, 180, 90);
			result.CloseFigure();

			return result;
		}

		private void DrawBackground(Graphics dc)
		{
			//Draw the base background
			dc.FillRectangle(new SolidBrush(Color.FromArgb(unchecked((int)0xFF292929))),
				new Rectangle(new Point(0, 0), this.Size));

			//Then the side gradient
			dc.FillRectangle(new LinearGradientBrush(new Rectangle(0, 0, 338, Math.Max(1, ClientSize.Height)),
					Color.FromArgb(unchecked((int)0xFF363636)),
					Color.FromArgb(unchecked((int)0xFF292929)), 0.0),
				0, 0, 338, ClientSize.Height);

			//Draw the top background
			dc.FillRectangle(new SolidBrush(Color.FromArgb(unchecked((int)0xFF414141))),
				new Rectangle(0, 0, ClientSize.Width, 32));

			//The top gradient
			dc.DrawImage(Properties.Resources.BackgroundGradient, new Point(0, 0));

			//And the logo
			Bitmap logo = Properties.Resources.BackgroundLogo;
			dc.DrawImage(logo, new Point(ClientSize.Width - logo.Width - 10, 10));

			dc.SmoothingMode = SmoothingMode.AntiAlias;
			dc.FillPath(Brushes.White, CreateRoundRect(11, 74, contentPanel.Width + 8, ClientSize.Height - 85, 3));
		}

		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SetClip(new Rectangle(0, 0, Width, Height), CombineMode.Intersect);
			DrawBackground(e.Graphics);
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			Bitmap bmp = new Bitmap(Width, Height);
			Graphics dc = Graphics.FromImage(bmp);
			DrawBackground(dc);

			CreateGraphics().DrawImage(bmp, new Point(0, 0));
		}
	}
}