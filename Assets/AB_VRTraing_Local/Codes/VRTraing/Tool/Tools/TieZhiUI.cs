using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeinLab.Util;
using Valve.VR.InteractionSystem;
using BeinLab.VRTraing.Conf;

namespace BeinLab.VRTraing.UI
{
    public class TieZhiUI : MonoBehaviour
    {
        private Button btnYellow1;
        private Button btnOrange1;
        private Button btnRed1;
        private Button btnCyan1;
        private Button btnYellow2;
        private Button btnOrange2;
        private Button btnRed2;
        private Button btnCyan2;
        public ToolConf toolYellow;
        public ToolConf toolOrange;
        public ToolConf toolRed;
        public ToolConf toolCyan;

        private void Awake()
        {
            btnYellow1 = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnYellow1");
            btnOrange1 = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnOrange1");
            btnRed1    = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnRed1");
            btnCyan1   = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnCyan1");
            btnYellow2 = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnYellow2");
            btnOrange2 = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnOrange2");
            btnRed2 = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnRed2");
            btnCyan2 = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnCyan2");

            btnYellow1.onClick.AddListener(() => {
                Player.instance.rightHand.AttachObject(toolYellow.toolBasic.gameObject, GrabTypes.Pinch, toolYellow.catchFlags);
            });
            btnYellow2.onClick.AddListener(() => {
                Player.instance.rightHand.AttachObject(toolYellow.toolBasic.gameObject, GrabTypes.Pinch, toolYellow.catchFlags);
            });
            btnOrange1.onClick.AddListener(() =>
            {
                Player.instance.rightHand.AttachObject(toolOrange.toolBasic.gameObject, GrabTypes.Pinch, toolOrange.catchFlags);
            });
            btnOrange2.onClick.AddListener(() =>
            {
                Player.instance.rightHand.AttachObject(toolOrange.toolBasic.gameObject, GrabTypes.Pinch, toolOrange.catchFlags);
            });
            btnRed1.onClick.AddListener(() =>
            {
                Player.instance.rightHand.AttachObject(toolRed.toolBasic.gameObject, GrabTypes.Pinch, toolRed.catchFlags);
            });
            btnRed2.onClick.AddListener(() =>
            {
                Player.instance.rightHand.AttachObject(toolRed.toolBasic.gameObject, GrabTypes.Pinch, toolRed.catchFlags);
            });
            btnCyan1.onClick.AddListener(() =>
            {
                Player.instance.rightHand.AttachObject(toolCyan.toolBasic.gameObject, GrabTypes.Pinch, toolCyan.catchFlags);
            });
            btnCyan2.onClick.AddListener(() =>
            {
                Player.instance.rightHand.AttachObject(toolCyan.toolBasic.gameObject, GrabTypes.Pinch, toolCyan.catchFlags);
            });
        }

    }
}

