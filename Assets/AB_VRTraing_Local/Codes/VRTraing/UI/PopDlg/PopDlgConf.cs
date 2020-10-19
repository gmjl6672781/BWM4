using BeinLab.FengYun.Gamer;
using BeinLab.Util;
using BeinLab.VRTraing.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BeinLab.VRTraing.Conf
{
    public class PopDlgConf : FllowHandConf
    {
        public string titleKey;
        public string msgKey;
        /// <summary>
        /// 非必填
        /// </summary>
        public string skipKey;
        /// <summary>
        /// 是否在点击时完成此任务
        /// </summary>
        public bool isCompleteTaskOnClick;
        /// <summary>
        /// 是否在点击时隐藏该面板
        /// </summary>
        public bool isHideOnClick = true;
        public bool isDesOnClick = true;

        public Vector2 alignment = Vector2.zero;
        public TaskConf jumpToTask;
        public Vector3[] linePath;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            PopDlg pop = gameDynamicer.GetComponentInChildren<PopDlg>();
            //pop.SetData(this);
            TimerMgr.Instance.CreateTimer(() =>
        {
            //if (pop.SimpleBG)
            //{
            //    Vector2 pos = pop.SimpleBG.sizeDelta / 2f;
            //    pos.x *= alignment.x;
            //    pos.y *= alignment.y;
            //    pop.SimpleBG.anchoredPosition = pos;
            //}
        }, 0.02f);
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/PopDlgConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<PopDlgConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<PopDlgConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(PopDlgConf) + " is null");
                }
            }
        }

#endif
    }
}