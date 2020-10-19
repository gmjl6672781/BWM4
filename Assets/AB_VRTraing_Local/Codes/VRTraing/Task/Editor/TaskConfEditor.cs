using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BeinLab.VRTraing.Conf;

//[CustomEditor(typeof(TaskConf))]
public class TaskConfEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    TaskConf taskConf = target as TaskConf;
    //    taskConf.taskName = EditorGUILayout.TextField("任务名", taskConf.taskName);
    //    taskConf.index = EditorGUILayout.IntField("Index", taskConf.index);

    //    LinkConf parent = EditorGUILayout.ObjectField("Parent", taskConf.parent, typeof(TaskConf), true) as LinkConf;
    //    if (parent != taskConf.parent)
    //    {
    //        if(parent)
    //            parent.child = taskConf;
    //        taskConf.parent = parent;
    //    }
    //    LinkConf child = EditorGUILayout.ObjectField("Child", taskConf.child, typeof(TaskConf), true) as LinkConf;
    //    if (child != taskConf.child)
    //    {
    //        if(child)
    //            child.parent = taskConf;
    //        taskConf.child = child;
    //    }

    //    LinkConf oldBrother = taskConf.oldBrother = EditorGUILayout.ObjectField("OldBrother", taskConf.oldBrother, typeof(TaskConf), true) as LinkConf;
    //    if(oldBrother != taskConf.oldBrother)
    //    {
    //        if(oldBrother)
    //            oldBrother.littleBrother = taskConf;
    //        taskConf.oldBrother = oldBrother;
    //    }
    //    LinkConf littleBrother = EditorGUILayout.ObjectField("LittleBrother", taskConf.littleBrother, typeof(TaskConf), true) as LinkConf;
    //    if (littleBrother != taskConf.littleBrother)
    //    {
    //        if(littleBrother)
    //            littleBrother.oldBrother = taskConf;
    //        taskConf.littleBrother = littleBrother;
    //    }


    //}
}
