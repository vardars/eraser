// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Runtime.Hosting.Interop
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [System.Security.SecurityCritical]
    [ComImport, Guid("9065597E-D1A1-4FB2-B6BA-7E1FCE230F61"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IClrControl
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetClrManager(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId,
            [MarshalAs(UnmanagedType.Interface)] out object clrManager);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetAppDomainManagerType(
            [In, MarshalAs(UnmanagedType.LPWStr)] string appDomainManagerAssembly,
            [In, MarshalAs(UnmanagedType.LPWStr)] string appDomainManagerType);
    }
}

