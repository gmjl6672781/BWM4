using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;
using System;
using BeinLab.VRTraing;

public class JsonTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //Dictionary<string, bool> dicResult = new Dictionary<string, bool>();
        //dicResult.Add("T0", true);
        //dicResult.Add("T1", true);
        //dicResult.Add("T2", false);
        //dicResult.Add("T3", true);
        //dicResult.Add("T4", false);
        //dicResult.Add("T5", true);

        List<TaskInfo> Result = new List<TaskInfo>();
        Result.Add(new TaskInfo { Key = "T0", Value = 0 });
        Result.Add(new TaskInfo { Key = "T1", Value = 0 });
        Result.Add(new TaskInfo { Key = "T2", Value = 0 });
        Result.Add(new TaskInfo { Key = "T3", Value = 1 });
        Result.Add(new TaskInfo { Key = "T4", Value = 1 });


        //string jsonResult = Json.Serialize(dicResult);

        RecordInfo recordInfo = new RecordInfo
        {
            USERCODE = "02",
            MODELTYPE = 0,
            EXAM_DATE = "2020.01.10.09.30.10",
            EXAM_SCORES = 95,
            EXAM_RESULT = Result
        }; 

        string jsonRecordInfo = JsonUtility.ToJson(recordInfo);
        Debug.Log(jsonRecordInfo);

        RecordInfo record = (RecordInfo)JsonUtility.FromJson(jsonRecordInfo, typeof(RecordInfo));
        Debug.Log(record.EXAM_DATE);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
