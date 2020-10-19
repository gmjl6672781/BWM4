using UnityEngine;
using Valve.VR;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.VRTraing.Conf
{
    public class VRInputAxesConf : VRInputConf
    {
        public SteamVR_Action_Vector2 vr_Action_Vector2;
        public SteamVR_ActionSet actionSet;
        public Vector2 GetDelt(SteamVR_Input_Sources sources)
        {
            return vr_Action_Vector2.GetAxis(sources);
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_VRTraining/VRInputAxesConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<VRInputAxesConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<VRInputAxesConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(VRInputAxesConf) + " is null");
                }
            }
        }
#endif
    }
}