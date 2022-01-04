using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerWalk : MonoBehaviour
{
    public HandController handController;
    public float max_distance = .1f;
    public float lerp_speed = .1f;
    public float length_modifier = 1f;
    public GameObject finger_root;
    public GameObject finger_ik;
    public GameObject finger_aim;
    public Vector3 finger_ik_pos;
    public Vector3 finger_target_pos;
    public Vector3 aim_offset = new Vector3(0, -8f, 0);
    // Start is called before the first frame update
    void Start()
    {
        finger_ik_pos = finger_ik.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(finger_root.transform.position, finger_ik.transform.position) > max_distance)
        {
            /* print("finger out of bounds"); */
            Vector3 newTargetPos = new Vector3();
            RaycastHit hit;
            if (Physics.Raycast(finger_aim.transform.position, aim_offset + (handController.direction.normalized * max_distance * 50 * length_modifier), out hit))
            {
                //print("FUCK");
                Debug.DrawRay(hit.point, Vector3.up * .1f);
                newTargetPos = hit.point;
                Vector3 rand = Random.onUnitSphere;
                rand.y = 0;
                rand *= .01f;
                newTargetPos += rand;
                // finger_target.transform.position = newTargetPos;
                finger_ik_pos = newTargetPos;
            }
            Debug.DrawRay(finger_aim.transform.position, aim_offset + (handController.direction.normalized * max_distance * length_modifier));
        }
        float fingerDist = Vector3.Distance(finger_ik.transform.position, finger_ik_pos);
        if (fingerDist > .05f)
        {
            if (fingerDist > .5f)
            {
                finger_ik.transform.position = finger_ik_pos;
            }
            else
            {
                finger_ik.transform.position = Vector3.Lerp(finger_ik.transform.position, finger_ik_pos, lerp_speed);
            }
        }
    }
}
