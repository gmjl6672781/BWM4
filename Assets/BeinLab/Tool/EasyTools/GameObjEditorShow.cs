using UnityEngine;

public class GameObjEditorShow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }
}
