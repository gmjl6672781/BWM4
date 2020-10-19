using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BeinLab.VRTraing
{
    /// <summary>
    /// 工具监听任务功能，保持对任务的监听
    /// </summary>
    public interface IToolTask<T>
    {
        /// <summary>
        /// 初始化触发
        /// </summary>
        void OnTaskStart(T t);
        /// <summary>
        /// 工作任务触发
        /// </summary>
        void OnTaskDoing(T t);
        /// <summary>
        /// 完成工作触发
        /// </summary>
        void OnTaskEnd(T t);

      
    }
}

