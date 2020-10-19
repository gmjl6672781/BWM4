using System.Collections;
using System.Collections.Generic;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BeinLab.VRTraing.Conf
{
    public class GoalAutoPosition : TaskGoalConf
    {
        public Vector3 autoPosition_Local;
        public Vector3 autoRotation_Local;
        /// <summary>
        /// 对应的扳手
        /// </summary>
        public ToolConf GameObjs;
        public string fatherName = "Scene";
        private Transform toolRoot;
        private bool result = false;
        private bool isDone;
        public bool isKinematic = false;
        /// <summary>
        /// 任务开始
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            toolRoot = GameObject.Find(fatherName).transform;
            FindParent(GameObjs.toolBasic.transform, fatherName);

            if (GameObjs.toolBasic.transform.GetComponent<Rigidbody>() != null)
            {
                GameObjs.toolBasic.transform.GetComponent<Rigidbody>().isKinematic = isKinematic;
            }
            if (!result)
            {
                GameObjs.toolBasic.transform.SetParent(toolRoot);
            }
            GameObjs.toolBasic.transform.localPosition = autoPosition_Local;
            GameObjs.toolBasic.transform.localRotation = Quaternion.Euler(autoRotation_Local);
            isDone = true;
            AchieveGoal(true);
        }

        public void FindParent(Transform current, string name)
        {
            result = false;
            if (current.parent != null)
            {
                if (current.parent.name == name)
                {
                    result = true;
                }
                else
                    FindParent(current.parent, name);
            }
        }

        /// <summary>
        /// 任务结束
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            if (!isDone)
            {
                OnTaskStart(taskConf);
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalAutoPosition", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalAutoPosition>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalAutoPosition>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalToggleLuoSuan) + " is null");
                }
            }
        }
#endif
    }
}