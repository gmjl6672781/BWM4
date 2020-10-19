using BeinLab.UI;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Mgr;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HandleTriggerOperation : MonoBehaviour
{
    /// 哪个按键
    /// </summary>
    public VRInputConf vrInputConf;
    /// <summary>
    /// 哪个手柄
    /// </summary>
    public SteamVR_Input_Sources handType;
    public bool isShock = true;
    public TaskConf taskConf;
    private ControllerButtonHints hints;
    private Coroutine cor;
    private CoroutineHelper coroutineHelper;
    private Hand hand;
    private bool isStart;
   
    private void OnEnable()
    {
        isStart = true;
        if (coroutineHelper == null)
        {
            GameObject tmp = new GameObject("CorHelper");
            coroutineHelper = tmp.AddComponent<CoroutineHelper>();
        }
        if (TaskManager.Instance.CurrentTask == taskConf)
            Show();
    }

    private void Show()
    {
        if (VRHandHelper.Instance && Player.instance)
        {
            for (int i = 0; i < Player.instance.hands.Length; i++)
            {
                if (handType == SteamVR_Input_Sources.Any || Player.instance.hands[i].handType == handType)
                {
                    hand = Player.instance.hands[i];
                    hints = hand.GetComponentInChildren<ControllerButtonHints>();
                    if (isShock)
                    {
                        cor = coroutineHelper.StartCoroutine(VRHandHelper.Instance.TeleportHintCoroutine(hand, vrInputConf.vr_Action_Button, "选中记录时按住扳机键, \n上下移动手柄可滑动面版"));
                        //StartCoroutine(TeleportHintCoroutine(hand, vrInputConf.vr_Action_Button, "选中记录时按住扳机键, \n上下移动手柄可滑动面版"));
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (isStart && vrInputConf.GetKeyDown(handType))
        {
            coroutineHelper.StopCoroutine(cor);
            hints.HideText(vrInputConf.vr_Action_Button);
            hand.ShowController(true);
            isStart = false;
            ////print("isStart" + isStart);
            Destroy(coroutineHelper.gameObject);
        }
    }

}
