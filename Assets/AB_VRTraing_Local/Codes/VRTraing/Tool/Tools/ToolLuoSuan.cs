using BeinLab.VRTraing.Conf;
using System;
using UnityEngine;

namespace BeinLab.VRTraing
{
    public enum LuoSuanState
    {
        /// <summary>
        /// 拧紧
        /// </summary>
        Close = 0,
        /// <summary>
        /// 拧松
        /// </summary>
        Open = 1
    }
    /// <summary>
    /// 螺栓的类
    /// </summary>
    public class ToolLuoSuan : ToolBasic
    {
        /// <summary>
        /// 螺栓当前的状态，默认是闭合的状态
        /// </summary>
        private LuoSuanState lsState = LuoSuanState.Close;
        /// <summary>
        /// 当前的角度
        /// </summary>
        private float curAngle;
        /// <summary>
        /// 目标的角度，表示变化的累加
        /// </summary>
        private float targetAngle = 720;
        public event Action OnChangeState;
        private int direct = 1;

        public int Direct { get => direct; set => direct = value; }
        /// <summary>
        /// 需要的套筒，拆卸此螺栓需要的套筒
        /// </summary>
        public ToolConf taotong;
        /// <summary>
        /// 美术
        /// </summary>
        /// <param name="luoSuanState"></param>
        /// <param name="targetAngle"></param>
        /// <param name="direct"></param>
        public void InitLuoSuan(LuoSuanState luoSuanState, float targetAngle, int direct)
        {
            this.lsState = luoSuanState;
            this.targetAngle = targetAngle;
            this.Direct = direct;
            curAngle = 0;
        }
        /// <summary>
        /// 是否已经完成指定的目标状态
        /// 如果完成目标状态返回true
        /// </summary>
        /// <param name="luoSuanState"></param>
        /// <returns></returns>
        public bool CheckIsComplete(LuoSuanState luoSuanState)
        {
            return this.lsState == luoSuanState;
        }
        /// <summary>
        /// 外部施加压力
        /// 如果跟螺栓的改变方向一致，则接受此角度改变，并累加相关的角度数
        /// </summary>
        public bool AddForce(float angle, out float delt)
        {
            delt = 0.1f;
            if (Direct * angle > 0)
            {
                curAngle += Mathf.Abs(angle);
                if (targetAngle > 0)
                {
                    float tmp = curAngle / targetAngle;
                    if (delt < tmp)
                    {
                        delt = tmp;
                    }
                }
                if (curAngle >= targetAngle)
                {
                    curAngle = 0;
                    lsState = lsState == LuoSuanState.Close ? LuoSuanState.Open : LuoSuanState.Close;
                    OnChangeState?.Invoke();
                }
                return true;
            }
            return false;
        }
    }
}