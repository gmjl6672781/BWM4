using BeinLab.Util;
using UnityEngine;
using BeinLab.FengYun.Gamer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 控制某个层级的隐藏和显示
    /// 用来做相机的算法等等
    /// </summary>
    [SerializeField]
    public class DlgDynamicConf : DynamicConf
    {
        /// <summary>
        /// Node
        /// </summary>
        public NodeConf nodeConf;
        /// <summary>
        /// 
        /// </summary>
        public bool isActive = true;
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool isDel = false;
        /// <summary>
        /// 控制节点的隐藏和显示
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (GameNoder.Instance)
            {
                GameObject obj = GameNoder.Instance.GetNode(nodeConf, false);
                if (obj)
                {
                    if (!isDel)
                    {
                        obj.SetActive(isActive);
                        Debug.Log(obj.ToString()+ isActive);
                    }
                    else
                    {
                        Destroy(obj);
                    }
                }
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/DlgDynamicConf", false, 4)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<DlgDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<DlgDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DlgDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}