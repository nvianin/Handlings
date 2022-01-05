using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMasterController : MonoBehaviour
{
    public bool Retracted = true;
    public float cameraSpeed = 50;
    public GameObject cameraPivot;
    public GameObject playerModel;
    public Rigidbody rigidbody;
    public GameObject ArmTarget;
    private Vector3 ArmTargetOrigin;
    public float retraction_scale = 5;
    public Vector3 direction;
    public List<Vector3> prevPos;
    private int prevPosCount = 20;
    public GameObject[] fingerJoints = new GameObject[5];
    private Quaternion original_rot;
    void Start()
    {
        prevPos = new List<Vector3>();
        ArmTargetOrigin = ArmTarget.transform.position;
        original_rot = playerModel.transform.rotation;
    }

    void FixedUpdate()
    {
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
    }
}