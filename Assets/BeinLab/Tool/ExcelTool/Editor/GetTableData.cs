using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Excel;
using System.Data;
using System;
using UnityEditor;
using BeinLab.VRTraing.Conf;
using BeinLab.Util;
using BeinLab.VRTraing.Controller;

public class GetTableData : EditorWindow
{
    public static GetTableData instance;

    /// <summary>
    /// 语言包的配置文件路径
    /// </summary>
    public string languagePath = "GameData/Chinese";

    private DataSet result;

    /// <summary>
    /// 表格路径
    /// </summary>
    public string excelTablePath = "BMW_2.xlsx";

    /// <summary>
    /// 需要转换的列
    /// </summary>
    public string ConversionColumn = "4|5";

    public string key;
    public string msg;

    public List<int> columns_Num = new List<int>();
    
    private int rows;
    private int columns;
    public List<List<string>> allTaskData = new List<List<string>>();

    public bool isGetDataOver = false;

    public LanguageConf languageConf;
    // Use this for initialization
    void Awake()
    {
        instance = this;
        //XLSX();
    }

    [MenuItem("Tools/GetTableData")]
    static void BuildWindow()
    {
        GetTableData bw = EditorWindow.GetWindow(typeof(GetTableData), false, "数据转换器", true) as GetTableData;
        bw.ShowPopup();
        bw.autoRepaintOnSceneChange = true;
    }

    private void OnGUI()
    {
        languagePath = EditorGUILayout.TextField("语言包存放路径", languagePath);
        excelTablePath = EditorGUILayout.TextField("表格路径", excelTablePath);
        ConversionColumn = EditorGUILayout.TextField("有效列", ConversionColumn);
        key= EditorGUILayout.TextField("key", key);
        msg = EditorGUILayout.TextField("msg", msg);
        if (GUILayout.Button("转换", GUILayout.Width(80), GUILayout.Height(20)))
        {
            XLSX();
        }
    }

    void XLSX()
    {
        columns_Num.Clear();
        string[] arr = ConversionColumn.Split('|');
        for (int i = 0; i < arr.Length; i++)
        {
            columns_Num.Add(int.Parse(arr[i]));
            Debug.Log(columns_Num[i]);
        }

        FileStream stream = File.Open(Application.dataPath + "/" + excelTablePath, FileMode.Open, FileAccess.Read);

        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        
        result = excelReader.AsDataSet();
        columns = result.Tables[0].Columns.Count;
        rows = result.Tables[0].Rows.Count;
        for (int i = 1; i < rows; i++)
        {
            List<string> column = new List<string>();
            for (int j = 0; j < columns-1; j++)
            {
                for (int q = 0; q < columns_Num.Count; q++)
                {
                    if (j == columns_Num[q])
                    {
                        column.Add(result.Tables[0].Rows[i][j].ToString());
                        //print(result.Tables[0].Rows[i][j].ToString());
                    }
                }
            }
            allTaskData.Add(column);
        }

        string realDataPath = Path.Combine(Application.dataPath, languagePath);
        if (!Directory.Exists(realDataPath))
        {
            Directory.CreateDirectory(realDataPath);
        }

        if (languageConf == null)
        {
            languageConf = new LanguageConf();
        }
        //Debug.Log(languageConf);
        for (int i = 0; i < allTaskData.Count; i++)
        {
            languageConf.PriKey = "T" + allTaskData[i][int.Parse(key)];
            languageConf.Message = allTaskData[i][int.Parse(msg)];
            languageConf.AudioPath = "test";
            UnityUtil.WriteXMLData<LanguageConf>(realDataPath, languageConf);
        }
        AssetDatabase.Refresh();
    }
}