using UnityEngine;
using BeinLab.Util;
using DG.Tweening;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class ImageFadeDynamicConf : TweenDynamicConf
    {
        public Color startColor;
        public Color endColor;
        public string imageName = "Image";
        private Image targetImage;
        public bool isUseStart = true;
        public override Tweener CreateTweener(Transform body)
        {
            Tweener tweener = null;
            if (!targetImage)
            {
                targetImage = UnityUtil.GetTypeChildByName<Image>(body.gameObject, imageName);
            }
            if (targetImage)
            {
                if (isKillOld)
                {
                    targetImage.DOKill();
                }
                if (isUseStart)
                {
                    targetImage.color = startColor;
                }
                tweener = targetImage.DOColor(endColor, doTime);
            }
            return tweener;
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/TweenDynamic/ImageFadeDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<ImageFadeDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ImageFadeDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(ImageFadeDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}