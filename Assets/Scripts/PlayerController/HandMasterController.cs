using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMasterController : MonoBehaviour
{
    public float cameraSpeed = 50;
    public GameObject cameraPivot;
    public GameObject playerModel;
    public GameObject ArmTarget;
    public GameObject targetPositionMarker;
    private Vector3 ArmTargetOrigin;
    public GameObject[] finger_targets = new GameObject[5];
    public Vector3[] finger_offsets = new Vector3[5];
    public GameObject fingerCenter;
    public string state = "idle";
    public GameObject Target = null;
    private Vector3 original_position;
    public float horizontalSpeed = 10;
    public float verticalSpeed = 10;
    public float grab_factor = 0;
    public float grab_speed = .02f;
    void Start()
    {
        original_position = transform.position;
        ArmTargetOrigin = ArmTarget.transform.position;
        setFingersOrigin();
    }

    void setFingersOrigin()
    {
        int i = 0;
        foreach (GameObject finger in finger_targets)
        {
            finger_offsets[i] = finger.transform.position;
            i++;
        }
    }

    void FixedUpdate()
    {
        // CAMERA ROTATION

        Vector3 rot = cameraPivot.transform.eulerAngles;
        rot.x -= Input.GetAxis("Mouse Y") * cameraSpeed;
        rot.y += Input.GetAxis("Mouse X") * cameraSpeed;
        rot.z = 0;
        /* rot.x = Mathf.Clamp(rot.x, -40, 89); */
        /* rot.x = rot.x > 89 ? 89 : rot.x; */
        rot.x = rot.x < -40 ? -40 : rot.x;

        cameraPivot.transform.eulerAngles = rot;

        // STATE MACHINE

        Vector3 targetPos = ArmTarget.transform.position;
        switch (state)
        {
            case "idle":
                var gobs = GameObject.FindGameObjectsWithTag("Grabbable");
                if (gobs.Length > 0)
                {
                    print("got " + gobs.Length + " objects");
                    state = "hunting";
                    Target = gobs[0];
                }
                break;
            case "hunting":
                targetPos = playerModel.transform.position;
                targetPos.x = Target.transform.position.x;
                targetPos.z = Target.transform.position.z;

                if (Vector3.Distance(playerModel.transform.position, targetPos) > .05f)
                {
                    /* ArmTarget.transform.position = Vector3.Lerp(ArmTarget.transform.position, targetPos, Time.deltaTime * horizontalSpeed); */
                    playerModel.transform.position =
                    Vector3.Lerp(
                        playerModel.transform.position,
                         playerModel.transform.position + (targetPos - playerModel.transform.position).normalized,
                          Time.deltaTime * horizontalSpeed
                          );
                }
                else
                {
                    setFingersOrigin();
                    state = "grabbing";
                }
                break;
            case "grabbing":
                targetPos = playerModel.transform.position;
                targetPos.x = Target.transform.position.x;
                targetPos.z = Target.transform.position.z;
                if (Vector3.Distance(playerModel.transform.position, targetPos) <= .05f)
                {
                    if (Vector3.Distance(ArmTarget.transform.position, Target.transform.position) > .05f)
                    {
                        ArmTarget.transform.position =
                        Vector3.Lerp(
                            ArmTarget.transform.position,
                             ArmTarget.transform.position + (Target.transform.position - ArmTarget.transform.position).normalized,
                              Time.deltaTime * verticalSpeed
                              );
                    }
                    else
                    {
                        int i = 0;
                        foreach (GameObject finger_target in finger_targets)
                        {
                            finger_target.transform.position =
                            Vector3.Lerp(finger_offsets[i], fingerCenter.transform.position, grab_factor);
                            i++;
                        }
                        if (grab_factor < .8f)
                        {
                            grab_factor += grab_speed;
                        }
                        else
                        {
                            state = "travelling";
                        }
                    }
                }
                else if (Target)
                {
                    state = "hunting";
                }
                else
                {
                    state = "idle";
                }
                break;
            case "travelling":
                targetPos = playerModel.transform.position;
                targetPos.x = targetPositionMarker.transform.position.x;
                targetPos.z = targetPositionMarker.transform.position.z;
                if (Vector3.Distance(ArmTarget.transform.position, targetPositionMarker.transform.position) > .05f)
                {
                    ArmTarget.transform.position =
                     Vector3.Lerp(
                         ArmTarget.transform.position,
                          ArmTarget.transform.position + (targetPositionMarker.transform.position - ArmTarget.transform.position).normalized,
                           Time.deltaTime * verticalSpeed
                           );
                }
                else
                {
                    if (Vector3.Distance(playerModel.transform.position, targetPos) > .05f)
                    {
                        playerModel.transform.position =
                        Vector3.Lerp(
                            playerModel.transform.position,
                             playerModel.transform.position + (targetPos - playerModel.transform.position).normalized,
                              Time.deltaTime * horizontalSpeed
                              );
                    }
                    else
                    {
                        setFingersOrigin();
                        state = "releasing";
                    }
                    Target.transform.position = ArmTarget.transform.position;
                }
                break;
            case "releasing":
                if (grab_factor > 0)
                {
                    grab_factor -= grab_speed;
                    int i = 0;
                    foreach (GameObject finger_target in finger_targets)
                    {
                        finger_target.transform.position =
                        Vector3.Lerp(finger_offsets[i], fingerCenter.transform.position, grab_factor);
                        i++;
                    }
                }
                else
                {
                    grab_factor = 0;
                    Target.tag = "Untagged";
                    state = "idle";
                    Target = null;
                }
                break;
        }
    }
}