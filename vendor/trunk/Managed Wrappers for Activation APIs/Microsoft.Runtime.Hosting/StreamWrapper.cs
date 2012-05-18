// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Runtime.Hosting.Interop;
using System.IO;

namespace Microsoft.Runtime.Hosting {

    /// <summary>
    /// An internal wrapper for a stream that provides the IStream interface used by the metahost APIs.
    /// </summary>
    /// <remarks>
    /// It only implements the Read operation
    /// </remarks>
    class StreamWrapper : IStream, IDisposable {

        Stream _Stream;
        bool _ShouldDispose;

        public StreamWrapper(Stream stream, bool ownsStream) {
            _Stream = stream;
            _ShouldDispose = ownsStream;
        }

        #region IStream Members

        public void Read(byte[] pv, int cb, out int pcbRead) {
            pcbRead = _Stream.Read(pv, 0, cb);
        }

        public void Write(byte[] pv, int cb, out int pcbWritten) {
            throw new NotImplementedException();
        }

        public void Seek(long dlibMove, int dwOrigin, out ulong plibNewPosition) {
            throw new NotImplementedException();
        }

        public void SetSize(long libNewSize) {
            throw new NotImplementedException();
        }

        public void CopyTo(IStream pstm, ulong cb, out ulong pcbRead, out ulong pcbWritten) {
            throw new NotImplementedException();
        }

        public void Commit(int grfCommitFlags) {
            throw new NotImplementedException();
        }

        public void Revert() {
            throw new NotImplementedException();
        }

        public void LockRegion(long libOffset, long cb, int dwLockType) {
            throw new NotImplementedException();
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType) {
            throw new NotImplementedException();
        }

        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag) {
            throw new NotImplementedException();
        }

        public void Clone(out IStream ppstm) {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            if (_ShouldDispose) _Stream.Dispose();
        }

        #endregion
    }
}
