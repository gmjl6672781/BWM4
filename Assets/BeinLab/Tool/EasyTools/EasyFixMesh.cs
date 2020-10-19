using UnityEngine;

public class EasyFixMesh : MonoBehaviour
{
    public Transform fixTarget;
    public Transform selfBody;
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter[] selfMesh = selfBody.GetComponentsInChildren<MeshFilter>();
        MeshFilter[] targetMesh = fixTarget.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < selfMesh.Length; i++)
        {
            if (selfMesh[i].gameObject.name == targetMesh[i].gameObject.name)
            {
                if (selfMesh[i].sharedMesh == null)
                {
                    selfMesh[i].sharedMesh = targetMesh[i].sharedMesh;
                    print(selfMesh[i].gameObject.name + " is fixed");
                }
            }
            else
            {
                print(selfMesh[i].gameObject.name);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
