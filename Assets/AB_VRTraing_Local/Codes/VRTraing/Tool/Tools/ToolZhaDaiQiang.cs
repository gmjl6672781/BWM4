using System;
using System.Collections;
using System.Collections.Generic;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Mgr;
using BeinLab.VRTraing.UI;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing
{
    public class ToolZhaDaiQiang : ToolBasic
    {
        private int mGear = 0;

        public GameObject fs;
        public GameObject flashObj;//高亮闪烁的图片
        private RectTransform rectT;
        private bool isFirst;

        protected override void Awake()
        {
            //Debug.Log("zhaDaiQiang Awake");
            base.OnDestroy();
            base.Awake();
            OnPutAoCao += OnToolPutAoCao;
            isFirst = true;
            rectT = flashObj.GetComponent<RectTransform>();
        }

        //protected override void Start()
        //{
        //    Debug.Log("zhaDaiQiang Start");
        //    base.Start();
        //    OnPutAoCao += OnToolPutAoCao;
        //    isFirst = true;
        //    rectT = flashObj.GetComponent<RectTransform>();
        //}

        private void OnToolPutAoCao(PutTooConf obj)
        {
            fs.SetActive(false);
        }

        public int Gear {
            get {
                return mGear;
            }

            set {
                mGear = value;
            }
        }

        public void setFlash()
        {
            RectTransform t = rectT.transform.parent.gameObject.GetComponent<RectTransform>();
            if (t!=null)
            {
                rectT.sizeDelta = t.sizeDelta;
            }
            flashObj.SetActive(!flashObj.activeSelf);
        }
        /// <summary>
        /// 当拿起的时候
        /// </summary>
        /// <param name="hand"></param>
        protected override void OnCatch_(Hand hand)
        {
            base.OnCatch_(hand);
            fs.SetActive(true);
            //Debug.Log("fs--OnCatch_"+fs.activeSelf);
            if (isFirst)
            {
                InvokeRepeating("setFlash",0.5f,0.5f); 
            }
        }
        /// <summary>
        /// 松手的时候，去掉引用
        /// </summary>
        /// <param name="hand"></param>
        protected override void OnDetach_(Hand hand)
        {
            base.OnDetach_(hand);
            fs.SetActive(false);
            if (isFirst)
            {
                CancelInvoke("setFlash");
                flashObj.SetActive(false);
            }
            isFirst = false;
        }
        
    }

}
