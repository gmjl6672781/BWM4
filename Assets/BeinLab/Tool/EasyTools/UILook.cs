using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILook : MonoBehaviour
{
    private GameObject target=null;
    void Update()
    {
        if (target)
        {
            Vector3 forward = transform.position - target.transform.position;
            forward.y = 0;
            transform.forward = forward;
        }
    }
}
