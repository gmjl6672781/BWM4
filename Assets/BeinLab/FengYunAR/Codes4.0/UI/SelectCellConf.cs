using BeinLab.FengYun.Modu;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.FengYun.Modus
{
    /// <summary>
    /// 选关卡的Cell扩展
    /// </summary>
    public class SelectCellConf : CellConf
    {
        /// <summary>
        /// 代表的路径，要对齐GameDataMgr.ShowPath
        /// </summary>
        public GameDataConf dataConf;
        /// <summary>
        /// 加载时显示的图片
        /// </summary>
        public Sprite loadImg;
        /// <summary>
        /// 预加载资源列表，可为空
        /// </summary>
        public List<string> loadList;
        public override void OnClickMainBtn()
        {
            base.OnClickMainBtn();
            GameDataMgr.Instance.ShowPath = dataConf.dataPath;
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Conf/SelectCellConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<SelectCellConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<SelectCellConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(SelectCellConf) + " is null");
                }
            }
        }
#endif
    }
}
