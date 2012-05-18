// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Runtime.Hosting.Interop
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [Flags]
    [System.Security.SecurityCritical]
    public enum StartupFlags
    {
        None = 0,
        AlwaysFlowImpersonation = 0x40000,
        AppdomainResourceMonitoring = 0x400000, // Appdomain Resource Monitoring feature
        ConcurrentGC = 1,
        DisableCommitThreadStack = 0x20000,
        // Abbreviation fine here; it's not hungarian notation!
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        Etw = 0x100000,
        // Wants "GC" and "VM" rather than "Gc" and "Vm" but then complains that "GCVM" should be "Gcvm".
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly")]
        HoardGCVM = 0x2000,
        LegacyImpersonation = 0x10000,
        LoaderOptimizationMask = 6,
        LoaderOptimizationMultipleDomain = 4,
        LoaderOptimizationMultipleDomainHost = 6,
        LoaderOptimizationSingleDomain = 2,
        LoaderSafeMode = 0x10,
        LoaderSetPreference = 0x100,
        ServerBuild = 0x200000,
        ServerGC = 0x1000,
        SingleVersionHostingInterface = 0x4000,
        TrimGCCommit = 0x80000
    }
}

