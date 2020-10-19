using UnityEngine;
using System.Collections;

public class MeshUtil : MonoBehaviour
{
    public float length = 1;
    public Material material;
    public int count = 100;
    // Use this for initialization
    void Start()
    {
        //CreateSixObj(length);
        BetterCreateMul(count, 1);
        //CreateMul(6, 1);
    }

    /// <summary>
    /// 创建多边形
    /// </summary>
    /// <param name="len"></param>
    /// <param name="weith"></param>
    void CreateMul(int len, float weith)
    {
        GameObject obj = new GameObject("six");
        float angle = 360.0f / len;
        Vector3 dir = Vector3.zero;
        Vector3 forward = Vector3.forward;
        Vector3[] vertices = new Vector3[len + 1];
        vertices[0] = Vector3.zero;
        for (int i = 1; i < vertices.Length; i++)
        {
            var rot = Quaternion.Euler(0, angle * (i - 1), 0);
            print(rot.eulerAngles);
            dir = rot * forward;
            vertices[i] = dir * weith;
            //print(Vector3.Angle(Vector3.forward,dir));
        }
        int[] triangles = new int[(vertices.Length - 1) * 3];
        for (int i = 0; i < triangles.Length; i++)
        {
            if (i % 3 == 0)
            {
                triangles[i] = 0;
            }
            else
            {
                int z = i / 3;
                int y = i % 3;
                int f = z + y;
                if (f > vertices.Length - 1)
                {
                    f = 1;
                }
                triangles[i] = f;
            }
        }
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        obj.AddComponent<MeshRenderer>().material = material;
        mf.sharedMesh = mesh;


    }

    /// <summary>
    /// 创建多边形
    /// </summary>
    /// <param name="len"></param>
    /// <param name="weith"></param>
    void BetterCreateMul(int len, float weith)
    {
        GameObject obj = new GameObject("six");
        float angle = 360.0f / len;
        Vector3 dir = Vector3.zero;
        Vector3 forward = Vector3.forward;
        Vector3[] vertices = new Vector3[len];
        for (int i = 0; i < vertices.Length; i++)
        {
            var rot = Quaternion.Euler(0, angle * i, 0);
            dir = rot * forward;
            vertices[i] = dir * weith;
        }
        int[] triangles = new int[(vertices.Length - 2) * 3];
        for (int i = 0; i < triangles.Length; i++)
        {
            if (i % 3 == 0)
            {
                triangles[i] = vertices.Length - 1;

            }
            else
            {
                int z = (i - 1) / 3;
                int y = (i - 1) % 3;
                int f = z + y;
                triangles[i] = f;
            }
        }
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        obj.AddComponent<MeshRenderer>().material = material;
        mf.sharedMesh = mesh;
    }


}