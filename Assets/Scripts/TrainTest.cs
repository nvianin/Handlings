using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTest : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 1;
    public float amplitude = 100;
    public float cutoff = .1f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Clamp((Mathf.Sin(Time.time * speed) * amplitude + 1) / 2, 0 + cutoff, 1 - cutoff);
        transform.position = newPos;
    }
}
