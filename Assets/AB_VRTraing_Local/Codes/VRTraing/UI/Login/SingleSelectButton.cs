using BeinLab.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BeinLab.VRTraing.Gamer
{
    /// <summary>
    /// 单项选择按钮
    /// </summary>
    public class SingleSelectButton : MonoBehaviour
    {
        /// <summary>
        /// 选项列表
        /// </summary>
        public List<string> selectionKeys;
        /// <summary>
        /// 是否循环列表
        /// </summary>
        public bool isLoop;
        /// <summary>
        /// 默认选项
        /// </summary>
        private int defaultSelect = 0;

        private Button lastButton;
        private Button nextButton;
        private Text showLabel;
        public event Action<int> OnSelect;
        private bool isInit = false;
        // Start is called before the first frame update
        void Start()
        {
            InitComponent();
        }

        private void InitComponent()
        {
            if (!isInit)
            {
                lastButton = UnityUtil.GetTypeChildByName<Button>(gameObject, "LastButton");
                nextButton = UnityUtil.GetTypeChildByName<Button>(gameObject, "NextButton");
                showLabel = UnityUtil.GetTypeChildByName<Text>(gameObject, "ShowLabel");

                lastButton.onClick.AddListener(() => { OnClickSelect(-1); });
                nextButton.onClick.AddListener(() => { OnClickSelect(1); });
                isInit = true;
            }
        }
        public void SetKeys(List<string> selectionKeys, int selectIndex = 0)
        {
            this.selectionKeys = selectionKeys;
            defaultSelect = selectIndex;
            ///主动调起事件
            OnClickSelect(0);
        }
        public void OnClickSelect(int delt)
        {
            if (!isInit)
            {
                InitComponent();
            }
            defaultSelect += delt;
            if (defaultSelect < 0)
            {
                if (isLoop)
                {
                    defaultSelect += selectionKeys.Count;
                }
            }
            if (defaultSelect >= selectionKeys.Count)
            {
                if (isLoop)
                {
                    defaultSelect = 0;
                }
            }
            defaultSelect = Mathf.Clamp(defaultSelect, 0, selectionKeys.Count - 1);
            showLabel.text = selectionKeys[defaultSelect];
            OnSelect?.Invoke(defaultSelect);
            lastButton.interactable = true;
            nextButton.interactable = true;
            ///不循环时，隐藏对应的按钮
            if (!isLoop)
            {
                lastButton.interactable = (defaultSelect != 0);
                nextButton.interactable = (defaultSelect != selectionKeys.Count - 1);
            }
        }
    }
}