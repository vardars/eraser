// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Runtime.Hosting.Interop
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Diagnostics.CodeAnalysis;

    [System.Security.SecurityCritical]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int FExecuteInAppDomainCallback(IntPtr cookie);

    [System.Security.SecurityCritical]
    [ComImport, Guid("90F1A06C-7712-4762-86B5-7A5EBA6BDB02"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IClrRuntimeHost
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Start();

        // Legitimate reflection of native API name.
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords")]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Stop();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetHostControl(
            [In, MarshalAs(UnmanagedType.Interface)] /*IHostControl*/ object hostControl);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetClrControl(
            [MarshalAs(UnmanagedType.Interface)] out /*IClrControl*/ object clrControl);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void UnloadAppDomain(
            [In, MarshalAs(UnmanagedType.U4)] int appDomainId,
            [In, MarshalAs(UnmanagedType.Bool)] bool waitUntilDone);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ExecuteInAppDomain(
            [In, MarshalAs(UnmanagedType.U4)] int appDomainId,
            [In] FExecuteInAppDomainCallback callback,
            [In] IntPtr cookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCurrentAppDomainId(
            [MarshalAs(UnmanagedType.U4)] out int appDomainId);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ExecuteApplication(
            [In, MarshalAs(UnmanagedType.LPWStr)] string appFullName,
            [In, MarshalAs(UnmanagedType.U4)] int manifestPathCount,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPWStr, SizeParamIndex=1)] string[] manifestPaths,
            [In, MarshalAs(UnmanagedType.U4)] int activationDataCount,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPWStr, SizeParamIndex=3)] string[] activationData,
            out int returnValue);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ExecuteInDefaultAppDomain(
            [In, MarshalAs(UnmanagedType.LPWStr)] string assemblyPath,
            [In, MarshalAs(UnmanagedType.LPWStr)] string typeName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string methodName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string argument,
            [MarshalAs(UnmanagedType.U4)] out int returnValue);
    }
}

