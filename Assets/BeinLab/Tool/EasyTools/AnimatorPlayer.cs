using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BeinLab.Util
{
    /// <summary>
    /// 动画播放器
    /// </summary>
    public class AnimatorPlayer : MonoBehaviour
    {
        private Animator animator;

        private void Start()
        {
            if (!animator)
                animator = GetComponent<Animator>();
        }
        /// <summary>
        /// 播放指定名称的动画
        /// </summary>
        public void PlayAnimator(string aniName)
        {
            animator.Play(aniName);
        }

        public void SetAnimatorController(RuntimeAnimatorController rac)
        {
            if (!animator) animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = rac;
        }

        /// <summary>
        /// 设置状态
        /// 参数一般有三种类型
        /// </summary>
        public void SetAnimatorState(string stateName, System.Object arg)
        {
            if (!animator)
                animator = GetComponent<Animator>();
            if (arg != null)
            {
                if (arg is bool)
                {
                    animator.SetBool(stateName, (bool)arg);
                }
                else if (arg is int)
                {
                    animator.SetInteger(stateName, (int)arg);
                }
                else if (arg is float)
                {
                    animator.SetFloat(stateName, (float)arg);
                }
            }
        }
    }
}