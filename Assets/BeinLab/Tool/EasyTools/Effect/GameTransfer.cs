using BeinLab.Conf;
using System.Collections.Generic;
using UnityEngine;

public class GameTransfer : MonoBehaviour
{
    /// <summary>
    /// Hover动效
    /// </summary>
    public List<TransferConf> hoverEffectList;
    // Use this for initialization
    void Start()
    {
        if (hoverEffectList.Count > 0)
        {
            for (int i = 0; i < hoverEffectList.Count; i++)
            {
                hoverEffectList[i].CreateTweener(transform);
            }
        }
    }
}
