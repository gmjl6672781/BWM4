using System.Collections;
using BeinLab.FengYun.Gamer;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    public class WheelFllowMateDynamicConf : FllowMateDynamicConf
    {
        private Transform carWheelRoot;
        public string carWheelRootName = "CarWheelRoot";
        public Vector3 direct = Vector3.forward;
        public float miniSpeed = 0;
        /// <summary>
        /// 使用本身移动的速度
        /// </summary>
        public bool useTransfer = false;
        public override IEnumerator FllowTarget(GameDynamicer gameDynamicer)
        {
            if (!carWheelRoot)
            {
                var obj = UnityUtil.GetChildByName(gameDynamicer.gameObject, carWheelRootName);
                if (obj)
                {
                    carWheelRoot = obj.transform;
                }
            }
            if (carWheelRoot)
            {
                Transform[] wheels = new Transform[carWheelRoot.transform.childCount];
                for (int i = 0; i < carWheelRoot.transform.childCount; i++)
                {
                    wheels[i] = carWheelRoot.transform.GetChild(i);
                }
                while (resMate || mateConf.resMate)
                {
                    ///每隔半秒检测一次，减少次数
                    Vector2 lastV = GetOffSet();
                    Vector3 lastPos = gameDynamicer.transform.localPosition;
                    yield return new WaitForFixedUpdate();
                    Vector2 curV = GetOffSet();
                    Vector3 curPos = gameDynamicer.transform.localPosition;
                    //realSpeed = ((Vector2.Distance(curV, lastV)) / Time.deltaTime) * gameDynamicer.Scale;
                    realSpeed = ((Vector2.Distance(curV, lastV)) / Time.deltaTime);
                    float transferSpeed = 0;
                    if (useTransfer)
                    {
                        transferSpeed = Vector3.Distance(lastPos, curPos) / Time.deltaTime * gameDynamicer.Scale;
                    }
                    float speed = mateMoveSpeed;
                    if (isFllowRealSpeed)
                    {
                        speed = realSpeed;
                    }
                    //Debug.Log(speed * fllowSpeed * Time.deltaTime);
                    for (int i = 0; i < wheels.Length; i++)
                    {
                        wheels[i].Rotate(direct, (speed + miniSpeed* transferSpeed) * fllowSpeed * Time.deltaTime, Space.Self);
                    }
                }
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/FllowMateDynamic/WheelFllowMateDynamicConf", false, 2)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<WheelFllowMateDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<WheelFllowMateDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(WheelFllowMateDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}