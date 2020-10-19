using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Gamer;
using BeinLab.Util;
using BeinLab.VRTraing.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR;

namespace BeinLab.VRTraing.UI
{

    public enum FollowType
    {
        None,
        FollowHead,
        FollowHand
    }

    public class FollowDlgConf : ScriptableObject
    {
        public FollowType followType;

        /// <summary>
        /// FollowHead 设置
        /// </summary>
        public float fllowDis = 0.45f;
        public float fllowMinDis = 0.28f;
        public float flowSpeed = 1;

        /// <summary>
        /// FollowHead 设置
        /// </summary>
        public SteamVR_Input_Sources handType;
        public int forward = 1;//方向

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/FollowDlgConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<FollowDlgConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<FollowDlgConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(FollowDlgConf) + " is null");
                }
            }
        }

#endif
    }

}
