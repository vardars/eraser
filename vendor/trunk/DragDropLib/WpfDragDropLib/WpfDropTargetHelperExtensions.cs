namespace DragDropLib
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

    public static class WpfDropTargetHelperExtensions
    {
        /// <summary>
        /// Notifies the DragDropHelper that the specified Window received
        /// a DragEnter event.
        /// </summary>
        /// <param name="dropHelper">The DragDropHelper instance to notify.</param>
        /// <param name="window">The Window the received the DragEnter event.</param>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void DragEnter(this IDropTargetHelper dropHelper, Window window, IDataObject data, Point cursorOffset, DragDropEffects effect)
        {
            IntPtr windowHandle = IntPtr.Zero;
            if (window != null)
                windowHandle = (new WindowInteropHelper(window)).Handle;
            Win32Point pt = WpfDragDropLibExtensions.ToWin32Point(cursorOffset);
            dropHelper.DragEnter(windowHandle, (ComIDataObject)data, ref pt, (int)effect);
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Window received
        /// a DragOver event.
        /// </summary>
        /// <param name="dropHelper">The DragDropHelper instance to notify.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void DragOver(this IDropTargetHelper dropHelper, Point cursorOffset, DragDropEffects effect)
        {
            Win32Point pt = WpfDragDropLibExtensions.ToWin32Point(cursorOffset);
            dropHelper.DragOver(ref pt, (int)effect);
        }

        /// <summary>
        /// Notifies the DragDropHelper that the current Window received
        /// a Drop event.
        /// </summary>
        /// <param name="dropHelper">The DragDropHelper instance to notify.</param>
        /// <param name="data">The DataObject containing a drag image.</param>
        /// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
        /// <param name="effect">The accepted drag drop effect.</param>
        public static void Drop(this IDropTargetHelper dropHelper, IDataObject data, Point cursorOffset, DragDropEffects effect)
        {
            Win32Point pt = WpfDragDropLibExtensions.ToWin32Point(cursorOffset);
            dropHelper.Drop((ComIDataObject)data, ref pt, (int)effect);
        }
    }
}
