using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Modus;
using BeinLab.RS5.Mgr;
using Karler.WarFire.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BeinLab.UI
{
    /// <summary>
    /// 开关的事件控制
    /// </summary>
    public class ToggleActionDlg : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public ToggleActionConf toggleConf;
        private Toggle actionToggle;

        public Toggle ActionToggle
        {
            get { return actionToggle; }
            set
            {
                actionToggle = value;
            }
        }
        private bool isInit = false;
        private void Start()
        {
            if (toggleConf && !isInit)
            {
                SetData(toggleConf);
            }
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="toggleAction"></param>
        public void SetData(ToggleActionConf toggleAction)
        {
            toggleConf = toggleAction;
            BaseDlg dlg = GetComponent<BaseDlg>();
            ActionToggle = dlg.GetChildComponent<Toggle>(toggleConf.toggleName);
            ActionToggle.isOn = toggleConf.toggleIsOn;
            if (toggleConf.isCacheLastSelect)
            {
                if (XRController.Instance.isHaveState(toggleConf))
                {
                    ActionToggle.isOn = toggleConf.cacheState;
                }
                else
                {
                    XRController.Instance.AddToggleState(toggleConf);
                }
            }
            (ActionToggle.graphic as Image).sprite = toggleConf.offIcon;
            print(toggleConf.offIcon);
            (ActionToggle.targetGraphic as Image).sprite = toggleConf.openIcon;
            ActionToggle.onValueChanged.AddListener(OnValueChanged);
            if (toggleConf.isDoOnActive)
            {
                OnValueChanged(ActionToggle.isOn);
            }
            isInit = true;
        }

        /// <summary>
        /// 当物体失活时，调用关闭状态的代码
        /// </summary>
        private void OnDestroy()
        {
            if (toggleConf && toggleConf.offAction && toggleConf.isOffOnDel)
            {
                OnValueChanged(false);
                if (toggleConf.isCacheLastSelect)
                {
                    toggleConf.cacheState = false;
                }
            }
        }


        /// <summary>
        /// 当开关变化时
        /// </summary>
        /// <param name="isOn"></param>
        private void OnValueChanged(bool isOn)
        {
            if (DynamicActionController.Instance)
            {
                if (isOn)
                {
                    DynamicActionController.Instance.DoAction(toggleConf.openAction);
                }
                else
                {
                    DynamicActionController.Instance.DoAction(toggleConf.offAction);
                }
                if (toggleConf.isCacheLastSelect)
                    toggleConf.cacheState = isOn;
            }
        }
    }
}