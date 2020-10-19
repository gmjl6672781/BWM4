using UnityEngine;
using BeinLab.Util;
using BeinLab.FengYun.Gamer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 通用性动效配置
    /// 处理的对象是GameObject本身，ShowBody
    /// </summary>
    [SerializeField]
    public class SwitchLogoDynamicConf : DynamicConf
    {
        public string logoName;
        public bool isActive;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            GameObject logo = UnityUtil.GetChildByName(gameDynamicer.gameObject, logoName);
            if (logo)
            {
                logo.SetActive(isActive);
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/SwitchLogoDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<SwitchLogoDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<SwitchLogoDynamicConf>(path);
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