using SanAndreasSoundTest.Data;
using System;
using System.IO;
using UnityEngine;

/// <summary>
/// San Andreas sound test namespace
/// </summary>
namespace SanAndreasSoundTest
{
    /// <summary>
    /// Settings class
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Default settings path
        /// </summary>
        private static readonly string defaultSettingsPath = Path.Combine(Application.persistentDataPath, "settings.json");

        /// <summary>
        /// Data
        /// </summary>
        private static SettingsData data;

        /// <summary>
        /// Data
        /// </summary>
        public static SettingsData Data
        {
            get
            {
                if (data == null)
                {
                    if (!(Load()))
                    {
                        data = new SettingsData();
                    }
                }
                return data;
            }
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <returns>"true" if successful, otherwise "false"</returns>
        public static bool Load()
        {
            bool ret = false;
            try
            {
                if (File.Exists(defaultSettingsPath))
                {
                    using (FileStream stream = File.Open(defaultSettingsPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            SettingsData d = JsonUtility.FromJson<SettingsData>(reader.ReadToEnd());
                            if (d != null)
                            {
                                data = d;
                                ret = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return ret;
        }

        /// <summary>
        /// Save settings
        /// </summary>
        public static void Save()
        {
            try
            {
                if (data != null)
                {
                    if (File.Exists(defaultSettingsPath))
                    {
                        File.Delete(defaultSettingsPath);
                    }
                    using (FileStream stream = File.Open(defaultSettingsPath, FileMode.Create))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(JsonUtility.ToJson(data));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
