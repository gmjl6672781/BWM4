using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BeinLab.VRTraing.Mgr;
using System;

public class EnterTaskScene : MonoBehaviour
{
    public float time = 0;
    public string sceneName = "";
    public Slider slider;
    public GameObject LogoDlg;
    public GameObject OpenDlg;
    public static Action ChangScene;

    // Start is called before the first frame update
    void Start()
    {
        LogoDlg.SetActive(false);
        OpenDlg.SetActive(false);
        ChooseLanguage.action += Dlg;
        //Dlg();
    }

    public void Dlg()
    {
        LogoDlg.SetActive(true);
        OpenDlg.SetActive(false);
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        ao.allowSceneActivation = false;
        slider.value = 0;
        while (slider.value != 1)
        {
            if (slider.value + Time.deltaTime / time < 1)
                slider.value += Time.deltaTime / time;
            else
                slider.value = 1;
            yield return new WaitForFixedUpdate();
        }
        LogoDlg.SetActive(false);
        OpenDlg.SetActive(true);

        //while (!SelectLanguage.isOver)
        //{
        //    yield return new WaitForFixedUpdate();
        //}
        //if (ChangScene != null)
        //    ChangScene();
        //while(!PlayrController.canShow)
        //{
        //    yield return new WaitForFixedUpdate();
        //}
        ao.allowSceneActivation = true;
    }

    private void OnDestroy()
    {
        ChooseLanguage.action -= Dlg;
    }

}
