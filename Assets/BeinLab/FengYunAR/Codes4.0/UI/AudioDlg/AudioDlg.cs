using BeinLab.Util;
using Karler.WarFire.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace BeinLab.FengYun.UI
{
    public class AudioDlg : MonoBehaviour
    {
        //public ActionConf onActionConf;
        //public ActionConf offActionConf;
        // Start is called before the first frame update
        public static float curVolume = 1;
        public static event Action<float> OnChangeVolume;
        void Start()
        {
            BaseDlg dlg = GetComponent<BaseDlg>();
            Toggle toggle = dlg.GetChildComponent<Toggle>("Toggle");
            toggle.onValueChanged.AddListener(OnValueChanged);
            TimerMgr.Instance.CreateTimer(delegate ()
            {
                toggle.isOn = curVolume < 0.5f;
            }, 0.02f, 1);
        }

        private void OnValueChanged(bool isOn)
        {
            //GameObject.FindObjectOfType<AudioListener>().enabled = !isOn;
            curVolume = isOn ? 0 : 1;
            if (OnChangeVolume != null)
            {
                OnChangeVolume(curVolume);
            }
        }
    }
}