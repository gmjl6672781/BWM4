using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Mgr;
using System;
using System.Collections.Generic;

namespace BeinLab.VRTraing
{
    /// <summary>
    /// 电池的实现
    /// 电池：只要接触到指定容器，立刻吸附效果
    /// 吸附的话，只吸附到目标物体的ShowBody身上
    /// 当接触到其他物体时，再重新吸附
    /// Bug记录：当处于抬升状态时，松手之后，工具会飞升上天
    /// 
    /// 新的实现方案：采用凹槽自动吸附的方式，将电池放置在指定的位置
    /// 对凹槽进行管理：仅在特定步骤可以吸附
    /// 如果在执行过程中，用户松开抬升工具：电池即刻归位到上一个接触点
    /// </summary>
    public class ToolDianChi : ToolBasic
    {
        //private float taishengTime;
        //public float tsTime = 5f;
        public static event Action<bool> OnTaiSheng;
        private PutTooConf lastTool;
        protected override void Start()
        {
            base.Start();
            OnPutAoCao += OnTriggerTaiSheng;
            //IsAoCaoActive = false;
            lastTool = null;
            ToolTaiSheng.OnPutOut += OnPutOut;
        }
        
        private void OnPutOut()
        {
            if (lastTool)
            {
                PutToAoCao(lastTool, true);
                lastTool = null;
            }
        }

        /// <summary>
        /// 当接触到抬升工具时，立刻吸附到目标物体
        /// </summary>
        /// <param name="obj"></param>
        private void OnTriggerTaiSheng(PutTooConf obj)
        {
            if (obj.triggerTool.toolBasic is ToolTaiSheng)
            {
                //if (Time.time - taishengTime > tsTime)
                //{
                //taishengTime = Time.time;
                OnTaiSheng?.Invoke(true);
                //}
            }
            else
            {
                lastTool = obj;
                OnTaiSheng?.Invoke(false);
            }
        }
    }
}