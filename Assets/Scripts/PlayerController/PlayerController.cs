using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject camera;
    public GameObject cameraPivot;
    public GameObject playerModel;
    public Vector3 camRot = Vector3.zero;
    public bool lockedCamera = false;
    public float speed = 10;
    public float sprintSpeedAddendum = 5;
    public float strafeSpeedMultiplier = .25f;
    public float retrogradeSpeedMultiplier = .5f;
    public int cameraSpeed = 50;
    public bool invertCameraX = false;
    public bool invertCameraY = false;
    public bool landed = true;
    public float jumpForce = 10;
    public bool drawDebugLanded = false;
    public float landedThreshold = .05f;
    public Vector3 direction = Vector3.zero;
    public float movementSpeed = 0;
    public bool movingArms = false;
    public Animator animator;
    private Rigidbody rb;
    public List<Vector3> last_positions;
    private bool cursorLock = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        last_positions = new List<Vector3>();
        last_positions.Add(transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // Forward / Strafe
        Vector3 force = Vector3.zero;

        force += cameraPivot.transform.rotation * Vector3.right * Input.GetAxis("Horizontal") * strafeSpeedMultiplier;
        force += cameraPivot.transform.rotation * Vector3.forward * ((Input.GetAxis("Vertical") > 0) ? Input.GetAxis("Vertical") : Input.GetAxis("Vertical"));

        /* force = cameraPivot.transform.rotation * force; */
        force.y = 0;
        force = force.normalized;
        force *= speed + (sprintSpeedAddendum * Input.GetAxis("Sprint"));

        // Jump
        if (landed && Input.GetAxis("Jump") > 0)
        {
            animator.SetTrigger("Jump");
            landed = false;
            force += Vector3.up * jumpForce * Input.GetAxis("Jump");
        }
        else
        {
            if (Physics.Raycast(transform.position, Vector3.down, landedThreshold, ~LayerMask.NameToLayer("Player"))) landed = true;
            if (drawDebugLanded) Debug.DrawLine(transform.position, Vector3.down * landedThreshold, landed ? Color.green : Color.red, 1);
        }

        rb.AddForce(force);

        // Camera rotation

        if (!movingArms)
        {
            Vector3 rot = Vector3.zero;
            rot.x -= Input.GetAxis("Mouse Y") * (invertCameraY ? -1 : 1) * cameraSpeed;
            rot.y += Input.GetAxis("Mouse X") * (invertCameraX ? -1 : 1) * cameraSpeed;
            rot.z = 0;


            camRot.y += rot.y;
            camRot.x = Mathf.Clamp((camRot.x + rot.x), 1, 89);

            cameraPivot.transform.eulerAngles = camRot;
        }


        /* Vector3 _normalized_pivot = cameraPivot.transform.rotation.eulerAngles;
        _normalized_pivot.z = 0;
        _normalized_pivot.x = 0;
        Quaternion normalized_pivot = Quaternion.Euler(_normalized_pivot); */


        /* if (rot.x > 0 && cameraPivot.transform.rotation.eulerAngles.x < 89)
        {
            cameraPivot.transform.RotateAround(cameraPivot.transform.position, normalized_pivot * Vector3.right, rot.x);
        }
        else if (rot.x < 0 && cameraPivot.transform.rotation.eulerAngles.y > -40)
        {
            cameraPivot.transform.RotateAround(cameraPivot.transform.position, normalized_pivot * Vector3.right, rot.x);
        } */
        /* Debug.DrawLine(cameraPivot.transform.position, transform.) */
        /* cameraPivot.transform.RotateAround(cameraPivot.transform.position, Vector3.up, rot.y); */


        //          ROTATE PLAYERMODEL

        Vector3 lastPos = Vector3.zero;
        last_positions.ForEach(pos =>
        {
            lastPos += pos;
        });
        lastPos /= last_positions.Count;
        direction = transform.position - lastPos;
        movementSpeed = direction.magnitude;

        if (Input.GetMouseButtonDown(2)) lockedCamera = !lockedCamera;

        if (lockedCamera || true)
        /* {
            if (!movingArms)
            {

                // PLAYERMODEL ALIGNS TO CAMERA

                Vector3 _newRot = cameraPivot.transform.rotation.eulerAngles;

                _newRot.x = 0;
                _newRot.z = 0;

                Quaternion newRot = Quaternion.Euler(_newRot);
                playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRot, .1f);
            }
        }
        else */
        {
            // PLAYERMODEL FOLLOWS VELOCITY

            if (Vector3.Distance(transform.position, lastPos) > .05f)
            {
                /* Debug.DrawLine(transform.position, lastPos, Color.blue); */
                Quaternion newRot = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.LookRotation(transform.position - lastPos, Vector3.up), .1f);
                /* Quaternion newRot = Quaternion.LookRotation(transform.position - lastPos, Vector3.up); */
                Vector3 _newRot = newRot.eulerAngles;
                _newRot.x = 0;
                _newRot.z = 0;
                /* _newRot.x = -90; */
                /* _newRot.y = 0; */
                newRot = Quaternion.Euler(_newRot);
                playerModel.transform.rotation = newRot;
            }
        }
        last_positions.Add(transform.position);
        while (last_positions.Count > 10)
        {
            last_positions.RemoveAt(0);
        }

        if (Input.GetKey("f1"))
        {
            cursorLock = !cursorLock;
            Cursor.lockState = cursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        }

    }
}
