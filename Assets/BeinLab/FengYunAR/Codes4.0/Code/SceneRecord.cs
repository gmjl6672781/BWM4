using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRecord : Singleton<SceneRecord>
{
    public static string lastSceneName = "";
    private string currentSceneName = "";

    // Start is called before the first frame update
    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void ChangeRecord()
    {
        lastSceneName = currentSceneName;
        currentSceneName = SceneManager.GetActiveScene().name;
        print(lastSceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
