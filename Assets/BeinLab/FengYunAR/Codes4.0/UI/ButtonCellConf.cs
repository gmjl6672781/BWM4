using BeinLab.Util;
using UnityEngine;
using UnityEngine.UI;
using BeinLab.FengYun.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.FengYun.Modus
{
    public class ButtonCellConf : CellConf
    {
        public string btnName;
        public Color btnColor = Color.white;
        public override void SetData(GameCell cell,int index)
        {
            base.SetData(cell, index);
            Text btnLabel = UnityUtil.GetTypeChildByName<Text>(cell.gameObject, cellName);
            if (btnLabel)
            {
                btnLabel.text = btnName;
            }
            background.image.color = btnColor;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Conf/ButtonCellConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<ButtonCellConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ButtonCellConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(ButtonCellConf) + " is null");
                }
            }
        }
#endif
    }
}