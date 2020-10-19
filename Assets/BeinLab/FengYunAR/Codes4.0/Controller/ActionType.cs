using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BeinLab.Util
{
    /// <summary>
    /// 动作的表达方式，可能是展示图片，播放电影，或者开启某个指令
    /// </summary>
    public enum ActionType
    {
        Null = -1,
        /// <summary>
        /// 图文展示
        /// </summary>
        Picture = 0,
        /// <summary>
        /// 影片,视频展示
        /// </summary>
        Movie = 1,
        /// <summary>
        /// 某个命令，用代码写死的接口
        /// </summary>
        Action = 2,
        /// <summary>
        /// 动效指令，读取动效配置，实例化动效
        /// </summary>
        GameAction = 4,
        /// <summary>
        /// 打开菜单层级
        /// </summary>
        Menu = 8,
        /// <summary>
        /// 动效事件
        /// </summary>
        Dynamic = 16,
        /// <summary>
        /// 展示AR场景
        /// </summary>
        ARShow = 32,
        /// <summary>
        /// 动效事件列表
        /// </summary>
        DynamicActionList = 64,
        /// <summary>
        /// 动效事件
        /// </summary>
        DynamicAction = 128,
        /// <summary>
        /// 动效组
        /// </summary>
        DynamicGroup = 256,
        XRState = 512,
        LoadScene = 1024
    }

    public enum TransferType
    {
        Move = 1,
        Rote = 2,
        Scale = 3,
        Move_Rote = 4
    }
}