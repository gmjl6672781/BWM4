using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Modus;
using BeinLab.Util;
//using HVRCORE;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.RS5.Mgr
{
    /// <summary>
    /*
     1，拆卸低压插头     1

2，安装低压插头     1

3，撬开高压插头     2

4，拒绝非授权人员进入  2

5，维修完成   3

6，拆卸高压电池螺栓  4

7，紧固高压电池螺栓  4

8，安装盖罩前检查工作  4
         */
    /// </summary>
    public class XRController : Singleton<XRController>
    {
        public ActionConf initConf;
        public ActionConf resetConf;
        public ActionConf workConf;
        public ActionConf showConf;
        public float zoomSpeed = 0.1f;
        public float minScale = 0.1f;
        public float maxScale = 1;
        public float roteSpeed = 180;
        public float defSize = 0.2f;

        public List<ToggleActionConf> cacheStateMap = new List<ToggleActionConf>();

        public GameObject editorCamera;
        /// <summary>
        /// 坐标系的建立者
        /// </summary>
        public GameObject worldCenter;
        public bool isNormalAR;
        public bool isHaveState(ToggleActionConf toggleConf)
        {
            return cacheStateMap.Contains(toggleConf);
        }

        public void AddToggleState(ToggleActionConf toggleConf)
        {
            if (!cacheStateMap.Contains(toggleConf))
            {
                cacheStateMap.Add(toggleConf);
            }
        }


        private void Start()
        {
            if (DynamicActionController.Instance)
            {
                DynamicActionController.Instance.DoAction(initConf);
                DynamicActionController.Instance.OnDoAction += OnDoAction;
                //TimerMgr.Instance.CreateTimer(ResetXR, 0.1f, 1);
            }
            else
            {
                DynamicActionController.InitComplte += delegate ()
                {
                    DynamicActionController.Instance.DoAction(initConf);
                    DynamicActionController.Instance.OnDoAction += OnDoAction;
                    //TimerMgr.Instance.CreateTimer(ResetXR, 0.1f, 1);
                };
            }
        }

        private void OnDoAction(ActionConf obj)
        {
            if (obj.actionType == ActionType.XRState && obj.action == "Reset")
            {
                GameNoder.Instance.ReSetRoot();
                cacheStateMap.Clear();

                isShow = false;
            }
        }

        public void ResetXR()
        {
            //RemoveListener();
            isShow = false;
            DynamicActionController.Instance.DoAction(resetConf);
            TimerMgr.Instance.CreateTimer(XRWork, 0.1f);
        }
        public void XRWork()
        {
            GameNoder.Instance.ReSetRoot();

            DynamicActionController.Instance.DoAction(workConf);
        }
        //public void RemoveListener()
        //{
        //    AudioListener audio = GameObject.FindObjectOfType<AudioListener>();
        //    if (audio)
        //    {
        //        audio.enabled = false;
        //    }
        //}

        private bool isShow = false;
        public void XRShow()
        {
            //RemoveListener();
            //if (GameNoder.Instance)
            //{
            //    GameNoder.Instance.SetShowRoot(XRTracker.arAnchorPos, XRTracker.localAngle,
            //        Vector3.one * defSize, true);
            //}
            DynamicActionController.Instance.DoAction(showConf);
            TimerMgr.Instance.CreateTimer(XRWork, 10);
            //isShow = true;
        }
        private Camera aRCamera;

        public Camera ARCamera
        {
            get { return aRCamera; }
            set
            {
                aRCamera = value;
            }
        }
        private void Update()
        {
            if (!ARCamera)
            {
                Application.targetFrameRate = -1;
                if (Player.instance && Player.instance.hmdTransform)
                {
                    ARCamera = Player.instance.hmdTransform.GetComponentInChildren<Camera>();
                }
                return;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetXR();
            }
            if (isShow)
            {
                //if (GameNoder.Instance.WorldAnchorZoom())
                //{
                //    GameNoder.Instance.WorldAnchorRote();
                //}
            }

            if (ARCamera)
            {
                if (!worldCenter)
                {
                    worldCenter = new GameObject("worldCenter");
                }
                worldCenter.transform.position = ARCamera.transform.position;
                Vector3 forward = ARCamera.transform.forward;
                forward.y = 0;
                worldCenter.transform.forward = forward;
            }
        }
    }
}