using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.VRTraing.Conf;

public class TestLoad : MonoBehaviour
{
    public string path;
    // Start is called before the first frame update
    void Start()
    {
        var obj= GameAssetLoader.Instance.LoadObject(path);
   
        print(path);

        if (obj)
        {
            Instantiate(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
