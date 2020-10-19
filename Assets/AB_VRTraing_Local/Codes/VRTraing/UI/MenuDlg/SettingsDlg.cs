using BeinLab.VRTraing.Controller;
using BeinLab.VRTraing.Gamer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BeinLab.VRTraing.UI
{
    public class SettingsDlg : MonoBehaviour
    {
        public SingleSelectButton languageButton;
        public Toggle textToggle;
        public Toggle audioToggle;
        public List<Toggle> sceneViewBtns;
        public Button loginOut;
        public List<Cubemap> skyList;
        public Material viewMat;
        public ToggleGroup group;
        private void Start()
        {
            //print("Start111111111111111111111111111111111111111111");
            languageButton.OnSelect += OnClickSelect;
            loginOut.onClick.AddListener(OutScenes);
            Invoke("InitComponent", 0.1f);
            audioToggle.onValueChanged.AddListener(LanguageMgr.Instance.ToggleAudio);
            textToggle.onValueChanged.AddListener(OnToggleText);
            for (int i = 0; i < sceneViewBtns.Count; i++)
            {
                int index = i;
                sceneViewBtns[i].onValueChanged.AddListener((bool isOn) =>
                {
                    if (isOn)
                    {
                        OnSwitchSceneView(index);
                    }
                });
                if (sceneViewBtns[i].isOn)
                {
                    OnSwitchSceneView(i);
                }
                sceneViewBtns[i].group = group;
            }
        }
        private void OnDestroy()
        {
            OnSwitchSceneView(0);
        }
        /// <summary>
        /// 选中相关的场景
        /// </summary>
        /// <param name="index"></param>
        private void OnSwitchSceneView(int index)
        {
            viewMat.SetTexture("_Tex", skyList[index]);
        }

        /// <summary>
        /// 关闭文字提示
        /// </summary>
        /// <param name="isOn"></param>
        private void OnToggleText(bool isOn)
        {
            print(isOn);
        }

        private void OutScenes()
        {
            SceneManager.LoadScene(0);
        }

        private void InitComponent()
        {
            List<string> languages = new List<string>();
            for (int i = 0; i < LanguageMgr.Instance.LanguageList.Count; i++)
            {
                languages.Add(LanguageMgr.Instance.LanguageList[i].Language);
            }
            languageButton.SetKeys(languages, LanguageMgr.Instance.settingsConf[0].LanguageIndex);
        }
        private void OnClickSelect(int index)
        {
            //print(index);
            LanguageMgr.Instance.SelectLanguage(index);
        }
    }
}