using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Karler.Lib.Data;
/// <summary>
/// 程序运行前先读取关键数据文件
/// 如果文件存在则进入场景中
/// 否则从系统的拷贝全部文件
/// </summary>
public class LoadingData : MonoBehaviour
{
    void Start()
    {
        LoadLocalData();
    }
    public void LoadLocalData() {
        
    }
}
