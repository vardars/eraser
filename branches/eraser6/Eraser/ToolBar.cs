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
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace Eraser
{
	public partial class ToolBar : Control
	{
		public ToolBar()
		{
			//Create the base component
			InitializeComponent();

			//Initialize the toolbar item list
			Items = new ToolbarItemCollection(this);

			//Hook mouse move events to show the currently selected item
			MouseMove += new MouseEventHandler(ToolBar_MouseMove);
			MouseLeave += new EventHandler(ToolBar_MouseLeave);
			MouseClick += new MouseEventHandler(ToolBar_MouseClick);
		}

		void ToolBar_MouseMove(object sender, MouseEventArgs e)
		{
			Redraw();
		}

		void ToolBar_MouseLeave(object sender, EventArgs e)
		{
			Redraw();
		}

		void ToolBar_MouseClick(object sender, MouseEventArgs e)
		{
			//See if the click was on any item's arrow.
			Rectangle mouse_rect = new Rectangle(e.Location, new Size(1, 1));
			foreach (ToolBarItem i in Items)
			{
				if (i.Menu != null && mouse_rect.IntersectsWith(i.MenuRect))
				{
					//Show the menu below the toolbar item.
					Point mouse_point = PointToScreen(i.Rectangle.Location);
					i.Menu.Show(mouse_point.X, mouse_point.Y + i.Rectangle.Height);
				}
				else if (mouse_rect.IntersectsWith(i.Rectangle))
				{
					//Broadcast the item click event
					i.OnToolbarItemClicked(this);
				}
			}
		}
		
		/// <summary>
		/// Draws the Tool Bar on the given graphics object.
		/// </summary>
		/// <param name="dc">Graphics object to draw on.</param>
		public void Draw(Graphics rawDC)
		{
			//Create a backing bitmap buffer to prevent flicker
			Bitmap back_bmp = new Bitmap(Width, Height);
			Graphics dc = Graphics.FromImage(back_bmp);

			//Draw the parent background image. This is not portable in that it will render
			//this code unreusable, but for the lack of anything better this will have to suffice!
			dc.DrawImage(Properties.Resources.BackgroundGradient, new Point(-Left, -Top));
			
			Point mouse_pos = PointToClient(MousePosition);
			int x = 0;

			foreach (ToolBarItem i in Items)
			{
				{
					Point pos = i.Rectangle.Location;
					pos.X = x;
					pos.Y = 0;
					i.Rectangle.Location = pos;
				}

				if (i.Bitmap != null)
				{
					i.BitmapRect = new Rectangle(x, 0, i.Bitmap.Width, i.Bitmap.Height);
					dc.DrawImage(i.Bitmap, i.BitmapRect);

					x += i.BitmapRect.Width + 4;
				}

				//Draw the toolbar item text
				SizeF string_size = dc.MeasureString(i.Text, Font);
				i.TextRect = new Rectangle(x, (int)(Height - string_size.Height) / 2,
					(int)string_size.Width, (int)string_size.Height);
				dc.DrawString(i.Text, Font, Brushes.White, i.TextRect.Location);
				x += i.TextRect.Width;
				
				//If there is a menu associated draw a drop-down glyph
				if (i.Menu != null)
				{
					Bitmap menu_arrow = Properties.Resources.ToolbarArrow;
					i.MenuRect = new Rectangle(x += 6, i.TextRect.Y,
						menu_arrow.Width, menu_arrow.Height);
					dc.DrawImage(menu_arrow, i.MenuRect);
					x += i.MenuRect.Width;
				}

				//Update the size of the item rectangle
				{
					Size size = i.Rectangle.Size;
					size.Width = x - i.Rectangle.Location.X;
					size.Height = Height;
					i.Rectangle.Size = size;
				}

				//If the mouse cursor intersects with the item then draw an underline.
				if (i.Rectangle.IntersectsWith(new Rectangle(mouse_pos, new Size(1, 1))))
					dc.DrawLine(Pens.White, new Point(i.TextRect.Left, i.TextRect.Bottom + 1),
						new Point(i.TextRect.Right, i.TextRect.Bottom + 1));

				//Padding between items.
				x += 16;
			}

			rawDC.DrawImage(back_bmp, new Point(0, 0));
		}

		/// <summary>
		/// Redraws the Tool Bar by creating a Graphics object.
		/// </summary>
		public void Redraw()
		{
			Draw(CreateGraphics());
		}

		/// <summary>
		/// Paints the control.
		/// </summary>
		/// <param name="pe">Paint event object.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Draw(e.Graphics);

			// Calling the base class OnPaint
			base.OnPaint(e);
		}

		/// <summary>
		/// Stores the items in the Tool Bar.
		/// </summary>
		public ToolbarItemCollection Items
		{
			get;
			set;
		}
	}

	public class ToolBarItem
	{
		/// <summary>
		/// Tool bar item text.
		/// </summary>
		[Description("Toolbar item text")]
		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				if (Window != null)
					Window.Redraw();
			}
		}

		/// <summary>
		/// Item bitmap.
		/// </summary>
		[Description("Item Bitmap")]
		public Bitmap Bitmap
		{
			get { return bitmap; }
			set
			{
				bitmap = value;
				if (Window != null)
					Window.Redraw();
			}
		}

		/// <summary>
		/// Item drop-down menu
		/// </summary>
		public ContextMenuStrip Menu
		{
			get { return menu; }
			set
			{
				menu = value;
				if (Window != null)
					Window.Redraw();
			}
		}

		/// <summary>
		/// Item click event handler
		/// </summary>
		public EventHandler<EventArgs> ToolBarItemClicked
		{
			get;
			set;
		}

		internal void OnToolbarItemClicked(object sender)
		{
			if (ToolBarItemClicked != null)
				ToolBarItemClicked(sender, new EventArgs());
		}

		private string text;
		private Bitmap bitmap;
		private ContextMenuStrip menu;

		/// <summary>
		/// The owning window of this item.
		/// </summary>
		internal ToolBar Window;

		/// <summary>
		/// Stores the rectangle of this item.
		/// </summary>
		internal Rectangle Rectangle;

		/// <summary>
		/// Stores the rectangle of the bitmap.
		/// </summary>
		internal Rectangle BitmapRect;

		/// <summary>
		/// Stores the rectangle of the text.
		/// </summary>
		internal Rectangle TextRect;

		/// <summary>
		/// Stores the rectangle of the drop-down arrow.
		/// </summary>
		internal Rectangle MenuRect;
	}

	public class ToolbarItemCollection : ICollection<ToolBarItem>, IList<ToolBarItem>,
		IEnumerable<ToolBarItem>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="win">The owning toolbar window.</param>
		internal ToolbarItemCollection(ToolBar win)
		{
			window = win;
		}

		#region ICollection<ToolBarItem> Members
		public void Add(ToolBarItem item)
		{
			if (item.Window != null)
				throw new ArgumentException("The item being added already is owned by " +
					"another ToolBar control. Remove the item from the other control " +
					"before inserting it into this one.");

			item.Window = window;
			list.Add(item);
			window.Redraw();
		}

		public void Clear()
		{
			foreach (ToolBarItem item in list)
				item.Window = null;
			list.Clear();
			window.Redraw();
		}

		public bool Contains(ToolBarItem item)
		{
			return list.Contains(item);
		}

		public void CopyTo(ToolBarItem[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(ToolBarItem item)
		{
			item.Window = null;
			bool result = list.Remove(item);
			window.Redraw();
			return result;
		}
		#endregion

		#region IEnumerable<ToolBarItem> Members
		public IEnumerator<ToolBarItem> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion

		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion

		#region IList<ToolBarItem> Members
		public int IndexOf(ToolBarItem item)
		{
			return list.IndexOf(item);
		}

		public void Insert(int index, ToolBarItem item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public ToolBarItem this[int index]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		/// <summary>
		/// The window owning the items in this list.
		/// </summary>
		private ToolBar window;

		/// <summary>
		/// The list storing the toolbar items.
		/// </summary>
		private List<ToolBarItem> list = new List<ToolBarItem>();
	}
}
