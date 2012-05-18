// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.Runtime.Hosting {

    /// <summary>
    /// A class that helps do interop calls that take/return buffers.
    /// </summary>
    /// <remarks>
    /// This holds onto a StringBuilder and an int that represent the
    /// arguments typically passed to such functions.
    /// DoBufferAction's overloads contain the necessary buffer sizing
    /// logic for doing calls that take string buffers.
    /// </remarks>
    class BufferData {
        StringBuilder Buffer = null;

        /// <summary>
        /// The length of the buffer, exposed so that it can be passed by ref
        /// </summary>
        public int Length = 0;

        /// <summary>
        /// Sets the length based on the current capacity of the buffer
        /// </summary>
        void SetLength() {
            Length = Buffer == null ? 0 : Buffer.Capacity;
        }

        /// <summary>
        /// Creates a buffer with a default initial capacity. (based on <see cref="StringBuilder"/>'s defaults)
        /// </summary>
        public BufferData() {
            Buffer = new StringBuilder();
            SetLength();
        }

        /// <summary>
        /// Creates a buffer with an initial capacity
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer</param>
        public BufferData(int capacity) {
            Buffer = new StringBuilder(capacity);
            SetLength();
        }

        /// <summary>
        /// Creates a buffer containing the specified string and appropriate capacity.
        /// Useful for in/out parameters.
        /// </summary>
        /// <param name="str">The initial contents of the buffer</param>
        public BufferData(string str) {
            Buffer = new StringBuilder(str);
            SetLength();
        }

        /// <summary>
        /// Creates a buffer containing the specified string and with a specified capacity
        /// </summary>
        /// <param name="str">The initial contents of the buffer</param>
        /// <param name="capacity">The initial capacity of the buffer</param>
        public BufferData(string str, int capacity) {
            Buffer = new StringBuilder(str, capacity);
            SetLength();
        }

        /// <summary>
        /// Creates/grows the internal buffer as necessary to accomodate the current <see cref="Length"/>.
        /// </summary>
        /// <returns></returns>
        bool SetCapacity() {
            if (Buffer == null) {
                Buffer = new StringBuilder(Length);
                return true;
            }
            else if (Buffer.Capacity < Length) {
                Buffer.Capacity = Length;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the contents of the internal buffer
        /// </summary>
        /// <returns>The contents of the internal buffer as a string</returns>
        public override string ToString() {
            return Buffer == null ? null : Buffer.ToString();
        }

        /// <summary>
        /// Invokes <paramref name="action"/> and resizes the contents of <paramref name="buffers"/> appropriately
        /// depending on the hresult returned from action and calls <paramref name="action"/> again as necessary.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="buffers"></param>
        public static void DoBufferAction(Func<int> action, params BufferData[] buffers) {
            DoBufferAction(action, () => { }, buffers);
        }

        /// <summary>
        /// Invokes <paramref name="action"/> and resizes the contents of <paramref name="buffers"/> appropriately
        /// depending on the hresult returned from action and calls <paramref name="action"/> again, using
        /// <paramref name="resetAction"/> to get things back in a good state before doing so.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="resetAction"></param>
        /// <param name="buffers"></param>
        public static void DoBufferAction(Func<int> action, Action resetAction, params BufferData[] buffers) {
            int hr = HResult.S_OK;
            while (true) {
                hr = action();
                if (hr == HResult.ERROR_INSUFFICIENT_BUFFER) {
                    var resized = (from buffer in buffers
                                   where buffer.SetCapacity()
                                   select buffer).Count();
                    if (resized == 0) break;
                }
                else {
                    break;
                }
                resetAction();
            }
            Marshal.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Allows BufferData to be passed to callsites expecting <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator StringBuilder(BufferData data) {
            return data.Buffer;
        }
    }
}
