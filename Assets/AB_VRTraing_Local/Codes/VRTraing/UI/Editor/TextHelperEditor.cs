using BeinLab.VRTraing.Controller;
using UnityEditor;
namespace BeinLab.VRTraing.Gamer
{
    [CustomEditor(typeof(TextHelper), true)]
    public class TextHelperEditor : Editor
    {
        private string key;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var target = (TextHelper)(serializedObject.targetObject);
            target.messageKey = EditorGUILayout.TextField("主键", target.messageKey);
            if (key != target.messageKey)
            {
                if (LanguageMgr.Instance && LanguageMgr.Instance.CurLanguage != null)
                {
                    target.realMessage = "Reading";
                    if (LanguageMgr.Instance.CurLanguage.LanguageMap == null)
                    {
                        LanguageMgr.Instance.CurLanguage.ReadLanguageMap();
                    }
                    target.OnChangeLaunge(null);
                    key = target.messageKey;
                }
                else
                {
                    target.realMessage = ("No Data Read");
                }
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }

    }
}