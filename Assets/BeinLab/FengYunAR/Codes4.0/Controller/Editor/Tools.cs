using UnityEngine;
using UnityEditor;

public class Tools
{
    [MenuItem("Assets/Svn Commit")]
    static void SvnCommit()
    {
        string path = "Assets";
        string[] strs = Selection.assetGUIDs;
        if (strs != null)
        {
            path = "\"";
            for (int i = 0; i < strs.Length; i++)
            {
                if (i != 0)
                    path += "*";
                path += AssetDatabase.GUIDToAssetPath(strs[i]);
                if (AssetDatabase.GUIDToAssetPath(strs[i]) != "Assets")
                    path += "*" + AssetDatabase.GUIDToAssetPath(strs[i]) + ".meta";
            }
            path += "\"";
        }
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "TortoiseProc.exe";
        process.StartInfo.Arguments = "/command:commit /path:" + path;
        process.Start();
    }

    [MenuItem("Assets/Svn Update")]
    static void SvnUpdate()
    {
        string path = "Assets";
        string[] strs = Selection.assetGUIDs;
        if (strs != null)
        {
            path = "\"";
            for (int i = 0; i < strs.Length; i++)
            {
                if (i != 0)
                    path += "*";
                path += AssetDatabase.GUIDToAssetPath(strs[i]);
                if (AssetDatabase.GUIDToAssetPath(strs[i]) != "Assets")
                    path += "*" + AssetDatabase.GUIDToAssetPath(strs[i]) + ".meta";
            }
            path += "\"";
        }
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "TortoiseProc.exe";
        process.StartInfo.Arguments = "/command:update /path:" + path;
        process.Start();
    }
}
