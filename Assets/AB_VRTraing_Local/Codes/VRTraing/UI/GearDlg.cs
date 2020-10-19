using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Gamer;
using UnityEngine.UI;
using System;
using Karler.WarFire.UI;
using Valve.VR;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing.UI
{
    [RequireComponent(typeof(FollowDlg))]
    public class GearDlg : Singleton<GearDlg>
    {
        private BaseDlg baseDlg;
        public event Action<int> SendAnswer;
        private Button btnThree;
        private Button btnFour;

       //public FScrollPage.FScrollPage fs;
        public ArcUIScroll arcSroll;
        public VRInputAxesConf vrPadInput;
        /// <summary>
        /// 哪个手柄
        /// </summary>
        public SteamVR_Input_Sources handType;
        public SteamVR_Input_Sources handType2;

       public float time = .5f;
        public float xianshitime = 20f;
        protected override void Awake()
        {
            base.Awake();
            InitComponent();
            //fs = GameObject.Find("Scene").transform.Find("ToolRoot/zhadaiqiang/zhadaiqiang/gearscrollCanvas/GameObject/gearscroll").GetComponent<FScrollPage.FScrollPage>();
            arcSroll = GameObject.Find("Scene").transform.Find("ToolRoot/zhadaiqiang/zhadaiqiang/gearscrollCanvas/GameObject/arcScroll").GetComponent<ArcUIScroll>();
            arcSroll?.setTargetObj(3);
            //AddListener();
        }

        private void Start()
        {
            HideDlg();
            //Debug.Log(TaskManager.Instance.firstTask.name+"++++++++++++++++++");
        }

        private void Update()
        {
            


            if (baseDlg.UiRoot.gameObject.activeSelf == true)
            {
                if (xianshitime >= 0f)
                {
                    xianshitime -= Time.deltaTime;
                }
                if (xianshitime <= 0f)
                {
                    //fs.gameObject.SetActive(false);
                    //arcSroll.gameObject.SetActive(false);
                }
                if (time >= 0f)
                {
                    time -= Time.deltaTime;

                }


                if (TaskManager.Instance.CurrentTask.name == "Task18")
                {
                    //Debug.Log("................................." + vrPadInput.vr_Action_Vector2.axis);
                    arcSroll?.UpdateNum(vrPadInput.vr_Action_Vector2.axis.x);
                    xianshitime = 10f;
                    //if (time <= 0f)
                    //{
                    //设置滑动
                    //if (vrPadInput.vr_Action_Vector2.GetAxisDelta(handType).x >= .5f || vrPadInput.vr_Action_Vector2.GetAxisDelta(handType2).x >= .5f)
                    //{

                    //    time = .5f;
                    //    fs.gameObject.SetActive(true);
                    //    xianshitime = 10f;
                    //    Debug.Log("///获取当前的输入轴===" + "===大于.5fGetAxisDelta");
                    //    MoveModel(1);
                    //}
                    //else if (vrPadInput.vr_Action_Vector2.GetAxisDelta(handType).x <= -.5f || vrPadInput.vr_Action_Vector2.GetAxisDelta(handType2).x <= -.5f)
                    //{

                    //    time = .5f;
                    //    fs.gameObject.SetActive(true);
                    //    xianshitime = 10f;
                    //    Debug.Log("///获取当前的输入轴===" + "===大于.5fGetAxisDelta");
                    //    MoveModel(-1);

                    //}

                    //}
                }
                
            }
               
            
        }
      
       //public void MoveModel(int dit)
       // {
       //     if (dit==1)
       //     {
       //         Debug.Log("右滑");
       //         if (fs.NowItemID < 5)
       //         {
       //             fs.MoveToItemID(fs.NowItemID + 1);
       //         }
       //     }
       //     else if (dit == -1)
       //     {
       //         Debug.Log("左滑");
       //         if (fs.NowItemID > 0)
       //         {
       //             fs.MoveToItemID(fs.NowItemID - 1);
       //         }
       //     }
       // }
        private void InitComponent()
        {
            baseDlg = GetComponent<BaseDlg>();

            btnThree = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnThree");
            btnFour = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnFour");
        }

        public void HideDlg()
        {
            if (baseDlg)
            {
                vrPadInput.actionSet.Deactivate(handType);
                baseDlg.UiRoot.gameObject.SetActive(false);
                
            }
               
        }


        public void ShowDlg()
        {
            if (baseDlg)
            {
                vrPadInput.actionSet.Activate(handType);
                
                baseDlg.UiRoot.gameObject.SetActive(true);
            }
                
        }

        //private void AddListener()
        //{
        //    btnThree.onClick.AddListener(() => {
        //        if (SendAnswer != null)
        //            SendAnswer(3);
        //    });

        //    btnFour.onClick.AddListener(() => {
        //        if (SendAnswer != null)
        //            SendAnswer(4);
        //        HideDlg();
        //    });
        //}

        private void RemoveListener()
        {
            btnThree.onClick.RemoveAllListeners();
            btnFour.onClick.RemoveAllListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListener();
        }
    }
}


