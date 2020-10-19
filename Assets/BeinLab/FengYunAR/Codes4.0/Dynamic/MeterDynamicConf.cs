using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Gamer;
using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[SerializeField]
public class MeterDynamicConf : DynamicConf
{
    public float startNum;
    public float endNum;
    public float changeTime;
    private TextMesh numLabel;
    private Transform pointRoot;
    private Coroutine updateCoroutine;

    public override void DoDynamic(GameDynamicer gameDynamicer)
    {
        base.DoDynamic(gameDynamicer);
        numLabel = UnityUtil.GetChildByName(gameDynamicer.gameObject, "NumLabel").GetComponent<TextMesh>();
        pointRoot = UnityUtil.GetChildByName(gameDynamicer.gameObject, "PointRoot").GetComponent<Transform>();
        if (updateCoroutine != null)
        {
            gameDynamicer.StopCoroutine(updateCoroutine);
        }
        updateCoroutine = gameDynamicer.StartCoroutine(ChangeNum(gameDynamicer));
    }

    private IEnumerator ChangeNum(GameDynamicer gameDynamicer)
    {
        numLabel.text = startNum.ToString("f0");
        float tmp = startNum;
        Vector3 angle = pointRoot.localEulerAngles;
        float det = endNum - tmp;
        while (Mathf.Abs(tmp - endNum) > 1 && changeTime > 0)
        {
            tmp += Time.deltaTime * det / changeTime;
            yield return new WaitForFixedUpdate();
            numLabel.text = tmp.ToString("f0");
            angle.z = tmp * 270f / 120f;
            pointRoot.localEulerAngles = angle;
        }
        numLabel.text = endNum.ToString("f0");
        angle.z = endNum * 270f / 120f;
        pointRoot.localEulerAngles = angle;
    }
    public override void OnStop(GameDynamicer gameDynamicer)
    {
        base.OnStop(gameDynamicer);
        if (updateCoroutine != null)
        {
            gameDynamicer.StopCoroutine(updateCoroutine);
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Bein_Dynamic/MeterDynamicConf", false, 0)]
    static void CreateDynamicConf()
    {
        UnityEngine.Object obj = Selection.activeObject;
        if (obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            ScriptableObject bullet = ScriptableObject.CreateInstance<MeterDynamicConf>();
            if (bullet)
            {
                string confName = UnityUtil.TryGetName<MeterDynamicConf>(path);
                AssetDatabase.CreateAsset(bullet, confName);
            }
            else
            {
                Debug.Log(typeof(MeterDynamicConf) + " is null");
            }
        }
    }
#endif
}
