using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using BeinLab.Util;
public class AssetMaker : Editor
{
    [MenuItem("Tools/AssetMaker")]
    static void Create()
    {        // 实例化类  Bullet        
        ScriptableObject bullet = ScriptableObject.CreateInstance<BuildConfig>();
        // 如果实例化 Bullet 类为空，返回
        if (!bullet)
        {
            Debug.LogWarning("Bullet not found");
            return;
        }
        // 自定义资源保存路径
        string path = Application.dataPath + "/BeinLab/FengYunAR/Config";
        // 如果项目总不包含该路径，创建一个
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        //将类名 Bullet 转换为字符串
        //拼接保存自定义资源（.asset） 路径
        string assetPath = "Assets/" + path.Substring(Application.dataPath.Length + 1) + "/" + typeof(BuildConfig).Name + ".asset";
        Debug.Log(assetPath);
        // 生成自定义资源到指定路径
        AssetDatabase.CreateAsset(bullet, assetPath);
    }
}
