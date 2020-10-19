using UnityEngine;
using BeinLab.Util;
using BeinLab.FengYun.Gamer;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class DriverDynamicConf : TweenDynamicConf
    {
        public Vector3 startAngle;
        public Vector3 endAngle;
        private Transform targetFangXiangPan;
        public string driverName = "CarDriver_FangXiangPan";
        public override Tweener CreateTweener(Transform body)
        {
            GameObject obj = UnityUtil.GetChildByName(body.gameObject, driverName);
            if (obj)
            {
                targetFangXiangPan = obj.transform;
            }
            Tweener weener = null;
            if (targetFangXiangPan)
            {
                targetFangXiangPan.DOKill();
                targetFangXiangPan.localEulerAngles = startAngle;
                Tweener tweener = targetFangXiangPan.DOLocalRotate(endAngle, doTime);
            }
            return weener;
        }
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            if (targetFangXiangPan)
            {
                targetFangXiangPan.DOKill();
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/TweenDynamic/DriverDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<DriverDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<DriverDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DriverDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}