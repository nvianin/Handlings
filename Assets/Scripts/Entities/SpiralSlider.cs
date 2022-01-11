using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiralSlider : MonoBehaviour
{
    public GameObject spiralRail;
    Mesh spiralMesh;
    Vector3[] vertices;
    void Start()
    {
        spiralMesh = spiralRail.GetComponent<MeshFilter>().mesh;
        vertices = spiralMesh.vertices;

        foreach (Vector3 vert in vertices)
        {
            print(vert);
        }
    }
    void Update()
    {

    }

    Vector3 GetPositionAt(float t)
    {
        t = Mathf.Clamp01(t);
        float t_tenth = Mathf.Floor(t*10)/10;


        return Vector3.zero;
    }
}