using BeinLab.Util;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///路径移动
/// </summary>
public class EffectLineRenderer : MonoBehaviour
{
    private float showTime = 2f;
    public float hideTime = 0.3f;
    public GameObject obj;
    // Use this for initialization
    void Start()
    {
        string[] times = name.Split('_');
        float time = float.Parse(times[times.Length - 1]);
        Transform[] tfs = GetComponentsInChildren<Transform>();
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < tfs.Length; i++)
        {
            points.Add(tfs[i].localPosition);
        }
        points.RemoveAt(0);
        var obj1 = Instantiate(obj, points[0], Quaternion.identity);
        obj1.transform.SetParent(transform);
        Vector3 initPos = GameNoder.Instance.Root.position;
        //initPos.y -=Random.Range(-0.5f,0.5f);
        //obj1.transform.position = GameNoder.Instance.Root.position;
        obj1.transform.position = initPos;
        obj1.GetComponent<TrailRenderer>().time = showTime;
        obj1.GetComponent<TrailRenderer>().widthMultiplier = transform.lossyScale.x * 0.5f;
        obj1.transform.DOLocalPath(points.ToArray(), 2f, PathType.CatmullRom).SetEase(Ease.OutCubic).onComplete += delegate ()
        {
            points.Reverse();
            obj1.GetComponent<TrailRenderer>().time = showTime;
            obj1.transform.DOLocalPath(points.ToArray(), 2f, PathType.CatmullRom).SetEase(Ease.OutCubic).onComplete += delegate ()
            {
                //obj1.transform.DOMove(GameNoder.Instance.Root.position,0.3f);
                obj1.transform.DOMove(initPos, 0.3f);
                obj1.GetComponent<TrailRenderer>().time = hideTime;
            };
        };
    }
}
