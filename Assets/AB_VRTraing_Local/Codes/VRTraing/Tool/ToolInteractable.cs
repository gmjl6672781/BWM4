using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.VRTraing.Conf;
using Valve.VR.InteractionSystem;
using Valve.VR;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing
{
    /// <summary>
    /// 工具基本交互
    /// </summary>
    public abstract class ToolInteractable : MonoBehaviour
    {
        protected bool isCanHover = true;
        protected bool isCanCatch = true;
        protected Throwable throwable;
        protected virtual void Awake()
        {
            throwable = GetComponent<Throwable>();
            if (throwable)
            {
                //Debug.Log("Awake-------------throwable");
                throwable.onPickUp.AddListener(OnPickUp);
                throwable.onDetachFromHand.AddListener(OnDetachFromHand);
                throwable.onHeldUpdate.AddListener(OnHeldUpdate);
            }
        }

        protected virtual void OnDestroy()
        {
            if (throwable)
            {
                //Debug.Log("OnDestroy-------------throwable");
                throwable.onPickUp.RemoveListener(OnPickUp);
                throwable.onDetachFromHand.RemoveListener(OnDetachFromHand);
                throwable.onHeldUpdate.RemoveListener(OnHeldUpdate);
            }
        }

        #region Hand.SendMessage Hand调用
        /// <summary>
        /// 当手柄触碰到可交互物体
        /// </summary>
        /// <param name="hand"></param>
        private void OnHandHoverBegin(Hand hand)
        {
            if (!isCanHover)
                return;
            OnHoverBegin_(hand);
        }

        /// <summary>
        /// 当手柄触碰到可交互物体时,并扣动了扳机，每帧UPdate调用
        /// </summary>
        /// <param name="hand"></param>
        private void HandHoverUpdate(Hand hand)
        {
            if (!isCanHover)
                return;

            OnHover_(hand);

            //扳机按下 握持 松开
            if (VRHandHelper.Instance.VRInput_GrabPinch.GetKeyDown(hand))
            {
                OnPressDown_(hand);
            }

            if (VRHandHelper.Instance.VRInput_GrabPinch.GetKey(hand))
            {
                OnPress_(hand);
            }

            if (VRHandHelper.Instance.VRInput_GrabPinch.GetKeyUp(hand))
            {
                OnPressUp_(hand);
            }
        }


        /// <summary>
        /// 手柄离开可交互物体
        /// </summary>
        /// <param name="hand"></param>
        private void OnHandHoverEnd(Hand hand)
        {
            if (!isCanHover)
                return;
            OnHoverEnd_(hand);
        }
        #endregion

        #region Throwable调用事件，被抓取的物体不会直接穿过固定静止的障碍物，而是跟随Hand沿障碍物边界滑动
        /// <summary>
        /// 拿起工具
        /// </summary>
        /// <param name="hand"></param>
        private void OnPickUp(Hand hand)
        {
            if (!isCanCatch)
                return;
            OnCatch_(hand);
        }

        /// <summary>
        /// 丢掉工具
        /// </summary>
        /// <param name="hand"></param>
        private void OnDetachFromHand(Hand hand)
        {
            if (!isCanCatch)
                return;
            OnDetach_(hand);
        }

        /// <summary>
        /// 握持工具
        /// </summary>
        /// <param name="hand"></param>
        private void OnHeldUpdate(Hand hand)
        {
            if (!isCanCatch)
                return;
            OnCatching_(hand);
        }

        #endregion


        #region Hand基本交互
        /// <summary>
        /// 扣下扳机
        /// </summary>
        /// <param name="hand"></param>
        protected abstract void OnPressDown_(Hand hand);


        /// <summary>
        /// 握持扳机
        /// </summary>
        /// <param name="hand"></param>
        protected abstract void OnPress_(Hand hand);


        /// <summary>
        /// 松开扳机
        /// </summary>
        /// <param name="hand"></param>
        protected abstract void OnPressUp_(Hand hand);

        /// <summary>
        /// 手柄触碰物体每帧调用
        /// </summary>
        /// <param name="hand"></param>
        protected abstract void OnHover_(Hand hand);
        /// <summary>
        /// 手柄离开物体
        /// </summary>
        /// <param name="hand"></param>
        protected abstract void OnHoverEnd_(Hand hand);

        /// <summary>
        /// 手柄触碰物体
        /// </summary>
        /// <param name="hand"></param>
        protected abstract void OnHoverBegin_(Hand hand);
        #endregion

        #region Throwable交互
        /// <summary>
        /// 拿起工具
        /// </summary>
        /// <param name="hand"></param>
        protected abstract void OnCatch_(Hand hand);

        /// <summary>
        /// 握持工具
        /// </summary>
        /// <param name="hand"></param>
        protected abstract void OnCatching_(Hand hand);

        /// <summary>
        /// 放下工具
        /// </summary>
        /// <param name="hand"></param>
        protected abstract void OnDetach_(Hand hand);

        #endregion



    }
}

