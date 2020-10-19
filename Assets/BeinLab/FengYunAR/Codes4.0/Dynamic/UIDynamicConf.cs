using BeinLab.FengYun.Gamer;
using BeinLab.Util;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.CarShow.Modus
{
    public class UIDynamicConf : DynamicConf
    {
        public Vector2 pos = Vector2.up * -100;
        public Vector2 pivot = Vector2.one / 2f;
        public string maintargetName = "Text";
        [HideInInspector]
        public RectTransform mainRect;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            mainRect = UnityUtil.GetTypeChildByName<RectTransform>(gameDynamicer.gameObject, maintargetName);
            if (mainRect)
            {
                mainRect.pivot = pivot;
                mainRect.anchoredPosition3D = pos;
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/UIDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<UIDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<UIDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(UIDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}