using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing.Mgr
{
    /// <summary>
    /// 这是管理手柄的插件的类，封装手柄的相关接口
    /// 监听手柄初始化，并通知外界
    /// </summary>
    public class VRHandHelper : Singleton<VRHandHelper>
    {
        /// <summary>
        /// 侧键
        /// </summary>
        public VRInputConf VRInput_GrabGrip;
        /// <summary>
        /// 扳机键
        /// </summary>
        public VRInputConf VRInput_GrabPinch;
        public VRInputConf VRInput_InteractUI;
        /// <summary>
        /// 菜单键
        /// </summary>
        public VRInputConf VRInput_Menu;
        /// <summary>
        /// 圆盘键
        /// </summary>
        public VRInputConf VRInput_Teleport;
        public VRInputAxesConf VRInputAxes;
        /// <summary>
        /// 手柄完成初始化的事件
        /// </summary>
        public event Action OnVRHandActive;
        /// <summary>
        /// 当某手柄激活时事件
        /// </summary>
        public event Action<Hand> OnHandActive;
        private Transform target;

        public Transform Target { get => target; set => target = value; }
        public Camera CenterCamera;
        private Dictionary<string, Coroutine> hintTips;
        [HideInInspector]
        public bool isActiveHandController = false;
        /// <summary>
        /// 加入一个手柄按键监听事件
        /// </summary>
        /// <param name="hintName"></param>
        /// <param name="hand"></param>
        /// <param name="teleportAction"></param>
        /// <param name="message"></param>
        public void AddHint(string hintName, Hand hand, SteamVR_Action_Boolean teleportAction, string message)
        {
            if (hintTips == null)
            {
                hintTips = new Dictionary<string, Coroutine>();
            }
            Coroutine hint = null;
            if (hintTips.ContainsKey(hintName))
            {
                hint = hintTips[hintName];
                StopCoroutine(hint);
                hintTips.Remove(hintName);
            }
            hint = StartCoroutine(TeleportHintCoroutine(hand, teleportAction, message));
            hintTips.Add(hintName, hint);
        }
        /// <summary>
        /// 移除一个手柄按键事件
        /// </summary>
        /// <param name="hintName"></param>
        public void RemoveHint(string hintName)
        {
            if (hintTips != null && hintTips.ContainsKey(hintName))
            {
                Coroutine hint = hintTips[hintName];
                StopCoroutine(hint);
                hintTips.Remove(hintName);
                hint = null;
                StopAllHint();
            }
        }
        public void StopAllHint()
        {
            for (int i = 0; i < Player.instance.hands.Length; i++)
            {
                var hand = Player.instance.hands[i];
                //VRHandHelper.Instance.HideHint(hand, vrInputConf.vr_Action_Button);
                ControllerButtonHints.HideAllTextHints(hand);
                hand.ShowController(true);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
            isActiveHandController = false;
            OnHandActive += OnHandActiveEvent;
            OnVRHandActive += OnVRHandActiveEvent;
            Target = new GameObject("Target").transform;
            CenterCamera.transform.SetParent(transform);
        }
        private void Update()
        {
            if(Target==null)
            {
                GameObject obj = GameObject.Find("Target");
                if (obj)
                    Target = obj.transform;
                else
                    Target = new GameObject("Target").transform;
            }
            if (Player.instance)
            {
                //Debug.Log("Target----------------" + Target);
                //Debug.Log("Player.instance.hmdTransform----------------" + Player.instance.hmdTransform);
                Target.position = Player.instance.hmdTransform.position;
                Vector3 forward = Player.instance.hmdTransform.transform.forward;
                forward.y = 0;
                Target.transform.forward = forward;
                CenterCamera.transform.position = Player.instance.hmdTransform.position;
                Vector3 tmpAngle = Player.instance.hmdTransform.eulerAngles;
                tmpAngle.z = 0;
                CenterCamera.transform.eulerAngles = tmpAngle;
            }
        }
        private void OnVRHandActiveEvent()
        {
            isActiveHandController = true;
        }
        /// <summary>
        /// 手柄完成初始化事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnHandActiveEvent(Hand hand)
        {
            if (hand && hand.handType == SteamVR_Input_Sources.LeftHand
                || hand.handType == SteamVR_Input_Sources.RightHand)
            {
                if (Player.instance.leftHand && Player.instance.leftHand.isActiveAndEnabled && Player.instance.leftHand.isPoseValid &&
                    Player.instance.rightHand && Player.instance.rightHand && Player.instance.rightHand.isPoseValid)
                {
                    OnVRHandActive?.Invoke();
                }
            }
        }

        /// <summary>
        /// 设置手柄激活
        /// </summary>
        public void SetHandActive(Hand hand)
        {
            OnHandActive?.Invoke(hand);
        }

        /// <summary>
        /// 震动手柄
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="microSecondsDuration">振幅，值越大，震动约强烈，不超过2000</param>
        public void ShockHand(Hand hand, ushort microSecondsDuration = 500)
        {
            hand.TriggerHapticPulse(microSecondsDuration);
        }
        /// <summary>
        /// 在手柄某个位置创建提示文本
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="teleportAction"></param>
        /// <param name="message"></param>
        public void ShowHint(Hand hand, SteamVR_Action_Boolean teleportAction, string message)
        {
            ControllerButtonHints.ShowTextHint(hand, teleportAction, message);
        }
        /// <summary>
        /// 取消显示按键文本提示
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="teleportAction"></param>
        /// <param name="message"></param>
        public void HideHint(Hand hand, SteamVR_Action_Boolean teleportAction)
        {
            ControllerButtonHints.HideTextHint(hand, teleportAction);
        }
        /// <summary>
        /// 持续性震动手柄，并且高亮手柄上的按钮
        /// </summary>
        /// <param name="hand">手柄</param>
        /// <param name="teleportAction">要高亮的按键</param>
        /// <param name="message">提示的信息</param>
        /// <returns></returns>
        public IEnumerator TeleportHintCoroutine(Hand hand, SteamVR_Action_Boolean teleportAction, string message)
        {
            float prevBreakTime = Time.time;
            float prevHapticPulseTime = Time.time;
            while (true)
            {
                bool pulsed = false;
                bool showHint = IsEligibleForTeleport(hand);
                bool isShowingHint = !string.IsNullOrEmpty(ControllerButtonHints.GetActiveHintText(hand, teleportAction));
                if (showHint)
                {
                    if (!isShowingHint)
                    {
                        //ControllerButtonHints.ShowTextHint(hand, teleportAction, message);
                        ShowHint(hand, teleportAction, message);
                        prevBreakTime = Time.time;
                        prevHapticPulseTime = Time.time;
                    }
                    if (Time.time > prevHapticPulseTime + 0.05f)
                    {
                        //Haptic pulse for a few seconds
                        pulsed = true;
                        //hand.TriggerHapticPulse(500);
                        ShockHand(hand);
                    }
                }
                else if (!showHint && isShowingHint)
                {
                    //ControllerButtonHints.HideTextHint(hand, teleportAction);
                    HideHint(hand, teleportAction);
                }
                if (Time.time > prevBreakTime + 3.0f)
                {
                    //Take a break for a few seconds
                    yield return new WaitForSeconds(3.0f);

                    prevBreakTime = Time.time;
                }

                if (pulsed)
                {
                    prevHapticPulseTime = Time.time;
                }

                yield return null;
            }
        }


        /// <summary>
        /// 是否可跳跃
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public bool IsEligibleForTeleport(Hand hand)
        {
            if (hand == null)
            {
                return false;
            }

            if (!hand.gameObject.activeInHierarchy)
            {
                return false;
            }

            if (hand.hoveringInteractable != null)
            {
                return false;
            }

            if (hand.noSteamVRFallbackCamera == null)
            {
                if (hand.isActive == false)
                {
                    return false;
                }

                //Something is attached to the hand
                if (hand.currentAttachedObject != null)
                {
                    AllowTeleportWhileAttachedToHand allowTeleportWhileAttachedToHand = hand.currentAttachedObject.GetComponent<AllowTeleportWhileAttachedToHand>();

                    if (allowTeleportWhileAttachedToHand != null && allowTeleportWhileAttachedToHand.teleportAllowed == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //Debug.Log("------------------------OnDestroy");
            OnHandActive -= OnHandActiveEvent;
        }
        /// <summary>
        /// 释放某个手的物体
        /// 注意，释放可能不成功，可以通过设计的方式进行规避
        /// </summary>
        /// <param name="hand"></param>
        public void ReleaseObject(Hand hand)
        {

        }
    }
}
