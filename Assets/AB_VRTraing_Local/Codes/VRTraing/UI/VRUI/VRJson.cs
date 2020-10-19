//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Bindings;

//[NativeHeader("Modules/JSONSerialize/Public/JsonUtility.bindings.h")]
//public static class VRJson
//{
//    public static T FromJson<T>(string json) =>
//            ((T)FromJson(json, typeof(T)));

//    public static object FromJson(string json, System.Type type)
//    {
//        if (string.IsNullOrEmpty(json))
//        {
//            return null;
//        }
//        if (type == null)
//        {
//            throw new ArgumentNullException("type");
//        }
//        if (type.IsAbstract || type.IsSubclassOf(typeof(UnityEngine.Object)))
//        {
//            throw new ArgumentException("Cannot deserialize JSON to new instances of type '" + type.Name + ".'");
//        }
//        return FromJsonInternal(json, null, type);
//    }

//    [MethodImpl(MethodImplOptions.InternalCall), FreeFunction("FromJsonInternal", true, ThrowsException = true), ThreadSafe]
//    private static extern object FromJsonInternal(string json, object objectToOverwrite, System.Type type);
//    public static void FromJsonOverwrite(string json, object objectToOverwrite)
//    {
//        if (!string.IsNullOrEmpty(json))
//        {
//            if (objectToOverwrite == null)
//            {
//                throw new ArgumentNullException("objectToOverwrite");
//            }
//            if (((objectToOverwrite is UnityEngine.Object) && !(objectToOverwrite is MonoBehaviour)) && !(objectToOverwrite is ScriptableObject))
//            {
//                throw new ArgumentException("Engine types cannot be overwritten from JSON outside of the Editor.");
//            }
//            FromJsonInternal(json, objectToOverwrite, objectToOverwrite.GetType());
//        }
//    }

//    public static string ToJson(object obj) =>
//        ToJson(obj, false);

//    public static string ToJson(object obj, bool prettyPrint)
//    {
//        if (obj == null)
//        {
//            return "";
//        }
//        if (((obj is UnityEngine.Object) && !(obj is MonoBehaviour)) && !(obj is ScriptableObject))
//        {
//            throw new ArgumentException("JsonUtility.ToJson does not support engine types.");
//        }
//        return ToJsonInternal(obj, prettyPrint);
//    }

//    //[MethodImpl(MethodImplOptions.InternalCall), FreeFunction("ToJsonInternal", true), ThreadSafe]
//    //private static extern string ToJsonInternal([NotNull] object obj, bool prettyPrint);
//}
///*
//namespace UnityEngine
//{
//    using System;
//    using System.Runtime.CompilerServices;
//    using UnityEngine.Bindings;

//    [NativeHeader("Modules/JSONSerialize/Public/JsonUtility.bindings.h")]
//    public static class JsonUtility
//    {
//        public static T FromJson<T>(string json) =>
//            ((T)FromJson(json, typeof(T)));

//        public static object FromJson(string json, System.Type type)
//        {
//            if (string.IsNullOrEmpty(json))
//            {
//                return null;
//            }
//            if (type == null)
//            {
//                throw new ArgumentNullException("type");
//            }
//            if (type.IsAbstract || type.IsSubclassOf(typeof(UnityEngine.Object)))
//            {
//                throw new ArgumentException("Cannot deserialize JSON to new instances of type '" + type.Name + ".'");
//            }
//            return FromJsonInternal(json, null, type);
//        }

//        [MethodImpl(MethodImplOptions.InternalCall), FreeFunction("FromJsonInternal", true, ThrowsException = true), ThreadSafe]
//        private static extern object FromJsonInternal(string json, object objectToOverwrite, System.Type type);
//        public static void FromJsonOverwrite(string json, object objectToOverwrite)
//        {
//            if (!string.IsNullOrEmpty(json))
//            {
//                if (objectToOverwrite == null)
//                {
//                    throw new ArgumentNullException("objectToOverwrite");
//                }
//                if (((objectToOverwrite is UnityEngine.Object) && !(objectToOverwrite is MonoBehaviour)) && !(objectToOverwrite is ScriptableObject))
//                {
//                    throw new ArgumentException("Engine types cannot be overwritten from JSON outside of the Editor.");
//                }
//                FromJsonInternal(json, objectToOverwrite, objectToOverwrite.GetType());
//            }
//        }

//        public static string ToJson(object obj) =>
//            ToJson(obj, false);

//        public static string ToJson(object obj, bool prettyPrint)
//        {
//            if (obj == null)
//            {
//                return "";
//            }
//            if (((obj is UnityEngine.Object) && !(obj is MonoBehaviour)) && !(obj is ScriptableObject))
//            {
//                throw new ArgumentException("JsonUtility.ToJson does not support engine types.");
//            }
//            return ToJsonInternal(obj, prettyPrint);
//        }

//        [MethodImpl(MethodImplOptions.InternalCall), FreeFunction("ToJsonInternal", true), ThreadSafe]
//        private static extern string ToJsonInternal([NotNull] object obj, bool prettyPrint);
//    }
//}

//    */