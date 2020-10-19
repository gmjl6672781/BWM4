using BeinLab.Util;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.VRTraing.UI;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System.Collections.Generic;
using BeinLab.VRTraing.Mgr;
using DG.Tweening;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 使用工具
    /// </summary>
    public class GoalUseToolConf : TaskGoalConf
    {
        protected virtual void Check(ToolConf toolConf)
        {

        }

        protected virtual void Check(Hand hand, ToolConf toolConf)
        {

        }

        protected virtual void Check(string content)
        {
 
        }

        protected virtual void SetToolInfo(ToolGoalConf toolGoalConf)
        {
            ToolConf toolConf = toolGoalConf.toolConf;
            if (toolGoalConf.isSetCanHover)
                toolConf.toolBasic.SetToolHover(toolGoalConf.isCanHover);

            if (toolGoalConf.isSetCanCatch)
                toolConf.toolBasic.SetToolCatch(toolGoalConf.isCanCatch);

            if (toolGoalConf.isSetKinematic)
                toolConf.toolBasic.SetToolKinematic(toolGoalConf.isKinematic);


            if (toolGoalConf.isSetHide)
                toolConf.toolBasic.gameObject.SetActive(!toolGoalConf.isHide);

            if (toolGoalConf.isSetScaleSize)
                toolConf.toolBasic.transform.DOScale(toolGoalConf.scaleSize,1.0f);

            if (toolGoalConf.isSetHighlight)
                toolConf.toolBasic.SetToolHighlight(toolGoalConf.isHighlight);

            if (toolGoalConf.isSetPose && toolConf.toolBasic.catchHand == null)
            {
                toolConf.toolBasic.transform.localPosition = toolGoalConf.Position;
                toolConf.toolBasic.transform.localEulerAngles = toolGoalConf.Angle;
            }

            
        }
    }
}

