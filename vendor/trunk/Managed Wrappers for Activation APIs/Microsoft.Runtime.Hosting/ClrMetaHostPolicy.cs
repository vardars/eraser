// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Runtime.Hosting.Interop;
using System.IO;

namespace Microsoft.Runtime.Hosting {

    /// <summary>
    /// Managed abstraction of the functionality provided by ICLRMetaHostPolicy.
    /// </summary>
    public static class ClrMetaHostPolicy {
        static IClrMetaHostPolicy _MetaHostPolicy = HostingInteropHelper.GetClrMetaHostPolicy<IClrMetaHostPolicy>();

        /// <summary>
        /// Returns the results of runtime selection policy given a set of "version artifacts".
        /// </summary>
        /// <remarks>
        /// All parameters following <paramref name="policyFlags"/> have default values, which allows you to use
        /// named parameters to simplify your calls.
        /// </remarks>
        /// <param name="policyFlags">The policy flags used for runtime selection</param>
        /// <param name="binaryPath">A path to a managed assembly on which to base runtime selection.</param>
        /// <param name="configStream">A stream containing configuration information such as a list of supported runtimes</param>
        /// <param name="version">A version string</param>
        /// <returns>A <see cref="PolicyResult"/> object that provides the results of policy.</returns>
        public static PolicyResult GetRequestedRuntime(MetaHostPolicyFlags policyFlags, string binaryPath = null, Stream configStream = null,
            string version = null) {
            MetaHostConfigFlags configFlags = MetaHostConfigFlags.LegacyV2ActivationPolicyUnset;
            var versionBuf = new BufferData(version);
            var imageVersionBuf = new BufferData();
            object runtimeInfoObject = null;
            var streamPosition = configStream.Position;
            using (var wrapper = new StreamWrapper(configStream, ownsStream: false)) {
                BufferData.DoBufferAction(() =>
                        _MetaHostPolicy.GetRequestedRuntime(
                            policyFlags,
                            binaryPath,
                            wrapper,
                            versionBuf, ref versionBuf.Length,
                            imageVersionBuf, ref imageVersionBuf.Length,
                            out configFlags,
                            typeof(IClrRuntimeInfo).GUID,
                            out runtimeInfoObject),
                        () => configStream.Seek(streamPosition, SeekOrigin.Begin),
                        versionBuf,
                        imageVersionBuf);
            }
            return new PolicyResult {
                Runtime = new ClrRuntimeInfo((IClrRuntimeInfo)runtimeInfoObject),
                Version = versionBuf.ToString(),
                ImageVersion = imageVersionBuf.ToString(),
                ConfigFlags = configFlags
            };
        }
    }
}
