using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Eraser
{
	public partial class ToolBar : Control
	{
		public ToolBar()
		{
			//Create the base component
			InitializeComponent();

			//Initialize the toolbar item list
			items = new List<ToolBarItem>();

			//Hook mouse move events to show the currently selected item
			MouseMove += new MouseEventHandler(ToolBar_MouseMove);
			MouseLeave += new EventHandler(ToolBar_MouseLeave);
			MouseClick += new MouseEventHandler(ToolBar_MouseClick);
		}

		[DllImport("user32.dll")]
		extern static int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y,
			int nReserved, IntPtr hWnd, IntPtr prcRect);

		[DllImport("user32.dll")]
		extern static int GetSystemMetrics(int nIndex);

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
			foreach (ToolBarItem i in items)
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
		public void Draw(Graphics sdc)
		{
			//Create a backing bitmap buffer to prevent flicker
			Bitmap back_bmp = new Bitmap(Width, Height);
			Graphics dc = Graphics.FromImage(back_bmp);

			//Draw the parent background image. This is not portable in that it will render
			//this code unreusable, but for the lack of anything better this will have to suffice!
			dc.DrawImage(Properties.Resources.BackgroundGradient, new Point(-Left, -Top));
			
			Point mouse_pos = PointToClient(MousePosition);
			int x = 0;

			foreach (ToolBarItem i in items)
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
					i.MenuRect = new Rectangle(x += 6, (Height - menu_arrow.Height) / 2,
						menu_arrow.Width, menu_arrow.Height);
					dc.DrawImage(menu_arrow, i.MenuRect.Location);
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

			sdc.DrawImage(back_bmp, new Point(0, 0));
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
		protected override void OnPaint(PaintEventArgs pe)
		{
			// TODO: Add custom paint code here
			Draw(pe.Graphics);

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}

		/// <summary>
		/// Stores the items in the Tool Bar.
		/// </summary>
		private List<ToolBarItem> items;

		/// <summary>
		/// Accesses or modifies the items in the tool bar.
		/// </summary>
		/// <param name="i">Index of item.</param>
		/// <returns>ToolBarItem describing the item at index i.</returns>
		public ToolBarItem this[int i]
		{
			get
			{
				return items[i];
			}

			set
			{
				items[i] = value;
				Redraw();
			}
		}

		public List<ToolBarItem> Items
		{
			get { return items; }
			set { items = value; }
		}
	}

	public class ToolBarItem
	{
		/// <summary>
		/// Tool bar item text.
		/// </summary>
		[Description("Tool bar item text")]
		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		/// <summary>
		/// Item bitmap.
		/// </summary>
		[Description("Item Bitmap")]
		public Bitmap Bitmap
		{
			get { return bitmap; }
			set { bitmap = value; }
		}

		/// <summary>
		/// Item drop-down menu
		/// </summary>
		public ContextMenuStrip Menu
		{
			get { return menu; }
			set { menu = value; }
		}

		/// <summary>
		/// Item click event handler
		/// </summary>
		public ToolbarItemClickedHandler ToolbarItemClicked
		{
			get { return toolbarItemClicked; }
			set { toolbarItemClicked = value; }
		}

		public delegate void ToolbarItemClickedHandler(object sender, EventArgs e);
		private event ToolbarItemClickedHandler toolbarItemClicked;
		internal void OnToolbarItemClicked(object sender)
		{
			if (ToolbarItemClicked != null)
				ToolbarItemClicked(sender, new EventArgs());
		}

		private string text;
		private Bitmap bitmap;
		private ContextMenuStrip menu;

		/// <summary>
		/// Stores the rectangle of this item.
		/// </summary>
		internal Rectangle Rectangle = new Rectangle();

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
}
