using BeinLab.VRTraing.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseLanguage : MonoBehaviour
{
    public static Action action;
    public GameObject logoDlg;
    public Button butChinese;
    public Button butEnglish;
    //public Text debugText;

    // Start is called before the first frame update
    void Start()
    {
        butChinese.onClick.AddListener(delegate () {
            Debug.Log("ClickChinese");
            LanguageMgr.Instance.SelectLanguage(0);
            if (action != null)
                action();
            gameObject.SetActive(false);
        });
        butEnglish.onClick.AddListener(delegate () {
            LanguageMgr.Instance.SelectLanguage(1);
            if (action != null)
                action();
            gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    LanguageMgr.Instance.SelectLanguage(0);
        //    if (action != null)
        //        action();
        //    gameObject.SetActive(false);
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    LanguageMgr.Instance.SelectLanguage(1);
        //    if (action != null)
        //        action();
        //    gameObject.SetActive(false);
        //}
    }
}
