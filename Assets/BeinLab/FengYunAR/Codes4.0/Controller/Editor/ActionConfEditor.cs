using BeinLab.CarShow.Modus;
using BeinLab.Util;
using UnityEditor;
namespace BeinLab.ConfEditor
{
    [CustomEditor(typeof(ActionConf), true)]
    public class ActionConfEditor : Editor
    {
        private System.Type sysType;

        //private void Awake()
        //{
        //    if (sysType == null)
        //    {
        //        var target = (ActionConf)(serializedObject.targetObject);
        //        if (target.prefab)
        //        {
        //            sysType = target.prefab.GetType();
        //        }
        //    }
        //}
        //private void OnEnable()
        //{
        //    if (sysType == null)
        //    {
        //        if()
        //        var target = (ActionConf)(serializedObject.targetObject);
        //        if (target.prefab)
        //        {
        //            sysType = target.prefab.GetType();
        //        }
        //    }
        //}

        //UnityEngine.Object test;
        //public override void OnInspectorGUI()
        //{
        //    serializedObject.Update();
        //    var target = (ActionConf)(serializedObject.targetObject);
        //    if (target.actionType == ActionType.Dynamic || target.actionType == ActionType.DynamicAction ||
        //        target.actionType == ActionType.DynamicActionList || target.actionType == ActionType.DynamicGroup)
        //    {
        //        System.Type curType = null;
        //        if (target.actionType == ActionType.Dynamic)
        //        {
        //            curType = typeof(DynamicConf);
        //        }
        //        else if (target.actionType == ActionType.DynamicAction)
        //        {
        //            curType = typeof(DynamicActionConf);
        //        }
        //        else if (target.actionType == ActionType.DynamicActionList)
        //        {
        //            curType = typeof(DynamicActionListConf);
        //        }
        //        else if (target.actionType == ActionType.DynamicGroup)
        //        {
        //            curType = typeof(DynamicGroupConf);
        //        }
        //        if (sysType != curType)
        //        {
        //            //target.prefab = null;
        //        }
        //        sysType = curType;
        //        test = EditorGUILayout.ObjectField("事件对象", test, sysType);
        //        if (test)
        //        {
        //            target.action = AssetDatabase.GetAssetPath(test).Replace("Assets/", "").ToLower();
        //        }
        //    }
        //    serializedObject.ApplyModifiedProperties();
        //    base.OnInspectorGUI();
        //}
    }
}