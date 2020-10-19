using BeinLab.Util;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.FengYun.Modu
{
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    public class SceneConf : ScriptableObject
    {
        public List<ActionConf> actionList;
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Conf/SceneConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<SceneConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<SceneConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(SceneConf) + " is null");
                }
            }
        }
#endif
    }
}