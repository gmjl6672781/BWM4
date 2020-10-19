using BeinLab.Util;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif
namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class DynamicBase : ScriptableObject
    {
        /// <summary>
        /// 动效执行的对象
        /// 两种赋值方式，配置或者通过事件组进行赋值
        /// 指定式配置，代表此动效仅在对象存在时执行
        /// 非指定式配置，代表此动效将通过事件系统支配，事件系统将初始化对象，并管理动效的执行
        /// 指定式用来简化动效配置，例如配置一个简易的换色，销毁某个动效
        /// </summary>
        [Tooltip("目标对象，动效的接收者")]
        public GameDynamicConf gameDynamicConf;
        [Tooltip("是否强制创建动效对象")]
        public bool isForeceCreate = true;
        public bool isListenAction = false;
        /// <summary>
        /// 当在某些事件范围内时执行
        /// </summary>
        public List<ActionConf> listenAction;
        /// <summary>
        /// 当在某些事件范围内时不执行
        /// </summary>
        public List<ActionConf> unListenAction;
    }
}