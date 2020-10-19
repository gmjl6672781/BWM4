using BeinLab.FengYun.Controller;
using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatLogoDlg : Singleton<CreatLogoDlg>
{
    public List<ActionConf> actions;
    public List<GameObject> logos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateDlg(string st)
    {
        if (st == "ButtonCellConf2_WG")
        {
            for (int i = 0; i < logos.Count; i++)
            {
                if (logos[i].activeSelf)
                {
                    DynamicActionController.Instance.DoAction(actions[i]);
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
