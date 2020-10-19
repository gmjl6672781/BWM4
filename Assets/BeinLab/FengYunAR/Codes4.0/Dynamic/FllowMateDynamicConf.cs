using System.Collections;
using BeinLab.FengYun.Gamer;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class FllowMateDynamicConf : DynamicConf
    {
        [Tooltip("跟随的材质动画")]
        public MateAnimationDynamicConf mateConf;

        public Material resMate;
        public string attributes;

        private Coroutine updateCoroutine;
        public float fllowSpeed;
        public bool isFllowRealSpeed = false;
        public float mateMoveSpeed;
        public float realSpeed = 0;
        public Vector3 startPos;
        public Vector3 endPos;
        public bool isUseStartPos = true;
        public bool isUseFllowTime = false;
        public float fllowTime = 2;
        public Vector2 GetOffSet()
        {
            Vector2 lastV = Vector2.zero;
            if (string.IsNullOrEmpty(attributes))
            {
                lastV = resMate.mainTextureOffset;
            }
            else
            {
                lastV = resMate.GetTextureOffset(attributes);
            }
            return lastV;
        }

        //private IEnumerator UpdateSpeed(GameDynamicer gameDynamicer)
        //{
        //    while (resMate)
        //    {
        //        Vector2 lastV = GetOffSet();
        //        yield return new WaitForFixedUpdate();
        //        Vector2 curV = GetOffSet();
        //        realSpeed = ((Vector2.Distance(curV, lastV)) / Time.deltaTime) * gameDynamicer.Scale;
        //    }
        //}

        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (mateConf)
            {
                mateMoveSpeed = Vector2.Distance(mateConf.startVector, mateConf.endVector) / mateConf.doTime;
            }
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
            updateCoroutine = gameDynamicer.StartCoroutine(FllowTarget(gameDynamicer));
        }

        public virtual IEnumerator FllowTarget(GameDynamicer gameDynamicer)
        {
            if (isUseStartPos)
            {
                gameDynamicer.transform.localPosition = startPos;
            }
            //float detDis = Vector3.Distance(startPos, endPos);
            //float sTime = Time.time;
            ///Vector3.Distance(gameDynamicer.transform.localPosition, endPos) > gameDynamicer.Scale * 0.02f
            float curTime = Time.time;
            while ((resMate || mateConf.resMate) &&
                Vector3.Distance(gameDynamicer.transform.localPosition, endPos) > gameDynamicer.Scale * 0.2f)
            {
                ///每隔半秒检测一次，减少次数
                Vector2 lastV = GetOffSet();
                yield return new WaitForFixedUpdate();
                Vector2 curV = GetOffSet();
                realSpeed = ((Vector2.Distance(curV, lastV)) / Time.fixedDeltaTime) * gameDynamicer.Scale;
                float speed = mateMoveSpeed;
                if (isFllowRealSpeed)
                {
                    speed = realSpeed;
                    //if (Mathf.Abs( (speed * fllowSpeed)
                    //* Time.deltaTime * gameDynamicer.Scale) <= 0f)
                    //{
                    //    Debug.Log("break");
                    //    gameDynamicer.transform.DOLocalMove(endPos, 1);
                    //    break;
                    //}
                }
                gameDynamicer.transform.Translate((endPos - startPos).normalized * (speed * fllowSpeed)
                    * Time.fixedDeltaTime, Space.Self);
                if (isUseFllowTime)
                {
                    if (Time.time - curTime > fllowTime)
                    {
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/FllowMateDynamic/FllowMateDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<FllowMateDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<FllowMateDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(FllowMateDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}