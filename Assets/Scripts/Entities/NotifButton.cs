using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NotifButton : MonoBehaviour
{
    AudioSource vibration;
    AudioSource notification;
    GameObject[] buttons = new GameObject[3];
    public GameObject button;
    BoxCollider collider;
    CollisionListener collisionListener;
    public float pressed = 0;
    float prevPressed = 0;
    public bool playerColliding = false;
    public float TravelDistance = .1f;
    public bool CanPlay = true;
    void Start()
    {
        AudioSource[] components = new AudioSource[2];
        components = GetComponents<AudioSource>();
        vibration = components[0];
        notification = components[1];

        for (int i = 0; i < 3; i++)
        {
            buttons[i] = transform.GetChild(i + 1).gameObject;
            if (buttons[i].activeInHierarchy) button = buttons[i];
        }
        collider = button.GetComponent<BoxCollider>();
        collisionListener = button.GetComponent<CollisionListener>();
        collisionListener.onEnter = (Collision collision) =>
        {
            playerColliding = true;
            print("FUCK");
        };
        collisionListener.onExit = (Collision collision) =>
        {
            playerColliding = false;
        };
    }
    void Update()
    {
        if (playerColliding && pressed < 1)
        {
            pressed = Mathf.Lerp(pressed, 1, Time.deltaTime * 15f);
            if (CanPlay && pressed >= .8f)
            {
                vibration.Play();
                notification.Play();
                CanPlay = false;
            }
        }
        else if (pressed > 0)
        {
            pressed = Mathf.Lerp(pressed, 0, Time.deltaTime * 15f);
            if (pressed < .2f) { CanPlay = true; }
        }

        if (prevPressed != pressed)
        {
            Vector3 newPos = button.transform.position;

            newPos.y = -pressed * TravelDistance;

            button.transform.position = newPos;
        }

        prevPressed = pressed;
    }
}