using BeinLab.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;
/// <summary>
/// Xml的数据存储代码
/// </summary>
namespace Karler.Lib.Data
{
    public class XmlDataBase
    {
        private static XmlDataBase dataBase;
        private XmlDocBase xmlBase;

        public static XmlDataBase Instance
        {
            get
            {
                if (dataBase == null)
                {
                    dataBase = new XmlDataBase();
                }
                return dataBase;
            }
        }

        /// <summary>
        /// 通过传过来的路径加载xml文档
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool LoadXmlByPath(string path, bool isCreate = true)
        {
            if (xmlBase != null)
            {
                xmlBase.Close();
            }
            else
            {
                xmlBase = new XmlDocBase(path);
            }
            return xmlBase.Connect(isCreate);
        }
        /// <summary>
        /// 通过传过来的路径加载xml文档
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool LoadXmlByPath(string file, string msg)
        {
            if (xmlBase != null)
            {
                xmlBase.Close();
            }
            else
            {
                xmlBase = new XmlDocBase();
            }
            return xmlBase.ConnectStr(file, msg);
        }

        /// <summary>
        /// 保存xml，即操作完成后的关闭
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="path"></param>
        public void Close()
        {
            if (xmlBase != null)
                xmlBase.Close();
            xmlBase = null;
        }

        public void Insert<T>(T t, bool isReplace = false, string key = "PriKey", string version = "")
        {
            ClassAnalyze<T> ca = new ClassAnalyze<T>(t);
            xmlBase.Insert(ca.clazzName, ca.field, isReplace, ca.field[key], version);
        }


        public T Select<T>(T t, string key = "PriKey")
        {
            ClassAnalyze<T> ca = new ClassAnalyze<T>(t);
            t = ca.GetClass(t, xmlBase.Select(ca.clazzName, ca.field[key]));
            return t;
        }
        public string SelectMain<T>(T t, string version = "Version")
        {
            ClassAnalyze<T> ca = new ClassAnalyze<T>(t);
            return xmlBase.SelectMain(ca.clazzName, version);
        }

