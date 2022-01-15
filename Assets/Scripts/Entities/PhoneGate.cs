using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneGate : MonoBehaviour
{

    GameObject player;
    GameObject phone;
    Material phone_mat;
    MeshCollider phone_collider;
    GameObject unlock_text;
    float lock_factor = 1;
    public bool open = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<HandController>().playerModel;
        phone = transform.Find("phone_mid").gameObject;
        phone_mat = phone.GetComponent<MeshRenderer>().materials[0];
        phone_collider = phone.GetComponent<MeshCollider>();
        unlock_text = transform.Find("Text").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            unlock_text.SetActive(false);
            phone_mat.SetVector("Vector3_e3701f765cd246d6ae2ad100c3d6b1c6", player.transform.position);
            phone_collider.enabled = false;
            if (lock_factor > 0)
            {
                lock_factor = Mathf.Lerp(lock_factor, 0, Time.deltaTime * .4f);
                phone_mat.SetFloat("Vector1_8f2b2c407a6945c2b8da85d3b445164a", lock_factor);
            }
            else if (lock_factor < 0)
            {
                lock_factor = 0;
                phone_mat.SetFloat("Vector1_8f2b2c407a6945c2b8da85d3b445164a", lock_factor);
            }
        }
        else
        {
            phone_mat.SetVector("Vector3_e3701f765cd246d6ae2ad100c3d6b1c6", Vector3.zero);
            phone_collider.enabled = true;
            lock_factor = 1;
            phone_mat.SetFloat("Vector1_8f2b2c407a6945c2b8da85d3b445164a", lock_factor);
        }
    }
}