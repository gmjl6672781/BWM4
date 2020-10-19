using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BeinLab.Util;

namespace Karler.Lib.Data
{
    /// <summary>
    /// 游戏数据加载和创建管理
    /// 动态对象管理类
    /// 事件执行者，处理对象事件和消息，发送状态
    /// 这个过程是如何管理的呢？
    /// 发布的事件？对象管理器？创建加载和销毁
    /// </summary>
    public class GameDataLoader : Singleton<GameDataLoader>
    {
        public UnityEngine.Object LoadObjectBy(string path)
        {
            return GameAssetLoader.Instance.LoadObject(path);
        }
    }
}