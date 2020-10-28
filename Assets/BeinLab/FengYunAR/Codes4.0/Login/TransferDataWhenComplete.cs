using BeinLab.UI;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TransferDataWhenComplete : MonoBehaviour
{
    LoginRoot lj = new LoginRoot();
    public static bool isCanRestart = false;

    private void Awake()
    {
    }

    // Use this for initialization
    void Start () {
       
    }

    public void TransformData(int grade)
    {
        lj.vrKeyCode = LoadingHubMgr.inst.lj.vrKeyCode;
        lj.userCode = LoadingHubMgr.inst.lj.userCode;
        lj.vrTrainingCode = LoadingHubMgr.inst.lj.vrTrainingCode;
        isCanRestart = false;
        lj.trainingStatus = 3;
        lj.vrTrainingDetailList = new List<VrTrainingDetailListItem>();
        lj.startTime = LoadingHubMgr.inst.lj.startTime;
        lj.endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        lj.trainingMode = LoadingHubMgr.inst.lj.trainingMode;
        lj.grade = grade;
        lj.learningTime = "0";
        lj.standardTime = "60";
        lj.completeTime = null;
        lj.isError = 0;
        lj.learningNum = 1;

        string msg = JsonMapper.ToJson(lj);// JsonUtility.ToJson(lj);
        StartCoroutine(HttpJsonPost(msg));
    }

    /// <summary>
    /// application/json发送数据请求
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public IEnumerator HttpJsonPost(string jsonParam, bool isPost = true)
    {
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

        isCanRestart = true;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
