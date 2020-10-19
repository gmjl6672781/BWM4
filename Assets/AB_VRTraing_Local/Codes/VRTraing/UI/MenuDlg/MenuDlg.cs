using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Gamer;
using BeinLab.VRTraing.UI;
using DG.Tweening;
using Karler.WarFire.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace BeinLab.UI
{
    /// <summary>
    /// 任务菜单
    /// </summary>
    public class MenuDlg : Singleton<MenuDlg>
    {
        public VRInputConf vrInputShow;
        public VRInputAxesConf vrPadInput;
        /// <summary>
        /// 哪个手柄
        /// </summary>
        public SteamVR_Input_Sources handType;
        public Vector3[] linePath;

        private BaseDlg dlg;
        private VRHand target;
        public int forward = 1;
        private float lastPos;
        private bool isTouch = false;
        private bool isReset = false;
        public List<Transform> loopsTrans;
        public List<Transform> targetTrans;
        private int curIndex;
        private List<Vector3> rootPosList;
        public float doTime = 0.5f;
        public event Action OnSwipeMenu;
        private void Start()
        {
            Dlg = GetComponent<BaseDlg>();
            target = null;
            for (int i = 0; i < Player.instance.handCount; i++)
            {
                if (handType == SteamVR_Input_Sources.Any || Player.instance.hands[i].handType == handType)
                {
                    target = Player.instance.hands[i].GetComponent<VRHand>();
                    break;
                }
            }
            Dlg.UiRoot.gameObject.SetActive(false);
            rootPosList = new List<Vector3>();
            for (int i = 0; i < loopsTrans.Count; i++)
            {
                rootPosList.Add(loopsTrans[i].localPosition);
            }
        }

        public void CloseDlg()
        {
            Dlg.UiRoot.gameObject.SetActive(false);
        }

        public int dir = 1;

        public BaseDlg Dlg { get => dlg; set => dlg = value; }
        private void FixedUpdate()
        {
            if (target && Dlg.UiRoot.gameObject.activeSelf)
            {
                transform.position = target.GetDlgLinePos(dlg.transPos);
                UnityUtil.LookAtV(transform, Player.instance.hmdTransform, forward);
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                MoveModel(dir);
            }
            if (Input.GetKeyDown(KeyCode.O) || vrInputShow.GetKeyDown(handType))
            {
                Dlg.UiRoot.gameObject.SetActive(!Dlg.UiRoot.gameObject.activeSelf);
                if (Dlg.UiRoot.gameObject.activeSelf)
                {
                    vrPadInput.actionSet.Activate(handType);
                    if (linePath != null && linePath.Length > 1)
                    {
                        if (target)
                        {
                            target.SetLinePath(linePath);
                        }
                    }
                    MoveModel(0);
                    PopDlg.Instance.HideDlg();
                    ResultDlg.Instance.gameObject.SetActive(false);
                }
                else
                {
                    vrPadInput.actionSet.Deactivate(handType);
                    lastPos = 0;
                    isTouch = false;
                }
            }
            if (target && Dlg.UiRoot.gameObject.activeSelf)
            {

                //transform.position = target.GetDlgLinePos(dlg.transPos);
                //UnityUtil.LookAtV(transform, Player.instance.hmdTransform, forward);
                ///获取当前的输入轴
                float tmp = vrPadInput.vr_Action_Vector2.GetAxisDelta(handType).x;
                ///获取按下时的坐标
                if (!isTouch)
                {
                    if (Mathf.Abs(tmp) > 0.1f)
                    {
                        isTouch = true;
                    }
                    lastPos = tmp;
                    isReset = false;
                }
                ///按住
                if (isTouch)
                {
                    if (!isReset)
                    {
                        if (Mathf.Abs(tmp - lastPos) > 0.5f)
                        {
                            lastPos = tmp;
                            isReset = true;
                        }
                        lastPos = tmp;
                    }
                    else
                    {
                        if (Mathf.Abs(tmp - lastPos) > 0.5f)
                        {
                            Swipe(tmp - lastPos);
                            lastPos = 0;
                            isTouch = false;
                            isReset = false;
                        }
                    }
                }
            }
        }
        public void Swipe(float delt)
        {
            MoveModel(delt > 0 ? 1 : -1);
        }
        /// <summary>
        /// 循环Loop转动
        /// </summary>
        /// <param name="dir"></param>
        private void MoveModel(int dir)
        {
            if (dir != 0)
            {
                OnSwipeMenu?.Invoke();
            }
            curIndex += dir;
            for (int i = 0; i < targetTrans.Count; i++)
            {
                Transform show = targetTrans[i];
                bool isNormal = true;
                int next = (i + curIndex) % targetTrans.Count;
                if (next < 0)
                {
                    next += targetTrans.Count;
                    isNormal = false;
                }
                if (dir > 0)
                {
                    if (next == 0)
                    {
                        isNormal = false;
                    }
                }
                else if (dir < 0)
                {
                    if (next == targetTrans.Count - 1)
                    {
                        isNormal = false;
                    }
                }
                show.DOKill();
                if (show.localScale != Vector3.one)
                {
                    show.localScale = Vector3.one;
                }
                Vector3 nextPos = rootPosList[next];
                int index = next;
                show.DOLocalMove(nextPos, doTime).onComplete += () =>
                {
                    Vector3 endV = Vector3.zero;
                    if (index == targetTrans.Count / 2)
                    {
                        endV = Vector3.one;
                    }
                    show.DOScale(endV, doTime).SetDelay(doTime * Mathf.PI);
                };
                if (!isNormal)
                {
                    show.DOScale(Vector3.zero, doTime / 2).onComplete += () =>
                      {
                          show.DOScale(Vector3.one, doTime / 2);
                      };
                }
                //Animator[] anis = show.GetComponentsInChildren<Animator>();
                //for (int j = 0; j < anis.Length; j++)
                //{
                //    anis[j].SetInteger("Anim", next == 0 ? 1 : 0);
                //}
                //DoEffect(show, next);

            }
        }
    }
}