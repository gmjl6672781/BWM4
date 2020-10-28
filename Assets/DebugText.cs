using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public static Text debugText;
    // Start is called before the first frame update
    void Awake()
    {
        debugText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
