// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Runtime.Hosting.Interop;

namespace Microsoft.Runtime.Hosting {

    /// <summary>
    /// Represents the results of a metahost policy decision.
    /// This is returned by <see cref="ClrMetaHostPolicy.GetRequestedRuntime"/>.
    /// </summary>
    public class PolicyResult {
        /// <summary>
        /// The chosen runtime
        /// </summary>
        public ClrRuntimeInfo Runtime { get; set; }
        /// <summary>
        /// The version of the chosen runtime
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// The "image version" applied
        /// </summary>
        public string ImageVersion { get; set; }
        /// <summary>
        /// Relevant MetaHostConfigFlags
        /// </summary>
        public MetaHostConfigFlags ConfigFlags { get; set; }
    }
}
