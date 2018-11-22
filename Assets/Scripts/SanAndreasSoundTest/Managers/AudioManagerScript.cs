using GTAAudioSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// San Andreas sound test namespace
/// </summary>
namespace SanAndreasSoundTest
{
    /// <summary>
    /// Audio manager script class
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManagerScript : MonoBehaviour
    {
        /// <summary>
        /// Use external OGG decoder toggle
        /// </summary>
        [SerializeField]
        private Toggle useExternalOGGDecoderToggle;

        /// <summary>
        /// Audio progress slider
        /// </summary>
        [SerializeField]
        private Slider audioProgressSlider;

        /// <summary>
        /// Audio progress time text
        /// </summary>
        [SerializeField]
        private Text audioProgressTimeText;

        /// <summary>
        /// Text
        /// </summary>
        [SerializeField]
        private Text statusText;

        /// <summary>
        /// Audio files directory input field
        /// </summary>
        [SerializeField]
        private InputField audioFilesDirectoryInputField;

        /// <summary>
        /// SFX file dropdown
        /// </summary>
        [SerializeField]
        private Dropdown sfxFileDropdown;

        /// <summary>
        /// Streams file dropdown
        /// </summary>
        [SerializeField]
        private Dropdown streamsFileDropdown;

        /// <summary>
        /// SFX bank slider
        /// </summary>
        [SerializeField]
        private Slider sfxBankSlider;

        /// <summary>
        /// SFX bank index text
        /// </summary>
        [SerializeField]
        private Text sfxBankIndexText;

        /// <summary>
        /// SFX audio slider
        /// </summary>
        [SerializeField]
        private Slider sfxAudioSlider;

        /// <summary>
        /// SFX audio index text
        /// </summary>
        [SerializeField]
        private Text sfxAudioIndexText;

        /// <summary>
        /// Streams bank slider
        /// </summary>
        [SerializeField]
        private Slider streamsBankSlider;

        /// <summary>
        /// Streams bank index text
        /// </summary>
        [SerializeField]
        private Text streamsBankIndexText;

        /// <summary>
        /// Audio source
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// GTA audio directory
        /// </summary>
        private string gtaAudioDirectory;

        /// <summary>
        /// SFX audio clips
        /// </summary>
        private Dictionary<string, AudioClip> sfxAudioClips = new Dictionary<string, AudioClip>();

        /// <summary>
        /// Streams audio clips
        /// </summary>
        private Dictionary<string, AudioClip> streamsAudioClips = new Dictionary<string, AudioClip>();

        /// <summary>
        /// GTA audio files
        /// </summary>
        private GTAAudioFiles gtaAudioFiles;

        /// <summary>
        /// Enable audio progress slider change event
        /// </summary>
        private bool enableAudioProgressSliderChangeEvent = true;

        /// <summary>
        /// Use external OGG decoder
        /// </summary>
        private bool UseExternalOGGDecoder
        {
            get
            {
                return ((useExternalOGGDecoderToggle == null) ? false : useExternalOGGDecoderToggle.isOn);
            }
        }

        /// <summary>
        /// GTA audio files directory
        /// </summary>
        public string GTAAudioDirectory
        {
            get
            {
                if (gtaAudioDirectory == null)
                {
                    gtaAudioDirectory = "";
                }
                return gtaAudioDirectory;
            }
            set
            {
                if (value != null)
                {
                    DisposeGTAAudioFiles();
                    gtaAudioFiles = GTAAudio.OpenRead(value);
                    if (gtaAudioFiles != null)
                    {
                        gtaAudioDirectory = value;
                        Settings.Data.GTAAudioFilesDirectory = gtaAudioDirectory;
                        Settings.Save();

                        List<string> options = new List<string>();
                        if (sfxFileDropdown != null)
                        {
                            sfxFileDropdown.ClearOptions();
                            foreach (GTAAudioSFXFile sfx_file in gtaAudioFiles.SFXAudioFiles)
                            {
                                options.Add(sfx_file.Name);
                            }
                            sfxFileDropdown.AddOptions(options);
                            options.Clear();
                        }
                        if (streamsFileDropdown != null)
                        {
                            streamsFileDropdown.ClearOptions();
                            foreach (GTAAudioStreamsFile streams_file in gtaAudioFiles.StreamsAudioFiles)
                            {
                                options.Add(streams_file.Name);
                            }
                            streamsFileDropdown.AddOptions(options);
                            options.Clear();
                        }
                        UpdateSFXSelection();
                        UpdateStreamsSelection();
                        StatusText = "Select an audio to play";
                    }
                }
            }
        }

        /// <summary>
        /// SFX file index
        /// </summary>
        private int SFXFileIndex
        {
            get
            {
                return ((sfxFileDropdown == null) ? -1 : sfxFileDropdown.value);
            }
        }

        /// <summary>
        /// Streams file index
        /// </summary>
        private int StreamsFileIndex
        {
            get
            {
                return ((streamsFileDropdown == null) ? -1 : streamsFileDropdown.value);
            }
        }

        /// <summary>
        /// SFX file name
        /// </summary>
        private string SFXFileName
        {
            get
            {
                string ret = null;
                if (gtaAudioFiles != null)
                {
                    int index = SFXFileIndex;
                    if (index >= 0)
                    {
                        GTAAudioSFXFile[] sfx_files = gtaAudioFiles.SFXAudioFiles;
                        if (sfx_files != null)
                        {
                            if (index < sfx_files.Length)
                            {
                                ret = sfx_files[index].Name;
                            }
                        }
                    }
                }
                if (ret == null)
                {
                    ret = "";
                }
                return ret;
            }
        }

        /// <summary>
        /// Streams file name
        /// </summary>
        private string StreamsFileName
        {
            get
            {
                string ret = null;
                if (gtaAudioFiles != null)
                {
                    int index = StreamsFileIndex;
                    if (index >= 0)
                    {
                        GTAAudioStreamsFile[] streams_files = gtaAudioFiles.StreamsAudioFiles;
                        if (streams_files != null)
                        {
                            if (index < streams_files.Length)
                            {
                                ret = streams_files[index].Name;
                            }
                        }
                    }
                }
                if (ret == null)
                {
                    ret = "";
                }
                return ret;
            }
        }

        /// <summary>
        /// SFX bank index value
        /// </summary>
        public float SFXBankIndexValue
        {
            get
            {
                return ((sfxBankSlider == null) ? -1 : sfxBankSlider.value);
            }
            set
            {
                if (sfxBankIndexText != null)
                {
                    sfxBankIndexText.text = value.ToString();
                }
            }
        }

        /// <summary>
        /// SFX bank index
        /// </summary>
        private int SFXBankIndex
        {
            get
            {
                return Mathf.RoundToInt(SFXBankIndexValue);
            }
            set
            {
                SFXBankIndexValue = value;
            }
        }

        /// <summary>
        /// SFX bank index value
        /// </summary>
        public float SFXAudioIndexValue
        {
            get
            {
                return ((sfxAudioSlider == null) ? -1 : sfxAudioSlider.value);
            }
            set
            {
                if (sfxAudioIndexText != null)
                {
                    sfxAudioIndexText.text = value.ToString();
                }
            }
        }

        /// <summary>
        /// SFX audio index
        /// </summary>
        private int SFXAudioIndex
        {
            get
            {
                return Mathf.RoundToInt(SFXAudioIndexValue);
            }
            set
            {
                SFXAudioIndexValue = value;
            }
        }

        /// <summary>
        /// SFX bank index value
        /// </summary>
        public float StreamsBankIndexValue
        {
            get
            {
                return ((streamsBankSlider == null) ? -1 : streamsBankSlider.value);
            }
            set
            {
                if (streamsBankIndexText != null)
                {
                    streamsBankIndexText.text = value.ToString();
                }
            }
        }

        /// <summary>
        /// Streams bank index
        /// </summary>
        private int StreamsBankIndex
        {
            get
            {
                return Mathf.RoundToInt(StreamsBankIndexValue);
            }
            set
            {
                StreamsBankIndexValue = value;
            }
        }

        /// <summary>
        /// SFX audio clip
        /// </summary>
        private AudioClip SFXAudioClip
        {
            get
            {
                AudioClip ret = null;
                string sfx_file_name = SFXFileName;
                int sfx_bank_index = SFXBankIndex;
                int sfx_audio_index = SFXAudioIndex;
                string key = sfx_file_name + "." + sfx_bank_index + "." + sfx_audio_index;
                if (sfxAudioClips.ContainsKey(key))
                {
                    ret = sfxAudioClips[key];
                }
                else
                {
                    if ((gtaAudioFiles != null) && (sfx_bank_index >= 0) && (sfx_audio_index >= 0))
                    {
                        try
                        {
                            using (GTAAudioStream audio_stream = gtaAudioFiles.OpenSFXAudioStreamByName(sfx_file_name, (uint)sfx_bank_index, (uint)sfx_audio_index))
                            {
                                if (audio_stream != null)
                                {
                                    byte[] int_pcm = new byte[audio_stream.Length];
                                    if (audio_stream.Read(int_pcm, 0, int_pcm.Length) == int_pcm.Length)
                                    {
                                        float[] float_pcm = new float[int_pcm.Length / sizeof(short)];
                                        for (int i = 0; i < float_pcm.Length; i++)
                                        {
                                            float_pcm[i] = Mathf.Clamp(((short)(int_pcm[i * sizeof(short)] | (int_pcm[(i * sizeof(short)) + 1] << 8)) / 32767.5f), -1.0f, 1.0f);
                                        }
                                        ret = AudioClip.Create(key, float_pcm.Length, 1, audio_stream.SampleRate, false);
                                        ret.SetData(float_pcm, 0);
                                        sfxAudioClips.Add(key, ret);
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
                return ret;
            }
        }

        /// <summary>
        /// Streams audio clip
        /// </summary>
        private AudioClip StreamsAudioClip
        {
            get
            {
                AudioClip ret = null;
                DateTime time = DateTime.Now;
                string streams_file_name = StreamsFileName;
                int streams_bank_index = StreamsBankIndex;
                string key = streams_file_name + "." + streams_bank_index;
                if (streamsAudioClips.ContainsKey(key))
                {
                    ret = streamsAudioClips[key];
                }
                else
                {
                    if ((gtaAudioFiles != null) && (streams_bank_index >= 0))
                    {
                        try
                        {
                            using (Stream audio_stream = gtaAudioFiles.OpenStreamsAudioStreamByName(streams_file_name, (uint)streams_bank_index))
                            {
                                if (audio_stream != null)
                                {
                                    using (NVorbis.VorbisReader reader = new NVorbis.VorbisReader(audio_stream, false))
                                    {
                                        float[] float_pcm = new float[reader.TotalSamples];
                                        if (reader.ReadSamples(float_pcm, 0, float_pcm.Length) == float_pcm.Length)
                                        {
                                            ret = AudioClip.Create(key, float_pcm.Length, reader.Channels, reader.SampleRate, false);
                                            ret.SetData(float_pcm, 0);
                                            streamsAudioClips.Add(key, ret);
                                        }
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
                if (ret != null)
                {
                    TimeSpan time_span = DateTime.Now - time;
                    Debug.Log("\"" + ret.name + "\" took " + time_span.TotalSeconds + " seconds.");
                }
                return ret;
            }
        }

        /// <summary>
        /// Audio progress
        /// </summary>
        public float AudioProgress
        {
            get
            {
                return ((audioSource == null) ? 0.0f : audioSource.time);
            }
            set
            {
                if (enableAudioProgressSliderChangeEvent && (audioSource != null))
                {
                    audioSource.time = Mathf.Clamp(value, 0.0f, Mathf.Max(0.0f, ((audioSource.clip == null) ? 0.0f : (audioSource.clip.length - 0.01f))));
                }
            }
        }

        /// <summary>
        /// Status text
        /// </summary>
        private string StatusText
        {
            get
            {
                return ((statusText == null) ? "" : statusText.text);
            }
            set
            {
                if (statusText != null)
                {
                    statusText.text = ((value == null) ? "" : value);
                }
            }
        }

        /// <summary>
        /// Dispose GTA audio files
        /// </summary>
        private void DisposeGTAAudioFiles()
        {
            if (gtaAudioFiles != null)
            {
                gtaAudioFiles.Dispose();
                gtaAudioFiles = null;
            }
            sfxAudioClips.Clear();
            streamsAudioClips.Clear();
            StatusText = "Select a GTA audio files directory";
        }

        /// <summary>
        /// Play SFX audio
        /// </summary>
        public void PlaySFXAudio()
        {
            AudioClip clip = SFXAudioClip;
            if ((audioSource != null) && (clip != null))
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        /// <summary>
        /// Play streams audio coroutine
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator PlayStreamsAudioCoroutine()
        {
            DateTime time = DateTime.Now;
            int streams_bank_index = StreamsBankIndex;
            string streams_file_name = StreamsFileName;
            string key = streams_file_name + "." + streams_bank_index;
            AudioClip clip = null;
            if (streamsAudioClips.ContainsKey(key))
            {
                clip = streamsAudioClips[key];
            }
            else
            {
                if ((gtaAudioFiles != null) && (streams_bank_index >= 0))
                {
                    bool success = false;
                    string path = Path.Combine(Application.temporaryCachePath, key + ".ogg");
                    try
                    {
                        using (Stream audio_stream = gtaAudioFiles.OpenStreamsAudioStreamByName(streams_file_name, (uint)streams_bank_index))
                        {
                            if (audio_stream != null)
                            {
                                if (File.Exists(path))
                                {
                                    File.Delete(path);
                                }
                                using (FileStream file_stream = File.Open(path, FileMode.Create))
                                {
                                    byte[] buffer = new byte[2048];
                                    int len;
                                    while ((len = Mathf.Min((int)(audio_stream.Length - audio_stream.Position), buffer.Length)) > 0)
                                    {
                                        if (audio_stream.Read(buffer, 0, len) == len)
                                        {
                                            file_stream.Write(buffer, 0, len);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    success = true;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    if (success)
                    {
                        WWW www = new WWW((new Uri(path)).AbsoluteUri);
                        while (!(www.isDone))
                        {
                            yield return null;
                        }
                        clip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
                        if (clip != null)
                        {
                            clip.name = key;
                            streamsAudioClips.Add(key, clip);
                        }
                    }
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
            if ((audioSource != null) && (clip != null))
            {
                audioSource.time = 0.0f;
                audioSource.clip = clip;
                audioSource.Play();
                StatusText = "Playing \"" + clip.name + "\"";
            }
            if (clip != null)
            {
                TimeSpan time_span = DateTime.Now - time;
                Debug.Log("\"" + clip.name + "\" took " + time_span.TotalSeconds + " seconds.");
            }
        }

        /// <summary>
        /// Play streams audio
        /// </summary>
        public void PlayStreamsAudio()
        {
            if (UseExternalOGGDecoder)
            {
                AudioClip clip = StreamsAudioClip;
                if ((audioSource != null) && (clip != null))
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
            else
            {
                StartCoroutine(PlayStreamsAudioCoroutine());
            }
        }

        /// <summary>
        /// Stop audio
        /// </summary>
        public void StopAudio()
        {
            if (audioSource != null)
            {
                StatusText = "Select an audio to play";
                audioSource.Stop();
            }
        }

        /// <summary>
        /// Get music time
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>Music time as string</returns>
        private static string GetMusicTime(float time)
        {
            int seconds = Mathf.RoundToInt(time);
            int minutes = seconds / 60;
            return minutes + ":" + (seconds % 60).ToString("D2");
        }

        /// <summary>
        /// Update SFX selection
        /// </summary>
        public void UpdateSFXSelection()
        {
            int sfx_file_index = SFXFileIndex;
            int num_banks = 0;
            int num_audios = 0;
            if (sfx_file_index >= 0)
            {
                GTAAudioSFXFile[] sfx_audio_files = gtaAudioFiles.SFXAudioFiles;
                if (sfx_audio_files != null)
                {
                    if (sfx_file_index < sfx_audio_files.Length)
                    {
                        GTAAudioSFXFile sfx_audio_file = sfx_audio_files[sfx_file_index];
                        if (sfx_audio_file != null)
                        {
                            num_banks = sfx_audio_file.NumBanks - 1;
                            num_audios = sfx_audio_file.NumAudios - 1;
                        }
                    }
                }
            }
            if (sfxBankSlider != null)
            {
                sfxBankSlider.value = 0.0f;
                sfxBankSlider.maxValue = Mathf.Max(0, num_banks);
            }
            if (sfxAudioSlider != null)
            {
                sfxAudioSlider.value = 0.0f;
                sfxAudioSlider.maxValue = Mathf.Max(0, num_audios);
            }
        }

        /// <summary>
        /// Update streams selection
        /// </summary>
        public void UpdateStreamsSelection()
        {
            int streams_file_index = StreamsFileIndex;
            int num_banks = 0;
            if (streams_file_index >= 0)
            {
                GTAAudioStreamsFile[] streams_audio_files = gtaAudioFiles.StreamsAudioFiles;
                if (streams_audio_files != null)
                {
                    if (streams_file_index < streams_audio_files.Length)
                    {
                        GTAAudioStreamsFile streams_audio_file = streams_audio_files[streams_file_index];
                        if (streams_audio_file != null)
                        {
                            num_banks = streams_audio_file.NumBanks - 1;
                        }
                    }
                }
            }
            if (streamsBankSlider != null)
            {
                streamsBankSlider.value = 0.0f;
                streamsBankSlider.maxValue = Mathf.Max(0, num_banks);
            }
        }

        /// <summary>
        /// On enable
        /// </summary>
        private void OnEnable()
        {
            DisposeGTAAudioFiles();
            string directory = Settings.Data.GTAAudioFilesDirectory;
            if (audioFilesDirectoryInputField != null)
            {
                audioFilesDirectoryInputField.text = directory;
            }
            //GTAAudioDirectory = directory;
        }

        /// <summary>
        /// On disable
        /// </summary>
        private void OnDisable()
        {
            DisposeGTAAudioFiles();
        }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            if (audioSource != null)
            {
                AudioClip clip = audioSource.clip;
                if (audioProgressTimeText != null)
                {
                    audioProgressTimeText.text = GetMusicTime(((clip == null) ? 0.0f : audioSource.time)) + " / " + GetMusicTime((clip == null) ? 0.0f : clip.length);
                }
                if (audioProgressSlider != null)
                {
                    enableAudioProgressSliderChangeEvent = false;
                    audioProgressSlider.value = ((clip == null) ? 0.0f : audioSource.time);
                    audioProgressSlider.maxValue = ((clip == null) ? 0.0f : clip.length);
                    enableAudioProgressSliderChangeEvent = true;
                }
            }
        }
    }
}
