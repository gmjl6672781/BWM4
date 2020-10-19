using BeinLab.Util;
using BeinLab.VRTraing.Controller;
using System.Collections.Generic;
using System.IO;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 语言包的配置
    /// </summary>
    public class LanguagePackConf
    {
        private string priKey = "Chinese";
        private string language = "简体中文";
        private string packagePath;
        /// <summary>
        /// 语言包的主键
        /// </summary>
        public string PriKey { get => priKey; set => priKey = value; }
        /// <summary>
        /// 语言包的名称
        /// </summary>
        public string Language { get => language; set => language = value; }
        /// <summary>
        /// 语言包的路径
        /// </summary>
        public string PackagePath { get => packagePath; set => packagePath = value; }
        public Dictionary<string, LanguageConf> LanguageMap { get => languageMap; set => languageMap = value; }

        /// <summary>
        /// 语言文本映射
        /// </summary>
        private Dictionary<string, LanguageConf> languageMap;
        private void CreateDefault(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            LanguageConf lc = new LanguageConf();
            lc.PriKey = "Title";
            lc.Message = "标题";
            lc.AudioPath = "test";
            UnityUtil.WriteXMLData(path, lc);
        }
        public void ReadLanguageMap()
        {
            if (LanguageMap == null)
            {
                string path = Path.Combine(LanguageMgr.Instance.RealDataPath, PackagePath);
                var languageList = UnityUtil.ReadXMLData<LanguageConf>(path, true);
                if (languageList == null || languageList.Count <= 0)
                {
                    CreateDefault(path);
                    languageList = UnityUtil.ReadXMLData<LanguageConf>(path, false);
                }
                LanguageMap = new Dictionary<string, LanguageConf>();
                if (languageList != null)
                {
                    for (int i = 0; i < languageList.Count; i++)
                    {
                        if (!LanguageMap.ContainsKey(languageList[i].PriKey))
                        {
                            LanguageMap.Add(languageList[i].PriKey, languageList[i]);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取对应的语言文本的配置
        /// 
        /// </summary>
        /// <param name="prikey"></param>
        public LanguageConf GetLanguage(string prikey)
        {
            ReadLanguageMap();
            if (LanguageMap != null && LanguageMap.ContainsKey(prikey))
            {
                return LanguageMap[prikey];
            }
            return null;
        }

        public string GetAudioPath(string key)
        {
            string path = null;
            LanguageConf lc = GetLanguage(key);
            if (lc != null)
            {
                path = Path.Combine(LanguageMgr.Instance.RealDataPath, PackagePath, lc.AudioPath);
            }
            return path;
        }
    }
}