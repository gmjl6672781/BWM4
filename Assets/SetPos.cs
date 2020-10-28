using BeinLab.VRTraing.Mgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPos : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (VRHandChooseLanguage.Instance.Target != null)
        {
            transform.position = VRHandChooseLanguage.Instance.Target.transform.position;
            transform.rotation = VRHandChooseLanguage.Instance.Target.transform.rotation;
        }
    }
}
