﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KRTQuestTools
{
    enum CacheServerMode
    {
        Local = 0,
        Remote,
        Disable
    }

    static class UnitySettings
    {
        private static class EditorPrefsKeys
        {
            public const string CacheServerMode = "CacheServerMode";
            public const string CompressTexturesOnImport = "kCompressTexturesOnImport";
        }

        public static bool ValidateAll()
        {
            return ValidateCacheServerMode() && ValidateAndroidTextureCompression();
        }

        public static CacheServerMode GetCacheServerMode()
        {
            var mode = EditorPrefs.GetInt(EditorPrefsKeys.CacheServerMode, (int)CacheServerMode.Disable);
            return (CacheServerMode)System.Enum.ToObject(typeof(CacheServerMode), mode);
        }

        public static bool ValidateCacheServerMode()
        {
            return GetCacheServerMode() != CacheServerMode.Disable;
        }

        public static void EnableLocalCacheServer()
        {
            EditorPrefs.SetInt(EditorPrefsKeys.CacheServerMode, (int)CacheServerMode.Local);
        }

        public static MobileTextureSubtarget GetAndroidTextureCompression()
        {
            return EditorUserBuildSettings.androidBuildSubtarget;
        }

        public static bool ValidateAndroidTextureCompression()
        {
            return GetAndroidTextureCompression() == MobileTextureSubtarget.ASTC;
        }

        public static void EnableAndroidASTC()
        {
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
        }
    }

    [InitializeOnLoad]
    public class UnitySettingsWindow : EditorWindow
    {
        private delegate void Action();
        private readonly UnitySettingsI18nBase i18n = UnitySettingsI18n.Create();

        static UnitySettingsWindow()
        {
            EditorApplication.delayCall += DelayInit;
        }

        static void DelayInit()
        {
            EditorApplication.delayCall -= DelayInit;
            var lastVersion = KRTQuestToolsSettings.LastVersion;
            var hasUpdated = !lastVersion.Equals(KRTQuestTools.Version);
            if (hasUpdated)
            {
                KRTQuestToolsSettings.LastVersion = KRTQuestTools.Version;
            }
            var hasInvalidSettings = !UnitySettings.ValidateAll();

            if (hasInvalidSettings && (KRTQuestToolsSettings.IsShowSettingsWindowOnLoadEnabled || hasUpdated))
            {
                Init();
            }
        }

        [MenuItem(MenuPaths.UnitySettings, false, (int)MenuPriorities.UnitySettings)]
        static void Init()
        {
            var window = GetWindow(typeof(UnitySettingsWindow));
            window.Show();
        }

        private void OnGUI()
        {
            titleContent.text = "KRT Unity Settings";
            EditorGUILayout.LabelField("Unity Preferences", EditorStyles.boldLabel);
            var allActions = new List<Action>();

            EditorGUILayout.LabelField($"{i18n.CacheServerModeLabel}: {UnitySettings.GetCacheServerMode()}");

            if (!UnitySettings.ValidateCacheServerMode())
            {
                EditorGUILayout.HelpBox(i18n.CacheServerHelp, MessageType.Warning);
                allActions.Add(UnitySettings.EnableLocalCacheServer);
                if (GUILayout.Button(i18n.CacheServerButtonLabel))
                {
                    UnitySettings.EnableLocalCacheServer();
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);

            EditorGUILayout.LabelField($"{i18n.TextureCompressionLabel}: {UnitySettings.GetAndroidTextureCompression()}");
            if (!UnitySettings.ValidateAndroidTextureCompression())
            {
                EditorGUILayout.HelpBox(i18n.TextureCompressionHelp, MessageType.Warning);
                allActions.Add(UnitySettings.EnableAndroidASTC);
                if (GUILayout.Button(i18n.TextureCompressionButtonLabel))
                {
                    UnitySettings.EnableAndroidASTC();
                }
            }

            EditorGUILayout.Space();

            if (allActions.Count >= 2)
            {
                if (GUILayout.Button(i18n.ApplyAllButtonLabel))
                {
                    foreach (var action in allActions)
                    {
                        action();
                    }
                }
            }
            else if (allActions.Count == 0)
            {
                EditorGUILayout.HelpBox(i18n.AllAppliedHelp, MessageType.Info);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var showOnLoad = KRTQuestToolsSettings.IsShowSettingsWindowOnLoadEnabled;
            showOnLoad = EditorGUILayout.Toggle(i18n.ShowOnStartupLabel, showOnLoad);
            KRTQuestToolsSettings.IsShowSettingsWindowOnLoadEnabled = showOnLoad;
        }
    }

    static class UnitySettingsI18n
    {
        public static UnitySettingsI18nBase Create()
        {
            if (System.Globalization.CultureInfo.CurrentCulture.Name == "ja-JP")
            {
                return new UnitySettingsI18nJapanese();
            }
            else
            {
                return new UnitySettingsI18nEnglish();
            }
        }
    }

    abstract class UnitySettingsI18nBase
    {
        public abstract string CacheServerModeLabel { get; }
        public abstract string CacheServerHelp { get; }
        public abstract string CacheServerButtonLabel { get; }
        public abstract string TextureCompressionLabel { get; }
        public abstract string TextureCompressionHelp { get; }
        public abstract string TextureCompressionButtonLabel { get; }
        public abstract string ApplyAllButtonLabel { get; }
        public abstract string ShowOnStartupLabel { get; }
        public abstract string AllAppliedHelp { get; }
    }

    class UnitySettingsI18nEnglish : UnitySettingsI18nBase
    {
        public override string CacheServerModeLabel => "Cache Server Mode";
        public override string CacheServerHelp => "By enabling the local cache server, you can save time for such as texture compression from the next. In default preferences, the server takes 10 GB from C drive at maximum.";
        public override string CacheServerButtonLabel => "Enable Local Cache Server";
        public override string TextureCompressionLabel => "Android Texture Compression";
        public override string TextureCompressionHelp => "ASTC improves Quest texture quality in exchange for long compression time";
        public override string TextureCompressionButtonLabel => "Set texture compression to ASTC";
        public override string ApplyAllButtonLabel => "Apply All Settings";
        public override string ShowOnStartupLabel => "Show on startup";
        public override string AllAppliedHelp => "OK, all recommended settings are applied.";
    }

    class UnitySettingsI18nJapanese : UnitySettingsI18nBase
    {
        public override string CacheServerModeLabel => "キャッシュサーバー";
        public override string CacheServerHelp => "ローカルキャッシュサーバーを使用すると、テクスチャ圧縮などの結果を保存して次回にかかる時間を短縮できます。デフォルト設定では C ドライブを最大 10 GB 使用します。";
        public override string CacheServerButtonLabel => "ローカルキャッシュサーバーを有効化";
        public override string TextureCompressionLabel => "Android テクスチャ圧縮";
        public override string TextureCompressionHelp => "ASTCを使用するとQuest用のテクスチャ圧縮に時間がかかる代わりに画質が向上します。";
        public override string TextureCompressionButtonLabel => "ASTCでテクスチャを圧縮";
        public override string ApplyAllButtonLabel => "すべての設定を適用";
        public override string ShowOnStartupLabel => "起動時に表示";
        public override string AllAppliedHelp => "すべての推奨設定が適用されています。";
    }
}
