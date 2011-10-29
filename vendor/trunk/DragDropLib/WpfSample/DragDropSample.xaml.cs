using System;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DragDropLib;
using Bitmap = System.Drawing.Bitmap;
using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using DataObject = System.Windows.DataObject;
using System.Collections.Generic;

namespace WpfSample
{
    public partial class DragDropSample : Window
    {
        public DragDropSample()
        {
            InitializeComponent();
        }

        #region Window drag handlers

        //
        // These provide a drag image while dragging over the Window
        //

        protected override void OnDragEnter(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            DropTargetHelper.DragEnter(this, e.Data, e.GetPosition(this), DragDropEffects.None);
            e.Handled = true;
            base.OnDragEnter(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            DropTargetHelper.DragOver(e.GetPosition(this), DragDropEffects.None);
            e.Handled = true;
            base.OnDragOver(e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            DropTargetHelper.DragLeave(e.Data);
            e.Handled = true;
            base.OnDragLeave(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            DropTargetHelper.Drop(e.Data, e.GetPosition(this), DragDropEffects.None);
            e.Handled = true;
            base.OnDrop(e);
        }

        #endregion // Window drag handlers

        #region Drag source handlers

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                // Normally, you'd detect a drag by if the mouse has moved a minimum distance
                // as defined by the system. In .NET 3.0 you can get that info from the
                // SystemParameters class, I'm not sure where it is in .NET 2.0.

                // Remember! You can use any Bitmap you want to use to initialize the drag image
                // by calling the appropriate override. I am just using the control itself.

                DragSourceHelper.DoDragDrop(border, e.GetPosition(border), DragDropEffects.Link | DragDropEffects.Copy,
                    new KeyValuePair<string, object>(DataFormats.Text, "Hello Drag and Drop!"),
                    new KeyValuePair<string, object>(DataFormats.Html, "<h4>Hello Drag and Drop!</h4>"));
            }
        }

        #endregion // Drag source handlers

        #region Drop target handlers

        #region Drop target accepting FileDrop

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = e.AllowedEffects & DragDropEffects.Copy;
                DropTargetHelper.DragEnter(this, e.Data, e.GetPosition(this), e.Effects, "Copy to %1", "Here");
            }
            else
            {
                e.Effects = DragDropEffects.None;
                DropTargetHelper.DragEnter(this, e.Data, e.GetPosition(this), e.Effects);
            }
            
            e.Handled = true;
        }

        private void textBox2_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = e.AllowedEffects & DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            DropTargetHelper.DragOver(e.GetPosition(this), e.Effects);

            e.Handled = true;
        }

        private void textBox2_DragLeave(object sender, DragEventArgs e)
        {
            DropTargetHelper.DragLeave(e.Data);

            e.Handled = true;
        }

        private void textBox2_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = e.AllowedEffects & DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            DropTargetHelper.Drop(e.Data, e.GetPosition(this), e.Effects);

            if (e.Effects == DragDropEffects.Copy)
                AcceptFileDrop(textBox2, e.Data);

            e.Handled = true;
        }

        #endregion // Drop target accepting FileDrop

        #region Drop target accepting Text

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effects = e.AllowedEffects & DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            DropTargetHelper.DragEnter(this, e.Data, e.GetPosition(this), e.Effects);

            e.Handled = true;
        }

        private void textBox1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effects = e.AllowedEffects & DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            DropTargetHelper.DragOver(e.GetPosition(this), e.Effects);

            e.Handled = true;
        }

        private void textBox1_DragLeave(object sender, DragEventArgs e)
        {
            DropTargetHelper.DragLeave(e.Data);

            e.Handled = true;
        }

        private void textBox1_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effects = e.AllowedEffects & DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            DropTargetHelper.Drop(e.Data, e.GetPosition(this), e.Effects);

            if (e.Effects == DragDropEffects.Copy)
                AcceptText(textBox1, e.Data);

            e.Handled = true;
        }

        #endregion // Drop target accepting Text

        #region Drop target accepting HTML

        private void textBox3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Html))
                e.Effects = e.AllowedEffects & DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void textBox3_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Html))
                e.Effects = e.AllowedEffects & DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void textBox3_DragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void textBox3_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Html))
                e.Effects = e.AllowedEffects & DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;

            if (e.Effects == DragDropEffects.Link)
                AcceptHtml(textBox3, e.Data);

            e.Handled = true;
        }

        #endregion // Drop target accepting HTML

        #endregion // Drop target handlers

        #region Helpers

        private void AcceptText(TextBox tb, IDataObject data)
        {
            if (data.GetDataPresent(DataFormats.Text))
            {
                string text = data.GetData(DataFormats.Text) as string;
                if (text != null)
                {
                    tb.Select(tb.Text.Length, 0);
                    tb.SelectedText = text + Environment.NewLine;
                }
            }
        }

        private void AcceptHtml(TextBox tb, IDataObject data)
        {
            if (data.GetDataPresent(DataFormats.Html))
            {
                string html = data.GetData(DataFormats.Html) as string;
                if (html != null)
                {
                    tb.Select(tb.Text.Length, 0);
                    tb.SelectedText = html + Environment.NewLine;
                }
            }
        }

        private void AcceptFileDrop(TextBox tb, IDataObject data)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = data.GetData(DataFormats.FileDrop) as string[];
                if (files != null)
                {
                    foreach (string file in files)
                    {
                        tb.Select(tb.Text.Length, 0);
                        tb.SelectedText = file + Environment.NewLine;
                    }
                }
            }
        }

        #endregion // Helpers
    }
}