using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoiseController : MonoBehaviour
{
    public List<AudioClip> Clicks;
    public List<AudioClip> Coughs;
    public List<AudioClip> Heys;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyUp("1"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = Clicks[(int)Mathf.Floor(Random.value * Clicks.Count)];
                audioSource.pitch = 1 + Random.value * .2f;
                audioSource.volume = 1 - Random.value * .3f;
                audioSource.Play();
            }
        }
        if (Input.GetKeyUp("2"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = Coughs[(int)Mathf.Floor(Random.value * Coughs.Count)];
                audioSource.pitch = 1 + Random.value * .2f;
                audioSource.volume = 1 - Random.value * .3f;
                audioSource.Play();
            }
        }
        if (Input.GetKeyUp("3"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = Heys[(int)Mathf.Floor(Random.value * Heys.Count)];
                audioSource.pitch = 1 + Random.value * .2f;
                audioSource.volume = 1 - Random.value * .3f;
                audioSource.Play();
            }
        }
    }
}