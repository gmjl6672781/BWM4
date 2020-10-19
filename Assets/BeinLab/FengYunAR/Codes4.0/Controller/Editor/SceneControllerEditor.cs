using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Modu;
using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace BeinLab.ConfEditor
{
    [CustomEditor(typeof(SceneController), true)]
    public class SceneControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var target = (SceneController)(serializedObject.targetObject);
            target.sceneConfPrefab = EditorGUILayout.ObjectField("场景配置文件", target.sceneConfPrefab, typeof(ActionConf));
            if (target.sceneConfPrefab)
            {
                target.actionPath = AssetDatabase.GetAssetPath(target.sceneConfPrefab).Replace("Assets/", "").ToLower();
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}