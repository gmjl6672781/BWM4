using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class PlayrController : MonoBehaviour
{
    public Player playerInScene2;
    public Player playerInScene1;

    private Player player2;
    private Player player1;

    public static bool canShow;

    // Start is called before the first frame update

    private void Awake()
    {
        canShow = false;
        if (player2 != null)
            Destroy(player2);
        player1 = Instantiate(playerInScene1, null);
        Debug.Log(player1.name);
        EnterTaskScene.ChangScene += ChangePlayer;
    }

    void ChangePlayer()
    {
        Debug.Log("ChangePlayer");
        StartCoroutine(WaitCreatePlayer());
    }

    IEnumerator WaitCreatePlayer()
    {
        Destroy(player1);
        player2 = Instantiate(playerInScene2, null);
        while (player1 != null || player2 == null)
        {
            yield return new WaitForFixedUpdate();
        }
        canShow = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (SceneManager.GetActiveScene().buildIndex != sceneName)
        //{
        //    if (sceneName == 0)
        //    {
        //        Destroy(player1);
        //        player2 = Instantiate(playerInScene2, null);
        //    }
        //    else
        //    {
        //        Destroy(player2);
        //        player2 = Instantiate(playerInScene1, null);
        //    }
        //}
    }
}
