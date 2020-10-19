using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace BeinLab.Util
{
    public class AssetsViewWindow : EditorWindow
    {
        private Rect rectOfAsset;
        private string assetPath;
        //public bool IsRelativelyPath = true;
        public UnityEngine.Object targetObj;
        private bool isToLower;

        [MenuItem("Tools/AssetView %Y")]
        static void BuildViewWindow()
        {
            AssetsViewWindow bw = EditorWindow.GetWindow(typeof(AssetsViewWindow), false, "资源路径浏览器", true) as AssetsViewWindow;
            bw.ShowPopup();
        }
        private void OnGUI()
        {
            rectOfAsset = EditorGUILayout.GetControlRect(GUILayout.Width(600));
            assetPath = EditorGUI.TextField(rectOfAsset, "对象路径", assetPath);
            if (GUILayout.Button("刷新", GUILayout.Width(100)))
            {
                targetObj = AssetDatabase.LoadAssetAtPath("Assets/" + assetPath, typeof(UnityEngine.Object));
            }
            bool isChange = isToLower;
            isToLower = EditorGUILayout.ToggleLeft("自动小写", isToLower);
            if (isChange != isToLower)
            {
                isChange = isToLower;
                if (targetObj)
                {
                    assetPath = AssetDatabase.GetAssetPath(targetObj).Replace("Assets/", "");
                    if (isToLower)
                    {
                        assetPath = assetPath.ToLower();
                    }
                }
            }
            targetObj = EditorGUILayout.ObjectField(targetObj, typeof(UnityEngine.Object), null) as UnityEngine.Object;
            //targetObj = EditorGUI.ObjectField(new Rect(0, 5, 300, 20), targetObj, typeof(UnityEngine.Object), true);
            //IsRelativelyPath = EditorGUILayout.ToggleLeft("美术包相对路径", IsRelativelyPath);
            if (Event.current.type == EventType.DragUpdated
              || Event.current.type == EventType.DragExited)
            {
                if (rectOfAsset.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        assetPath = DragAndDrop.paths[0].Replace("Assets/", "");
                        if (isToLower)
                        {
                            assetPath = assetPath.ToLower();
                        }
                        //if (IsRelativelyPath)
                        //{
                        //    assetPath = assetPath.Substring(assetPath.IndexOf("/") + 1);
                        //}
                        targetObj = AssetDatabase.LoadAssetAtPath(DragAndDrop.paths[0], typeof(UnityEngine.Object));
                    }
                }
            }

        }
    }
}