
using System.Collections.Generic;
using UnityEngine;

namespace Karler.Lib.Data
{

    public static class MySql
    {

        public static string WorkPath;
        public static void Insert<T>(T t, bool isReplace = false, string key = "PriKey", string version = "")
        {
            XmlDataBase.Instance.Insert<T>(t, isReplace, key, version);
        }
        /// <summary>
        /// 读取本地文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isCreate"></param>
        /// <returns></returns>
        public static bool Open<T>(bool isCreate = true)
        {
            return XmlDataBase.Instance.LoadXmlByPath(typeof(T).Name, isCreate);
        }
        /// <summary>
        /// 读取网络上的XML文件
        /// 读取给定的XML文本
        /// </summary>
        /// <param name="file"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool Open(string file, string msg)
        {
            return XmlDataBase.Instance.LoadXmlByPath(file, msg);
        }

        public static void Close()
        {
            XmlDataBase.Instance.Close();
        }
        /// <summary>
        /// 选中给定的的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Select<T>(T t, string key = "PriKey")
        {
            T tmp = XmlDataBase.Instance.Select<T>(t, key);
            return tmp;
        }
        public static string SelectMain<T>(T t, string key = "Version")
        {
            return XmlDataBase.Instance.SelectMain<T>(t, key);
        }

        /// <summary>
        /// 获取表中集合
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> SelectAll<T>()
        {
            List<T> ts = XmlDataBase.Instance.SelectAll<T>();
            return ts;
        }
        /// <summary>
        /// 更新指定的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="prikey"></param>
        public static void Update<T>(T t, string prikey = "PriKey")
        {
            XmlDataBase.Instance.Update<T>(t, prikey);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="key"></param>
        public static void Delete<T>(T t, string key = "PriKey")
        {
            XmlDataBase.Instance.Delete<T>(t, key);
        }
        /// <summary>
        /// 删除所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void DeleteAll<T>()
        {
            XmlDataBase.Instance.DeleteAll<T>();
        }
    }
}