using BeinLab.Conf;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace BeinLab.VRTraing.Controller
{
    /// <summary>
    /// 语言控制器
    /// 提供语言切换时事件
    /// 提供语言包读取，解析
    /// 提供语言信息，语音的访问
    /// </summary>
    public class LanguageMgr : Singleton<LanguageMgr>
    {
        /// <summary>
        /// 语言包的配置文件路径
        /// </summary>
        public string languagePath = "GameData";
        /// <summary>
        /// 改变语言的事件
        /// </summary>
        public event Action<LanguagePackConf> changeLanuage;
        /// <summary>
        /// 当前的语言
        /// </summary>
        private LanguagePackConf curLanguage;
        /// <summary>
        /// 语言包列表
        /// </summary>
        private List<LanguagePackConf> languageList;
        private AudioSource audioPlayer;
        /// <summary>
        /// 当前的语言
        /// </summary>
        public LanguagePackConf CurLanguage { get => curLanguage; set => curLanguage = value; }
        public string RealDataPath { get => realDataPath; set => realDataPath = value; }
        public List<LanguagePackConf> LanguageList { get => languageList; set => languageList = value; }

        private string realDataPath;
        [HideInInspector]
        public int languageIndex;
        private Coroutine audioCoroutine;
        private string localSettingFilePath;
        public List<SystemSettingsConf> settingsConf;
        public SystemSettingsConf settings;
        //public string url = "https://training1.bmw.com.cn/BMWTAP/emailConfirmation/checkUser";
        private bool isReadSettins;

        //public Transform head;
        public AudioClip[] EffectAudio;
        public event Action OnPlayComplete;
        private void ReadExtraFile()
        {
            localSettingFilePath = Application.streamingAssetsPath;
            if (!isReadSettins)
            {
                if (settingsConf == null || settingsConf.Count < 1)
                {
                    settingsConf = UnityUtil.ReadXMLData<SystemSettingsConf>(localSettingFilePath);
                }
                if (settingsConf != null && settingsConf.Count > 0)
                {
                    //url = settingsConf[0].ServerURL;
                    settings = settingsConf[0];
                }
                else
                {
                    SystemSettingsConf conf = new SystemSettingsConf();
                    conf.PriKey = "DefaultSetting";
                    //conf.ServerURL = url;
                    UnityUtil.WriteXMLData(localSettingFilePath, conf);
                }
                isReadSettins = true;
            }

        }
        public void EditorAwake()
        {
            Awake();

        }
        public void Start()
        {
            string rootPath = Application.dataPath;

            audioPlayer = GetComponentInChildren<AudioSource>();

            if (GameDataMgr.Instance && !string.IsNullOrEmpty(GameDataMgr.Instance.AssetPath))
            {
                rootPath = GameDataMgr.Instance.AssetPath;
            }
            RealDataPath = Path.Combine(rootPath, languagePath);
            if (!Directory.Exists(RealDataPath))
            {
                Directory.CreateDirectory(RealDataPath);
            }
            ReadLanguage(RealDataPath);
            ReadExtraFile();

        }

        private bool isCreate = false;
        /// <summary>
        /// 读取语言包
        /// </summary>
        public void ReadLanguage(string path)
        {
            LanguageList = UnityUtil.ReadXMLData<LanguagePackConf>(path, true);
            if (!isCreate && (LanguageList == null || LanguageList.Count < 1))
            {
                CreateDefault();
                ReadLanguage(path);
                isCreate = true;
            }
            CurLanguage = LanguageList[0];
            changeLanuage?.Invoke(CurLanguage);
        }
        public void CreateDefault()
        {
            LanguagePackConf lpc = new LanguagePackConf();
            lpc.PriKey = "Chinese";
            lpc.Language = "简体中文";
            lpc.PackagePath = "Chinese";
            UnityUtil.WriteXMLData<LanguagePackConf>(RealDataPath, lpc);
        }
        /// <summary>
        /// 选中某一个语言时，默认是中文
        /// </summary>
        /// <param name="index"></param>
        public void SelectLanguage(int index)
        {
            if (index < LanguageList.Count)
            {
                if (CurLanguage != LanguageList[index])
                {
                    CurLanguage = LanguageList[index];
                }
                changeLanuage?.Invoke(CurLanguage);
                if (CurLanguage.LanguageMap == null)
                {
                    CurLanguage.ReadLanguageMap();
                }
                this.languageIndex = index;


                if (settingsConf != null && settingsConf.Count > 0)
                {
                    if (settingsConf[0].LanguageIndex != index)
                    {
                        settingsConf[0].LanguageIndex = index;
                        UnityUtil.WriteXMLData(localSettingFilePath, settingsConf[0]);
                    }
                }
            }
        }
        /// <summary>
        /// 通过Key值获取对应的语言
        /// </summary>
        /// <param name="prikey"></param>
        /// <returns></returns>
        public LanguageConf GetMessage(string prikey)
        {
            if (CurLanguage != null)
            {
                return CurLanguage.GetLanguage(prikey);
            }
            return null;
        }
        public string GetClipPath(string key)
        {
            string path = "";
            if (CurLanguage != null)
            {
                path = CurLanguage.GetAudioPath(key);
            }
            return path;
        }

        /// <summary>
        /// 通过路径获取音频文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLoadClip"></param>
        /// <returns></returns>
        public IEnumerator GetClip(string path, Action<AudioClip> onLoadClip)
        {
            AudioType audioType = AudioType.WAV | AudioType.MPEG;
            var req = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
            yield return req.SendWebRequest();
            if (req.isNetworkError)
            {
                Debug.LogError(req.error);
            }
            else
            {
                onLoadClip?.Invoke(DownloadHandlerAudioClip.GetContent(req));
            }
        }
        /// <summary>
        /// 通过路径获取音频文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLoadClip"></param>
        /// <returns></returns>
        public IEnumerator LoadClipByKey(string key, Action<AudioClip> onLoadClip)
        {
            AudioType audioType = AudioType.WAV | AudioType.MPEG;
            string url = GetClipPath(key);
            print(url);
            if (string.IsNullOrEmpty(url))
            {
                yield break;
            }
            var req = UnityWebRequestMultimedia.GetAudioClip(GetClipPath(key), audioType);
            yield return req.SendWebRequest();
            if (req.isNetworkError)
            {
                Debug.LogError(req.error);
                onLoadClip?.Invoke(null);
            }
            else
            {
                onLoadClip?.Invoke(DownloadHandlerAudioClip.GetContent(req));
            }
        }

        /// <summary>
        /// 开关声音
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleAudio(bool isOn)
        {
            audioPlayer.volume = isOn ? 1 : 0;
        }

        public IEnumerator LoadClipByKeyWWW(string key, Action<AudioClip> onLoadClip)
        {
            AudioType audioType = AudioType.WAV | AudioType.MPEG;
            string url = GetClipPath(key);
            //print(url);
            if (string.IsNullOrEmpty(url))
            {
                yield break;
            }
            WWW www = new WWW(url);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                onLoadClip?.Invoke(null);
            }
            else
            {
                onLoadClip?.Invoke(www.GetAudioClip());
            }
        }

        private void ClearAudioCoroutine()
        {
            if (audioCoroutine != null)
            {
                StopCoroutine(audioCoroutine);
            }
            audioCoroutine = null;
            if (audioPlayer.isPlaying)
            {
                audioPlayer.Stop();
            }
            audioPlayer.clip = null;
        }
        /// <summary>
        /// 播放一组音频
        /// </summary>
        /// <param name="audioKeys"></param>
        public void PlayAudioByKey(List<string> audioKeys, float playTime = 0,Action<float> playBackTime = null)
        {
            if (audioKeys == null || audioKeys.Count < 1)
            {
                return;
            }
            ClearAudioCoroutine();
            audioCoroutine = StartCoroutine(PlayAudios(audioKeys, playTime, playBackTime));
        }
        private IEnumerator PlayAudios(List<string> audioKeys, float playDeltTime = 0,Action<float> playTime=null)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < audioKeys.Count; i++)
            {
                audioPlayer.clip = null;
                bool isLoad = false;
                yield return LoadClipByKeyWWW(audioKeys[i], (AudioClip clip) =>
                {
                    if (clip)
                    {
                        audioPlayer.clip = clip;
                        playTime?.Invoke(audioPlayer.clip.length);
                    }
                    isLoad = true;
                });
                while (!isLoad)
                {
                    yield return new WaitForFixedUpdate();
                }
                if (audioPlayer.clip)
                {
                    audioPlayer.Play();
                    //print(audioPlayer.clip);
                    yield return new WaitForSeconds(audioPlayer.clip.length + playDeltTime);
                }
                yield return new WaitForEndOfFrame();
            }
            ClearAudioCoroutine();
            OnPlayComplete?.Invoke();
        }
        public void StopPlay()
        {
            ClearAudioCoroutine();
        }
        /// <summary>
        /// 播放单个音频
        /// </summary>
        /// <param name="key"></param>
        public void PlayAudioByKey(string key, float playTime = 0)
        {
            List<string> keys = new List<string>();
            keys.Add(key);
            PlayAudioByKey(keys, playTime);
        }
        /// <summary>
        /// 播放特效音频
        /// </summary>
        /// <param name="key"></param>
        public void PlayEffectAudioByKey(int EffectAudioint)
        {
            //AudioSource.PlayClipAtPoint(EffectAudio[EffectAudioint], head.position);
        }

    }
}