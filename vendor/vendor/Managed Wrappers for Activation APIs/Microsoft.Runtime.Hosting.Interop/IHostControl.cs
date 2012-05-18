// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Runtime.Hosting.Interop
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [System.Security.SecurityCritical]
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("02CA073C-7079-4860-880A-C2F7A449C991")]
    public interface IHostControl
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetHostManager(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId,
            [MarshalAs(UnmanagedType.Interface)] out object hostManager);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetAppDomainManager(
            [In, MarshalAs(UnmanagedType.U4)] int appDomainId,
            [In, MarshalAs(UnmanagedType.IUnknown)] object appDomainManager);
    }
}

