using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Gamer;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 材质配置
    /// </summary>
    [SerializeField]
    public class MateDynamicConf : DynamicConf
    {
        [Tooltip("源材质，要操作的材质")]
        public Material resMate;
        [Tooltip("是否重置材质")]
        public bool isReSetMate;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (isReSetMate)
            {
                DynamicActionController.Instance.ResetMate(resMate);
            }
            DynamicActionController.Instance.CopyMater(resMate);
            DoMateDynamic();
        }

        public virtual void DoMateDynamic()
        {

        }
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            if (isReSetMate)
            {
                DynamicActionController.Instance.ResetMate(resMate);
            }
        }
        public override void Complete()
        {
            base.Complete();
            if (isReSetMate)
            {
                DynamicActionController.Instance.ResetMate(resMate);
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/MateDynamic/MateDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<MateDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<MateDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(MateDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}