

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
/// <summary>
/// 1，先找到所有引用目标材质的对象，并存储在列表中
///2，根据当前所选的下标，将当前的材质切换到指定的材质中去
///
/// </summary>
[ExecuteInEditMode]
#endif
public class EditorCarColor : MonoBehaviour
{
    public Material targetColor;
    /// <summary>
    /// 请把默认材质放到第一位
    /// </summary>
    public Material[] materialList;
    [Range(0, 10)]
    public int selectColor;
    private List<Renderer> renderList;

    private int lastSelect = -1;

    public bool isFollow;
    public Material followMateria;
    // Use this for initialization
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "0") {
            enabled = false;
        }
#if !UNITY_EDITOR
        enabled = false;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetColor) return;
        selectColor = Mathf.Clamp(selectColor, 0, materialList.Length - 1);
        if (isFollow)
        {
            if (!followMateria) return;
            targetColor.shader = followMateria.shader;
            targetColor.CopyPropertiesFromMaterial(followMateria);
        }
        else if (materialList != null && materialList.Length > 0)
        {

            ///切换材质时
            if (lastSelect != selectColor && selectColor >= 0 && selectColor < materialList.Length)
            {
                lastSelect = selectColor;
                if (materialList.Length > 0)
                {
                    targetColor.shader = materialList[lastSelect].shader;
                    targetColor.CopyPropertiesFromMaterial(materialList[lastSelect]);
                }
            }
            ///更新材质
            else
            {
                materialList[lastSelect].CopyPropertiesFromMaterial(targetColor);
            }
        }
    }
}
