using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoom : MonoBehaviour
{
    public float minDist = .2f;
    public float maxDist = 5;
    public float targetDistance = 2;
    public float speed = 1;
    public float lerp = .1f;

    public int collisionCount = 0;
    private SphereCollider sphereCollider;
    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        targetDistance = (minDist + maxDist) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(Vector3.Distance(transform.position, transform.parent.position) - targetDistance) > .1)
        {
            Vector3 targetPos = transform.localPosition;
            targetPos.z = Mathf.Lerp(transform.localPosition.z, -targetDistance, lerp);
            transform.localPosition = targetPos;
        }
        if (collisionCount < 1)
        {
            targetDistance = Mathf.Clamp(targetDistance + .05f * speed, minDist, maxDist);
        }
        /* Vector3 newPos = transform.localPosition;
        newPos.z -= .01f;
        transform.localPosition = newPos; */
    }

    void OnCollisionStay(Collision collision)
    {
        print(collision);
        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white, 10000);
        }

        float cameraToOrbit = Vector3.Distance(transform.position, transform.parent.position);
        float contactToOrbit = Vector3.Distance(collision.contacts[0].point, transform.parent.position);

        Vector3 newPos = transform.position;
        if (cameraToOrbit > contactToOrbit)
        {
            targetDistance = Mathf.Clamp(targetDistance + .1f * speed, minDist, maxDist);
        }
        else
        {
            targetDistance = Mathf.Clamp(targetDistance - .1f * speed, minDist, maxDist);
        }

    }

    void OnCollisionEnter()
    {
        collisionCount++;
    }
    void OnCollisionExit()
    {
        collisionCount--;
    }
}
