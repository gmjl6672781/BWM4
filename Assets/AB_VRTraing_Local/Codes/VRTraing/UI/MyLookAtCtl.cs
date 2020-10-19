using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLookAtCtl : MonoBehaviour
{

    public GameObject _targetObj;
    public bool isLockY;

    private Vector3 vec;

    // Start is called before the first frame update
    void Start()
    {
        vec = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetObj!=null)
        {
            if (!isLockY)
            {
                transform.LookAt(_targetObj.transform);
            }
            else
            {
                vec.Set(_targetObj.transform.position.x,transform.position.y, _targetObj.transform.position.z);
                transform.LookAt(vec);
            }
            
        }
    }
}
