using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject followObject;
    public float followSpeed;
    public float minDist = .001f;
    public Vector3 offset = Vector3.zero;
    public Vector3 acceleration;
    public float drag = .9f;
    public float lerpSpeed;
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, followObject.transform.position + offset) > minDist)
        {
            /* Vector3 newPos = transform.position;

            newPos += acceleration;
            acceleration *= drag;

            acceleration += (transform.position - followObject.transform.position) * followSpeed; */

            transform.position = Vector3.Lerp(transform.position, followObject.transform.position + offset, lerpSpeed);
        }
    }
}
