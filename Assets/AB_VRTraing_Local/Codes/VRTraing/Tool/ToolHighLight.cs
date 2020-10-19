using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.VRTraing.Conf;
using BeinLab.Util;

namespace BeinLab.VRTraing
{
    /// <summary>
    /// 工具高亮功能
    /// </summary>
    public class ToolHighLight:MonoBehaviour
    {
        private ToolConf toolConf;
        /// <summary>
        /// 当前是否是高亮状态
        /// </summary>
        [HideInInspector]
        public bool isHighlight;
        /// <summary>
        /// 闪烁的计时器
        /// </summary>
        private Timer flashTimer;
        /// <summary>
        /// 高亮的部分的节点
        /// </summary>
        private GameObject rootHighlight;
        private ToolBasic toolBasic;

        /// <summary>
        /// 高亮的物体
        /// </summary>
        private List<MeshRenderer> meshRenderers;
        /// <summary>
        /// 保存原本的材质
        /// </summary>
        private Dictionary<MeshRenderer, Material> dicMeshMat;

        private void Awake()
        {
            toolBasic = GetComponent<ToolBasic>();
            if (toolBasic)
                ReadToolConf(toolBasic.toolConf);
        }

        /// <summary>
        /// 设置工具的高亮效果
        /// 判断是否需要CopyMesh
        /// </summary>
        /// <param name="toolConf"></param>
        public void ReadToolConf(ToolConf toolConf)
        {
            this.toolConf = toolConf;

            if (meshRenderers == null)
                meshRenderers = new List<MeshRenderer>();
            else
                meshRenderers.Clear();

            if (dicMeshMat == null)
                dicMeshMat = new Dictionary<MeshRenderer, Material>();
            else
                dicMeshMat.Clear();

            if (toolConf != null)
            {
                ///判断是否显示本体
                if (toolConf.isShowBody)
                {
                    if (rootHighlight == null)
                    {
                        rootHighlight = new GameObject("HighlightRoot");
                        UnityUtil.SetParent(transform, rootHighlight.transform);
                    }
                    List<MeshFilter> meshList = new List<MeshFilter>();
                    if (toolConf.isLightAll)
                    {
                        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
                        meshList.AddRange(meshFilters);
                    }
                    else
                    {
                        for (int i = 0; i < toolConf.lightObjNames.Count; i++)
                        {
                            MeshFilter mf = UnityUtil.GetTypeChildByName<MeshFilter>(gameObject, toolConf.lightObjNames[i]);
                            if (mf)
                            {
                                meshList.Add(mf);
                            }
                        }
                    }
                    for (int i = 0; i < meshList.Count; i++)
                    {
                        GameObject obj = new GameObject(meshList[i].name);
                        UnityUtil.SetParent(meshList[i].transform, obj.transform);
                        MeshFilter mf = obj.AddComponent<MeshFilter>();
                        mf.mesh = meshList[i].mesh;
                        obj.AddComponent<MeshRenderer>().material = toolConf.lightMat;
                        obj.transform.SetParent(rootHighlight.transform);
                    }
                    rootHighlight.SetActive(false);
                }
                else
                {
                    if (toolConf.isLightAll)
                    {
                        MeshRenderer[] mfs = GetComponentsInChildren<MeshRenderer>();
                        meshRenderers.AddRange(mfs);
                    }
                    else
                    {
                        for (int i = 0; i < toolConf.lightObjNames.Count; i++)
                        {
                            MeshRenderer meshRenderer = UnityUtil.GetTypeChildByName<MeshRenderer>(gameObject, toolConf.lightObjNames[i]);
                            if (meshRenderer)
                            {
                                meshRenderers.Add(meshRenderer);
                            }
                        }
                    }
                    for (int i = 0; i < meshRenderers.Count; i++)
                    {
                        dicMeshMat.Add(meshRenderers[i], meshRenderers[i].material);
                    }
                }
            }
        }

      
        /// <summary>
        /// 高亮物体
        /// </summary>
        private void ShowHighLight()
        {
            ///显示Body
            if (toolConf.isShowBody)
            {
                rootHighlight.SetActive(true);
               
            }
            else
            {
                for (int i = 0; i < meshRenderers.Count; i++)
                {
                    meshRenderers[i].material = toolConf.lightMat;
                }
            }
        }
        /// <summary>
        /// 隐藏高亮效果
        /// </summary>
        private void HideHighLight()
        {
            ///显示Body
            if (toolConf.isShowBody)
            {
                rootHighlight.SetActive(false);    
            }
            else
            {
                for (int i = 0; i < meshRenderers.Count; i++)
                {
                    if (dicMeshMat.ContainsKey(meshRenderers[i]))
                    {
                        meshRenderers[i].material = dicMeshMat[meshRenderers[i]];
                    }
                }

            }
        }
        /// <summary>
        /// 任务结束时，关闭高亮
        /// </summary>
        public void HideToolLight()
        {
            if (flashTimer != null)
            {
                TimerMgr.Instance.DestroyTimer(flashTimer);
                flashTimer = null;
            }

            if (isHighlight)
                HideHighLight();
            isHighlight = false;
        }

        /// <summary>
        /// 任务开始时高亮物体
        /// </summary>
        public void ShowToolLight()
        {
            HideToolLight();
            if (toolConf == null) return;
            isHighlight = true;
            if (!toolConf.isLightFlash)
            {
                ShowHighLight();
            }
            else
            {
                bool isHighlight = false;
                flashTimer = TimerMgr.Instance.CreateTimer(
                    () => {
                        isHighlight = !isHighlight;
                        if (isHighlight)
                            ShowHighLight();
                        else
                            HideHighLight();
                    }, toolConf.flashFrequency, -1);
            }
        }
    }
}

