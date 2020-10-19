using UnityEngine;

public class DelayActive : MonoBehaviour
{
    public float delayTime = 5;
    public GameObject delayTarget;
    // Use this for initialization
    void Awake()
    {
        if (delayTarget.activeSelf)
        {
            delayTarget.SetActive(false);
        }
        Invoke("ActiveTarget", delayTime);
    }
    private void ActiveTarget()
    {
        delayTarget.SetActive(true);
    }
}
