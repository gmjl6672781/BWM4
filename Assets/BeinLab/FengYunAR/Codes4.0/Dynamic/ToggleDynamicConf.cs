using BeinLab.FengYun.Gamer;
using BeinLab.FengYun.Modus;
using BeinLab.UI;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace BeinLab.CarShow.Modus
{
    public class ToggleDynamicConf : DynamicConf
    {
        public ToggleActionConf toggleAction;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            ToggleActionDlg dlg = gameDynamicer.GetComponentInChildren<ToggleActionDlg>();
            if (dlg)
            {
                dlg.SetData(toggleAction);
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/ToggleDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<ToggleDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ToggleDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DynamicConf) + " is null");
                }
            }
        }

#endif
    }
}