using UnityEngine;
using Valve.VR;
using BeinLab.Util;
using Valve.VR.InteractionSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// VR的输入配置
    /// </summary>
    public class VRInputConf : ScriptableObject
    {
        public SteamVR_Action_Boolean vr_Action_Button;
        
        public bool GetKeyDown(SteamVR_Input_Sources sources)
        {
            return vr_Action_Button.GetStateDown(sources);
        }
        public bool GetKeyUp(SteamVR_Input_Sources sources)
        {
            return vr_Action_Button.GetStateUp(sources);
        }
        public bool GetKey(SteamVR_Input_Sources sources)
        {
            return vr_Action_Button.GetState(sources);
        }
        public bool GetKeyDown(Hand hand)
        {
            return GetKeyDown(hand.handType);
        }
        public bool GetKeyUp(Hand hand)
        {
            return GetKeyUp(hand.handType);
        }
        public bool GetKey(Hand hand)
        {
            return GetKey(hand.handType);
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_VRTraining/VRInputConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<VRInputConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<VRInputConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(VRInputConf) + " is null");
                }
            }
        }

#endif
    }
}
