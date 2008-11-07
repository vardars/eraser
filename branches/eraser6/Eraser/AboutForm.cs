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
using System.Text;
using System.Windows.Forms;
using Eraser.Util;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Eraser
{
	public partial class AboutForm : Form
	{
		private Bitmap ParentBitmap;
		private int ParentBitmapOpacity;
		private Bitmap BaseBitmap;
		private readonly string AboutText = "The Gutmann method used for overwriting is based on Peter " +
			"Gutmann's paper \"Secure Deletion of Data from Magnetic and Solid-State Memory\".\n" +
			"The Schneier 7-pass method is based on Bruce Schneier's algorithm using a Random Number " +
			"Generator to wipe with random data.\n" +
			"The DoD 3-, 7- and N-pass method is based on the US Department of Defense's \"National " +
			"Industrial Security Program Operating Manual.\n\n" +
			"All the methods are selected to effectively remove magnetic remnants from the drive in a " +
			"secure and easy way.\n\n" +
			"Eraser Development Team:\n" +
			"\u2022 Joel Low: v6 Lead Developer\n" +
			"\u2022 Dennis van Lith: Designer\n" +
			"\u2022 Kasra Nasiri: Developer\n" +
			"\u2022 Garrett Trant: Mentor\n";
		private Bitmap AboutBitmap;
		private SizeF AboutBitmapSize;

		public AboutForm(Control parent)
		{
			//Get the parent dialog's screen buffer.
			ParentBitmap = new Bitmap(parent.ClientSize.Width, parent.ClientSize.Height);
			using (Graphics dest = Graphics.FromImage(ParentBitmap))
			using (Graphics g = parent.CreateGraphics())
			{
				Point parentPos = parent.PointToScreen(new Point(0, 0));
				dest.CopyFromScreen(parentPos, new Point(0, 0), parent.ClientSize);
			}

			//Create the dialog
			InitializeComponent();
			ClientSize = new Size(parent.ClientSize.Width, parent.ClientSize.Height);
			Point point = parent.PointToScreen(new Point(0, 0));
			Left = point.X;
			Top = point.Y;

			ParentBitmapOpacity = 0;
			BaseBitmap = null;
			fadeTimer_Tick(null, null);
		}

		private void fadeTimer_Tick(object sender, EventArgs e)
		{
			//Darken the thing.
			Rectangle rect = new Rectangle(0, 0, ParentBitmap.Width, ParentBitmap.Height);
			Bitmap newBmp = ParentBitmap.Clone(rect, System.Drawing.Imaging.PixelFormat.DontCare);
			using (Graphics g = Graphics.FromImage(newBmp))
			{
				Brush brush = new SolidBrush(Color.FromArgb(ParentBitmapOpacity += 8, 0, 0, 0));
				g.FillRectangle(brush, rect);

				//Draw the background bitmap
				Point boxPos = new Point((ClientSize.Width - Properties.Resources.AboutDialog.Width) / 2,
					(ClientSize.Height - Properties.Resources.AboutDialog.Height) / 2);
				g.DrawImage(Properties.Resources.AboutDialog, boxPos.X, boxPos.Y);
				boxPos.Offset(19, 20);

				//Version number
				Font boldFont = new Font(Font, FontStyle.Bold);
				Brush textBrush = new SolidBrush(Color.White);
				PointF eraserPos = new PointF(boxPos.X + 149, boxPos.Y + 60);
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

				//About text: Break it up into managable chunks.
				if (AboutBitmap == null)
				{
					Size textMaxSize = Properties.Resources.AboutDialog.Size;
					textMaxSize.Width -= (int)eraserPos.X - 42; //The left alignment and the padding
					AboutBitmap = new Bitmap(textMaxSize.Width, 500);
					using (Graphics aboutTextGraphics = Graphics.FromImage(AboutBitmap))
					{
						aboutTextGraphics.Clear(Color.FromArgb(0, 0, 0, 0));
						AboutBitmapSize = DrawMultilineString(aboutTextGraphics, AboutText, Font,
							textBrush, new PointF(0, 0), textMaxSize.Width);
					}
				}
				
				//Donation statement
				string donationText = S._("Please help us continue develop Eraser, donate some coffee...");
				PointF donationPos = new PointF(disclaimerPos.X, disclaimerPos.Y + 170);
				SizeF donationSize = g.MeasureString(donationText, Font);
				g.DrawString(donationText, Font, textBrush, donationPos);
			}

			aboutPicBox.Image = newBmp;
			if (ParentBitmapOpacity >= 128)
			{
				//We are at the end of the fading animation. Duplicate the backing bitmap
				//so that we can start text scrolling.
				BaseBitmap = newBmp;
				fadeTimer.Enabled = false;
				scrollTimer.Enabled = true;
				GC.Collect();
			}
		}

		private int top = 0;
		private void scrollTimer_Tick(object sender, EventArgs e)
		{
			Bitmap workingBitmap = BaseBitmap.Clone(new Rectangle(0, 0, BaseBitmap.Width,
				BaseBitmap.Height), System.Drawing.Imaging.PixelFormat.DontCare);
			using (Graphics g = Graphics.FromImage(workingBitmap))
			{
				Point boxPos = new Point((ClientSize.Width - Properties.Resources.AboutDialog.Width) / 2,
					(ClientSize.Height - Properties.Resources.AboutDialog.Height) / 2);
				boxPos.Offset(19, 20);
				boxPos.Offset(149, 147);

				g.Clip = new Region(new Rectangle(new Point(boxPos.X, boxPos.Y + 4), new Size(400, 130)));
				if (AboutBitmapSize.Height < -top)
					boxPos.Offset(0, top = 130);
				else
					boxPos.Offset(0, top -= 1);
				g.DrawImage(AboutBitmap, boxPos);
			}
			aboutPicBox.Image = workingBitmap;
		}

		private void AboutForm_Click(object sender, EventArgs e)
		{
			//This is the only component around, so when clicked, dismiss the
			//dialog.
			Close();
		}

		private static SizeF DrawMultilineString(Graphics g, string s, Font font,
			Brush brush, PointF pos, float wrapWidth)
		{
			List<string> aboutTexts = new List<string>();
			for (int i = 0, lastStart = 0; ; )
			{
				if (i >= s.Length)
				{
					if (lastStart < s.Length)
						aboutTexts.Add(s.Substring(lastStart));
					break;
				}
				else if (Environment.NewLine.IndexOf(s[i]) != -1)
				{
					aboutTexts.Add(s.Substring(lastStart, i - lastStart));
					lastStart = ++i;
				}
				else
					++i;
			}

			//Draw each line, one at a time, wrapping it as we go.
			foreach (string line in aboutTexts)
			{
				//Determine where we can wrap.
				List<int> wrapPos = new List<int>();
				for (int last = 0; last != -1; )
				{
					int wrap = line.IndexOfAny(new char[] { ' ', '\t' }, last);
					if (wrap != -1)
						wrapPos.Insert(0, wrap++);
					last = wrap;
				}

				List<string> wrapLines = new List<string>();
				List<int> reuseIndices = new List<int>();
				int lastIndex = 0;
				do
				{
					string thisLine = line.Substring(lastIndex);
					SizeF textSize = g.MeasureString(thisLine, font);
					while (textSize.Width > wrapWidth)
					{
						reuseIndices.Add(wrapPos[0]);
						thisLine = thisLine.Remove(wrapPos[0] - lastIndex);
						textSize = g.MeasureString(thisLine, font);
						wrapPos.RemoveAt(0);
					}
					wrapLines.Add(thisLine);
					lastIndex += thisLine.Length + 1;
					wrapPos.Clear();
					wrapPos.AddRange(reuseIndices);
					reuseIndices.Clear();
				}
				while (lastIndex < line.Length);

				//Then write the strings to screen.
				foreach (string wrapLine in wrapLines)
				{
					SizeF wrapLineSize = g.MeasureString(wrapLine == string.Empty ?
						" " : wrapLine, font);
					g.DrawString(wrapLine, font, brush, pos);
					pos.Y += wrapLineSize.Height;
				}
			}

			return new SizeF(wrapWidth, pos.Y);
		}
	}
}