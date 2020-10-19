using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeinLab.Util;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing.UI
{
    public class TieZhiBox : MonoBehaviour
    {
        private Button btnYellow;
        private Button btnOrange;
        private Button btnRed;
        private Button btnCyan;
        public GameObject goTieZhiYellow;
        public GameObject goTieZhiOrange;
        public GameObject goTieZhiRed;
        public GameObject goTieZHiCyan;

        private void Awake()
        {
            btnYellow = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnYellow");
            btnOrange = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnOrange");
            btnRed    = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnRed");
            btnCyan   = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnCyan");

            btnYellow.onClick.AddListener(() => {
                GameObject goInstantiate = Instantiate(goTieZhiYellow);
                Player.instance.rightHand.AttachObject(goInstantiate, GrabTypes.Pinch, goInstantiate.GetComponent<ToolBasic>().toolConf.catchFlags);
            });

            btnOrange.onClick.AddListener(() => {
                GameObject goInstantiate = Instantiate(goTieZhiOrange);
                Player.instance.rightHand.AttachObject(goInstantiate, GrabTypes.Pinch, goInstantiate.GetComponent<ToolBasic>().toolConf.catchFlags);
            });

            btnRed.onClick.AddListener(() => {
                GameObject goInstantiate = Instantiate(goTieZhiRed);
                Player.instance.rightHand.AttachObject(goInstantiate, GrabTypes.Pinch, goInstantiate.GetComponent<ToolBasic>().toolConf.catchFlags);
            });

            btnCyan.onClick.AddListener(() => {
                GameObject goInstantiate = Instantiate(goTieZHiCyan);
                Player.instance.rightHand.AttachObject(goInstantiate, GrabTypes.Pinch, goInstantiate.GetComponent<ToolBasic>().toolConf.catchFlags);
            });

        }

    }
}

