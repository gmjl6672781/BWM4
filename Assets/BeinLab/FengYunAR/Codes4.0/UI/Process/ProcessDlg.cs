using BeinLab.Util;
using DG.Tweening;
using Karler.WarFire.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BeinLab.FengYun.UI
{
    /// <summary>
    /// 进度条管理器
    /// </summary>
    public class ProcessDlg : Singleton<ProcessDlg>
    {
        private BaseDlg dlg;
        /// <summary>
        /// 滑块
        /// </summary>
        private Slider processSlider;
        /// <summary>
        /// 信息条
        /// </summary>
        private Text sliderLabel;

        public Slider ProcessSlider
        {
            get { return processSlider; }
            set
            {
                processSlider = value;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            dlg = GetComponent<BaseDlg>();
            ProcessSlider = UnityUtil.GetTypeChildByName<Slider>(gameObject, "Slider");
            sliderLabel = UnityUtil.GetTypeChildByName<Text>(ProcessSlider.gameObject, "SliderLabel");
        }
        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            gameObject.SetActive(false);
        }
        public void DoSliderEffect(string msg, float start, float end, float time, bool autoHide = true,bool isHideBg=true)
        {
            sliderLabel.text = msg;
            gameObject.SetActive(true);
            ProcessSlider.DOKill();
            ProcessSlider.value = start;
            ProcessSlider.DOValue(end, time).onComplete += () =>
            {
                if (autoHide)
                {
                    gameObject.SetActive(false);
                }
            };
            if (dlg.DlgBG)
            {
                dlg.DlgBG.gameObject.SetActive(!isHideBg);
            }
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="process"></param>
        /// <param name="isHideBg">为true 代表隐藏背景</param>
        public void UpdateProcess(string msg, float process, bool isHideBg = true)
        {
            if (sliderLabel)
            {
                sliderLabel.text = msg;
            }
            if (ProcessSlider)
            {
                ProcessSlider.value = process;
            }
            if (dlg.DlgBG)
            {
                dlg.DlgBG.gameObject.SetActive(!isHideBg);
            }
        }
        /// <summary>
        /// 单纯的更新进度
        /// </summary>
        /// <param name="process"></param>
        public void UpdateProcess(float process)
        {
            if (ProcessSlider)
            {
                ProcessSlider.value = process;
            }
        }
    }
}