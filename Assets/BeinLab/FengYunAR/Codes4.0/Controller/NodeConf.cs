using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.Util
{
    /// <summary>
    /// 节点的类型
    /// </summary>
    public enum NodeType
    {
        World = 0,
        UICanvas = 1,
        WorldCanvas = 2
    }

    /// <summary>
    /// 节点配置
    /// </summary>
    [SerializeField]
    public class NodeConf : ScriptableObject
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public NodeType nodeType = NodeType.World;
        /// <summary>
        /// 节点名称，可以通过/或|分割层级
        /// </summary>
        public string nodeName;
        /// <summary>
        /// 节点的坐标，相对
        /// </summary>
        public Vector3 position = Vector3.zero;
        /// <summary>
        /// 节点的相对角度
        /// </summary>
        public Vector3 angle = Vector3.zero;
        /// <summary>
        /// 节点的缩放尺寸
        /// </summary>
        public Vector3 scale = Vector3.one;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Conf/NodeConf", false, 0)]
        static void CreateNodeConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<NodeConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<NodeConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(NodeConf) + " is null");
                }
            }
        }
#endif
    }
}