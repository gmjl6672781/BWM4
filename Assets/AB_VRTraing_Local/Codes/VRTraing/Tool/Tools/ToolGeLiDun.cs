using System.Collections;
using System.Collections.Generic;
using BeinLab.VRTraing.Conf;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing
{
    public class ToolGeLiDun : ToolBasic
    {
        public ToolConf toolGeLiDai;
        protected override void OnPress_(Hand hand)
        {
            base.OnPress_(hand);
            ToolGeLiDai tool = (ToolGeLiDai)toolGeLiDai.toolBasic;
            //通知隔离带完成工作
            if (tool && tool.isStartPull)
                tool.OnPull_();
        }
    }
}


