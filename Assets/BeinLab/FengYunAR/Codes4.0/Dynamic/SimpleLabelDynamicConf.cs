using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeinLab.FengYun.Gamer;
using DG.Tweening;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 文字动效
    /// </summary>
    [SerializeField]
    public class SimpleLabelDynamicConf : DynamicConf
    {
        /// <summary>
        /// 背景图
        /// </summary>
        public Sprite bg;
        /// <summary>
        /// 文本信息
        /// </summary>
        public string message;
        /// <summary>
        /// 字体大小,默认60
        /// </summary>
        public int fontSize = 60;
        public TextAnchor labelAlignment = TextAnchor.MiddleCenter;
        /// <summary>
        /// 对齐方式 （0,0）代表居中对齐
        /// </summary>
        public Vector2 alignment = Vector2.zero;
        /// <summary>
        /// 连线
        /// </summary>
        public Vector3[] linePath;
        /// <summary>
        /// 连线的宽度
        /// </summary>
        public float lineRudis = 0.008f;
        public float lineTime = 0.3f;



        private Text showLabel;
        private Image simpleBG;
        private LineRenderer line;
        private Transform simpleRoot;
        private Timer lineTimer;
        private Coroutine updateCoroutine;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            InitCompent(gameDynamicer.gameObject);
            if (bg != null)
            {
                simpleBG.sprite = bg;
            }
            showLabel.text = UnityUtil.SplitToLine(message);
            showLabel.fontSize = fontSize;
            showLabel.alignment = labelAlignment;
            simpleRoot.localScale = Vector3.zero;

            line.gameObject.SetActive(false);
            if (linePath != null)
            {
                if (linePath.Length > 1)
                {
                    if (updateCoroutine != null)
                    {
                        gameDynamicer.StopCoroutine(updateCoroutine);
                    }
                    updateCoroutine = gameDynamicer.StartCoroutine(DrawLine());
                }
            }
            ClearTimer();
            lineTimer = TimerMgr.Instance.CreateTimer(delegate ()
            {
                Vector2 pos = simpleBG.rectTransform.sizeDelta / 2f;
                pos.x *= alignment.x * simpleBG.transform.localScale.x;
                pos.y *= alignment.y * simpleBG.transform.localScale.y;
                simpleBG.rectTransform.anchoredPosition = pos;
                simpleRoot.DOScale(Vector3.one, 0.1f);
            }, lineTime, 1);
        }

        private IEnumerator DrawLine()
        {
            GameObject tweenObj = new GameObject("TweenObj");
            UnityUtil.SetParent(line.transform, tweenObj.transform);
            line.positionCount = 1;
            line.SetPosition(0, linePath[0]);
            float time = lineTime / linePath.Length;
            line.widthMultiplier = lineRudis * line.transform.lossyScale.x;
            simpleRoot.position = line.transform.TransformPoint(linePath[linePath.Length - 1]);
            line.gameObject.SetActive(true);
            for (int i = 1; i < linePath.Length; i++)
            {
                line.positionCount = i + 1;
                tweenObj.transform.localPosition = linePath[i - 1];
                Tweener tween = tweenObj.transform.DOLocalMove(linePath[i], time);
                while (tween.IsPlaying())
                {
                    yield return new WaitForFixedUpdate();
                    line.SetPosition(i, tweenObj.transform.localPosition);
                }
            }
            Destroy(tweenObj);
            yield return 0;
        }

        private void ClearTimer()
        {
            if (lineTimer != null && TimerMgr.Instance)
            {
                TimerMgr.Instance.DestroyTimer(lineTimer);
                lineTimer = null;
            }
        }

        private void InitCompent(GameObject gameObject)
        {
            showLabel = UnityUtil.GetTypeChildByName<Text>(gameObject, "ShowLabel");
            simpleBG = UnityUtil.GetTypeChildByName<Image>(gameObject, "SimpleBG");
            line = UnityUtil.GetTypeChildByName<LineRenderer>(gameObject, "Line");
            simpleRoot = UnityUtil.GetChildByName(gameObject, "SimpleRoot").transform;
            simpleRoot.localScale = Vector3.zero;
            line.positionCount = 0;
        }
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
            ClearTimer();
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/SimpleLabelDynamicConf", false, 2)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<SimpleLabelDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<SimpleLabelDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(SimpleLabelDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}