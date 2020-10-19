using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BeinLab.Util
{
    /// <summary>
    /// Toggle的组件
    /// </summary>
    public class IToggle : Toggle
    {
        [SerializeField]
        public bool isHide = true;
        protected override void OnEnable()
        {
            base.OnEnable();
            onValueChanged.AddListener(ShowTarget);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            onValueChanged.RemoveListener(ShowTarget);
        }
        /// <summary>
        /// 是否显示
        /// </summary>
        /// <param name="isOn"></param>
        public void ShowTarget(bool isOn)
        {
            if (isOn)
            {
                if (isHide)
                {
                    if (targetGraphic)
                    {
                        targetGraphic.enabled = false;
                    }
                }
            }
            else
            {
                if (targetGraphic)
                {
                    targetGraphic.enabled = true;
                }
            }
        }
    }
}