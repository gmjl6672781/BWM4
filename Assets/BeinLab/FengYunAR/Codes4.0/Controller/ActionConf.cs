using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.Util
{
    /// <summary>
    /// 通用事件配置
    /// </summary>
    [SerializeField]
    public class ActionConf : ScriptableObject
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public ActionType actionType;
        /// <summary>
        /// 事件对应的信息，可能是路径或者配置文件
        /// </summary>
        public string action;
//#if UNITY_EDITOR
//        public UnityEngine.Object prefab;
//#endif

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Action/ActionConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<ActionConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ActionConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(ActionConf) + " is null");
                }
            }
        }
#endif
    }
}