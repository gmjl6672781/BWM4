using BeinLab.CarShow.Modus;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace BeinLab.ConfEditor
{
    //[CustomEditor(typeof(GameDynamicConf), true)]
    //public class GameDynamicConfEditor : Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();
    //        serializedObject.Update();
    //        var target = (GameDynamicConf)(serializedObject.targetObject);

    //        target.prefab = EditorGUILayout.ObjectField("预制体路径", target.prefab, typeof(Object));
    //        if (target.prefab)
    //        {
    //            target.prefabPath = AssetDatabase.GetAssetPath(target.prefab).Replace("Assets/", "").ToLower();
    //        }
    //        serializedObject.ApplyModifiedProperties();
    //    }
    //}
}