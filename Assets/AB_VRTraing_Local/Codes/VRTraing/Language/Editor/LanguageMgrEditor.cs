using BeinLab.VRTraing.Controller;
using UnityEditor;
using UnityEngine;

namespace BeinLab.VRTraing.Gamer
{
    [CustomEditor(typeof(LanguageMgr), true)]
    public class LanguageMgrEditor : Editor
    {
        public string editorPath;
        private int languageIndex = -1;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var target = (LanguageMgr)(serializedObject.targetObject);
            editorPath = EditorGUILayout.TextField("编辑器路径", editorPath);
            if (!LanguageMgr.Instance)
            {
                target.EditorAwake();
            }
            if (target.LanguageList == null)
            {
                target.Start();
            }
            target.languageIndex = EditorGUILayout.IntField("语言下标", target.languageIndex);
            target.languageIndex = Mathf.Clamp(target.languageIndex, 0, target.LanguageList.Count-1);

            ///改变了语言
            if (target.languageIndex != languageIndex)
            {
                target.SelectLanguage(target.languageIndex);
                languageIndex = target.languageIndex;
            }
            base.OnInspectorGUI();
        }
    }
}