        /// <summary>
        /// 获取表中集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> SelectAll<T>()
        {
            List<T> ts = new List<T>();
            List<Dictionary<string, string>> ds = xmlBase.SelectAll(typeof(T).Name);
            if (ds != null)
            {
                for (int i = 0; i < ds.Count; i++)
                {
                    T t = (T)(Assembly.GetExecutingAssembly().CreateInstance(typeof(T).Namespace + "." + typeof(T).Name));
                    ClassAnalyze<T> _t = new ClassAnalyze<T>(t);
                    t = _t.GetClass(t, ds[i]);
                    ts.Add(t);
                }
            }
            return ts;
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="prikey"></param>
        public void Update<T>(T t, string prikey = "PriKey")
        {
            ClassAnalyze<T> ca = new ClassAnalyze<T>(t);
            xmlBase.Update(ca.clazzName, ca.field, ca.field[prikey]);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="key"></param>
        public void Delete<T>(T t, string key = "PriKey")
        {
            ClassAnalyze<T> ca = new ClassAnalyze<T>(t);
            xmlBase.Delete(ca.clazzName, ca.field[key]);
        }

        public void DeleteAll<T>()
        {
            xmlBase.DeleteAll(typeof(T).Name);
        }
    }


    /// <summary>
    /// 封装处理xml的类XmlDocBase
    /// 每个XmlDosBase可以创建任意个表，同时支持读取任意个表
    /// 每个表指的是根节点
    /// </summary>
    public class XmlDocBase
    {
        public XmlDocument xmlDoc;                //可操作的xml文档
        public string path;                       //路径，相对于Asset路径
        public const string rootName = "Config";  //根节点的名称
        private XmlNode xmlRootNode;              //根节点
        private string xmlName;                   //xml的名称

        public XmlDocBase() { }
        public bool ConnectStr(string file, string msg)
        {
            bool isConnect = true;
            xmlDoc = new XmlDocument();
            xmlName = file;
            bool isLoad = true;
            try
            {
                StringReader stream = new StringReader(msg);
                stream.Read();
                xmlDoc.Load(stream);
            }
            catch (Exception ex)
            {
                isLoad = false;
                isConnect = false;
                Debug.Log(ex.ToString());
            }
            if (isLoad && xmlDoc != null)
            {
                xmlRootNode = xmlDoc.SelectSingleNode(rootName);
            }
            return isConnect;
        }
        public bool Connect(bool isCreate = true)
        {
            bool isConnect = true;
            xmlDoc = new XmlDocument();
            if (File.Exists(path))
            {
                bool isLoad = true;
                try
                {
                    xmlDoc.Load(path);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                    if (isCreate)
                    {
                        CreateDB();
                        Save();
                        xmlDoc.Load(path);
                    }
                }
                if (isLoad && xmlDoc != null)
                {
                    xmlRootNode = xmlDoc.SelectSingleNode(rootName);
                }
            }
            else if (isCreate)
            {
                CreateDB();
            }
            else
            {
                isConnect = false;
            }

            
            FileInfo file = new FileInfo(path);

            xmlName = file.Name.Split('.')[0];
            return isConnect;
        }

        public XmlDocBase(string path)
        {
            if (!path.EndsWith(".xml"))
            {
                path += ".xml";
            }
            if (File.Exists(MySql.WorkPath))
            {
                path = MySql.WorkPath;
            }
            else if (Directory.Exists(MySql.WorkPath))
            {
                path = MySql.WorkPath + "/" + path;
            }
            this.path = path;
        }

        public void Save()
        {
            if (xmlDoc != null && !string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    xmlDoc.Save(path);
                }
            }
        }

        public void Close()
        {
            Save();
            xmlDoc = null;
            path = null;
            xmlRootNode = null;
            xmlName = null;
        }

        /// <summary>
        /// 创建一个子节点
        /// </summary>
        /// <param name="tableName"></param>
        public XmlNode CreateTable(string tableName)
        {
            XmlNode table = xmlDoc.CreateElement(tableName);
            xmlRootNode.AppendChild(table);
            //Save();
            return table;
        }

        public void CreateDB()
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlRootNode = xmlDoc.CreateElement(rootName);
            xmlDoc.AppendChild(xmlDeclaration);
            xmlDoc.AppendChild(xmlRootNode);
            //Save();
        }

