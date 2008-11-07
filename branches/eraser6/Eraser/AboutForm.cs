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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using Eraser.Util;

namespace Eraser
{
	public partial class AboutForm : Form
	{
		private Bitmap AboutBitmap;
		private Point AboutBitmapPos;
		private string AboutText;
		private Bitmap AboutTextBitmap;
		private Rectangle AboutTextRect;

		private Bitmap ParentBitmap;
		private int ParentOpacity;
		private int AboutTextScrollTop;

		private Bitmap DoubleBufferBitmap;

		public AboutForm(Control parent)
		{
			//Create and position the dialog
			InitializeComponent();
			ClientSize = new Size(parent.ClientSize.Width, parent.ClientSize.Height);
			Point point = parent.PointToScreen(new Point(0, 0));
			Left = point.X;
			Top = point.Y;

			//Load the localised About Text
			AboutText = S._("The Gutmann method used for overwriting is based on Peter " +
				"Gutmann's paper \"Secure Deletion of Data from Magnetic and Solid-State Memory\".\n" +
				"The Schneier 7-pass method is based on Bruce Schneier's algorithm using a Random Number " +
				"Generator to wipe with random data.\n" +
				"The DoD 3-, 7- and N-pass method is based on the US Department of Defense's \"National " +
				"Industrial Security Program Operating Manual.\n\n" +
				"All the methods are selected to effectively remove magnetic remnants from the drive in a " +
				"secure and easy way.\n\n" +
				"Eraser Development Team:\n" +
				"\u2022 Garrett Trant: Mentor\n" +
				"\u2022 Joel Low: Developer Lead\n" +
				"\u2022 Dennis van Lith: Designer\n" +
				"\u2022 Kasra Nasiri: Developer\n");

			//Create the About bitmap localised for the current version (sans scrolling
			//text) so it can be drawn quickly later.
			AboutBitmap = Properties.Resources.AboutDialog;
			AboutBitmap = AboutBitmap.Clone(new Rectangle(0, 0, AboutBitmap.Width,
				AboutBitmap.Height), PixelFormat.DontCare);
			using (Graphics g = Graphics.FromImage(AboutBitmap))
			{
				//Version number
				Font boldFont = new Font(Font, FontStyle.Bold);
				Brush textBrush = new SolidBrush(Color.White);
				PointF eraserPos = new PointF(168, 80);
				SizeF eraserSize = g.MeasureString(S._("Eraser"), boldFont);
				g.DrawString(S._("Eraser"), boldFont, textBrush, eraserPos);
				g.DrawString(Assembly.GetExecutingAssembly().GetName().Version.ToString(),
					Font, textBrush, new PointF(eraserPos.X + eraserSize.Width, eraserPos.Y));

				//Copyright and Website
				string copyrightText = S._("copyright \u00a9 2008 The Eraser Project");
				PointF copyrightPos = new PointF(eraserPos.X, eraserPos.Y + eraserSize.Height);
				SizeF copyrightSize = g.MeasureString(copyrightText, Font);
				g.DrawString(copyrightText, Font, textBrush, copyrightPos);

				string websiteText = "http://eraser.sourceforge.net/";
				PointF websitePos = new PointF(copyrightPos.X, copyrightPos.Y + copyrightSize.Height);
				SizeF websiteSize = g.MeasureString(websiteText, Font);
				g.DrawString(websiteText, Font, textBrush, websitePos);

				//Open source disclaimer.
				string disclaimerText = S._("Eraser is free open-source software!");
				PointF disclaimerPos = new PointF(websitePos.X, websitePos.Y + websiteSize.Height * 1.5f);
				SizeF disclaimerSize = g.MeasureString(disclaimerText, Font);
				g.DrawString(disclaimerText, Font, textBrush, disclaimerPos);

				//Donation statement
				string donationText = S._("Please help us continue develop Eraser, donate some coffee...");
				PointF donationPos = new PointF(disclaimerPos.X, disclaimerPos.Y + 170);
				SizeF donationSize = g.MeasureString(donationText, Font);
				g.DrawString(donationText, Font, textBrush, donationPos);
			}

			//Calculate the position of the About bitmap
			AboutBitmapPos = new Point((ClientSize.Width - AboutBitmap.Width) / 2,
				(ClientSize.Height - AboutBitmap.Height) / 2);

			//And calculate the bounds of the About Text.
			AboutTextRect = new Rectangle(AboutBitmapPos.X + 19 + 149, AboutBitmapPos.Y + 20 + 147,
				AboutBitmap.Width - 19 - 149 - 20, 130);

			//Create the About Text laid out on screen.
			SizeF aboutTextSize = SizeF.Empty;
			using (Bitmap b = new Bitmap(1, 1))
			using (Graphics g = Graphics.FromImage(b))
				aboutTextSize = g.MeasureString(AboutText, Font, AboutTextRect.Width);
			AboutTextBitmap = new Bitmap(AboutTextRect.Width, (int)aboutTextSize.Height);
			using (Graphics g = Graphics.FromImage(AboutTextBitmap))
			{
				g.Clear(Color.FromArgb(0, 0, 0, 0));
				g.DrawString(AboutText, Font, new SolidBrush(Color.White), new RectangleF(
					0.0f, 0.0f, AboutTextRect.Width, aboutTextSize.Height));
			}

			//Get the parent dialog's screen buffer.
			ParentBitmap = new Bitmap(parent.ClientSize.Width, parent.ClientSize.Height);
			using (Graphics dest = Graphics.FromImage(ParentBitmap))
			{
				parent.Refresh();
				Point parentPos = parent.PointToScreen(new Point(0, 0));
				dest.CopyFromScreen(parentPos, new Point(0, 0), parent.ClientSize);
			}

			ParentOpacity = 0;
			AboutTextScrollTop = AboutTextRect.Height / 2;
			animationTimer_Tick(null, null);
		}

		private void AboutForm_Click(object sender, EventArgs e)
		{
			//This is the only component around, so when clicked, dismiss the
			//dialog.
			Close();
		}

		private void animationTimer_Tick(object sender, EventArgs e)
		{
			if (ParentOpacity <= 128)
				ParentOpacity += 8;
			if (AboutTextBitmap.Height < -AboutTextScrollTop)
			{
				AboutTextScrollTop = AboutTextRect.Height;
				GC.Collect();
			}
			else
				AboutTextScrollTop -= 1;

			DrawComposite();
		}

		private void DrawComposite()
		{
			if (DoubleBufferBitmap == null)
				DoubleBufferBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
			using (Graphics g = Graphics.FromImage(DoubleBufferBitmap))
			{
				//Draw the parent image with a fading out effect
				Brush brush = new SolidBrush(Color.FromArgb(ParentOpacity, 0, 0, 0));
				g.DrawImageUnscaled(ParentBitmap, 0, 0);
				g.FillRectangle(brush, ClientRectangle);

				//Then draw the About bitmap (which we cached in the constructor)
				g.DrawImageUnscaled(AboutBitmap, AboutBitmapPos);

				//And the scrolling text
				g.Clip = new Region(AboutTextRect);
				g.DrawImageUnscaled(AboutTextBitmap, AboutTextRect.Left,
					AboutTextRect.Top + AboutTextScrollTop);
				g.ResetClip();
			}

			using (Graphics g = CreateGraphics())
				g.DrawImageUnscaled(DoubleBufferBitmap, 0, 0);
		}
	}
}
