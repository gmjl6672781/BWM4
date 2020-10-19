using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace BeinLab.Util
{
    [ExecuteInEditMode]
    /// <summary>
    /// 刷新路径
    /// </summary>
    public class AssetFlash : Editor
    {
        /// <summary>
        /// 
        /// </summary>
        public static List<string> assetPathList;
        /// <summary>
        /// 路径改变时的调用函数
        /// </summary>
        public static event Action OnFlash;
        /// <summary>
        /// 强制性刷新目录
        /// 刷新项目快捷键，%E
        /// </summary>
        [MenuItem("Tools/FlashAssets %E")]
        static void FlashAssets()
        {
            ChangeProject();
        }

        /// <summary>
        /// 其他函数调用，editorwindow在更新的时候调用一次
        /// </summary>
        public static void ChangeProject()
        {
            if (assetPathList == null) assetPathList = new List<string>();

            string tmpPath = Application.dataPath;
            string[] paths = Directory.GetDirectories(tmpPath);
            bool isChange = false;
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = paths[i].Replace("\\","/");
                string path = paths[i].Substring(paths[i].LastIndexOf("/") + 1);
                if (!assetPathList.Contains(path))
                {
                    assetPathList.Add(path);
                    isChange = true;
                }
            }
            if (paths.Length != assetPathList.Count)
            {
                assetPathList.Clear();
                ChangeProject();
                if (OnFlash != null)
                {
                    OnFlash();
                }
            }
            if (isChange)
            {
                if (OnFlash != null)
                {
                    OnFlash();
                }
            }
        }


    }
}