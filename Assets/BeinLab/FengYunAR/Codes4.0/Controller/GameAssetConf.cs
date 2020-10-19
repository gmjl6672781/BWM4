using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BeinLab.Util
{
    /// <summary>
    /// 资源加载的配置
    /// 一个游戏对象的配置
    /// 对象的创建有一个过程：先加载资源引用到内存
    /// </summary>
    public class GameAssetConf : ScriptableObject
    {
        /// <summary>
        /// 父物体的名称,支持层级名称，层级使用/分割
        /// 需要对场景中的游戏对象进行分类
        /// </summary>
        public string parentName;
        /// <summary>
        /// 资源路径
        /// </summary>
        public string assetPath;
        /// <summary>
        /// 资源引用对象，不常用
        /// </summary>
        public GameObject asset;
        /// <summary>
        /// 相对位置
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// 相对角度
        /// </summary>
        public Vector3 angle;
        /// <summary>
        /// 相对缩放
        /// </summary>
        public Vector3 scale = Vector3.one;

    }
}