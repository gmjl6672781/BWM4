using BeinLab.Util;
using BeinLab.VRTraing;
using BeinLab.VRTraing.Conf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAllTools : Singleton<FindAllTools>
{
    private List<ToolBasic> toolBasics = new List<ToolBasic>();
    private List<TaskGoalConf> taskGoals = new List<TaskGoalConf>();
    //private string name = "";

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))//遍历场景中的所有物体
        {   
            if (obj.GetComponent<ToolBasic>())
            {
                toolBasics.Add(obj.GetComponent<ToolBasic>());
                //name += "     " + obj.name;
            }
        }
        //InitAllTools();
        //print("共有工具：" + toolBasics.Count + "个");
        //print(name);
    }

    public void InitAllTools()
    {
        foreach (var item in toolBasics)
        {
            item.ToolAwake();
            //print(item.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
