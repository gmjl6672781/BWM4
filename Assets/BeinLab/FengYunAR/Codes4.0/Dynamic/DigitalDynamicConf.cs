using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.FengYun.Gamer;
using BeinLab.CarShow.Modus;
using BeinLab.Util;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[SerializeField]
public class DigitalDynamicConf : DynamicConf
{
    public float startNum;
    public float endNum;
    public float changeTime;
    public string otherText = "mm";
    public string targetName;
    private Text numLabel;
    private Coroutine updateCoroutine;

    public override void DoDynamic(GameDynamicer gameDynamicer)
    {
        base.DoDynamic(gameDynamicer);
        numLabel = UnityUtil.GetChildByName(gameDynamicer.gameObject, targetName).GetComponent<Text>();
        if (updateCoroutine != null)
        {
            gameDynamicer.StopCoroutine(updateCoroutine);
        }
        updateCoroutine = gameDynamicer.StartCoroutine(ChangeNum(gameDynamicer));
    }

    private IEnumerator ChangeNum(GameDynamicer gameDynamicer)
    {
        numLabel.text = startNum.ToString("f0")+ otherText;
        float tmp = startNum;
        float det = endNum - tmp;
        while (tmp < endNum && changeTime > 0)
        {
            tmp += Time.deltaTime * det / changeTime;
            yield return new WaitForFixedUpdate();
            numLabel.text = tmp.ToString("f0")+ otherText;
        }
        numLabel.text = endNum.ToString("f0") + otherText;
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
    [MenuItem("Assets/Create/Bein_Dynamic/DigitalDynamicConf", false, 0)]
    static void CreateDynamicConf()
    {
        UnityEngine.Object obj = Selection.activeObject;
        if (obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            ScriptableObject bullet = ScriptableObject.CreateInstance<DigitalDynamicConf>();
            if (bullet)
            {
                string confName = UnityUtil.TryGetName<DigitalDynamicConf>(path);
                AssetDatabase.CreateAsset(bullet, confName);
            }
            else
            {
                Debug.Log(typeof(DigitalDynamicConf) + " is null");
            }
        }
    }
#endif
}
