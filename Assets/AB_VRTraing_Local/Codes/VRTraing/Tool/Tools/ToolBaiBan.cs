using System.Collections;
using System.Collections.Generic;
using BeinLab.VRTraing.Conf;
using UnityEngine;
using BeinLab.VRTraing.Controller;
using Valve.VR.InteractionSystem;
using System;

namespace BeinLab.VRTraing
{
    public class ToolBaiBan : ToolBasic
    {
        public BaiBanConf baiBanConf;
        private MeshRenderer meshRenderer;

        protected override void Awake()
        {
            base.Awake();
            meshRenderer = GetComponentInChildren<MeshRenderer>(); 
        }
        protected override void Start()
        {
            base.Start();
            LanguagePackConf curLanguage = LanguageMgr.Instance.CurLanguage;

            if(curLanguage!=null)
            {
                if (curLanguage.PriKey == BaiBanConf.chinesePriKey)
                {
                    SetMap(baiBanConf.chineseTextureNull);
                }
                else
                    SetMap(baiBanConf.englishTextureNull);
            }
        }

        public void SetMap(Texture texture)
        {
            if (meshRenderer)
            {
                meshRenderer.material.mainTexture = texture;
            }
               
        }

    }
}


