// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Security;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Runtime.Hosting.Interop;

namespace Microsoft.Runtime.Hosting {

    static class HostingInteropHelper {

        [System.Security.SecurityCritical]
        [DllImport("mscoree.dll", PreserveSig = false, EntryPoint = "CLRCreateInstance")]
        [return: MarshalAs(UnmanagedType.Interface)]
        private static extern object nCreateInterface(
                [MarshalAs(UnmanagedType.LPStruct)] Guid clsid,
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        private static Guid _metaHostClsIdGuid =
            new Guid(0x9280188D, 0xE8E, 0x4867, 0xB3, 0xC, 0x7F, 0xA8, 0x38, 0x84, 0xE8, 0xDE);


        private static Guid _metaHostGuid =
            new Guid(0xD332DB9E, 0xB9B3, 0x4125, 0x82, 0x07, 0xA1, 0x48, 0x84, 0xF5, 0x32, 0x16);


        [System.Security.SecurityCritical]
        public static T GetClrMetaHost<T>() {
            return (T)nCreateInterface(_metaHostClsIdGuid, _metaHostGuid);
        }

        private static Guid _metaHostPolicyClsIdGuid =
            new Guid(0x2EBCD49A, 0x1B47, 0x4A61, 0xB1, 0x3A, 0x4A, 0x3, 0x70, 0x1E, 0x59, 0x4B);

        private static Guid _metaHostPolicyGuid =
            new Guid(0xE2190695, 0x77B2, 0x492e, 0x8E, 0x14, 0xC4, 0xB3, 0xA7, 0xFD, 0xD5, 0x93);

        [System.Security.SecurityCritical]
        public static T GetClrMetaHostPolicy<T>() {
            return (T)nCreateInterface(_metaHostPolicyClsIdGuid, _metaHostPolicyGuid);
        }

        private static Guid _clrRuntimeInfoGuid =
            new Guid(0xBD39D1D2, 0xBA2F, 0x486a, 0x89, 0xB0, 0xB4, 0xB0, 0xCB, 0x46, 0x68, 0x91);

        // Returns the IClrRuntimeInfo that is executing the calling code.
        [System.Security.SecurityCritical]
        public static T GetClrRuntimeInfo<T>() {
            IClrMetaHost metaHost = GetClrMetaHost<IClrMetaHost>();
            return (T)metaHost.GetRuntime(RuntimeEnvironment.GetSystemVersion(),
                                           _clrRuntimeInfoGuid);
        }
    }
}
