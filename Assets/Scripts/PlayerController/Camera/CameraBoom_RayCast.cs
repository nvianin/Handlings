using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoom_RayCast : MonoBehaviour
{
    /* public PlayerController playerController; */
    public float minDist = .2f;
    public float maxDist = 5;
    public float targetDistance = 2;
    public float desiredDistance = 2;
    public float speed = 1;
    public float lerp = .1f;

    public float currentDist;

    public int collisionCount = 0;
    private SphereCollider sphereCollider;
    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        targetDistance = (minDist + maxDist) / 2;
        desiredDistance = targetDistance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentDist = Vector3.Distance(transform.position, transform.parent.position);
        if (Mathf.Abs(currentDist - targetDistance) > .01f)
        {
            Vector3 targetPos = transform.localPosition;
            targetPos.z = Mathf.Lerp(transform.localPosition.z, -targetDistance, lerp);
            transform.localPosition = targetPos;
        }
        if (collisionCount < 1)
        {
            targetDistance = Mathf.Clamp(targetDistance + .1f * speed, minDist, maxDist);
        }
        Vector3 newPos = transform.localPosition;
        newPos.z -= .01f;
        transform.localPosition = newPos;

        Vector3 direction = transform.position - transform.parent.position;
        /* direction.y -= .3f; */
        RaycastHit hit;

        /* Debug.DrawRay(transform.parent.position, direction, Color.red, 1); */
        if (Physics.Raycast(transform.parent.position, direction, out hit, currentDist, ~LayerMask.NameToLayer("Player")))
        {
            /* Debug.DrawRay(hit.point, Vector3.up, Color.yellow, 10); */
            targetDistance = Mathf.Clamp(Vector3.Distance(hit.point, transform.parent.position) - .05f, minDist, desiredDistance);
        }

        /* if (!playerController.movingArms)  */
        desiredDistance = Mathf.Clamp(desiredDistance + Input.GetAxis("Mouse ScrollWheel"), minDist, maxDist);
    }

}
