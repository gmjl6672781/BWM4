using System.Collections;
using System.Collections.Generic;
using BeinLab.VRTraing.Conf;
using UnityEngine;
using BeinLab.VRTraing.Controller;
using Valve.VR.InteractionSystem;
using System;
using BeinLab.VRTraing.Mgr;
using BeinLab.Util;

namespace BeinLab.VRTraing
{
    public class ToolPen : ToolBasic
    {
        /// <summary>
        /// 笔帽
        /// </summary>
        private GameObject goCap;

        protected override void Awake()
        {
            base.Awake();
            Transform trCap = UnityUtil.GetTypeChildByName<Transform>(gameObject, "Cap");
            if (trCap)
                goCap = trCap.gameObject;
        }
        protected override void OnCatch_(Hand hand)
        {
            base.OnCatch_(hand);
            if (goCap)
                goCap.SetActive(false);
        }

        protected override void OnDetach_(Hand hand)
        {
            base.OnDetach_(hand);
            if (goCap)
                goCap.SetActive(true);
        }
    }
}

