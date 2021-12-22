using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public bool Retracted = true;

    public float cameraSpeed = 50;
    public GameObject cameraPivot;
    public GameObject playerModel;
    public GameObject ArmTarget;
    private Vector3 ArmTargetOrigin;
    public float ArmBounce;
    public float bounceSpeed = .5f;
    public float bounceHeight = .05f;
    public float bounceOffset = .05f;
    public float speed = 10;
    public float lerpSpeed = 10;
    public float rotationSpeed = 10;
    public float strafeSpeedMultiplier = .25f;
    public float retrogradeSpeedMultiplier = .5f;
    public GameObject retraction_bone_1;
    public GameObject retraction_bone_2;
    public float retraction_scale = 5;
    public bool cursorLock = false;
    public Vector3 direction;
    public List<Vector3> prevPos;
    private int prevPosCount = 20;
    public GameObject[] fingerJoints = new GameObject[5];
    void Start()
    {
        prevPos = new List<Vector3>();
        ArmTargetOrigin = ArmTarget.transform.position;
    }

    void FixedUpdate()
    {
        // HAND RETRACTION

        /* if (Retracted)
        {
            retraction_bone_1.transform.localScale =
            Vector3.Lerp(retraction_bone_1.transform.localScale,
             new Vector3(1f / retraction_scale, 1f / retraction_scale, 1f / retraction_scale),
              .05f);
            retraction_bone_2.transform.localScale =
            Vector3.Lerp(retraction_bone_2.transform.localScale,
             new Vector3(retraction_scale, retraction_scale, retraction_scale),
              .05f);
        }
        else
        {
            retraction_bone_1.transform.localScale =
            Vector3.Lerp(retraction_bone_1.transform.localScale,
             new Vector3(1, 1, 1),
              .05f);
            retraction_bone_2.transform.localScale =
            Vector3.Lerp(retraction_bone_2.transform.localScale,
             new Vector3(1, 1, 1),
              .05f);
        } */

        if (Input.GetKeyUp(KeyCode.Space)) Retracted = !Retracted;

        // CAMERA ROTATION

        Vector3 rot = cameraPivot.transform.eulerAngles;
        rot.x -= Input.GetAxis("Mouse Y") * cameraSpeed;
        rot.y += Input.GetAxis("Mouse X") * cameraSpeed;
        rot.z = 0;
        /* rot.x = Mathf.Clamp(rot.x, -40, 89); */
        /* rot.x = rot.x > 89 ? 89 : rot.x; */
        rot.x = rot.x < -40 ? -40 : rot.x;

        cameraPivot.transform.eulerAngles = rot;

        // PLAYERMODEL MOVEMENT

        direction = new Vector3();

        direction += cameraPivot.transform.rotation * Vector3.right * Input.GetAxis("Horizontal") * strafeSpeedMultiplier;
        direction += cameraPivot.transform.rotation * Vector3.forward * Input.GetAxis("Vertical");
        direction.y = 0;
        direction = direction.normalized;

        direction *= speed;


        playerModel.transform.position = Vector3.Lerp(playerModel.transform.position, playerModel.transform.position + direction, Time.deltaTime * lerpSpeed);

        // PLAYERMODEL ROTATION

        Quaternion newRot = playerModel.transform.rotation;
        Vector3 medDir = Vector3.zero;
        foreach (Vector3 pos in prevPos)
        {
            medDir += pos;
        }
        medDir /= prevPos.Count;
        newRot = Quaternion.Slerp(newRot, Quaternion.LookRotation(playerModel.transform.position - medDir, Vector3.up), Time.deltaTime * rotationSpeed);


        // Hand Palm IK

        Vector3 newPos = ArmTargetOrigin;

        ArmBounce += Mathf.Sin(Time.fixedTime * bounceSpeed * (direction.magnitude + 1)) * (direction.magnitude + .05f) * bounceHeight + bounceOffset;
        ArmBounce = Mathf.Lerp(ArmBounce, 0, Time.deltaTime * 20);
        newPos.y += ArmBounce;
        newPos.x = ArmTarget.transform.position.x;
        newPos.z = ArmTarget.transform.position.z;

        ArmTarget.transform.position = Vector3.Lerp(ArmTarget.transform.position, newPos, Time.deltaTime * 10f);

        /* Vector3 palmRotation = Vector3.zero;
        foreach (GameObject joint in fingerJoints)
        {
            palmRotation += Quaternion.LookRotation(ArmTarget.transform.position - joint.transform.position, Vector3.up).eulerAngles;
        }
        palmRotation /= 5;

        ArmTarget.transform.rotation = Quaternion.Euler(palmRotation); */

        // Position history logic

        while (prevPos.Count > prevPosCount)
        {
            prevPos.RemoveAt(0);
        }
        prevPos.Add(playerModel.transform.position);


    }

    void Update()
    {
        // Lock cursor to window

        if (Input.GetKeyUp("f1"))
        {
            cursorLock = !cursorLock;
            Cursor.lockState = cursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}