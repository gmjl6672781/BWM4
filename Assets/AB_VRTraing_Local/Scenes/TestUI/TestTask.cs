using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing
{
    public class TestTask : MonoBehaviour
    {
        public TaskConf taskConf;
        // Start is called before the first frame update
        void Start()
        {
            TestNextLittle(taskConf);
            TestNextBig(taskConf);
        }


        void TestNextBig(TaskConf taskConf)
        {
            TaskConf next = TaskManager.Instance.GetNextTaskBig(taskConf);
            if(next)
                Debug.LogFormat("TestNextBig {0}", next.taskName);
        }

        void TestNextLittle(TaskConf taskConf)
        {
            TaskConf next = TaskManager.Instance.GetNextTaskLittle(taskConf);
            if (next)
                Debug.LogFormat("TestNextLittle {0}", next.taskName);
        }

    }
}


