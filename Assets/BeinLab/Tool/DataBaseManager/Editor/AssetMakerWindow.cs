using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace BeinLab.Util
{
    /// <summary>
    /// 快速创建Asset的插件
    /// 操作步骤：选中待打包的脚本，快捷键调出窗口，选择输出路径
    /// 输出路径选择完成后，判断输出路径是否已存在Asset，如果已存在不会打包
    /// 如果不存在，以名称创建数据输出路径和对应的数据模型Asset
    /// 如果已存在，给出预览图和数量
    /// 
    /// 需要参数，1，选中脚本对象
    /// 2，输出路径
    /// 3，Asset的数量
    /// 4，开启预览图显示
    /// </summary>
    public class AssetMakerWindow : EditorWindow
    {
        public string moduPath;
        public string outputPath;
        private Rect rectOfModuPath;
        //private static UnityEngine.Object activeObj;
        private string suffix = ".asset";
        private bool isShowRes;
        private FileSystemInfo[] fileInfos;
        private string assetView;
        public string dataPath = "DataBase";
        private string prefectPath;
        private int startNum = 1000;
        private int selectIndex;
        private string assetPath;
        private Rect rectOfAsset;
        public UnityEngine.Object obj;

        [MenuItem("Tools/AssetMakerWindow %T")]
        static void BuildWindow()
        {

            AssetMakerWindow bw = EditorWindow.GetWindow(typeof(AssetMakerWindow), false, "数据模型管理", true) as AssetMakerWindow;
            bw.ShowPopup();
        }

        private void OnEnable()
        {
            suffix = ".asset";
            AssetFlash.OnFlash += FlashAsset;
        }

        private void FlashAsset()
        {
            prefectPath = outputPath + "/" + dataPath + "/" + moduPath;
            string path = Application.dataPath + "/" + prefectPath;
            if (!Directory.Exists(path))
            {
                //Directory.CreateDirectory(path);
                fileInfos = null;
                assetView = "";
                assetPath = "";
                return;
            }
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            fileInfos = dirInfo.GetFileSystemInfos("*" + suffix);
            assetView = "";
            if (fileInfos != null)
            {
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    assetView += fileInfos[i].Name + "\n";
                    if (i == 0)
                    {
                        assetPath = fileInfos[i].FullName;
                        string iPath = assetPath.Substring(assetPath.IndexOf("Assets"));
                        assetPath = assetPath.Substring(assetPath.IndexOf("DataBase"));

                        obj = AssetDatabase.LoadAssetAtPath(iPath, typeof(UnityEngine.Object));
                    }
                }
            }
        }


        private void OnDisable()
        {
            AssetFlash.OnFlash -= FlashAsset;
        }
        bool isForce = false;
        private void OnGUI()
        {

            rectOfModuPath = EditorGUILayout.GetControlRect(GUILayout.Width(300));
            moduPath = EditorGUI.TextField(rectOfModuPath, "数据模型路径", moduPath);
            if (GUILayout.Button("刷新路径", GUILayout.Width(100)))
            {
                obj = null;
                AssetFlash.ChangeProject();
                isForce = true;
                FlashAsset();
            }
            if (AssetFlash.assetPathList != null)
            {
                int select = EditorGUILayout.Popup("资源路径", selectIndex, AssetFlash.assetPathList.ToArray());
                if (select != selectIndex || isForce)
                {
                    selectIndex = select;
                    outputPath = AssetFlash.assetPathList[selectIndex];
                    FlashAsset();
                    isForce = false;
                }
            }
            //如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内  
            if (Event.current.type == EventType.DragUpdated
              || Event.current.type == EventType.DragExited)
            {
                if (rectOfModuPath.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        string mPath = DragAndDrop.paths[0];
                        if (!string.IsNullOrEmpty(mPath))
                        {
                            if (mPath.EndsWith(".cs"))
                            {
                                string[] paths = mPath.Split('/');
                                mPath = paths[paths.Length - 1];
                                moduPath = mPath.Replace(Path.GetExtension(mPath), "");
                                OnProjectChange();
                            }
                        }
                        //string path = Application.dataPath + outputPath;
                    }
                }
            }
            if (fileInfos != null)
            {
                GUILayout.Label(moduPath + " Asset Count: " + fileInfos.Length);
                GUILayout.Label("路径" + prefectPath);
            }
            if (GUILayout.Button("创建asset", GUILayout.Width(100)))
            {
                if (string.IsNullOrEmpty(moduPath)) return;
                string path = "Assets/" + prefectPath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    AssetDatabase.Refresh();
                }
                path += "/" + moduPath + "_" + (GetMaxNum()) + suffix;

                ScriptableObject bullet = ScriptableObject.CreateInstance(moduPath);
                Debug.Log(path);
                AssetDatabase.CreateAsset(bullet, path);
                AssetDatabase.Refresh();
                FlashAsset();

                obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                assetPath = path.Substring(path.IndexOf("DataBase"));
            }
            rectOfAsset = EditorGUILayout.GetControlRect(GUILayout.Width(866));
            if (!string.IsNullOrEmpty(assetPath))
            {

                assetPath = EditorGUI.TextField(rectOfAsset, "对象路径", assetPath);
            }
            if (obj != null)
            {
                EditorGUILayout.ObjectField("目标对象", obj, obj.GetType());
                //EditorGUI.ObjectField(new Rect(0, 5, 300, 20), "目标对象", obj, obj.GetType(), true);
            }
            isShowRes = EditorGUILayout.BeginToggleGroup("资源列表预览", isShowRes);
            if (isShowRes)
            {
                EditorGUILayout.TextArea(assetView);
            }

            EditorGUILayout.EndToggleGroup();
        }

        private int GetMaxNum()
        {
            if (fileInfos != null && fileInfos.Length > 0)
            {
                string[] infos = fileInfos[fileInfos.Length - 1].Name.Split('_');
                string num = infos[infos.Length - 1];
                num = num.Replace(Path.GetExtension(num), "");
                Debug.Log(num);
                int index;
                if (int.TryParse(num, out index))
                {
                    int max = index + 1;
                    return max;
                }
            }
            return startNum;
        }

        private void OnProjectChange()
        {
            AssetFlash.ChangeProject();
        }

    }
}