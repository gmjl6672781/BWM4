using BeinLab.Util;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 多维链表的数据结构
    /// 并提供了遍历算法
    /// </summary>
    public class LinkConf : ScriptableObject
    {
        /// <summary>
        /// 父节点，可以为空，代表属于根节点的任务
        /// </summary>
        [HideInInspector]
        public LinkConf parent;

        /// <summary>
        /// 子任务节点，可以为空，代表此任务无子任务
        /// </summary>
        public LinkConf child;

        /// <summary>
        /// 同级下一个任务，弟弟，如果为空，代表同级不存在任务，属于同级任务的最后一个任务
        /// </summary>
        public LinkConf littleBrother;

        /// <summary>
        /// 同级上一个任务，哥哥，如果为空，代表此任务属于节点下第一个任务
        /// </summary>
        [HideInInspector]
        public LinkConf oldBrother;
        /// <summary>
        /// 是否已经遍历过此节点
        /// </summary>
        public bool isRead = false;


        /// <summary>
        /// 获取下一个任务,下一个任务可能为空，当无下一个任务时，代表所有任务已经完成。
        /// </summary>
        public LinkConf NextNode
        {
            get
            {
                LinkConf next = null;
                if (child && !isRead)
                {
                    isRead = true;
                    next = child;
                    next.parent = this;
                }
                else if (littleBrother)
                {
                    next = littleBrother;
                    next.oldBrother = this;
                    next.parent = this.parent;
                }
                else if (parent)
                {
                    return parent.NextNode;
                }
                return next;
            }
        }
        /// <summary>
        /// 初始化此任务，将此任务设置为未读取状态
        /// 因脚本配置文件会缓存属性，因此在读取任务之前需要先将任务设置为未读取状态
        /// </summary>
        public void InitTask()
        {
            isRead = false;
        }

//#if UNITY_EDITOR
//        [MenuItem("Assets/Create/VRTracing/LinkConf", false, 0)]
//        static void CreateDynamicConf()
//        {
//            UnityEngine.Object obj = Selection.activeObject;
//            if (obj)
//            {
//                string path = AssetDatabase.GetAssetPath(obj);
//                ScriptableObject bullet = CreateInstance<LinkConf>();
//                if (bullet)
//                {
//                    string confName = UnityUtil.TryGetName<LinkConf>(path);
//                    AssetDatabase.CreateAsset(bullet, confName);
//                }
//                else
//                {
//                    Debug.Log(typeof(LinkConf) + " is null");
//                }
//            }
//        }

//#endif
    }
}