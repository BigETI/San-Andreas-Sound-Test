using System;
using UnityEngine;

/// <summary>
/// San Andreas sound test
/// </summary>
namespace SanAndreasSoundTest.Data
{
    /// <summary>
    /// Settings data class
    /// </summary>
    [Serializable]
    public class SettingsData
    {
        /// <summary>
        /// GTA audio files directory
        /// </summary>
        [SerializeField]
        private string gtaAudioFilesDirectory;

        /// <summary>
        /// GTA audio files directory
        /// </summary>
        public string GTAAudioFilesDirectory
        {
            get
            {
                if (gtaAudioFilesDirectory == null)
                {
                    gtaAudioFilesDirectory = "";
                }
                return gtaAudioFilesDirectory;
            }
            set
            {
                if (value != null)
                {
                    gtaAudioFilesDirectory = value;
                }
            }
        }
    }
}
