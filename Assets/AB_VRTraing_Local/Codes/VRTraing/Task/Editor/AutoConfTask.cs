using BeinLab.Util;
using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 自动配置任务
    /// </summary>
    public class AutoConfTask : EditorWindow
    {
        private string folderPath;
        private string separtor ="_";
        private string format = "{0:000}";
        private List<TaskConf> taskConfs;

        [MenuItem("Tools/AutoConfTask #T")]
        static void BuildViewWindow()
        {
            AutoConfTask autoConfTask = EditorWindow.GetWindow(typeof(AutoConfTask), false, "任务自动配置器", true) as AutoConfTask;
            autoConfTask.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);
            autoConfTask.ShowPopup();
        }

        private void OnGUI()
        {
            EditorGUI.LabelField(EditorGUILayout.GetControlRect(GUILayout.Width(600)), "从1开始编码，不能从0开始");
            folderPath = EditorGUI.TextField(EditorGUILayout.GetControlRect(GUILayout.Width(600)), "文件夹路径", folderPath);
            separtor = EditorGUI.TextField(EditorGUILayout.GetControlRect(GUILayout.Width(600)), "分隔符", separtor);
            format = EditorGUI.TextField(EditorGUILayout.GetControlRect(GUILayout.Width(600)), "编码格式{0:000}", format);


            if (GUILayout.Button("开始配置", GUILayout.Width(100)))
            {
                DirectoryInfo direct = new DirectoryInfo(folderPath);
                FileInfo[] files = direct.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".Asset", StringComparison.OrdinalIgnoreCase))
                    {
                        var fullName = files[i].FullName;
                        var assetPath = fullName.Substring(fullName.IndexOf("Assets")).Replace("\\", "/");
                        var asset = AssetDatabase.LoadAssetAtPath<TaskConf>(assetPath);
                        if (asset)
                        {
                            taskConfs.Add(asset);
                            Debug.Log(asset.name);
                        }
                    }
                }

                taskConfs.ForEach(taskConf =>
                {
                    var subName = taskConf.name.Substring(0, taskConf.name.LastIndexOf(separtor));
                    Debug.Log(subName);
                    //配置Parent节点
                    taskConf.parent = GetParentNode(taskConf);
                    //配置Child节点
                    taskConf.child = GetChildNode(taskConf);
                    //配置OldBother节点
                    taskConf.oldBrother = GetOldBrotherNode(taskConf);
                    //配置LittlBrother节点
                    taskConf.littleBrother = GetLittleBrotherNode(taskConf);
                });

                taskConfs.Clear();
            }
        }

        private TaskConf GetParentNode(TaskConf taskConf)
        {
            //对于没有父节点的
            if (taskConf.name.Split(separtor.ToCharArray()).Length <= 2)
                return null;
            foreach (TaskConf item in taskConfs)
            {
                if (GetSubName(taskConf.name,2) == GetSubName(item.name,1))
                {
                    return item;
                }
            }
            return null;
        }

        private TaskConf GetChildNode(TaskConf taskConf)
        {
            foreach (TaskConf item in taskConfs)
            {
                if (GetSubName(taskConf.name, 1) + string.Format(format, 1) + separtor == GetSubName(item.name, 1))
                    return item;
            }
            return null;
        }

        private TaskConf GetLittleBrotherNode(TaskConf taskConf)
        {
            var splitName = string.Format(format, Convert.ToInt32(GetSplitName(taskConf.name, 2)) + 1);
            var subName = GetSubName(taskConf.name, 2);
            foreach(TaskConf item in taskConfs)
            {
                var itemSplitName = string.Format(format, Convert.ToInt32(GetSplitName(item.name, 2)));
                var itemSubName = GetSubName(item.name, 2);
                if (subName == itemSubName && splitName == itemSplitName)
                    return item;
            }
            return null;
        }

        private  TaskConf GetOldBrotherNode(TaskConf taskConf)
        {

            var splitName = string.Format(format, Convert.ToInt32(GetSplitName(taskConf.name, 2)) - 1);
            var subName = GetSubName(taskConf.name, 2);

            foreach (TaskConf item in taskConfs)
            {
                var itemSplitName = string.Format(format, Convert.ToInt32(GetSplitName(item.name, 2)));
                var itemSubName = GetSubName(item.name, 2);
                if (subName == itemSubName && splitName == itemSplitName)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// 从第0个字符开始取，到倒数第lastIndex个separtor结束
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="separtorIndex"></param>
        /// <returns></returns>
        private string GetSubName(string fullName,int lastIndex)
        {
            string temp = "";

            if (lastIndex <= 0)
                return fullName;

            var splitNames = fullName.Split(separtor.ToCharArray());

            if (lastIndex >= splitNames.Length)
                return temp;
            for (int i = 0; i < splitNames.Length - lastIndex; i++)
            {
                temp += splitNames[i] + separtor;
            }
            return temp;
        }

        /// <summary>
        /// 取001,002...
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="separtorIndex"></param>
        /// <returns></returns>
        private string GetSplitName(string fullName,int lastIndex)
        {
            if (lastIndex <= 0)
                return fullName;

            var splitNames = fullName.Split(separtor.ToCharArray());

            if (lastIndex > splitNames.Length)
                return "";

            return splitNames[splitNames.Length - lastIndex];
        }

    }
}


