using UnityEngine;
using BeinLab.Util;
using BeinLab.FengYun.Gamer;
using DG.Tweening;
using BeinLab.RS5.Mgr;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 逆向位置变换
    /// 把相机放到某物体的相对坐标，例如把相机放到驾驶位置，并同步视角
    /// 还有更多的可能，把相机放到某物体的某个位置，方便更好的观看展示
    /// 反转对象为GameNoder.Root;
    /// </summary>
    [SerializeField]
    public class InverTransferDynamicConf : TweenDynamicConf
    {
        /// <summary>
        /// 反转坐标
        /// </summary>
        public Vector3 inverPosition;
        /// <summary>
        /// 反转角度
        /// </summary>
        public Vector3 inverAngle;
        /// <summary>
        /// 反转尺寸
        /// </summary>
        public Vector3 inverSize = Vector3.one;
        /// <summary>
        /// 变换前的坐标
        /// </summary>
        private Vector3 lastInverPos;
        /// <summary>
        /// 变换前的角度
        /// </summary>
        private Vector3 lastInverAngle;
        /// <summary>
        /// 变换前的尺寸比例
        /// </summary>
        private Vector3 lastInverScale = Vector3.one;
        /// <summary>
        /// 创建一个Tween变换
        /// 变换的对象是GameNoder本身？好处是，只有变换模式和复原模式,变换模式禁用缩放
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public override Tweener CreateTweener(Transform body)
        {
            Transform target = null;
            if (XRController.Instance && XRController.Instance.ARCamera)
            {
                target = XRController.Instance.ARCamera.transform;
            }
            //GameNoder.Instance.transform.position = Vector3.zero;
            //inverAngle += obj.transform.eulerAngles;
            Vector3 pos = target.TransformPoint(inverPosition);
            Vector3 rot = target.eulerAngles + inverAngle;
            pos -= GameNoder.Instance.transform.InverseTransformPoint(GameNoder.Instance.Root.position);
            ///rot -= GameNoder.Instance.transform.InverseTransformVector(GameNoder.Instance.Root.eulerAngles);
            Tweener tween = null;
            GameNoder.Instance.Root.DOScale(Vector3.one, doTime / 2f);
            GameNoder.Instance.transform.rotation = Quaternion.identity;
            GameNoder.Instance.Root.eulerAngles = rot;
            tween = GameNoder.Instance.transform.DOMove(pos, doTime / 2);
            return tween;
        }
        /// <summary>
        /// 当动效停止时
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public override void OnStop(GameDynamicer gameDynamicer)
        {

        }
        //public void ReSetPos(Transform worldAnchorTrans, bool isWaiGuan = true)
        //{


        //    if (isWaiGuan)
        //    {
        //        //ReSetPos(worldAnchorTrans.TransformPoint(Vector3.zero), lastAngle, lastScale);
        //    }
        //    else
        //    {
        //        Vector3 targetPos = Vector3.zero;
        //        Vector3 targetAngle = Vector3.zero;
        //        Transform mainCam = XRController.Instance.ARCamera.transform;
        //        Vector3 oldAngle = mainCam.eulerAngles;
        //        mainCam.eulerAngles = Vector3.zero;
        //        targetAngle = oldAngle;
        //        ////算法，计算targetPos、targetAngle相对于相机的正确位置的世界坐标
        //        targetPos = mainCam.TransformPoint(targetPos);
        //        targetAngle.x = targetAngle.z = 0;
        //        mainCam.eulerAngles = oldAngle;
        //    }
        //}

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/TweenDynamic/TransferDynamic/InverTransferDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<InverTransferDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<InverTransferDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(InverTransferDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}