using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

public partial class DragDropSample : Form
{
    public DragDropSample()
    {
        // This isn't necessary for the controls to allow drop,
        // but for the Form to receive events, which we need so
        // that we can render the drag image, we need to allow
        // drop on the form.
        this.AllowDrop = true;

        InitializeComponent();
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        panel1.Width = ClientRectangle.Width / 2;
        panel3.Height = panel1.Height / 2;
        panel5.Height = panel2.Height / 2;

        base.OnLayout(levent);
    }

    #region Form drag handlers

    //
    // These provide a drag image while dragging over the Form
    //

    protected override void OnDragEnter(DragEventArgs drgevent)
    {
        DropTargetHelper.DragEnter(this, drgevent.Data, new Point(drgevent.X, drgevent.Y), DragDropEffects.None);
        base.OnDragEnter(drgevent);
    }

    protected override void OnDragOver(DragEventArgs drgevent)
    {
        DropTargetHelper.DragOver(new Point(drgevent.X, drgevent.Y), DragDropEffects.None);
        base.OnDragOver(drgevent);
    }

    protected override void OnDragLeave(EventArgs e)
    {
        DropTargetHelper.DragLeave(this);
        base.OnDragLeave(e);
    }

    protected override void OnDragDrop(DragEventArgs drgevent)
    {
        DropTargetHelper.Drop(drgevent.Data, new Point(drgevent.X, drgevent.Y), DragDropEffects.None);
        base.OnDragDrop(drgevent);
    }

    #endregion // Form drag handlers

    #region Drag source handlers

    private void button1_MouseDown(object sender, MouseEventArgs e)
    {
        // Normally, you'd detect a drag by if the mouse has moved a minimum distance
        // as defined by the system. In .NET 3.0 you can get that info from the
        // SystemParameters class, I'm not sure where it is in .NET 2.0.

        // Remember! You can use any Bitmap you want to use to initialize the drag image
        // by calling the appropriate override. I am just using the control itself.

        DragSourceHelper.DoDragDrop(button1, new Point(e.X, e.Y), DragDropEffects.Link | DragDropEffects.Copy,
            new KeyValuePair<string, object>(DataFormats.Text, "Hello Drag and Drop!"),
            new KeyValuePair<string, object>(DataFormats.Html, "<h4>Hello Drag and Drop!</h4>"));
    }

    #endregion // Drag source handlers

    #region Drop target handlers

    #region Drop target accepting FileDrop

    private void textBox2_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = e.AllowedEffect & DragDropEffects.Copy;
            DropTargetHelper.DragEnter(textBox2, e.Data, new Point(e.X, e.Y), e.Effect, "Copy to %1", "Here");
        }
        else
        {
            e.Effect = DragDropEffects.None;
            DropTargetHelper.DragEnter(textBox2, e.Data, new Point(e.X, e.Y), e.Effect);
        }
    }

    private void textBox2_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = e.AllowedEffect & DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
        DropTargetHelper.DragOver(new Point(e.X, e.Y), e.Effect);
    }

    private void textBox2_DragLeave(object sender, EventArgs e)
    {
        DropTargetHelper.DragLeave(textBox2);
    }

    private void textBox2_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = e.AllowedEffect & DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
        DropTargetHelper.Drop(e.Data, new Point(e.X, e.Y), e.Effect);

        if (e.Effect == DragDropEffects.Copy)
            AcceptFileDrop(textBox2, e.Data);
    }

    #endregion // Drop target accepting FileDrop

    #region Drop target accepting Text

    private void textBox1_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.Text))
            e.Effect = e.AllowedEffect & DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
        DropTargetHelper.DragEnter(textBox1, e.Data, new Point(e.X, e.Y), e.Effect);
    }

    private void textBox1_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.Text))
            e.Effect = e.AllowedEffect & DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
        DropTargetHelper.DragOver(new Point(e.X, e.Y), e.Effect);
    }

    private void textBox1_DragLeave(object sender, EventArgs e)
    {
        DropTargetHelper.DragLeave(textBox1);
    }

    private void textBox1_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.Text))
            e.Effect = e.AllowedEffect & DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
        DropTargetHelper.Drop(e.Data, new Point(e.X, e.Y), e.Effect);

        if (e.Effect == DragDropEffects.Copy)
            AcceptText(textBox1, e.Data);
    }

    #endregion // Drop target accepting Text

    #region Drop target accepting HTML

    private void textBox3_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.Html))
            e.Effect = e.AllowedEffect & DragDropEffects.Link;
        else
            e.Effect = DragDropEffects.None;
    }

    private void textBox3_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.Html))
            e.Effect = e.AllowedEffect & DragDropEffects.Link;
        else
            e.Effect = DragDropEffects.None;
    }

    private void textBox3_DragLeave(object sender, EventArgs e)
    {
    }

    private void textBox3_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.Html))
            e.Effect = e.AllowedEffect & DragDropEffects.Link;
        else
            e.Effect = DragDropEffects.None;

        if (e.Effect == DragDropEffects.Link)
            AcceptHtml(textBox3, e.Data);
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
                tb.Select(tb.TextLength, 0);
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
                tb.Select(tb.TextLength, 0);
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
                    tb.Select(tb.TextLength, 0);
                    tb.SelectedText = file + Environment.NewLine;
                }
            }
        }
    }

    #endregion // Helpers
}
