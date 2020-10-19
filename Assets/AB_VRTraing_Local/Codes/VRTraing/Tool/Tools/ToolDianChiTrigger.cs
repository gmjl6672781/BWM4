using System;
using System.Collections;
using System.Collections.Generic;
using BeinLab.VRTraing.Conf;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing
{
    public class ToolDianChiTrigger : ToolBasic
    {
        public ToolConf toolDianChiPlane;//需要照亮的电池某个面工具
        public event Action<ToolConf> OnEnter;//需要调用


        private void Update()
        {
        }

        public void OnEnter_()
        {
            //通知任务目标
            if (OnEnter != null)
                OnEnter(toolConf);
        }

        /// <summary>
        /// 当与其他工具发生碰撞时
        /// </summary>
        /// <param name="otherTool">其他工具</param>
        protected override void OnEnterTool_(ToolConf otherTool)
        {
            base.OnEnterTool_(otherTool);
            if (otherTool.toolName.Equals("zhaomingdeng"))
            {
                Debug.Log("照明灯进入了电池触发器工具");
                OnEnter_();
                //Debug.Log("名字为：" + toolDianChiPlane.toolName + "的电池面被照亮");
                //toolDianChiPlane.toolBasic.SetToolHighlight(true);//对应工具高亮
            }
        }
    }
}

