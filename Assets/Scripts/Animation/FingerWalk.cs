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
    AudioSource audioSource;

    float deadTime = 2;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = .2f;
        finger_ik_pos = finger_ik.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /* print(deadTime); */
        if (Vector3.Distance(finger_root.transform.position, finger_ik.transform.position) > max_distance)
        {
            /* print("finger out of bounds"); */
            Vector3 newTargetPos = new Vector3();
            RaycastHit hit;
            if (Physics.Raycast(finger_aim.transform.position, aim_offset + (handController.direction.normalized * max_distance * 50 * length_modifier), out hit))
            {
                Debug.DrawRay(hit.point, Vector3.up * .1f, Color.green, 1);
                newTargetPos = hit.point;
                Vector3 rand = Random.onUnitSphere;
                rand.y = 0;
                rand.x = 0;
                rand *= .01f;
                newTargetPos += rand;
                // finger_target.transform.position = newTargetPos;
                if (Vector3.Distance(finger_ik_pos, newTargetPos) > .1f)
                {
                    /* print(Vector3.Distance(finger_ik_pos, newTargetPos)); */
                    audioSource.pitch = (Random.value * 2 - 1) * .1f + 1f;
                    audioSource.Play();
                }
                finger_ik_pos = newTargetPos;
            }
            Debug.DrawRay(finger_aim.transform.position, aim_offset + (handController.direction.normalized * max_distance * 50 * length_modifier), Color.yellow, .5f);
        }
        else
        {
            if (deadTime > 0)
            {
                deadTime -= Time.deltaTime;
            }
            else
            {
                deadTime = 2;
            }
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
