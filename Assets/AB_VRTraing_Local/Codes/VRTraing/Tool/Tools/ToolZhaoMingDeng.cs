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
    /// <summary>
    /// 照明灯
    /// </summary>
    public class ToolZhaoMingDeng : ToolBasic
    {
        private bool isLightOn = false;
        private Transform trLight;
        private Light mLight;
        public event Action<string> OnCheckSurface;
        
        protected override void Awake()
        {
            base.OnDestroy();
            base.Awake();
            trLight = UnityUtil.GetTypeChildByName<Transform>(gameObject, "Light");
            mLight = trLight.GetComponent<Light>();
            mLight.enabled = false;
        }

        protected override void OnCatch_(Hand hand)
        {
            base.OnCatch_(hand);
            LightOnOff();
        }

        protected override void OnDetach_(Hand hand)
        {
            base.OnDetach_(hand);
            LightOnOff();
        }

        protected override void OnCatching_(Hand hand)
        {
            base.OnCatching_(hand);
            //扣动侧键
            //if (VRHandHelper.Instance.VRInput_GrabGrip.GetKeyDown(hand))
            //    LightOnOff();

            if(isLightOn)
                CheckSurface();
        }

        private void LightOnOff()
        {
            if(isLightOn)
            {
                Debug.Log("照明灯灯光关闭");
                isLightOn = false;
                if (mLight)
                    mLight.enabled = false;
            }
            else
            {
                Debug.Log("照明灯灯光打开");
                isLightOn = true;
                if (mLight)
                    mLight.enabled = true;
            }
        }

        private void CheckSurface()
        {
            Ray ray = new Ray(trLight.position, trLight.forward);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 0.5f))
            {
                if (OnCheckSurface != null)
                    OnCheckSurface(hit.collider.tag);
            }
        }
    }
}
