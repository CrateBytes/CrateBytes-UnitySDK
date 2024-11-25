using System.IO;
using UnityEditor;
using UnityEngine;

namespace CrateBytes
{
    public static class CrateBytesSettingsBootstrap
    {

        private static CrateBytesProjectSettings crateBytesProjectSettings;

        private static readonly string assetPath = "Assets/CrateBytes/Resources/CrateBytesProjectSettings.asset";
        private static readonly string directoryPath = "Assets/CrateBytes/Resources/";

        [InitializeOnLoadMethod]
        static void CreateSettingsFile()
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!AssetExists(assetPath))
            {
                CreateScriptableObject<CrateBytesProjectSettings>(assetPath);
            }
        }

        private static bool AssetExists(string path)
        {
            return AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) != null;
        }

        private static void CreateScriptableObject<T>(string path) where T : ScriptableObject
        {
            // Create a new instance of the ScriptableObject
            T newAsset = ScriptableObject.CreateInstance<T>();

            // Save it to the specified path
            AssetDatabase.CreateAsset(newAsset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"ScriptableObject of type {typeof(T).Name} created at path: {path}");
        }

        public static CrateBytesProjectSettings Instance
        {
            get
            {
                if (crateBytesProjectSettings == null)
                {
                    crateBytesProjectSettings = LoadSettings();
                }
                return crateBytesProjectSettings;
            }
        }

        private static CrateBytesProjectSettings LoadSettings()
        {
            var settings = Resources.Load<CrateBytesProjectSettings>("CrateBytesProjectSettings");
            if (settings == null)
            {
                Debug.LogError($"CrateBytesProjectSettings asset not found. Please create one at {assetPath}");
            }
            return settings;
        }

    }

}