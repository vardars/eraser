// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.Runtime.Hosting.Interop {
    [ComImport, Guid("CB2F6722-AB3A-11d2-9C40-00C04FA30A3E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICorRuntimeHost {
        void CreateLogicalThreadState();
        void DeleteLogicalThreadState();
        [PreserveSig]
        int SwitchInLogicalThreadState([In] ref uint pFiberCookie);
        [PreserveSig]
        int SwitchOutLogicalThreadState(out uint FiberCookie);
        [PreserveSig]
        int LocksHeldByLogicalThread(out uint pCount);
        [PreserveSig]
        int MapFile(IntPtr hFile, out IntPtr hMapAddress);
        [PreserveSig]
        int GetConfiguration([MarshalAs(UnmanagedType.IUnknown)] out object pConfiguration);
        [PreserveSig]
        int Start();
        [PreserveSig]
        int Stop();
        [PreserveSig]
        int CreateDomain(string pwzFriendlyName, [MarshalAs(UnmanagedType.IUnknown)] object pIdentityArray, [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);
        [return:MarshalAs(UnmanagedType.IUnknown)]
        object GetDefaultDomain();
        [PreserveSig]
        int EnumDomains(out IntPtr hEnum);
        [PreserveSig]
        int NextDomain(IntPtr hEnum, [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);
        [PreserveSig]
        int CloseEnum(IntPtr hEnum);
        [PreserveSig]
        int CreateDomainEx(string pwzFriendlyName, [MarshalAs(UnmanagedType.IUnknown)] object pSetup, [MarshalAs(UnmanagedType.IUnknown)] object pEvidence, [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);
        [PreserveSig]
        int CreateDomainSetup([MarshalAs(UnmanagedType.IUnknown)] out object pAppDomainSetup);
        [PreserveSig]
        int CreateEvidence([MarshalAs(UnmanagedType.IUnknown)] out object pEvidence);
        [PreserveSig]
        int UnloadDomain([MarshalAs(UnmanagedType.IUnknown)] object pAppDomain);
        [PreserveSig]
        int CurrentDomain([MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);
    }


}
