using BeinLab.CarShow.Modus;
using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace BeinLab.ConfEditor
{
    [CustomEditor(typeof(DistenceDynamicConf), true)]
    public class DistenceDynamicConfEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var target = (DistenceDynamicConf)(serializedObject.targetObject);

            target.nearActionConf = EditorGUILayout.ObjectField("靠近事件", target.nearActionConf, typeof(ActionConf));
            target.farActionConf = EditorGUILayout.ObjectField("远离事件", target.farActionConf, typeof(ActionConf));
            if (target.nearActionConf)
            {
                target.nearTargetAction = AssetDatabase.GetAssetPath(target.nearActionConf).Replace("Assets/", "");
            }
            if (target.farActionConf)
            {
                target.farTargetAction = AssetDatabase.GetAssetPath(target.farActionConf).Replace("Assets/", "");
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}