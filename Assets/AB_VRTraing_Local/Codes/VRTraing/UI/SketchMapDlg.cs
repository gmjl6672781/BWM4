using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Gamer;
using UnityEngine.UI;
using System;
using Karler.WarFire.UI;

namespace BeinLab.VRTraing.UI
{
    /// <summary>
    /// 电池示意图
    /// </summary>
    public class SketchMapDlg : Singleton<SketchMapDlg>
    {
        private BaseDlg baseDlg;
        public GameObject tips;

        public GameObject obj_1;
        public GameObject obj_1_line;
        public GameObject obj_2;
        public GameObject obj_2_line;
        public GameObject obj_1_m;//1-
        public GameObject obj_1_m_line;
        public GameObject obj_2_p;//2+
        public GameObject obj_2_p_line;


        protected override void Awake()
        {
            base.Awake();
            baseDlg = GetComponent<BaseDlg>();

        }

        private void Start()
        {
            HideDlg();
        }

        public void HideDlg()
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(false);
        }


        public void ShowDlg()
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(true);
            if (tips != null)
                tips.SetActive(true);

            obj_1.SetActive(true);
            obj_1_line.SetActive(true);
            obj_2.SetActive(true);
            obj_2_line.SetActive(true);
            obj_1_m.SetActive(true);//1-
            obj_1_m_line.SetActive(true);
            obj_2_p.SetActive(true);//2+
            obj_2_p_line.SetActive(true);
    }


        public void HideTips()
        {
            if (tips!=null)
            {
                tips.SetActive(false);
            }
        }

        public void SetHideTips(string collor)
        {
            //Debug.Log("___+++++++____"+ collor);
            switch (collor)
            {
                case "red":
                case "Red":
                    obj_1_m.SetActive(false);
                    obj_1_m_line.SetActive(false);
                    break;
                case "Yellow":
                case "yellow":
                    obj_1.SetActive(false);
                    obj_1_line.SetActive(false);
                    break;
                case "Orange":
                case "orange":
                    obj_2.SetActive(false);
                    obj_2_line.SetActive(false);
                    break;
                case "cyan":
                case "Cyan":
                    obj_2_p.SetActive(false);
                    obj_2_p_line.SetActive(false);
                    break;
            }
        }

    }
}

