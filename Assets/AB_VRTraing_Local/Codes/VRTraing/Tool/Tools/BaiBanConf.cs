using BeinLab.Util;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.VRTraing.UI;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System.Collections.Generic;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing.Conf
{
    public class BaiBanConf : ScriptableObject
    {
        public Texture englishTextureWrite;
        public Texture englishTextureNull;
        public Texture chineseTextureWrite;
        public Texture chineseTextureNull;

        public const string chinesePriKey = "Chinese";
        public const string englishPrikey = "English";

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/BaiBanConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<BaiBanConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<BaiBanConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(BaiBanConf) + " is null");
                }
            }
        }

#endif
    }

}
