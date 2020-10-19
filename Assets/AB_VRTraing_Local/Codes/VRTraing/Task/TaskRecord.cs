using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeinLab.VRTraing
{
    //[Serializable]
    public class RecordInfo
    {
        public string USERCODE;
        public int MODELTYPE;
        public string EXAM_DATE;
        public float EXAM_SCORES;
        public List<TaskInfo> EXAM_RESULT;

    }

    [Serializable]
    public class TaskInfo
    {
        public string Key;
        public int Value;
    }
}


