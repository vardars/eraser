// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Runtime.Hosting {

    /// <summary>
    /// A set of common, useful HRESULTS, and related functionality
    /// </summary>
    public static class HResult {
        /// <summary>
        /// OK/true/Success
        /// </summary>
        public static readonly int S_OK = 0;
        /// <summary>
        /// False
        /// </summary>
        public static readonly int S_FALSE = 1;
        /// <summary>
        /// The method is not implemented
        /// </summary>
        public static readonly int E_NOTIMPL = unchecked((int)0x80004001);
        /// <summary>
        /// The interface is not supported
        /// </summary>
        public static readonly int E_NOINTERFACE = unchecked((int)0x80004002);
        /// <summary>
        /// Bad Pointer
        /// </summary>
        public static readonly int E_POINTER = unchecked((int)0x8004003);
        /// <summary>
        /// General failure HRESULT
        /// </summary>
        public static readonly int E_FAIL = unchecked((int)0x8004005);
        /// <summary>
        /// Invalid Argument
        /// </summary>
        public static readonly int E_INVALIDARG = unchecked((int)0x80070057);
        /// <summary>
        /// Insufficient buffer
        /// </summary>
        public static readonly int ERROR_INSUFFICIENT_BUFFER = unchecked((int)0x8007007A);
        /// <summary>
        /// HRESULT for failure to find or load an appropriate runtime
        /// </summary>
        public static readonly int CLR_E_SHIM_RUNTIMELOAD = unchecked((int)0x80131700);

        private static readonly int SEVERITY = unchecked((int)0x80000000);

        /// <summary>
        /// Indicates whether an HRESULT value is a success based on its severity
        /// </summary>
        /// <param name="hr"></param>
        /// <returns></returns>
        public static bool IsSuccess(int hr) {
            return !IsFailure(hr);
        }

        /// <summary>
        /// Indicates whether an HRESULT value is a failure based on its severity
        /// </summary>
        /// <param name="hr"></param>
        /// <returns></returns>
        public static bool IsFailure(int hr) {
            return (hr & SEVERITY) == SEVERITY;
        }

    }
}
