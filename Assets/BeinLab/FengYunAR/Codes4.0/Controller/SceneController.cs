using BeinLab.CarShow.Modus;
using BeinLab.Util;
using UnityEngine;
namespace BeinLab.FengYun.Controller
{
    /// <summary>
    /// 场景控制
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        public string actionPath;
        public ActionConf actionConf;
        public DynamicActionListConf dynGroup;
#if UNITY_EDITOR
        [HideInInspector]
        public UnityEngine.Object sceneConfPrefab;
        public string editorScenes;
#endif
        private void Start()
        {
            if (DynamicActionController.Instance)
            {
                if (!string.IsNullOrEmpty(actionPath))
                {
                    DynamicActionController.Instance.DoAction(actionPath);
                }
                if (actionConf)
                {
                    DynamicActionController.Instance.DoAction(actionConf);
                }
                if (dynGroup)
                {
                    DynamicActionController.Instance.DoDynamicActionList(dynGroup);
                }
#if UNITY_EDITOR
                if(GameDataMgr.Instance.buildConf.buildType== BulidType.Editor)
                if (!string.IsNullOrEmpty(editorScenes))
                {
                    DynamicActionController.Instance.DoAction(editorScenes);
                }
#endif
            }
        }
    }
}