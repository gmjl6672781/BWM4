using Karler.Lib.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BeinLab.Util
{
    public enum VectorDir
    {
        forwar = 1,
        right = 2,
        up = 3
    }
    public enum RenderingMode
    {
        Opaque = 1,
        Cutout = 2,
        Fade = 3,
        Transparent = 4
    }
    public static class UnityUtil
    {
        public static Color yellow;
        public static Color green;
        public static Color blue;
        public static Color gray;
        public static string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        private static List<Type> btnComponents;
        public static Dictionary<string, GameObject> publicGameObject;
        static UnityUtil()
        {
            Color y, g, b;
            ColorUtility.TryParseHtmlString("#EFEE75FF", out y);
            ColorUtility.TryParseHtmlString("#0FFF5FFF", out g);
            ColorUtility.TryParseHtmlString("#18CBDFFF", out b);
            ColorUtility.TryParseHtmlString("#19FFFFFF", out gray);
            yellow = y;
            green = g;
            blue = b;

            btnComponents = new List<System.Type>();
            btnComponents.Add(typeof(RectTransform));
            btnComponents.Add(typeof(Image));
            btnComponents.Add(typeof(Button));
        }

        public static GameObject GetOrCreatePublicObj(string objName)
        {
            if (publicGameObject == null)
            {
                publicGameObject = new Dictionary<string, GameObject>();
            }
            GameObject obj = null;
            bool isAdd = true;
            if (publicGameObject.ContainsKey(objName))
            {
                obj = publicGameObject[objName];
                isAdd = false;
            }
            if (!obj)
            {
                obj = new GameObject(objName);
                if (isAdd)
                {
                    publicGameObject.Add(objName, obj);
                }
                else
                {
                    publicGameObject[objName] = obj;
                }
            }
            return obj;
        }

        public static void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }

        internal static Vector3 GetFrontPos(Transform transform, object distance, bool v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// string 转Vector
        /// (0,0,0)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 ParseVector(string vector)
        {
            Vector3 v = Vector3.zero;
            string[] vectors = vector.Replace("(", "").Replace(")", "").Split(',');
            for (int i = 0; i < vectors.Length; i++)
            {
                if (i == 0)
                {
                    v.x = float.Parse(vectors[i]);
                }
                else if (i == 1)
                {
                    v.y = float.Parse(vectors[i]);
                }
                else if (i == 2)
                {
                    v.z = float.Parse(vectors[i]);
                }
            }
            return v;
        }
        /// <summary>
        /// 读取网页上的XML文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="isCreate"></param>
        /// <returns></returns>
        public static List<T> ReadWebXMLData<T>(string xmlNode, string msg)
        {
            List<T> list = null;
            try
            {
                MySql.WorkPath = xmlNode;
                bool isOpen = MySql.Open(xmlNode, msg);
                if (isOpen)
                {
                    list = MySql.SelectAll<T>();
                    MySql.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(xmlNode + "\n" + msg + "\n" + ex.ToString());
                MySql.Close();
                return list;
            }
            return list;
        }

        public static List<T> ReadXMLData<T>(string path, bool isCreate = true)
        {
            List<T> list = null;
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (Directory.Exists(path))
            {
                try
                {
                    MySql.WorkPath = path;
                    bool isOpen = MySql.Open<T>(isCreate);
                    if (isOpen)
                    {
                        list = MySql.SelectAll<T>();
                        MySql.Close();
                    }
                    else
                    {
                        Debug.Log("XML文件未打开" + path);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(path + "\n" + ex.ToString());
                    MySql.Close();
                }
            }
            return list;
        }
        public static void WriteXMLData<T>(string path, T t, bool isReplace = true)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    MySql.WorkPath = path;
                    Debug.Log("Open Error " + path);
                    bool isOpen = MySql.Open<T>(true);
                    if (isOpen)
                    {
                        MySql.Insert<T>(t, isReplace);
                    }
                    else
                    {
                        Debug.Log("Open Error " + path);
                    }
                    MySql.Close();
                }
                catch (Exception ex)
                {
                    MySql.Close();
                    Debug.LogError(ex.ToString());
                }
            }
        }
        public static void DelXMLData<T>(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    MySql.WorkPath = path;
                    bool isOpen = MySql.Open<T>(false);
                    if (isOpen)
                    {
                        MySql.DeleteAll<T>();
                    }
                    MySql.Close();
                }
                catch (Exception ex)
                {
                    MySql.Close();
                    Debug.LogError(ex.ToString());
                }
            }
        }

        public static void Log(System.Object msg)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// string 转Color 
        /// RGBA（1,1,1,1）
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ParseColor(string color)
        {
            Color c = Color.white;
            string[] colors = color.Replace("RGBA", "").Replace("(", "").Replace(")", "").Split(',');
            float r = float.Parse(colors[0]);
            c.r = ColorValueChange(r);
            float g = float.Parse(colors[1]);
            c.g = ColorValueChange(g);
            c.b = ColorValueChange(float.Parse(colors[2]));
            c.a = ColorValueChange(float.Parse(colors[3]));
            return c;
        }

        private static float ColorValueChange(float r)
        {
            return r > 1.01 ? (r / 255.0f) : r;
        }

        public static GameObject GetChildByName(GameObject parent, string name, bool recursive = true)
        {
            if (parent.name == name) return parent;
            int count = parent.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                GameObject obj = parent.transform.GetChild(i).gameObject;
                if (obj.name == name)
                    return obj;
            }
            if (recursive)
            {
                for (int i = 0; i < count; ++i)
                {
                    GameObject cur = parent.transform.GetChild(i).gameObject;
                    GameObject obj = GetChildByName(cur, name, true);
                    if (obj)
                        return obj;
                }
            }
            return null;
        }



        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMaterialVaue(GameObject obj, float value)
        {
            Color color = obj.GetComponent<MeshRenderer>().material.color;
            color.a = value;
            obj.GetComponent<MeshRenderer>().material.color = color;
        }

        /// <summary>
        /// 指向目标 或者背对目标
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="target"></param>
        /// <param name="isLookAt"></param>
        public static void LookAt(Transform trans, Transform target, bool isLookAt = true)
        {
            if (isLookAt)
            {
                trans.LookAt(target);
                return;
            }
            trans.rotation = Quaternion.LookRotation(trans.position - target.position);
        }
        /// <summary>
        /// 看向目标或者背向目标，并保持竖直方向垂直
        /// </summary>
        /// <param name="trans">要设置朝向的物体</param>
        /// <param name="target">参考的目标</param>
        /// <param name="isLookAt">是否正对</param>
        public static void LookAtV(Transform trans, Transform target, int isLookAt = 1)
        {
            Vector3 forward = (target.position - trans.position).normalized;
            forward.y = 0;
            if (forward != Vector3.zero)
            {
                trans.forward = forward * isLookAt;
            }
        }

        public static void LookForwardV(Transform trans, Transform target, int isLookAt = 1)
        {
            Vector3 forward = -target.forward;
            forward.y = 0;
            if (forward != Vector3.zero)
            {
                trans.forward = forward * isLookAt;
            }
        }

        /// <summary>
        /// 获取水平距离
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static float GetHorDistance(Vector3 pos1, Vector3 pos2)
        {
            pos1.y = pos2.y;
            return Vector3.Distance(pos1, pos2);
        }


        public static T GetTypeChildByName<T>(GameObject parent, string name, bool recursive = true)
        {
            GameObject obj = GetChildByName(parent, name, recursive);
            if (obj != null)
            {
                return obj.GetComponent<T>();
            }
            return default(T);
        }
        /// <summary>
        /// 从子对象获得指定的组件
        /// 支持隐藏的物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="t"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static T GetTypeChildByName<T>(GameObject parent, bool recursive = true)
        {
            int count = parent.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                GameObject obj = parent.transform.GetChild(i).gameObject;
                if (obj.GetComponent<T>() != null)
                    return obj.GetComponent<T>();
            }
            if (recursive)
            {
                for (int i = 0; i < count; ++i)
                {
                    GameObject cur = parent.transform.GetChild(i).gameObject;
                    T obj = GetTypeChildByName<T>(cur, true);
                    if (obj != null)
                        return obj;
                }
            }
            return default(T);
        }

        public static List<T> GetTypeChildsByName<T>(GameObject parent, List<T> list, bool recursive = true)
        {
            if (list == null) list = new List<T>();
            int count = parent.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                GameObject obj = parent.transform.GetChild(i).gameObject;
                if (obj.GetComponent<T>() != null)
                    list.Add(obj.GetComponent<T>());
            }
            if (recursive)
            {
                for (int i = 0; i < count; ++i)
                {
                    GameObject cur = parent.transform.GetChild(i).gameObject;
                    T obj = GetTypeChildByName<T>(cur, true);
                    if (obj != null)
                    {
                        list.Add(obj);
                    }
                }
            }
            return list;
        }

        //public static void CreateDownLoader(string url, Action<WWW> callback)
        //{
        //    GameObject obj = new GameObject("loader", new Type[] { typeof(DownLoadHelper) });
        //    DownLoadHelper helper = obj.GetComponent<DownLoadHelper>();
        //    helper.DownLoadByPath(url, callback);
        //}

        /*/// <summary>
        /// 将N个子物体沿着某个轴平均放置到此物体中心
        /// 通过弧度计算方向，计算出某一个物体的落点
        /// <param name="parent"></param>中心物体
        /// <param name="childs"></param>子物体
        /// <param name="axis"></param>轴向，物体绕着某个轴向
        /// <param name="distance"></param>距离物体的距离
        /// <param name="isWorld"></param>是否是本地坐标轴
        /// </summary>*/
        /// <summary>
        /// 将N个子物体沿着某个轴平均或者等角度放置到此物体中心
        /// 通过弧度计算方向，计算出某一个物体的落点
        /// </summary>
        /// <param name="parent">中心物体</param>
        /// <param name="childs">子物体</param>
        /// <param name="stype">轴向，物体绕着某个轴向</param>
        /// <param name="distance">距离物体的距离</param>
        /// <param name="radian">间隔角度  如果小于1代表均布 </param>
        /// <param name="isWorld">是否是本地坐标轴</param>

        public static void AroundBall(Transform parent, Transform[] childs,
            VectorDir stype, float distance, float radian = -1, bool isWorld = false)
        {
            if (childs == null || childs.Length < 1) return;
            if (radian < 1)
            {
                radian = 360.0f / childs.Length;//弧度
            }
            Vector3 angle = parent.eulerAngles;
            Vector3 axis = parent.up;
            Vector3 dir = parent.forward;
            if (isWorld)
            {
                axis = Vector3.up;
            }
            if (stype == VectorDir.forwar)
            {
                axis = parent.forward;
                if (isWorld)
                {
                    axis = Vector3.forward;
                }
            }
            else if (stype == VectorDir.right)
            {
                axis = parent.right;
                if (isWorld)
                {
                    axis = Vector3.right;
                }
            }

            for (int i = 0; i < childs.Length; i++)
            {
                parent.Rotate(axis, radian, Space.World);
                dir = parent.forward;
                if (stype == VectorDir.forwar)
                {
                    dir = parent.right;
                }
                else if (stype == VectorDir.right)
                {
                    dir = parent.up;
                }
                childs[i].position = parent.position + dir * distance;
            }
            parent.eulerAngles = angle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="childs"></param>
        /// <param name="axis"></param>
        /// <param name="startDir"></param>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        public static void AroundBallPos(Vector3 point, Transform[] childs, Vector3 axis, Vector3 startDir, float angle, float distance)
        {
            GameObject obj = new GameObject("Tmp");
            obj.transform.position = point;
            obj.transform.up = axis;
            obj.transform.eulerAngles = startDir;
            if (Mathf.Abs(angle) < 1)
            {
                angle = 360.0f / childs.Length;//弧度
            }
            for (int i = 0; i < childs.Length; i++)
            {
                obj.transform.Rotate(axis, angle, Space.Self);
                childs[i].position = obj.transform.position + obj.transform.up * distance;
                Debug.Log(childs[i].position);
            }
            GameObject.Destroy(obj);
        }
        public static void SetColor(GameObject go, Color color)
        {
            go.GetComponent<Renderer>().material.color = color;
        }

        public static void SetTrack(LineRenderer line, Vector3 axis, float distance, int count = 50)
        {
            //line.useWorldSpace = false;
            line.transform.eulerAngles = axis;
            float radian = 360.0f / count;
            line.positionCount = (count + 1);
            Vector3 rotAxis = line.transform.forward;
            Vector3 dir = line.transform.forward;
            distance /= line.transform.lossyScale.y;
            for (int i = 0; i <= count; i++)
            {
                line.transform.Rotate(rotAxis, radian, Space.World);
                dir = line.transform.up;
                Vector3 localPos = dir * distance;

                //localPos.z *= 0;
                //localPos.x *=1/ line.transform.lossyScale.x;
                //localPos.y *=1/ line.transform.lossyScale.y/100;
                line.SetPosition(i, localPos);
            }
            line.transform.rotation = Quaternion.identity;
            //line.useWorldSpace = false;
        }

        /// <summary>
        /// 获取一个相对于物体前面的位置
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="distance"></param>
        /// <param name="isFllow"></param>
        /// <returns></returns>
        public static Vector3 GetFrontPos(Transform trans, float distance, bool isFllow = true)
        {
            Vector3 forward = trans.forward.normalized;
            if (!isFllow)
                forward.y = 0;
            return trans.position + forward * distance;
        }

        /// <summary>
        /// 将other物体放到于trans相对的距离。
        /// trans是本体，other是相对的物体
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="other"></param>
        /// <param name="distance"></param>
        public static void SetRelativePos(Transform trans, Transform other, float distance)
        {
            Vector3 forward = (other.position - trans.position).normalized;

            other.position = trans.position + forward * distance;
        }

        public static void PlaceObjectsAroundSphere(List<Transform> objects, Vector3 origin, float radius)
        {
            Vector3[] myPoints = GetPointsOnSphere(objects.Count); //get a point for each object
                                                                   //for each object
            for (int i = 0; i < objects.Count; i++)
            {
                Vector3 point = myPoints[i]; //get the position for this object
                Vector3 pos = origin + point.normalized * radius; //adjust for the radius
                Vector3 toOrigin = pos - origin; // get the vector from the origin, to the point

                objects[i].position = pos; // place the object
                objects[i].LookAt(pos + toOrigin); // rotate the object to face outward from the sphere
            }
        }
        /// <summary>
        /// 获取N个围绕着球体的点
        /// </summary>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        //get points, evenly spaced around a sphere
        public static Vector3[] GetPointsOnSphere(int numPoints)
        {
            Vector3[] points = new Vector3[numPoints];

            float increment = Mathf.PI * (3 - Mathf.Sqrt(5));
            float offset = 2f / numPoints;

            for (int i = 0; i < numPoints; i++)
            {
                float y = i * offset - 1 + (offset / 2);
                float r = Mathf.Sqrt(1 - y * y);
                float phi = i * increment;
                points[i] = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);
            }

            return points;
        }
        public static List<int> RandomCommon(int min, int max, int n)
        {
            List<int> arrNum = new List<int>();
            int i = 0;
            while (i < n)
            {
                int rand = (int)UnityEngine.Random.Range(min, max);
                if (!arrNum.Contains(rand))
                {
                    arrNum.Add(rand);
                    i++;
                }
            }
            return arrNum;
        }
        public static string GetShortTime()
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string[] times = now.Split(' ');
            //string shortTime = times[1];
            return times[1];
            //string[] watchs = shortTime.Split(':');
            //string hour = watchs[0].PadLeft(2, '0');
            //string mint = watchs[1].PadLeft(2, '0');
            //shortTime = hour + ":" + mint;
            //return shortTime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetDataTime()
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd");
            string[] data = now.Split('-');
            string month = data[1];
            string day = data[2];
            return month + "月" + day + "日";
        }

        /// <summary>
        /// 获取当前14位数字时间
        /// 年月日_时分秒
        /// 例如20180524_151530
        /// </summary>
        /// <returns></returns>
        public static string GetTime()
        {

            return DateTime.Now.ToString("yyyyMMdd_HHmmss");
        }
        public static string GetWeek()
        {
            string weekstr = DateTime.Now.DayOfWeek.ToString();
            switch (weekstr)
            {
                case "Monday":
                    weekstr = "星期一";
                    break;
                case "Tuesday":
                    weekstr = "星期二";
                    break;
                case "Wednesday":
                    weekstr = "星期三";
                    break;
                case "Thursday":
                    weekstr = "星期四";
                    break;
                case "Friday":
                    weekstr = "星期五";
                    break;
                case "Saturday":
                    weekstr = "星期六";
                    break;
                case "Sunday":
                    weekstr = "星期日";
                    break;
            }
            return weekstr;
        }
        /// <summary>
        /// 创建一个按钮，同时完成赋值（ICON）操作
        /// </summary>
        /// <param name="sprite">按钮ICON</param>
        /// <param name="components">组件，UI Button组件</param>
        /// <param name="btnName"></param>
        /// <returns></returns>
        public static Button CreateButton(Transform parent, Sprite sprite, string btnName)
        {
            Button btn = new GameObject(btnName, btnComponents.ToArray()).GetComponent<Button>();
            btn.image.sprite = sprite;
            btn.transform.SetParent(parent);
            btn.transform.localPosition = Vector3.zero;
            btn.transform.localRotation = Quaternion.identity;
            btn.transform.localScale = Vector3.one;
            return btn;
        }

        public static void SetParent(Transform parent, Transform child)
        {
            child.SetParent(parent);
            if (child is RectTransform)
            {
                child.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            else
            {
                child.localPosition = Vector3.zero;
            }
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
        }
        public static void ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
        /// <summary>
        /// 安卓端刷新相册代码
        /// </summary>
        /// <param name="path"></param>
        public static void ScanPhoto(string[] path)
        {
            using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null))
                {
                    Conn.CallStatic("scanFile", playerActivity, path, null, null);
                }
            }
        }

        /// <summary>
        /// DES加解密
        /// Add by 成长的小猪（Jason.Song） on 2017/11/20
        /// http://blog.csdn.net/jasonsong2008
        /// 
        /// DES是对称性加密里面常见一种，全称为Data Encryption Standard，即数据加密标准，
        /// 是一种使用密钥加密的块算法。密钥长度是64位(bit)，超过位数密钥被忽略。
        /// 所谓对称性加密，加密和解密密钥相同。对称性加密一般会按照固定长度，把待加密字符串分成块。
        /// 不足一整块或者刚好最后有特殊填充字符。往往跨语言做DES加密解密，经常会出现问题。
        /// 往往是填充方式不对、或者编码不一致、或者选择加密解密模式(ECB,CBC,CTR,OFB,CFB,NCFB,NOFB)没有对应上造成。
        /// 常见的填充模式有： 'pkcs5','pkcs7','iso10126','ansix923','zero' 类型，
        /// 包括DES-ECB,DES-CBC,DES-CTR,DES-OFB,DES-CFB。 
        /// </summary>
        #region DES 加密

        /// <summary>
        /// 加密（Hex）
        /// Add by 成长的小猪（Jason.Song） on 2017/07/26
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥(8位任意字符)</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string EncryptToHexString(string encryptString, string encryptKey, Encoding encoding,
            CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.Zeros)
        {
            //为了安全级别更高，这个地方建议将密钥进行MD5后取8位，这里我就不加此方法啦
            //var keyBytes = encoding.GetBytes(Md5Helper.GetMd5HashString(encryptKey, encoding).Substring(0, 8));
            var keyBytes = encoding.GetBytes(encryptKey);
            var inputBytes = encoding.GetBytes(encryptString);
            var outputBytes = EncryptToDesBytes(inputBytes, keyBytes, cipher, padding);
            var sBuilder = new StringBuilder();
            foreach (var b in outputBytes)
            {
                sBuilder.Append(b.ToString("X2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 加密（Base64）
        /// Add by 成长的小猪（Jason.Song） on 2017/07/26
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥(8位任意字符)</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string EncryptToBase64String(string encryptString, string encryptKey, Encoding encoding,
            CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.Zeros)
        {
            //为了安全级别更高，这个地方建议将密钥进行MD5后取8位，这里我就不加此方法啦
            //var keyBytes = encoding.GetBytes(Md5Helper.GetMd5HashString(encryptKey, encoding).Substring(0, 8));
            var keyBytes = encoding.GetBytes(encryptKey);
            var inputBytes = encoding.GetBytes(encryptString);
            var outputBytes = EncryptToDesBytes(inputBytes, keyBytes, cipher, padding);
            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// 加密
        /// Add by 成长的小猪（Jason.Song） on 2017/07/26
        /// </summary>
        /// <param name="encryptBytes">待加密的字节数组</param>
        /// <param name="keyBytes">加密密钥字节数组</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static byte[] EncryptToDesBytes(byte[] encryptBytes, byte[] keyBytes,
            CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.Zeros)
        {
            var des = new DESCryptoServiceProvider
            {
                Key = keyBytes,
                IV = keyBytes,
                Mode = cipher,
                Padding = padding
            };
            var outputBytes = des.CreateEncryptor().TransformFinalBlock(encryptBytes, 0, encryptBytes.Length);
            return outputBytes;
        }

        #endregion

        #region DES 解密

        /// <summary>
        /// 解密（Hex）
        /// Add by 成长的小猪（Jason.Song） on 2017/07/26
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥(8位任意字符)</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string DecryptByHexString(string decryptString, string decryptKey, Encoding encoding,
            CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.Zeros)
        {
            //为了安全级别更高，这个地方建议将密钥进行MD5后取8位，这里我就不加此方法啦
            //var keyBytes = encoding.GetBytes(Md5Helper.GetMd5HashString(encryptKey, encoding).Substring(0, 8));
            var keyBytes = encoding.GetBytes(decryptKey);
            var inputBytes = new byte[decryptString.Length / 2];
            for (var i = 0; i < inputBytes.Length; i++)
            {
                inputBytes[i] = Convert.ToByte(decryptString.Substring(i * 2, 2), 16);
            }
            var outputBytes = DecryptByDesBytes(inputBytes, keyBytes, cipher, padding);
            return encoding.GetString(outputBytes).TrimEnd('\0');
        }

        /// <summary>
        /// 解密（Base64）
        /// Add by 成长的小猪（Jason.Song） on 2017/07/26
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥(8位任意字符)</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string DecryptByBase64String(string decryptString, string decryptKey, Encoding encoding,
            CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.Zeros)
        {
            //为了安全级别更高，这个地方建议将密钥进行MD5后取8位，这里我就不加此方法啦
            //var keyBytes = encoding.GetBytes(Md5Helper.GetMd5HashString(encryptKey, encoding).Substring(0, 8));
            var keyBytes = encoding.GetBytes(decryptKey);
            var inputBytes = Convert.FromBase64String(decryptString);
            var outputBytes = DecryptByDesBytes(inputBytes, keyBytes, cipher, padding);
            return encoding.GetString(outputBytes).TrimEnd('\0');
        }

        /// <summary>
        /// 解密
        /// Add by 成长的小猪（Jason.Song） on 2017/07/26
        /// </summary>
        /// <param name="decryptBytes">待解密的字节数组</param>
        /// <param name="keyBytes">解密密钥字节数组</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static byte[] DecryptByDesBytes(byte[] decryptBytes, byte[] keyBytes,
            CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.Zeros)
        {
            var des = new DESCryptoServiceProvider
            {
                Key = keyBytes,
                IV = keyBytes,
                Mode = cipher,
                Padding = padding
            };
            var outputBytes = des.CreateDecryptor().TransformFinalBlock(decryptBytes, 0, decryptBytes.Length);
            return outputBytes;
        }

        #endregion
        /// <summary>
        /// 将文本转换为可换行的文本
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string SplitToLine(string message)
        {
            string[] str = message.ToString().Split('|');
            string msg = str[0];
            if (str.Length > 1)
            {
                for (int i = 1; i < str.Length; i++)
                {
                    msg += "\n" + str[i];
                }
            }
            return msg.ToString();
        }
        /// <summary>
        /// 自适应全屏
        /// </summary>
        /// <param name="size"></param>
        /// <param name="isHor">是否适应横向分辨率</param>
        /// <returns></returns>
        public static Vector2 AspectScreen(Vector2 size)
        {
            Vector2 preSize = size;
            bool isHor = size.x / Screen.width < size.y / Screen.height;
            if (isHor)
            {
                preSize.x = Screen.width;
                preSize.y = size.y * (Screen.width / size.x);
            }
            else
            {
                preSize.y = Screen.height;
                preSize.x = size.x * (Screen.height / size.y);
            }
            return preSize;
        }

        /// <summary>
        /// 自适应全屏
        /// </summary>
        /// <param name="size"></param>
        /// <param name="isHor">是否适应横向分辨率</param>
        /// <returns></returns>
        public static float AspectZoomScreen(Vector2 size)
        {
            float preSize = 1;
            bool isHor = size.x / Screen.width > size.y / Screen.height;
            if (isHor)
            {
                preSize = Screen.width / size.x;
            }
            else
            {
                preSize = Screen.height / size.y;
            }
            return preSize;
        }

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        public static Vector2 WorldToUI(Camera camera, Vector3 target, Canvas canvas)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(),
                camera.WorldToScreenPoint(target), canvas.worldCamera, out pos);
            return pos;
        }

        /// <summary>
        /// 加载服务器XML
        /// </summary>
        /// <param name="serverURL"></param>
        /// <param name="OnReadData"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public static IEnumerator ReqDataByWWW(string serverURL, Action<string, WWW> OnReadData, Action<float> process = null)
        {
            serverURL = serverURL.Replace("\\", "/");
            string fileName = serverURL.Substring(serverURL.LastIndexOf("/") + 1);
            fileName = fileName.Split('.')[0];

            WWW www = new WWW(serverURL);
            while (!www.isDone)
            {
                if (process != null)
                {
                    process(www.progress);
                }
                yield return new WaitForFixedUpdate();
            }
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (OnReadData != null)
                {
                    OnReadData(fileName, www);
                }
            }
            else
            {
                Debug.LogError(serverURL + "---" + www.error);
                if (process != null)
                {
                    process(1);
                }
                if (OnReadData != null)
                {
                    OnReadData(fileName, null);
                }
            }
            www.Dispose();
            if (process != null)
            {
                process(1);
            }
        }
        /// <summary>
        /// 申请WWW资源，
        /// </summary>
        /// <param name="serverURL">资源地址</param>
        /// <param name="OnReadData">申请回调</param>
        /// <param name="process">申请进度</param>
        /// <returns></returns>
        public static IEnumerator ReqDataByWWW(string serverURL, Action<WWW> OnReadData, Action<float> process = null)
        {
            serverURL = serverURL.Replace("\\", "/");
            WWW www = new WWW(serverURL);
            while (!www.isDone)
            {
                if (process != null)
                {
                    process(www.progress);
                }
                yield return new WaitForFixedUpdate();
            }
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (OnReadData != null)
                {
                    OnReadData(www);
                }
            }
            else
            {
                Debug.LogError(www.error);
                if (process != null)
                {
                    process(1);
                }
                if (OnReadData != null)
                {
                    OnReadData(null);
                }
            }
            www.Dispose();
            if (process != null)
            {
                process(1);
            }
        }
        /// <summary>
        /// 保存数据到本地
        /// </summary>
        /// <param name="bytes">www加载的数据</param>
        /// <param name="path">相对路径</param>
        public static void SaveFileToLocal(byte[] bytes, string path)
        {
            try
            {
                string filename = path;
                string dir = filename.Substring(0, filename.LastIndexOf("/"));
                ///如果保存的路径不存在，则创路径
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                ///如果文件存在，则直接覆盖
                FileStream fs = new FileStream(filename, FileMode.Create);
                // Create the writer for data.
                BinaryWriter w = new BinaryWriter(fs);
                // Write data to Test.data.
                w.Write(bytes);
                fs.Flush();
                w.Flush();
                w.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
        /// <summary>
        /// 遍历指定文件夹，并获取文件名以及对应的MD5值
        /// </summary>
        /// <param name="dir">绝对路径，完整的路径</param>
        /// <param name="assetPath">相对路径节点，绝对路径的前半段</param>
        /// <param name="map">图，文件名，以及对应的MD5码</param>
        /// <param name="isDirChild">是否检索子路径下的MD5码值</param>
        /// <returns></returns>
        public static void GetFileMD5FormDirectory(string dir, string assetPath, ref Dictionary<string, string> map, bool isDirChild = true)
        {
            if (Directory.Exists(dir))
            {
                DirectoryInfo d = new DirectoryInfo(dir);
                FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
                foreach (FileSystemInfo fsinfo in fsinfos)
                {
                    if (fsinfo is DirectoryInfo)     //判断是否为文件夹
                    {
                        if (isDirChild)
                        {
                            GetFileMD5FormDirectory(fsinfo.FullName, assetPath, ref map, isDirChild);//递归调用
                        }
                    }
                    else
                    {
                        string full = fsinfo.FullName;
                        full = full.Replace("\\", "/");
                        full = full.Replace(assetPath + "/", "");
                        string MD5 = GetMD5HashFromFile(fsinfo.FullName);
                        if (map == null)
                        {
                            map = new Dictionary<string, string>();
                        }
                        map.Add(full, MD5);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listConf"></param>
        public static long GetDataSize(List<AssetsVersionConf> listConf)
        {
            long size = 0;
            for (int i = 0; i < listConf.Count; i++)
            {
                size += listConf[i].Size;
            }
            return size;
        }
        public static float GetDataSize(long data)
        {
            return Mathf.Ceil(data / (1024f * 1024f));
        }
        public static bool IsInSceneCenter(Vector2 point, float bianjie = 0.2f)
        {
            bool isCanTouch = true;
            Vector2 scren = new Vector2(Screen.width, Screen.height);
            if (point.x < scren.x * bianjie || point.x > scren.x * (1 - bianjie) ||
                point.y < scren.y * bianjie || point.y > scren.y * (1 - bianjie))
            {
                isCanTouch = false;
            }
            return isCanTouch;
        }

        /// <summary>
        /// 删除多于的资源
        /// </summary>
        /// <param name="path"></param>
        public static void DelDataByPath(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandPosition(Vector3 min, Vector3 max)
        {
            Vector3 pos = Vector3.zero;
            pos.x = GetRandValue(min.x, max.x);
            pos.y = GetRandValue(min.y, max.y);
            pos.z = GetRandValue(min.z, max.z);
            return pos;
        }

        public static float GetRandValue(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
        public static float GetRandValue(Vector2 range)
        {
            return UnityEngine.Random.Range(range.x, range.y);
        }

        /// <summary>
        /// 获取当前物体和目标点的水平角度值
        /// 同时判断是左边还是右边
        /// </summary>
        /// <returns></returns>
        public static int IsPositionAtRight(Transform body, Vector3 point, out float distance)
        {
            Vector3 hPoint = point;
            hPoint.y = body.position.y;
            var forward = body.TransformDirection(Vector3.right);
            var toOther = hPoint - body.position;
            distance = Vector3.Distance(hPoint, body.position);
            float dot = Vector3.Dot(forward.normalized, toOther.normalized);
            if (Mathf.Abs(dot) < 0.003)
            {
                return 0;
            }
            else if (dot > 0)
            {
                return 1;
            }
            return -1;
        }
        /// <summary>
        /// 判断一个点是否在物体的前方
        /// </summary>
        /// <returns></returns>
        public static int IsPositionAtForward(Transform body, Vector3 point, out float distance)
        {
            Vector3 hPoint = point;
            hPoint.y = body.position.y;
            var forward = body.TransformDirection(Vector3.forward);
            var toOther = hPoint - body.position;
            distance = Vector3.Distance(hPoint, body.position);
            float dot = Vector3.Dot(forward.normalized, toOther.normalized);
            if (dot >= 0)
            {
                return 1;
            }
            return -1;
        }
        public static float HAngle(Transform body, Vector3 point)
        {
            Vector3 hPoint = point;
            hPoint.y = body.position.y;
            Vector3 dir = body.forward;
            dir.y = 0;
            return Vector3.Angle(hPoint, dir);
        }
        /// <summary>
        /// Find the closest raycast hit in the list of RaycastResults that is also included in the LayerMask list.  
        /// </summary>
        /// <param name="candidates">List of RaycastResults from a Unity UI raycast</param>
        /// <param name="layerMaskList">List of layers to support</param>
        /// <returns>RaycastResult if hit, or an empty RaycastResult if nothing was hit</returns>
        public static RaycastResult FindClosestRaycastHitInLayermasks(List<RaycastResult> candidates, LayerMask[] layerMaskList)
        {
            int combinedLayerMask = 0;
            for (int i = 0; i < layerMaskList.Length; i++)
            {
                combinedLayerMask = combinedLayerMask | layerMaskList[i].value;
            }

            RaycastResult? minHit = null;
            for (var i = 0; i < candidates.Count; ++i)
            {
                if (candidates[i].gameObject == null || !IsLayerInLayerMask(candidates[i].gameObject.layer, combinedLayerMask))
                {
                    continue;
                }
                if (minHit == null || candidates[i].distance < minHit.Value.distance)
                {
                    minHit = candidates[i];
                }
            }

            return minHit ?? new RaycastResult();
        }

        /// <summary>
        /// Look through the layerMaskList and find the index in that list for which the supplied layer is part of
        /// </summary>
        /// <param name="layer">Layer to search for</param>
        /// <param name="layerMaskList">List of LayerMasks to search</param>
        /// <returns>LayerMaskList index, or -1 for not found</returns>
        public static int FindLayerListIndex(int layer, LayerMask[] layerMaskList)
        {
            for (int i = 0; i < layerMaskList.Length; i++)
            {
                if (IsLayerInLayerMask(layer, layerMaskList[i].value))
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool IsLayerInLayerMask(int layer, int layerMask)
        {
            return ((1 << layer) & layerMask) != 0;
        }

        public static RaycastHit? PrioritizeHits(RaycastHit[] hits, LayerMask[] RaycastLayerMasks)
        {
            if (hits.Length == 0)
            {
                return null;
            }

            // Return the minimum distance hit within the first layer that has hits.
            // In other words, sort all hit objects first by layerMask, then by distance.
            for (int layerMaskIdx = 0; layerMaskIdx < RaycastLayerMasks.Length; layerMaskIdx++)
            {
                RaycastHit? minHit = null;

                for (int hitIdx = 0; hitIdx < hits.Length; hitIdx++)
                {
                    RaycastHit hit = hits[hitIdx];
                    if (IsLayerInLayerMask(hit.transform.gameObject.layer, RaycastLayerMasks[layerMaskIdx]) &&
                        (minHit == null || hit.distance < minHit.Value.distance))
                    {
                        minHit = hit;
                    }
                }

                if (minHit != null)
                {
                    return minHit;
                }
            }

            return null;
        }


#if UNITY_EDITOR
        public static string TryGetName<T>(string path, string suffix = ".asset")
        {
            int index = 0;
            string confName = "";
            UnityEngine.Object obj = null;
            do
            {
                confName = path + "/" + typeof(T).Name + "_" + index + suffix;
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath(confName, typeof(T));
                index++;
            } while (obj);
            return confName;
        }
#endif
        /// <summary>
        /// application/json发送数据请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IEnumerator HttpJsonPost(string url, string jsonParam, Action<string> OnReqData, bool isPost = true)
        {
            // print(url + "\n" + jsonParam);
            byte[] body = Encoding.UTF8.GetBytes(jsonParam);
            UnityWebRequest unityWeb = new UnityWebRequest(url, isPost ? "POST" : "GET");
            unityWeb.uploadHandler = new UploadHandlerRaw(body);
            //unityWeb.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            unityWeb.SetRequestHeader("Content-Type", "application/json");
            unityWeb.downloadHandler = new DownloadHandlerBuffer();
            yield return unityWeb.SendWebRequest();
            string result = null;
            if (unityWeb.isDone)
            {
                result = unityWeb.downloadHandler.text;
            }
            else
            {
                Debug.Log("Http 请求失败");
                Debug.Log(unityWeb.error);
            }
            OnReqData?.Invoke(result);
        }
    }
}