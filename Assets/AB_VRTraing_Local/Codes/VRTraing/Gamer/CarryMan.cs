using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Mgr;
using RootMotion.FinalIK;
using System.Collections.Generic;
using UnityEngine;
namespace BeinLab.VRTraing
{
    /// <summary>
    /// 一个帮忙抬工具的人物角色
    /// </summary>
    public class CarryMan : MonoBehaviour
    {
        private FullBodyBipedIK bodyIK;
        private Transform target;
        private Animator manAnimator;
        public Animator otherAnimator;
        public List<string> showTaskList;
        public FullBodyBipedIK otherBodyIK;
        private void Start()
        {
            bodyIK = GetComponent<FullBodyBipedIK>();
            manAnimator = GetComponent<Animator>();
            ///默认不搬运物体，当需要搬运的时候再凸显出来
            OnTaiSheng(false);
            if (!target && GameNoder.Instance)
            {
                target = GameNoder.Instance.DefPos;
            }
            ToolDianChi.OnTaiSheng += OnTaiSheng;
            ToolTaiSheng.OnUpdatePos += OnUpdatePos;
            if (TaskManager.Instance)
            {
                TaskManager.Instance.OnTaskChange += OnTaskChange;
            }
            gameObject.SetActive(false);
        }

        private void OnTaskChange(TaskConf obj)
        {
            if (obj && showTaskList != null && showTaskList.Contains(obj.name))
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnUpdatePos(Transform obj)
        {
            target = GameNoder.Instance.DefPos;
            if (obj)
            {
                target = obj;
            }
        }
        /// <summary>
        /// 简易的AI效果
        /// 当玩家放下抬升工具时，默认站在原位
        /// 当玩家拿起抬升工具时，跟随距离点
        /// 当玩家抬电池时，设置托举状态，并跟随
        /// </summary>
        private void Update()
        {
            if (target)
            {
                Vector3 targetPos = target.position;
                targetPos.y = transform.position.y;
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * Mathf.PI);
                if (Vector3.Distance(transform.position, targetPos) > 0.05f)
                {
                    manAnimator.SetInteger("State", 1);
                    otherAnimator.SetInteger("State", 1);
                }
                else
                {
                    manAnimator.SetInteger("State", 0);
                    otherAnimator.SetInteger("State", 0);
                }
                if (Vector3.Distance(transform.position, targetPos) > 1f)
                {
                    transform.LookAt(targetPos);
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// 是否处于抬升模式
        /// </summary>
        /// <param name="isTai"></param>
        private void OnTaiSheng(bool isTai)
        {
            bodyIK.enabled = isTai;
            otherBodyIK.enabled = isTai;
        }
    }
}