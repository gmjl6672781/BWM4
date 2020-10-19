using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Controller;
using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLogo : MonoBehaviour
{
    public ActionConf action;
    public DynamicConf matReset;
    public Button one;
    public Button oneChild;

    // Start is called before the first frame update
    void Start()
    {
        one.onClick.AddListener(LogoState);
        oneChild.onClick.AddListener(LogoState);
    }

    private void LogoState()
    {
        DynamicActionController.Instance.DoAction(action);
        if(matReset!=null)
            DynamicActionController.Instance.DoDynamic(matReset);
    }
}
