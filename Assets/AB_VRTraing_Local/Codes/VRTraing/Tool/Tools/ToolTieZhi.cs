using System.Collections;
using System.Collections.Generic;
using BeinLab.VRTraing.Conf;
using UnityEngine;
using BeinLab.VRTraing.Controller;
using Valve.VR.InteractionSystem;
using System;

namespace BeinLab.VRTraing
{
    public class ToolTieZhi : ToolBasic
    {
        /// <summary>
        /// 跟随手的协程
        /// </summary>
        private Coroutine fllowCoroutine;
        private bool isInHand;

        private Transform lastParent;

        protected override void Start()
        {           
            OnPressDown += MyPressEvent;
            OnPressUp += MyUpEvent;
            isInHand = false;
            lastParent = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnPressDown -= MyPressEvent;
            OnPressUp -= MyUpEvent;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("_______________________________________"+collision.gameObject.name);
            if (collision.gameObject!=null && collision.gameObject.name.Equals("TieZhiPool"))
            {
                ResetPos(collision.gameObject.transform);
            }
            if (collision.gameObject != null && collision.gameObject.name.Equals("DiYaChaTou1"))
            {
             
            }
        }
        /// <summary>
        /// 重置位置
        /// </summary>
        public void ResetPos(Transform tran)
        {
            isInHand = false;
            this.transform.SetParent(tran);
            Throwable t = GetComponent<Throwable>();
            if (t != null)
            {
                Destroy(t);
            }
            Rigidbody rid = GetComponent<Rigidbody>();
            if (rid != null)
            {
                Destroy(rid);
            }

            BoxCollider bc = GetComponentInChildren<BoxCollider>();
            if (bc != null)
            {
                bc.isTrigger = false;
            }
            transform.localPosition = toolConf.InitPosition;
            transform.localEulerAngles = toolConf.InitAngle;
        }

        private void MyPressEvent(Hand hand, ToolConf toolConf)
        {

            if (hand.otherHand == null || hand.otherHand.currentAttachedObject == null)
            {
                return;
            }
            if (!ToolTieZhiPool.isInHand || !hand.otherHand.currentAttachedObject.name.Equals("TieZhiPool"))
            {
                return;
            }
            isInHand = true;            
            hand.HideController();
            //hand.AttachObject(gameObject,GrabTypes.Grip, toolConf.catchFlags);
            lastParent = this.transform.parent;
            this.transform.SetParent(hand.transform);
            this.transform.localPosition = this.toolConf.handPosition;
            this.transform.localEulerAngles = this.toolConf.handAngle;

            //Rigidbody rid = GetComponent<Rigidbody>();
            //if (rid == null)
            //{
            //    rid=gameObject.AddComponent<Rigidbody>();
            //}
            //rid.useGravity = false;
            //rid.isKinematic = false;
            //BoxCollider bc = GetComponentInChildren<BoxCollider>();
            //if (bc != null)
            //{
            //    bc.isTrigger = true;
            //}
            //Throwable t = GetComponent<Throwable>();
            //if (t == null)
            //{
            //    t = gameObject.AddComponent<Throwable>();
            //    throwable = t;
            //}
        }

        private void MyUpEvent(Hand hand, ToolConf toolConf)
        {
            if (hand.otherHand == null || hand.otherHand.currentAttachedObject == null)
            {
                return;
            }
            hand.ShowController(true);
            if (!isInHand)
            {
                return;
            }
            isInHand = false;
            
           
            this.transform.SetParent(null);
            Rigidbody rid = GetComponent<Rigidbody>();
            if (rid==null)
            {
                gameObject.AddComponent<Rigidbody>();
            }
            Throwable t= GetComponent<Throwable>();
            if (t==null)
            {
                t=gameObject.AddComponent<Throwable>();
                throwable = t;
            }
            BoxCollider bc = GetComponentInChildren<BoxCollider>();
            if (bc!=null)
            {
                bc.isTrigger = false;
            }
        }


       
    }

}

