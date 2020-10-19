using UnityEngine;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.FengYun.Gamer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    public class DianChiHideConf : DynamicConf
    {
        public ToolConf targetToolConf;
        public bool isActive = false;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            targetToolConf.toolBasic.gameObject.SetActive(isActive);
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/DianChiHideConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<DianChiHideConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<DianChiHideConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DianChiHideConf) + " is null");
                }
            }
        }
#endif
    }
}