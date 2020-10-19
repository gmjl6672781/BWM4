using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.UI;

namespace BeinLab.VRTraing.Mgr
{
    public class UIManager : Singleton<UIManager>
    {
        //弹出选择题窗口
        public void ShowChoiceQuestionDlg(ChoiceQuestionConf choiceQuestionConf)
        {
            ChoiceQuestionDlg.Instance.ShowDlg(choiceQuestionConf);
        }

        public void ShowJudgementQuestion(JudgementQuestionConf judgementQuestionConf)
        {
            JudgementQuestionDlg.Instance.ShowDlg(judgementQuestionConf);
        }
    }
}

