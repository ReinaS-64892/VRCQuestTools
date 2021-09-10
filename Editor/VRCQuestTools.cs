﻿// <copyright file="VRCQuestTools.cs" company="kurotu">
// Copyright (c) kurotu.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace KRT.VRCQuestTools
{
    /// <summary>
    /// VRCQuestTools configuration.
    /// </summary>
    [InitializeOnLoad]
    public static class VRCQuestTools
    {
        /// <summary>
        /// Tool name.
        /// </summary>
        public const string Name = "VRCQuestTools";

        /// <summary>
        /// VRCQuestTools version (semver).
        /// </summary>
        public const string Version = "1.0.0";

        /// <summary>
        /// Days to delay update notification.
        /// </summary>
        internal const int DaysToDelayUpdateNotification = 1;

        /// <summary>
        /// GitHub repository (username/reponame).
        /// </summary>
        internal const string GitHubRepository = "kurotu/VRCQuestTools";

        /// <summary>
        /// Booth URL.
        /// </summary>
        internal const string BoothURL = "https://kurotu.booth.pm/items/2436054";

        private const string AssetRoot = "Assets/KRT/VRCQuestTools";

        static VRCQuestTools()
        {
#if VRC_SDK_VRCSDK3
            Debug.Log($"[{Name}] VRCSDK3 project.");
#elif VRC_SDK_VRCSDK2
            Debug.Log($"[{Name}] VRCSDK2 project.");
#else
            Debug.LogError($"[{Name}] VRCSDK is not imported. Please import VRCSDK.");
#endif
        }

        /// <summary>
        /// Export as .unitypackage for release.
        /// </summary>
        /// <param name="filename">File to export.</param>
        public static void ExportUnityPackage(string filename)
        {
            AssetDatabase.ExportPackage(AssetRoot, filename, ExportPackageOptions.Recurse);
        }

        private static void Export()
        {
            ExportUnityPackage("VRCQuestTools.unitypackage");
        }
    }
}
