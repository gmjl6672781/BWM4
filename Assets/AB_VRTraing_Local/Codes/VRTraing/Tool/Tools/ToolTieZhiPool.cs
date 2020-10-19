using System.Collections;
using System.Collections.Generic;
using BeinLab.VRTraing.Conf;
using UnityEngine;
using BeinLab.VRTraing.Controller;
using Valve.VR.InteractionSystem;
using System;

namespace BeinLab.VRTraing
{
    public class ToolTieZhiPool : ToolBasic
    {
        public List<GameObject> objList = new List<GameObject>();
        public static bool isInHand;

        protected override void Start()
        {
            OnPressDown += MyPressEvent;
            OnPressUp += MyUpEvent;
            OnHover += MyHoverEvent;
            isInHand = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnPressDown -= MyPressEvent;
            OnPressUp -= MyUpEvent;
            OnHover -= MyHoverEvent;
        }
        private void MyPressEvent(Hand hand, ToolConf toolConf)
        {
            isInHand = true;           
        }
        private void MyUpEvent(Hand hand, ToolConf toolConf)
        {
            isInHand = false;
            //Debug.Log("-----------------------------------------------");
        }
        private void MyHoverEvent(Hand hand, ToolConf toolConf)
        {
            Debug.Log("00000000000");
        }

        private void OnCollisionEnter(Collision collision)
        {
            
            ////9是layer   son对应的int值
            //if (collision.gameObject.layer==9)
            //{
            //    collision.gameObject.GetComponent<ToolTieZhi>()?.ResetPos(transform);
            //}
        }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name + "-----------------------------" + other.gameObject.layer);
            if (other.gameObject.layer == 9)
            {
                //这里触碰到的物体是子类的碰撞器
                other.transform.parent.GetComponent<ToolTieZhi>()?.ResetPos(transform);
            }
        }


        //private void showSon()
        //{
        //    if (objList != null)
        //    {
        //        for (int i = 0; i < objList.Count; i++)
        //        {
        //            if (objList[i]!=null && objList[i].transform.childCount>0)
        //            {
        //                objList[i]?.transform.GetChild(0).gameObject.SetActive(true);
        //            }

        //        }
        //    }
        //}


    }
}