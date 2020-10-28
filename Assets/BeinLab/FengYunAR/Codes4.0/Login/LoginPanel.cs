using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using System;
using LitJson;


public class LoginPanel : MonoBehaviour {

	public InputField id;
	public InputField password;
	public Text tips;
	public Button loginBtn;


    public Text t;
    LoginRoot lj = new LoginRoot();
    // Use this for initialization
    void Start () {
  //      if (tips!=null)
  //      {
		//	tips.gameObject.SetActive(false);

		//}
  //      if (loginBtn!=null)
  //      {
		//	loginBtn.onClick.AddListener(ClickEvent);
  //      }
  //      if (id!=null)
  //      {
		//	id.onValueChanged.AddListener(ChangeEvent);
		//}
  //      if (password!=null)
  //      {
		//	password.onValueChanged.AddListener(ChangeEvent);

		//}

        //string CommandLine = Environment.CommandLine;
        //string[] CommandLineArgs = Environment.GetCommandLineArgs();
        //t.text = CommandLineArgs.Length.ToString();
        //for (int i = 0; i < CommandLineArgs.Length; i++)
        //{
        //    t.text += " \n" + CommandLineArgs[i];
        //}
        //lj = new LoginRoot();
        //if (CommandLineArgs.Length==1)
        //{
        //    string[] str = CommandLineArgs[0].Split('=');
        //    if (str.Length<=1)
        //    {
        //        t.text = " \n  不是hub打开" ;
        //        Application.Quit();
        //    }
        //    else
        //    {
        //        for (int i = 0; i < str.Length; i++)
        //        {
        //            if (str[i].Contains("userCode"))
        //            {
        //                break;
        //            }
        //            if (i==str.Length-1)
        //            {
        //                Application.Quit();
        //                return;
        //            }
        //        }
        //        for (int i = 0; i < str.Length; i++)
        //        {
        //            t.text += " \n" + str[i];
        //        }
        //        t.text = " \n" + str[1].Split(' ')[0];
        //        t.text += " \n" + str[2].Split(' ')[0];
        //        t.text += " \n" + str[3];
        //        t.text += " \n" + CommandLineArgs[0];

        //        //lj.vrKeyCode = str[1].Split(' ')[0];
        //        //lj.userCode = str[2].Split(' ')[0];
        //        //lj.vrTrainingCode = str[3];
        //    }
            
        //}
        //Debug.LogError(CommandLineArgs.ToString());
    }
	void ChangeEvent(string str)
    {
		tips.gameObject.SetActive(false);
	}
	public void ClickEvent()
    {
        lj.trainingStatus = 3;       
        lj.vrTrainingDetailList = new List<VrTrainingDetailListItem>();
        lj.startTime = "2020-09-25 16:10:00";
        lj.endTime = null;
        lj.trainingMode = "10";
        lj.grade = -1;
        lj.learningTime = "0";
        lj.standardTime = "60";
        lj.completeTime = null;
        lj.isError = 0;
        lj.learningNum = 1;
       

        string msg = JsonMapper.ToJson(lj);// JsonUtility.ToJson(lj);
        t.text = msg;
        StartCoroutine(HttpJsonPost(msg));
        return;
        if (id.text.Equals(string.Empty) || password.text.Equals(string.Empty))
        {
			tips.gameObject.SetActive(true);
			tips.text="账号或密码不能为空！";
			return;
		}

        if (id.text=="321" && password.text=="321")
        {
			SceneManager.LoadScene("BMW_VR_SE16_Loading");
        }
        else
        {
			tips.gameObject.SetActive(true);
			tips.text = "账号或密码输入错误！";
		}
		Debug.Log(id.text+"   "+password.text);
	}
    public void ClickEvent2()
    {
        lj.trainingStatus = 2;
        lj.vrTrainingDetailList = new List<VrTrainingDetailListItem>();
        lj.startTime = "2020-09-25 16:10:00";
        lj.endTime = null;
        lj.trainingMode = "10";
        lj.grade = -1;
        lj.learningTime = "0";
        lj.standardTime = "60";
        lj.completeTime = null;
        lj.isError = 0;
        lj.learningNum = 1;


        string msg = JsonMapper.ToJson(lj);// JsonUtility.ToJson(lj);
        t.text = msg;
        StartCoroutine(HttpJsonPost(msg));
        return;
        if (id.text.Equals(string.Empty) || password.text.Equals(string.Empty))
        {
            tips.gameObject.SetActive(true);
            tips.text = "账号或密码不能为空！";
            return;
        }

        if (id.text == "321" && password.text == "321")
        {
            SceneManager.LoadScene("BMW_VR_SE16_Loading");
        }
        else
        {
            tips.gameObject.SetActive(true);
            tips.text = "账号或密码输入错误！";
        }
        Debug.Log(id.text + "   " + password.text);
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
            t.text += "\n"+result;
            Debug.LogError(result);
            
        }
        else
        {
            t.text += "\n"+unityWeb.error;
            Debug.LogError("Http 请求失败");
            
            Debug.LogError(unityWeb.error);
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}

public class VrTrainingDetailListItem
{
    /// <summary>
    /// 确认拆卸操作准备⼯作执⾏完毕
    /// </summary>
    public string stepDesc { get; set; }
    /// <summary>
    /// 步骤⼀
    /// </summary>
    public string stepName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string vrTrainingDescList { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string stepStatus { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int stepSequence { get; set; }
}

public class LoginRoot
{
    public string vrKeyCode { get; set; }
    public string userCode { get; set; }
    public string vrTrainingCode { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string standardTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int trainingStatus { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string completeTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int learningNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string trainingMode { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int isError { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int grade { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string learningTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string startTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string endTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<VrTrainingDetailListItem> vrTrainingDetailList { get; set; }
}
