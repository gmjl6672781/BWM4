using BeinLab.FengYun.Gamer;
using BeinLab.Util;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    public class UILabelDynamicConf : UIDynamicConf
    {
        public int fontSize = 36;
        public string message;
        private Text label;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (mainRect)
            {
                label = mainRect.GetComponent<Text>();
                label.fontSize = fontSize;
                label.text = UnityUtil.SplitToLine(message);
            }
        }
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            if(label)
            label.text = "";
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/UILabelDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<UILabelDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<UILabelDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(UILabelDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}