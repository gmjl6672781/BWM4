using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using System;
using LitJson;

public class LoadingHubMgr : MonoBehaviour {

    public LoginRoot lj = new LoginRoot();

    public static LoadingHubMgr inst;
    public static string mode = "";

    void Awake()
    {
        //Debug.Log("------------Awake");
        inst = this;
    }

    public void SelectMode()
    {
        //Debug.Log("------------SelectMode");
        string CommandLine = Environment.CommandLine;
        Debug.Log(CommandLine);
        string[] CommandLineArgs = Environment.GetCommandLineArgs();

        if (CommandLineArgs.Length == 1)
        {
            string[] str = CommandLineArgs[0].Split('=');
            if (str.Length <= 1)
            {
                Application.Quit();
            }
            else
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i].Contains("userCode"))
                    {
                        break;
                    }
                    if (i == str.Length - 1)
                    {
                        Application.Quit();
                        return;
                    }
                }

                lj.vrKeyCode = str[1].Split(' ')[0];
                lj.userCode = str[2].Split(' ')[0];
                lj.vrTrainingCode = str[3];

                Restart();
            }

        }
    }

    // Use this for initialization
    void Start () {
       //Debug.LogError(Guid.NewGuid().ToString());  
    }

    public void Restart()
    {
        //Debug.Log("------------Restart");
        lj.vrTrainingCode = Guid.NewGuid().ToString();
        lj.trainingStatus = 2;
        lj.vrTrainingDetailList = new List<VrTrainingDetailListItem>();
        lj.startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lj.endTime = null;
        if (mode == "training")
            lj.trainingMode = "20";
        else if (mode == "examing")
            lj.trainingMode = "30";
        lj.grade = -1;
        lj.learningTime = "0";
        lj.standardTime = "60";
        lj.completeTime = null;
        lj.isError = 0;
        lj.learningNum = 1;

        string msg = JsonMapper.ToJson(lj);// JsonUtility.ToJson(lj);
        //Debug.Log("msg----------------------" + msg);
        StartCoroutine(HttpJsonPost(msg));
    }
    /// <summary>
    /// application/json发送数据请求
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public IEnumerator HttpJsonPost(string jsonParam, bool isPost = true)
    {
        //Debug.Log("jsonParam-------------------" + jsonParam);
        // print(url + "\n" + jsonParam);
        string url = "http://localhost/api/v1/biz/proxy/saveVRCourseTrainingStatus";
        byte[] body = Encoding.UTF8.GetBytes(jsonParam);
        UnityWebRequest unityWeb = new UnityWebRequest(url, isPost ? "POST" : "GET");
        unityWeb.uploadHandler = new UploadHandlerRaw(body);
        //unityWeb.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        unityWeb.SetRequestHeader("Content-Type", "application/json");
        //unityWeb.SetRequestHeader("token","eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJzdWIiOiIxMTc0OTgxIiwiaWF0IjoxNTk1NTY4OTE5LCJleHAiOjE1OTYxNzM3MTl9.ekiPEvoyfzsFbPXu8Fm9F4hSiqQsfHsFcA_BL_yYyKl2UymTTBkDkn29-urqVOYvnAS28kmHg5nEm-c3WxVftA");
        unityWeb.downloadHandler = new DownloadHandlerBuffer();
        yield return unityWeb.SendWebRequest();
        string result = null;
        if (unityWeb.isDone)
        {
            result = unityWeb.downloadHandler.text;
            Debug.LogError(result);

        }
        else
        {
            Debug.LogError("Http 请求失败");
            Debug.LogError(unityWeb.error);
        }

    }
    // Update is called once per frame
    void Update () {
		
	}
}
