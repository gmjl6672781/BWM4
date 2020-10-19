using System;
using System.Collections;
using System.Collections.Generic;
using BeinLab.VRTraing.Conf;
using DG.Tweening;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing
{
    /// <summary>
    /// 棘轮扳手工具
    /// </summary>
    public class JiLunBanShou : ToolBasic
    {
        GameObject fllowTarget;
        GameObject lookTarget;
        bool isCanRotate = false;//是否可以转圈拧螺栓了
        bool isAnticlockwise = false;//是否是逆时针转动
        ToolConf otherToolConf;//螺栓
        private float changeAngle;//总的变化角度
        Vector3 lastDir = Vector3.zero;//上一帧从父物体到lookTarget的指向方向
        bool isFinish = false;//是否拧完了一个螺栓

        //扳手碰到螺栓，确定好扳手的位置和角度
        protected override void OnEnterTool_(ToolConf otherTool)
        {
            base.OnEnterTool_(otherTool);
            if (otherTool.name.Equals("luoshuan") && catchHand != null)//碰到螺栓
            {
                otherToolConf = otherTool;
                catchHand.DetachObject(gameObject);//从手柄上离开
                fllowTarget = new GameObject("fllowTarget");
                fllowTarget.transform.position = otherTool.toolBasic.transform.position;
                fllowTarget.transform.eulerAngles = otherTool.toolBasic.transform.eulerAngles;
                lookTarget = new GameObject("lookTarget");
                lookTarget.transform.position = toolConf.toolBasic.catchHand.transform.position;
                lookTarget.transform.SetParent(otherTool.toolBasic.transform);
                lookTarget.transform.localEulerAngles = Vector3.zero;
                lookTarget.transform.localPosition = new Vector3(lookTarget.transform.localPosition.x, 0,
                    lookTarget.transform.localPosition.z);
                lastDir = lookTarget.transform.localPosition;//上一帧从父物体到lookTarget的指向方向
                fllowTarget.transform.LookAt(lookTarget.transform, otherTool.toolBasic.transform.up);
                transform.SetParent(fllowTarget.transform);//设为跟随物体fllowTarget的子物体
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
                isCanRotate = true;
            }
        }

        private void Update()
        {
            //扳手跟随手柄转动
            if (isCanRotate)
            {
                //lookTarget一直跟随手柄移动，fllowTarget一直指向lookTarget，然后判断fllowTarget的变化角度
                lookTarget.transform.position = toolConf.toolBasic.catchHand.transform.position;
                lookTarget.transform.localPosition = new Vector3(lookTarget.transform.localPosition.x, 0,
                    lookTarget.transform.localPosition.z);
                Vector3 dir = lookTarget.transform.localPosition;//当前应指向方向
                if (Vector3.Cross(dir, lastDir).y > 0)//逆时针旋转
                {
                    isAnticlockwise = true;
                }
                lastDir = dir;

                if (isAnticlockwise)//逆时针旋转时
                {
                    fllowTarget.transform.LookAt(lookTarget.transform, otherToolConf.toolBasic.transform.up);
                    float detalAngle =  Mathf.Abs(Vector3.Dot(dir, lastDir));
                    changeAngle += detalAngle;
                    if (changeAngle >= 720)//转了2圈
                    {
                        isFinish = true;
                    }
                }
            }
        }//end_Update()

        //获取操作结果
        public bool GetResult()
        {
            return isFinish;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void Init()
        {
            isCanRotate = false;//是否可以转圈拧螺栓了
            isAnticlockwise = false;//是否是逆时针转动
            changeAngle = 0;//总的变化角度
            lastDir = Vector3.zero;//上一帧从父物体到lookTarget的指向方向
            isFinish = false;//是否拧完了一个螺栓
    }
    }
}