        /// <summary>
        /// 尝试获取xml的一个表，如果此xml没有此表，则创建一个
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public XmlNode TryGetTable(string tableName)
        {
            if (xmlRootNode.SelectSingleNode(tableName) != null)
            {
                return xmlRootNode.SelectSingleNode(tableName);
            }
            Debug.LogWarning(xmlName + "/" + tableName + "不存在，自动创建");
            return CreateTable(tableName);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param> 表格名称
        /// <param name="field"></param>  键值对，成员变量
        /// <param name="isReplace"></param> 是否替换
        /// <param name="priKey"></param> 主键，默认是PriKey
        /// <param name="version"></param> 版本号
        public void Insert(string tableName, Dictionary<string, string> field, bool isReplace, string priKey, string version)
        {
            XmlNode table = TryGetTable(tableName);
            //if (!string.IsNullOrEmpty(version))
            //{
            //    XmlAttribute ver = xmlDoc.CreateAttribute(version);
            //    ver.Value = version;
            //    table.Attributes.Append(ver);
            //}
            try
            {
                if (table.SelectSingleNode(priKey) != null)
                {
                    //待插入数据已经存在 替换掉原来的数据
                    if (isReplace)
                    {
                        bool isUpdate = Update(tableName, field, priKey);
                        if (isUpdate)
                        {
                            if (!string.IsNullOrEmpty(version))
                            {
                                XmlAttribute ver = xmlDoc.CreateAttribute(version);
                                ver.Value = UnityUtil.GetTime();
                                table.Attributes.Append(ver);
                            }
                        }
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(tableName + "  " + priKey + ex.ToString());
            }
            XmlElement element = xmlDoc.CreateElement(priKey);
            foreach (string key in field.Keys)
            {
                XmlAttribute xa = xmlDoc.CreateAttribute(key);
                xa.Value = field[key];
                element.Attributes.Append(xa);
            }
            table.AppendChild(element);

            if (!string.IsNullOrEmpty(version))
            {
                XmlAttribute ver = xmlDoc.CreateAttribute(version);
                ver.Value = UnityUtil.GetTime();
                table.Attributes.Append(ver);
            }
            //Save();
        }

        /// <summary>
        /// 支持按任意对象进行查找
        /// 但是，这样做效率如何呢？全部加载然后在内存里查找不是更快？
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public string[] Select(string tableName, string key, string value)
        {
            return null;
        }

        /// <summary>
        /// 按主键查找
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        public Dictionary<string, string> Select(string tableName, string key)
        {
            XmlNode table = TryGetTable(tableName);
            XmlNode obj = table.SelectSingleNode(key);//按照key值确定元素对象

            if (obj == null)
            {
                return null;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();

            for (int i = 0; i < obj.Attributes.Count; i++)
            {
                dic.Add(obj.Attributes[i].LocalName, obj.Attributes[i].Value);
            }

            return dic;
        }

        /// <summary>
        /// 按主键查找
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        public string SelectMain(string tableName, string version)
        {
            XmlNode table = TryGetTable(tableName);
            if (table == null)
            {
                return null;
            }
            string main = null;
            for (int i = 0; i < table.Attributes.Count; i++)
            {
                if (table.Attributes[i].LocalName == version)
                {
                    main = table.Attributes[i].Value;
                    break;
                }
            }
            return main;
        }

        public List<Dictionary<string, string>> SelectAll(string tableName)
        {
            XmlNode table = TryGetTable(tableName);
            if (table.ChildNodes.Count == 0)
            {
                return null;
            }
            List<Dictionary<string, string>> elements = new List<Dictionary<string, string>>();
            for (int i = 0; i < table.ChildNodes.Count; i++)
            {//遍历表的子节点
                Dictionary<string, string> values = new Dictionary<string, string>();
                for (int j = 0; j < table.ChildNodes[i].Attributes.Count; j++)
                {
                    values.Add(table.ChildNodes[i].Attributes[j].LocalName, table.ChildNodes[i].Attributes[j].Value);
                }
                elements.Add(values);
            }
            return elements;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="key"></param>
        public bool Update(string tableName, Dictionary<string, string> field, string key)
        {
            XmlNode table = TryGetTable(tableName);
            XmlNode obj = table.SelectSingleNode(key);
            if (obj == null)
            {//待更新数据不存在，更新失败
                return false;
            }
            bool isUpdate = false;
            for (int i = 0; i < obj.Attributes.Count; i++)
            {
                if (obj.Attributes[obj.Attributes[i].LocalName].Value != field[obj.Attributes[i].LocalName])
                {
                    obj.Attributes[obj.Attributes[i].LocalName].Value = field[obj.Attributes[i].LocalName];
                    isUpdate = true;
                }
            }
            return isUpdate;
            //Save();
        }

        /// <summary>
        /// 删除 key
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        public void Delete(string tableName, string key)
        {
            XmlNode table = TryGetTable(tableName);
            XmlNode obj = table.SelectSingleNode(key);//按照key值确定元素对象
            if (obj == null)
                return;
            obj.RemoveAll();
            table.RemoveChild(obj);
            //Save();
        }

        public void DeleteAll(string tableName)
        {
            XmlNode table = TryGetTable(tableName);
            table.RemoveAll();
            //Save();
        }
    }

    /// <summary>
    /// 解析类的封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassAnalyze<T>
    {
        public string clazzName;
        public Dictionary<string, string> field;

        public ClassAnalyze(T t)
        {
            ReadClass(t);
        }

        /// <summary>
        /// 解析类，将成员变量拆分成键值对
        /// </summary>
        /// <param name="t"></param>
        public void ReadClass(T t)
        {
            if (t == null) {
                t = default(T);
            }
            if (t != null)
            {
                PropertyInfo[] property = t.GetType().GetProperties();
                clazzName = t.GetType().Name;
                if (field == null)
                    field = new Dictionary<string, string>();
                field.Clear();
                for (int i = 0; i < property.Length; i++)
                {
                    string value = "";
                    string key = property[i].Name;
                    if (t.GetType().GetProperty(key).PropertyType.IsArray)
                    {
                        value = ArrToString(t.GetType().GetProperty(key).GetValue(t, null));
                    }
                    else
                    {
                        value = Convert.ToString(t.GetType().GetProperty(key).GetValue(t, null));
                    }
                    field.Add(key, value);
                }
            }
            else
            {
                Debug.LogError(typeof(T).Name + " is null");
            }
        }

        /// <summary>
        /// 将成员变量的值转化为字符串
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private string ArrToString(object arr)
        {
            string values = "";
            bool istest = false;
            if (istest)
            {
                Array array = arr as Array;
                foreach (string s in array)
                {
                    values += s + "|";
                }
                return values;
            }
            if (arr is int[])
            {
                foreach (var value in arr as int[])
                {
                    values += value + "|";
                }
            }
            else if (arr is Single[])
            {
                foreach (var value in arr as Single[])
                {
                    values += value + "|";
                }
            }
            else if (arr is float[])
            {
                foreach (var value in arr as float[])
                {
                    values += value + "|";
                }
            }
            else if (arr is double[])
            {
                foreach (var value in arr as double[])
                {
                    values += value + "|";
                }
            }
            else if (arr is long[])
            {
                foreach (var value in arr as long[])
                {
                    values += value + "|";
                }
            }
            else if (arr is string[])
            {
                foreach (var value in arr as string[])
                {
                    values += value + "|";
                }
            }
            else if (arr is System.Object[])
            {
                foreach (var value in arr as System.Object[])
                {
                    values += value + "|";
                }
            }


            values = values.TrimEnd(new char[] { '|' });
            return values;
        }

        public T GetClass(T t, Dictionary<string, string> field)
        {
            SetField(t, field);
            return t;
        }

        public void SetField(T t, Dictionary<string, string> field)
        {
            this.field = field;
            if (t == null)
            {
                t = default(T);
            }
            if (t != null)
            {
                PropertyInfo[] property = t.GetType().GetProperties();
                for (int i = 0; i < property.Length; i++)
                {
                    if (property[i].PropertyType.IsArray&& field.ContainsKey(property[i].Name))
                    {
                        var tempArr = StringToArr(property[i].PropertyType, field[property[i].Name].Split(new char[] { '|' }));
                        property[i].SetValue(t, tempArr, null);
                    }
                    else if(field.ContainsKey(property[i].Name))
                    {
                        property[i].SetValue(t, Convert.ChangeType(field[property[i].Name], property[i].PropertyType), null);
                    }
                }
            }
        }

        private object StringToArr(object arr, string[] values)
        {
            if (arr.ToString() == "System.int[]")
            {
                arr = new int[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    (arr as int[])[i] = int.Parse(values[i]);
                }

                return (int[])arr;
            }
            else if (arr.ToString() == "System.Single[]")
            {
                arr = new Single[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    (arr as Single[])[i] = Single.Parse(values[i]);
                }

                return (Single[])arr;
            }
            else if (arr.ToString() == "System.float[]")
            {
                arr = new float[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    (arr as float[])[i] = float.Parse(values[i]);
                }

                return (float[])arr;
            }
            else if (arr.ToString() == "System.Double[]")
            {
                arr = new double[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    (arr as double[])[i] = double.Parse(values[i]);
                }

                return (double[])arr;
            }

            else if (arr is long[])
            {
                arr = new long[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    (arr as long[])[i] = long.Parse(values[i]);
                }
                return (long[])arr;
            }
            else if (arr.ToString() == "System.String[]")
            {

                arr = new string[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    (arr as string[])[i] = values[i];
                }
                return (string[])arr;
            }

            else if (arr.ToString() == "System.Object[]")
            {
                arr = new System.Object[values.Length];

                for (int i = 0; i < values.Length; i++)
                {

                    (arr as System.Object[])[i] = values[i];
                }
                return (System.Object[])arr;
            }

            return arr;
        }
    }
}

