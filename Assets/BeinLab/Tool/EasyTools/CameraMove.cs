using BeinLab.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //方向灵敏度
    public float sensitivityX = 10F;
    public float sensitivityY = 10F;

    //上下最大视角(Y视角)
    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;
    public bool IsNeedPress = false;
    // Use this for initialization
    void Start()
    {
#if !UNITY_EDITOR
        enabled = false;
#endif
        //if (AssetBundleLoader.Instance)
        //{
        //    if (AssetBundleLoader.Instance.Model == BeinLab.RS5.Mgr.GameModel.XRConf_VR)
        //    {
        //        enabled = false;
        //    }
        //}
        //print(DateTime.Now.ToString("MMddhhmm"));
    }
    //private void FixedUpdate()
    //{
        
    //}
    private void FixedUpdate()
    {
        if (Input.GetButton("Horizontal"))
        {
            transform.Translate(transform.right * Input.GetAxis("Horizontal") * Time.fixedDeltaTime, Space.World);
        }
        if (Input.GetButton("Vertical"))
        {
            transform.Translate(transform.forward * Input.GetAxis("Vertical") * Time.fixedDeltaTime, Space.World);
        }
        if (!IsNeedPress || Input.GetMouseButton(1))
        {
            //根据鼠标移动的快慢(增量), 获得相机左右旋转的角度(处理X)
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            //根据鼠标移动的快慢(增量), 获得相机上下旋转的角度(处理Y)
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            //角度限制. rotationY小于min,返回min. 大于max,返回max. 否则返回value 
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            //总体设置一下相机角度
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
    }
    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
