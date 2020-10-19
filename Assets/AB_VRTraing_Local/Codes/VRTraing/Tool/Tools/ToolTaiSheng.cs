using BeinLab.Util;
using BeinLab.VRTraing.Mgr;
//using System;
using UnityEngine;
using Valve.VR.InteractionSystem;
namespace BeinLab.VRTraing
{
    /// <summary>
    /// 抬升工具
    /// </summary>
    public class ToolTaiSheng : ToolBasic
    {
        /// <summary>
        /// 手套模型的预制体
        /// </summary>
        public GameObject otherHand;
        public GameObject myLeftHand;
        public GameObject myRightHand;
        /// <summary>
        /// 抬升工具的节点
        /// </summary>
        //public Transform taishengRoot;
        public Transform dianchiRoot;
        public static event System.Action OnPutOut;
        public CarryMan carryMan;
        public Transform fllowRoot;
        public static event System.Action<Transform> OnUpdatePos;
        private bool isTaiSheng = false;
        //public Transform defultTras;
        protected override void Start()
        {
            otherHand.SetActive(false);
            myLeftHand.SetActive(false);
            myRightHand.SetActive(false);
            carryMan.transform.SetParent(null);

            OnUpdatePos?.Invoke(null);
            //carryMan.gameObject.SetActive(false);
            ToolDianChi.OnTaiSheng += OnTaiSheng;
        }

        private void OnTaiSheng(bool obj)
        {
            isTaiSheng = obj;
        }

        protected override void OnCatch_(Hand hand)
        {
            base.OnCatch_(hand);
            //otherHand.SetActive(true);
            SetToolHighlight(false);
            myLeftHand.SetActive(hand.handType == Valve.VR.SteamVR_Input_Sources.RightHand);
            myRightHand.SetActive(hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand);
            //TimerMgr.Instance.CreateTimer(() =>
            //{
            //    SetToolHighlight(false);
            //}, 1);
            //carryMan.gameObject.SetActive(true);
            //dianchiRoot.DetachChildren();
        }

        /// <summary>
        /// 当放下的时候
        /// </summary>
        /// <param name="hand"></param>
        protected override void OnDetach_(Hand hand)
        {
            base.OnDetach_(hand);
            otherHand.SetActive(false);
            //taishengRoot.localRotation = Quaternion.identity;
            otherHand.SetActive(false);
            myLeftHand.SetActive(false);
            myRightHand.SetActive(false);
            OnUpdatePos?.Invoke(null);
            TimerMgr.Instance.CreateTimer(() =>
            {
                OnUpdatePos?.Invoke(null);
                //SetToolHighlight(false);
                //taishengRoot.transform.localEulerAngles = Vector3.zero;
            }, 0.3f);
            //SetToolHighlight(false);
            //taishengRoot.transform.localEulerAngles = Vector3.zero;
            if (isTaiSheng)
            {
                OnPutOut?.Invoke();
            }
        }
        /// <summary>
        /// 当拿起工具的时候
        /// </summary>
        /// <param name="hand"></param>
        protected override void OnCatching_(Hand hand)
        {
            base.OnCatching_(hand);
            Vector3 angle = transform.eulerAngles;
            angle.x = angle.z = 0;
            //taishengRoot.transform.eulerAngles = angle;
            transform.eulerAngles = angle;
            Vector3 pos = fllowRoot.transform.position;
            pos.y = 0;
            fllowRoot.position = pos;
            OnUpdatePos?.Invoke(fllowRoot);
            VRHandHelper.Instance.ShockHand(Player.instance.leftHand, (ushort)(isTaiSheng ? 2000 : 500));
            VRHandHelper.Instance.ShockHand(Player.instance.rightHand, (ushort)(isTaiSheng ? 2000 : 500));
        }
    }
}