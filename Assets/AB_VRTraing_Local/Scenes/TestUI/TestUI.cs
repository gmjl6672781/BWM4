using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.VRTraing;
using BeinLab.UI;
using BeinLab.FengYun.UI;
using UnityEngine.UI;
namespace BeinLab.VRTraing.UI
{
    public class TestUI : MonoBehaviour
    {
        public JudgementQuestionConf JudgementQuestionConf;
        public ChoiceQuestionConf choiceQuestionConf;
        // Start is called before the first frame update
        private void Start()
        {
            //SelectModeDlg.Instance.ShowDlg();
            //JudgementQuestionDlg.Instance.ShowDlg(JudgementQuestionConf);
            //ChoiceQuestionDlg.Instance.ShowDlg(choiceQuestionConf);
            Debug.Log(System.DateTime.Now.ToString("yyyy/MM/dd/HH:mm:ss"));
        }

    }
}

