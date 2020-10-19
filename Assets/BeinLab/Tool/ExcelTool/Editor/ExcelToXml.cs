using BeinLab.VRTraing.Conf;
using Excel;
using Karler.Lib.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace BeinLab.Util
{
    /// <summary>
    /// Execl转XML对象文件
    /// </summary>
    public class ExcelToXml : EditorWindow
    {
        /// <summary>
        /// excel文件路径
        /// </summary>
        private string excelPath;
        /// <summary>
        /// 输出路径
        /// </summary>
        private string outPath;
        /// <summary>
        /// 转化的对象名称
        /// </summary>
        //private string objectName;
        /// <summary>
        /// 转化规则
        /// 键值对分别采用','和'|'分割
        /// 例如  1,PriKey|3,Message
        /// </summary>
        private string rule;
        private int startRow = 1;
        private bool isReadEnd;
        private int endRow = int.MaxValue;
        /// <summary>
        /// 
        /// </summary>
        private string prefix = "T";

        /// <summary>
        /// 强制更新
        /// </summary>
        private bool isForeceUpdate;

        [MenuItem("Tools/ExcelToXML")]
        static void ExcelToXML()
        {
            ExcelToXml bw = GetWindow(typeof(ExcelToXml), false, "Excel转XML", true) as ExcelToXml;
            bw.ShowPopup();
            bw.autoRepaintOnSceneChange = true;
        }

        /// <summary>
        /// 转换器
        /// </summary>
        private void OnGUI()
        {
            ///Excel的路径
            excelPath = EditorGUILayout.TextField("Excel路径", excelPath);
            ///输出路径
            outPath = EditorGUILayout.TextField("输出路径", outPath);

            //objectName = EditorGUILayout.TextField("转化对象", objectName);

            //targetObj = EditorGUILayout.ObjectField("转化对象", targetObj, typeof(UnityEngine.Object), null) as UnityEngine.Object;

            rule = EditorGUILayout.TextField("转化规则", rule);
            startRow = EditorGUILayout.IntField("起始行", startRow);
            isReadEnd = EditorGUILayout.BeginToggleGroup("启用终止行", isReadEnd);
            endRow = EditorGUILayout.IntField("终止行", endRow);
            EditorGUILayout.EndToggleGroup();
            prefix = EditorGUILayout.TextField("主键前缀", prefix);

            isForeceUpdate = EditorGUILayout.ToggleLeft("强制刷新", isForeceUpdate);
            ///刷新指定路径下的所有资源的标签
            if (GUILayout.Button("转换", GUILayout.Width(100)))
            {
                ConversionToXML();
            }
        }

        /// <summary>
        /// 开始转换
        /// </summary>
        public void ConversionToXML()
        {
            ///打开或者创建Excel
            FileStream stream = File.Open(Application.dataPath + "/" + excelPath, FileMode.OpenOrCreate);

            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet dataSet = excelReader.AsDataSet();
            ///获取列
            DataColumnCollection columns = dataSet.Tables[0].Columns;
            ///获取行
            DataRowCollection rows = dataSet.Tables[0].Rows;

            ///解析出对应的元素，元素包含要解析的列和对应的映射
            List<string> elementList = new List<string>(rule.Split('|'));
            Dictionary<int, string> elementHash = new Dictionary<int, string>();
            for (int i = 0; i < elementList.Count; i++)
            {
                ///对应的列和映射的key
                string[] keys = elementList[i].Split(',');

                elementHash.Add(int.Parse(keys[0]), keys[1]);
            }


            ///每一行取指定列
            int maxRow = endRow < rows.Count ? endRow : rows.Count;
            //List<Dictionary<string, string>> excelData = new List<Dictionary<string, string>>();


            string realDataPath = Path.Combine(Application.dataPath, outPath);
            if (!Directory.Exists(realDataPath))
            {
                Directory.CreateDirectory(realDataPath);
            }
            MySql.WorkPath = realDataPath;
            try
            {
                bool isOpen = MySql.Open<LanguageConf>(true);
                if (isOpen)
                {
                    for (int i = startRow; i < maxRow; i++)
                    {
                        ///以行为单位开始遍历取值
                        Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                        foreach (var item in elementHash)
                        {
                            //Debug.Log(item.Value + " -- " + dataSet.Tables[0].Rows[i][item.Key].ToString());
                            string keyValue = dataSet.Tables[0].Rows[i][item.Key].ToString();
                            if (string.IsNullOrEmpty(keyValue))
                            {
                                break;
                            }
                            if (item.Value == "PriKey")
                            {
                                int num = -1;
                                if (int.TryParse(keyValue, out num))
                                {
                                    keyValue = prefix + keyValue;
                                }
                            }
                            valuePairs.Add(item.Value, keyValue);
                        }
                        //excelData.Add(valuePairs);

                        LanguageConf t = new LanguageConf();
                        ClassAnalyze<LanguageConf> _t = new ClassAnalyze<LanguageConf>(t);
                        t = _t.GetClass(t, valuePairs);
                        MySql.Insert(t, true);
                    }
                }
                else
                {
                    Debug.LogError("Error open " + realDataPath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            MySql.Close();
            AssetDatabase.Refresh();
        }
    }
}