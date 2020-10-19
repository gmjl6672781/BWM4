using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试获取指定key的文本
/// 测试监听语言变化
/// 测试基本的语言包读取
/// 测试语言编辑
/// </summary>
public class TestLanguage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LanguageConf my = new LanguageConf();
        my.PriKey = "Title1";
        my.Message = "标题1";
        my.AudioPath = "test1";
        UnityUtil.WriteXMLData<LanguageConf>(LanguageMgr.Instance.RealDataPath, my);
        LanguageMgr.Instance.changeLanuage += OnchangeLanuage;
    }
    /// <summary>
    /// 当检测到语言变化时
    /// </summary>
    /// <param name="languagePack"></param>
    private void OnchangeLanuage(LanguagePackConf languagePack)
    {
        print("OnchangeLanuage");
    }
    public void TestGetLanguage()
    {
        print(LanguageMgr.Instance);
        print(LanguageMgr.Instance.GetMessage("Title"));
        LanguageConf lc = LanguageMgr.Instance.GetMessage("Title");
        print(lc.Message);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestGetLanguage();
        }

    }
}
