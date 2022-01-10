using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiralSlider : MonoBehaviour
{
    public GameObject spiralRail;
    private Mesh spiralMesh;
    void Start()
    {
        spiralMesh = spiralRail.GetComponent<MeshFilter>().mesh;

        foreach (Vector3 vert in spiralMesh.vertices)
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
        return Vector3.zero;
    }
}