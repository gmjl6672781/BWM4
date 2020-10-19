using BeinLab.Util;
using BeinLab.VRTraing.Mgr;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 描述工具对应某个任务的具体信息
    /// </summary>
    public class ToolTaskConf : ScriptableObject
    {
        public ToolConf toolConf;

        //任务开始时设置(OnTaskStart)
        public bool isSetStartCanHover;
        public bool isStartCanHover;
        public bool isSetStartCanCatch;
        public bool isStartCanCatch;
        public bool isSetStartKinematic;
        public bool isStartKinematic;
        public bool isSetStartHide;
        public bool isStartHide;
        public bool isSetStartHighlight;
        public bool isStartHighlight;
        public bool isSetStartScaleSize;
        public Vector3 startScaleSize;
        public bool isSetStartPose;
        public Vector3 StartPosition;
        public Vector3 StartAngle;
        public bool isSetStartCollider = false;
        public bool isStartCollider = false;

        //任务完成时设置(OnTaskEnd)   
        public bool isSetEndCanHover;
        public bool isEndCanHover;
        public bool isSetEndCanCatch;
        public bool isEndCanCatch;
        public bool isSetEndKinematic;
        public bool isEndKinematic;
        public bool isSetEndHide;
        public bool isEndHide;
        public bool isSetEndHighlight;
        public bool isEndHighlight;
        public bool isSetEndScaleSize;
        public Vector3 endScaleSize;
        public bool isSetEndPose;
        public Vector3 EndPosition;
        public Vector3 EndAngle;
        public bool isSetEndCollider = false;
        public bool isEndCollider = false;
        //public bool isHighLightOnExam = false;
        //public bool isHideOnExam = false;
        /// <summary>
        /// 设置凹槽的位置
        /// </summary>
        public int aocaoStartIndex = -1;
        public int aocaoEndIndex = -1;
        public bool isSetAoCaoStart = false;
        public bool isAoCaoStart = false;
        public bool isSetAoCaoEnd = false;
        public bool isAoCaoEnd = false;
        /// <summary>
        /// 在某些模式下的状态
        /// </summary>
        public TaskMode activeMode = TaskMode.Teaching;
        
        public void OnTaskInit(TaskConf taskConf)
        {
            //Debug.Log(taskConf + "====OnTaskInit====" + toolConf);
            if (activeMode == TaskMode.Teaching || activeMode == TaskManager.Instance.taskMode)
            {
                toolConf.toolBasic?.SetToolTaskInit(this);
                toolConf.toolBasic?.OnTaskInit(taskConf);
            }
        }

        public void OnTaskStart(TaskConf taskConf)
        {
            if (activeMode == TaskMode.Teaching || activeMode == TaskManager.Instance.taskMode)
            {
                toolConf.toolBasic?.SetToolTaskStart(this);
                toolConf.toolBasic?.OnTaskStart(taskConf);
            }
        }


        public void OnTaskDoing(TaskConf taskConf)
        {
            if (activeMode == TaskMode.Teaching || activeMode == TaskManager.Instance.taskMode)
            {
                toolConf.toolBasic?.OnTaskDoing(taskConf);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskConf"></param>
        public void OnTaskEnd(TaskConf taskConf)
        {
            if (activeMode == TaskMode.Teaching || activeMode == TaskManager.Instance.taskMode)
            {
                toolConf.toolBasic?.SetToolTaskEnd(this);
                toolConf.toolBasic?.OnTaskEnd(taskConf);
            }
        }



#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/ToolTaskConf", false, 0)]
        static void CreateDynamicConf()
        {
            Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<ToolTaskConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ToolTaskConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(ToolTaskConf) + " is null");
                }
            }
        }

#endif
    }
}

