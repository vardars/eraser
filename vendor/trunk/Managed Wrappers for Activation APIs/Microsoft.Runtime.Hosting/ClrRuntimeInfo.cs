// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Runtime.Hosting.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Security;
using System.Runtime.ConstrainedExecution;

namespace Microsoft.Runtime.Hosting {
    /// <summary>
    /// Managed abstraction of the functionality provided by ICLRRuntimeInfo.
    /// </summary>
    public class ClrRuntimeInfo {

        /// <summary>
        /// SafeHandle for loaded libraries (does FreeLibrary on release)
        /// </summary>
        sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid {

            /// <summary>
            /// Creates a SafeLibraryHandle over a raw handle
            /// </summary>
            /// <param name="handle"></param>
            public SafeLibraryHandle(IntPtr handle)
                : base(true) {
                this.handle = handle;
            }

            /// <summary>
            /// Frees the underlying handle
            /// </summary>
            /// <returns></returns>
            protected override bool ReleaseHandle() {
                return FreeLibrary(handle);
            }

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool FreeLibrary(IntPtr hModule);
        }

        IClrRuntimeInfo _RuntimeInfo;

        /// <summary>
        /// Constructor that wraps an ICLRRuntimeInfo (used internally)
        /// </summary>
        internal ClrRuntimeInfo(IClrRuntimeInfo info) {
            if (info == null) {
                throw new ArgumentNullException("info");
            }
            _RuntimeInfo = info;
        }

        /// <summary>
        /// The directory in which the runtime is installed
        /// </summary>
        public string RuntimeDirectory {
            get {
                //TODO: caching?
                var directoryBuffer = new BufferData();
                BufferData.DoBufferAction(
                    () => _RuntimeInfo.GetRuntimeDirectory(directoryBuffer, ref directoryBuffer.Length),
                    directoryBuffer);
                return directoryBuffer.ToString();
            }
        }

        /// <summary>
        /// The full version string of the current runtime
        /// </summary>
        public string VersionString {
            get {
                //TODO: caching?
                var versionBuffer = new BufferData();
                BufferData.DoBufferAction(
                    () => _RuntimeInfo.GetVersionString(versionBuffer, ref versionBuffer.Length),
                    versionBuffer);
                return versionBuffer.ToString();
            }
        }

        /// <summary>
        /// Indicates whether this runtime is loaded in the current process
        /// </summary>
        public bool IsLoadedInCurrentProcess {
            get {
                return IsLoadedInProcess(Process.GetCurrentProcess());
            }
        }

        /// <summary>
        /// Indicates whether this runtime is loaded into another process
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public bool IsLoadedInProcess(Process process) {
            return _RuntimeInfo.IsLoaded(process.Handle);
        }

        /// <summary>
        /// Loads a library that is part of this runtime's installation
        /// </summary>
        public SafeHandle LoadLibrary(string dllName) {
            return new SafeLibraryHandle(_RuntimeInfo.LoadLibrary(dllName));
        }

        /// <summary>
        /// Loads a resource string from this runtime, given a resourceId
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public string LoadErrorString(int resourceId) {
            var errorStringBuffer = new BufferData();
            BufferData.DoBufferAction(()=>_RuntimeInfo.LoadErrorString(resourceId, errorStringBuffer, ref errorStringBuffer.Length),
                errorStringBuffer);
            return errorStringBuffer.ToString();
        }

        /// <summary>
        /// Gets an interface provided by this runtime, such as ICLRRuntimeHost.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to be returned.  This must be an RCW interface</typeparam>
        /// <param name="clsid">The CLSID to be created</param>
        public TInterface GetInterface<TInterface>(Guid clsid) {
            return (TInterface)_RuntimeInfo.GetInterface(clsid, typeof(TInterface).GUID);
        }

        /// <summary>
        /// Returns a delegate corresponding to a named function provided by the runtime
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public TDelegate GetRuntimeFunction<TDelegate>(string name) {
            if (!typeof(Delegate).IsAssignableFrom(typeof(TDelegate))) {
                throw new ArgumentException("TDelegate is not a delegate type");
            }
            var ptr = _RuntimeInfo.GetProcAddress(name);
            //TODO: better way to handle the cast?
            return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(TDelegate));
        }

        /// <summary>
        /// Indicates whether this runtime is loadable into the current process
        /// </summary>
        public bool IsLoadable {
            get { return _RuntimeInfo.IsLoadable(); }
        }

        /// <summary>
        /// Sets the default startup flags for this runtime
        /// </summary>
        /// <param name="startupFlags">The startup flags to apply</param>
        /// <param name="hostConfigFile">The host configuration file to apply (default null)</param>
        public void SetDefaultStartupFlags(StartupFlags startupFlags = StartupFlags.None, string hostConfigFile = null) {
            _RuntimeInfo.SetDefaultStartupFlags(startupFlags, hostConfigFile);
        }

        /// <summary>
        /// Gets the current default startup flags for this runtime, as well as any host configuration file
        /// </summary>
        public StartupFlags GetDefaultStartupFlags(out string hostConfigFile) {
            StartupFlags startupFlags = 0;
            var hostConfigBuffer = new BufferData();
            BufferData.DoBufferAction(() => _RuntimeInfo.GetDefaultStartupFlags(out startupFlags, hostConfigBuffer, ref hostConfigBuffer.Length),
                hostConfigBuffer);
            hostConfigFile = hostConfigBuffer.ToString();
            return startupFlags;
        }

        /// <summary>
        /// Gets the current default startup flags for this runtime
        /// </summary>
        public StartupFlags GetDefaultStartupFlags() {
            string hostConfig;
            return GetDefaultStartupFlags(out hostConfig);
        }
    }
}
