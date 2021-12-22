using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationScript : MonoBehaviour
{
    // Start is called before the first frame update

    private AudioSource audioSource;
    private Animator animator;
    public List<Animation> animations;
    /* public Animation */
    public PlayerController playerController;
    public float movementIntensityMultiplier = 1;
    public float movementSpeedMultiplier = 1;
    public float minAnimationSpeed = 1;
    public float maxAnimationSpeed = 3;
    public float currentAnimationSpeed = 1;
    public float currentPlayerSpeed = 0;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        animations = new List<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        /* Debug.DrawRay(transform.position, Vector3.forward * (playerController.playerModel.transform.InverseTransformDirection(playerController.direction)).z, Color.blue, 1);
        Debug.DrawRay(transform.position, Vector3.right * (playerController.playerModel.transform.InverseTransformDirection(playerController.direction)).x, Color.red, 1); */
        animator.SetFloat("Forward", (playerController.playerModel.transform.InverseTransformDirection(playerController.direction)).normalized.z * playerController.movementSpeed * movementIntensityMultiplier);
        animator.SetFloat("Sideways", (playerController.playerModel.transform.InverseTransformDirection(playerController.direction)).normalized.x * playerController.movementSpeed * movementIntensityMultiplier);

        currentAnimationSpeed = Mathf.Clamp(Mathf.Lerp(animator.speed, playerController.movementSpeed * movementSpeedMultiplier, .1f), minAnimationSpeed, maxAnimationSpeed);
        animator.speed = currentAnimationSpeed;


        currentPlayerSpeed = playerController.movementSpeed;
    }

    void footstep()
    {
        audioSource.pitch = .5f + Random.value * .5f;
        audioSource.Play();
    }
}
