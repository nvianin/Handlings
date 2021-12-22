using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ArmController : MonoBehaviour
{
    protected Animator animator;
    public bool ikActive = false;
    public GameObject ArmLeft;
    public GameObject ArmRight;
    public GameObject ArmBoneLeft;
    public GameObject ArmBoneRight;
    public bool leftActive = false;
    public bool rightActive = false;
    public float maxDist = 1;
    public float minDist = .2f;
    public float lerpSpeed = .1f;
    public float planarSpeed = 1;
    public float depthSpeed = 5;
    private Vector3 basePosL;
    private Vector3 basePosR;

    public PlayerController playerController;
    void Start()
    {
        animator = GetComponent<Animator>();

        /* basePosL = ArmLeft.transform.localPosition;
        basePosR = ArmRight.transform.position; */
    }

    void Update()
    {

        if (leftActive || rightActive)
        {
            if (Input.GetAxis("rightClick") == 0) rightActive = false;
            if (Input.GetAxis("leftClick") == 0) leftActive = false;
            /* print("mouse up"); */
            playerController.movingArms = true;
        }
        else if (!leftActive && !rightActive) playerController.movingArms = false;

        if (Input.GetAxis("rightClick") == 1) rightActive = true;
        if (Input.GetAxis("leftClick") == 1) leftActive = true;
        /* print("mouse down"); */


        if (playerController.lockedCamera)
        {
            ikActive = true;
            Vector3 newPos = ArmLeft.transform.position;
            newPos.y += Input.GetAxis("Mouse ScrollWheel") * depthSpeed;
            if (leftActive)
            {

                newPos += playerController.playerModel.transform.rotation * Vector3.right * Input.GetAxis("Mouse X") * planarSpeed;
                /* newPos.y += Input.GetAxis("Mouse ScrollWheel") * depthSpeed; */

                newPos += playerController.playerModel.transform.forward * Input.GetAxis("Mouse Y") * planarSpeed;

                /* newPos = Clamp(newPos, minDist, maxDist); */

                ArmLeft.transform.position = Vector3.Lerp(ArmLeft.transform.position, newPos, lerpSpeed);
                ArmLeft.transform.position = Vector3.Slerp(ArmLeft.transform.position, ArmBoneLeft.transform.position, lerpSpeed / 2);
            }

            newPos = ArmRight.transform.position;
            newPos.y += Input.GetAxis("Mouse ScrollWheel") * depthSpeed;
            if (rightActive)
            {

                newPos += playerController.playerModel.transform.rotation * Vector3.right * Input.GetAxis("Mouse X") * planarSpeed;


                newPos += playerController.playerModel.transform.forward * Input.GetAxis("Mouse Y") * planarSpeed;

                /* newPos = Clamp(newPos, minDist, maxDist); */

                ArmRight.transform.position = Vector3.Slerp(ArmRight.transform.position, newPos, lerpSpeed);
                ArmRight.transform.position = Vector3.Slerp(ArmRight.transform.position, ArmBoneRight.transform.position, lerpSpeed / 2);
            }
        }
        else
        {
            ikActive = false;
            ArmRight.transform.position = Vector3.Lerp(ArmRight.transform.position, ArmBoneRight.transform.position, lerpSpeed / 2);
            ArmLeft.transform.position = Vector3.Lerp(ArmLeft.transform.position, ArmBoneLeft.transform.position, lerpSpeed / 2);
        }
    }

    void OnAnimatorIK()
    {
        if (ikActive)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            /*     animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1); */

            animator.SetIKRotation(AvatarIKGoal.LeftHand, ArmLeft.transform.rotation);
            animator.SetIKRotation(AvatarIKGoal.RightHand, ArmRight.transform.rotation);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, ArmLeft.transform.position);
            animator.SetIKPosition(AvatarIKGoal.RightHand, ArmRight.transform.position);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            /* animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0); */
        }
    }

    private Vector3 Clamp(Vector3 v, float min, float max)
    {

        v.x = Mathf.Clamp(v.x, min, max);
        v.y = Mathf.Clamp(v.y, min, max);
        v.z = Mathf.Clamp(v.z, min, max);

        return v;
    }
}