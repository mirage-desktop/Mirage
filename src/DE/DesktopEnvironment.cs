/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
using Mirage.SurfaceKit;

namespace Mirage.DE
{
    /// <summary>
    /// Responsible for initialising and running the desktop environment.
    /// </summary>
    public static class DesktopEnvironment
    {
        internal static string DistributionName { get; private set; } = string.Empty;
        internal static string DistributionVersion { get; private set; } = string.Empty;

        /// <summary>
        /// Start the Mirage desktop environment.
        /// </summary>
        /// <param name="distributionName">The name of the host distribution.</param>
        /// <param name="distributionVersion">The version of the host distribution.</param>
        public static void Start(string distributionName, string distributionVersion)
        {
            DistributionName = distributionName;
            DistributionVersion = distributionVersion;
            SurfaceManager surfaceManager = new SurfaceManager();
            _ = new OOBE(surfaceManager);
            for (;;)
            {
                surfaceManager.Update();
                Cosmos.Core.Memory.Heap.Collect();
            }
        }
    }
}
