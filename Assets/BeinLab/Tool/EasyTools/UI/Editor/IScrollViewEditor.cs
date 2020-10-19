using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using SixEco.Util;

namespace SixEco.Util
{
    [CustomEditor(typeof(IScrollView), true)]
    public class MyScrollViewEditor : ScrollRectEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var target = (IScrollView)(serializedObject.targetObject);
            target.moveSpeed = EditorGUILayout.FloatField("回复速度", target.moveSpeed);
            target.showCount = EditorGUILayout.IntField("显示数量", target.showCount);
            target.isShowNext = EditorGUILayout.Toggle("是否显示Next", target.isShowNext);
            target.watieTime = EditorGUILayout.FloatField("回弹时间", target.watieTime);
            target.swipeDet = EditorGUILayout.FloatField("滑动比例", target.swipeDet);
            target.isFreeMove = EditorGUILayout.Toggle("是否自由移动", target.isFreeMove);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}