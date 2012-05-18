// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.Runtime.Hosting {

    /// <summary>
    /// Extensions for getting Stream object from some common sources
    /// </summary>
    public static class StreamingExtensions {

        /// <summary>
        /// Creates an in-memory <see cref="Stream"/> containing the given XML, suitable for passing to <see cref="ClrMetaHostPolicy.GetRequestedRuntime"/>
        /// </summary>
        public static Stream ToStream(this XElement xml) {
            return xml.ToString().ToStream();
        }

        /// <summary>
        /// Creates an in-memory <see cref="Stream"/> containing the given string, suitable for passing to <see cref="ClrMetaHostPolicy.GetRequestedRuntime"/>
        /// </summary>
        public static Stream ToStream(this string str) {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }
    }
}
