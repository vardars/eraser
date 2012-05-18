// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Runtime.Hosting.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Microsoft.Runtime.Hosting {

    /// <summary>
    /// Managed abstraction of the functionality provided by ICLRMetaHost.
    /// </summary>
    public static class ClrMetaHost {

        static IClrMetaHost _MetaHost = HostingInteropHelper.GetClrMetaHost<IClrMetaHost>();

        //ordering of initializers matters
        static ClrRuntimeInfo _CurrentRuntime = GetRuntime(RuntimeEnvironment.GetSystemVersion());

        /// <summary>
        /// An enumeration of the installed runtimes
        /// </summary>
        public static IEnumerable<ClrRuntimeInfo> InstalledRuntimes {
            get {
                return EnumerateRuntimesFromEnumUnknown(_MetaHost.EnumerateInstalledRuntimes());
            }
        }

        /// <summary>
        /// Enumerates the runtimes loaded in <paramref name="process"/>
        /// </summary>
        /// <param name="process">The process to inspect</param>
        public static IEnumerable<ClrRuntimeInfo> GetLoadedRuntimes(Process process) {
            return EnumerateRuntimesFromEnumUnknown(_MetaHost.EnumerateLoadedRuntimes(process.Handle));
        }

        /// <summary>
        /// Enumerates the runtimes loaded in the current process
        /// </summary>
        public static IEnumerable<ClrRuntimeInfo> GetLoadedRuntimesInCurrentProcess() {
            return GetLoadedRuntimes(Process.GetCurrentProcess());
        }

        /// <summary>
        /// Internal helper to enumerate the contents of an IEnumUnknown that contains IClrRuntimeInfo
        /// </summary>
        /// <param name="enumUnknown"></param>
        /// <returns></returns>
        static IEnumerable<ClrRuntimeInfo> EnumerateRuntimesFromEnumUnknown(IEnumUnknown enumUnknown) {
            //clone and reset the IEnumUnknown to give the right enumerable semantics
            IEnumUnknown cloned;
            enumUnknown.Clone(out cloned);
            cloned.Reset();

            object[] objs = new object[1];
            int fetched;
            while (0 == cloned.Next(1, objs, out fetched)) {
                Debug.Assert(fetched == 1, "fetch == 1");
                yield return new ClrRuntimeInfo((IClrRuntimeInfo)objs[0]);
            }
        }

        /// <summary>
        /// Gets the "built-against" version from an assembly's CLR header.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRuntimeVersionFromAssembly(string path) {
            var versionBuffer = new BufferData();
            BufferData.DoBufferAction(() => _MetaHost.GetVersionFromFile(path, versionBuffer, ref versionBuffer.Length),
                versionBuffer);
            return versionBuffer.ToString();
        }

        /// <summary>
        /// Gets the <see cref="ClrRuntimeInfo"/> corresponding to the current runtime.
        /// That is, the runtime executing currently.
        /// </summary>
        public static ClrRuntimeInfo CurrentRuntime {
            get {
                return _CurrentRuntime;
            }
        }

        /// <summary>
        /// Gets the <see cref="ClrRuntimeInfo"/> corresponding to a particular version string.
        /// </summary>
        public static ClrRuntimeInfo GetRuntime(string version) {
            return new ClrRuntimeInfo((IClrRuntimeInfo)_MetaHost.GetRuntime(version, typeof(IClrRuntimeInfo).GUID));
        }

        /// <summary>
        /// Gets the <see cref="ClrRuntimeInfo"/> corresponding to the runtime that is bound to the v2 and prior
        /// "legacy" APIs.
        /// </summary>
        /// <returns></returns>
        public static ClrRuntimeInfo GetLegacyRuntime() {
            var info = (IClrRuntimeInfo)_MetaHost.QueryLegacyV2RuntimeBinding(typeof(IClrRuntimeInfo).GUID);
            return info == null ? null : new ClrRuntimeInfo(info);
        }

        /// <summary>
        /// Exits the process with the given exit code
        /// </summary>
        public static void ExitProcess(int exitCode) {
            _MetaHost.ExitProcess(exitCode);
        }
    }
}
