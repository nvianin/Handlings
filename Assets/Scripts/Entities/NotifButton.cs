using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NotifButton : MonoBehaviour
{
    AudioSource audioSource;
    GameObject[] buttons = new GameObject[3];
    public GameObject button;
    BoxCollider collider;
    float pressed = 0;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < 3; i++)
        {
            buttons[i] = transform.GetChild(i + 1).gameObject;
            if (buttons[i].activeInHierarchy) button = buttons[i];
        }
        collider = button.GetComponent<BoxCollider>();
    }
    void Update()
    {

        Vector3 newPos = button.transform.position;

        newPos.y = -pressed;

        button.transform.position = newPos;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("player colliding");
            RaycastHit hit;
            if (Physics.Raycast(collision.gameObject.transform.position, Vector3.down, out hit, 1f))
            {
                if (hit.rigidbody.gameObject.tag == "Button")
                {
                    print("player on button");
                }
            }
        }
    }
    void OnCollisionExit(Collision other)
    {

    }
}