using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpiralSlider : MonoBehaviour
{
    public GameObject button;
    public PhoneGate phoneGate;
    public bool debug_draw = true;
    public float t = 0;
    public int turns = 10;
    public float growth = 1f;
    public float scale = 1f;
    public float offset_start = 0f;
    public float offset_end = 0f;
    public float activationDistance = 5f;
    public float stickyDistance = .05f;
    public bool docked = false;
    float buttonHeight;
    Quaternion button_rotation;
    public GameObject player;

    void Start()
    {
        buttonHeight = button.transform.position.y;
        button_rotation = button.transform.rotation;
        player = GameObject.FindWithTag("Player").GetComponent<HandController>().ArmTarget;
    }
    void Update()
    {
        if (docked)
        {
            t = Mathf.Lerp(t, 1, Time.deltaTime * 5f);
            SetButtonLocation(t);
        }
        else
        {
            if (Vector3.Distance(player.transform.position, transform.position) < activationDistance)
            {
                float time = findNearestLocation(player.transform.position);
                float button_dist = Vector3.Distance(button.transform.position, player.transform.position);
                /* print("found player, distance:" + button_dist + ", t:" + time); */
                if (button_dist < stickyDistance)
                {
                    /* SetButtonLocation(time); */
                    t = Mathf.Clamp01(Mathf.Lerp(t, time, Time.deltaTime * 7f));
                }
                else if (t > 0)
                {
                    t = Mathf.Lerp(t, 0, Time.deltaTime * 1f);
                }
            }
            else if (t > 0) t = Mathf.Lerp(t, 0, Time.deltaTime * 5f);
            if (t > .975)
            {
                docked = true;
                GetComponent<AudioSource>().Play();
                phoneGate.open = true;
                print("spiral lock unlocked");
            }
            SetButtonLocation(t);
        }
        // else if (debug_draw && false)
        // {
        //     t += .01f;
        //     if (t >= 1f)
        //     {
        //         t = 0f;
        //     }
        //     button.transform.position = GetPositionAt(t);
        //     button.transform.rotation = Quaternion.LookRotation(button.transform.position - transform.position, Vector3.up);
        //     /* print(button.transform.eulerAngles); */
        //     Vector3 rot = button.transform.eulerAngles;
        //     rot.x = 90;
        //     rot.y -= 90;
        //     button.transform.eulerAngles = rot;
        // }
        /* button.transform.eulerAngles = new Vector3(90, 0, 0); */
        if (debug_draw)
        {
            for (float t = 0; t < 1; t += .001f)
            {
                Debug.DrawRay(GetPositionAt(t), Vector3.up * .1f, Color.yellow, .01f);
            }
        }
    }

    void SetButtonLocation(float time)
    {
        button.transform.position = GetPositionAt(time);
        button.transform.rotation = Quaternion.LookRotation(button.transform.position - transform.position, Vector3.up);
        Vector3 rot = button.transform.eulerAngles;
        rot.x = 90;
        rot.y -= 90;
        button.transform.eulerAngles = rot;
    }

    Vector3 GetPositionAt(float time)
    {
        float t = Utils.Map(time, 0, 1, offset_start, offset_end);
        return new Vector3(
            Mathf.Sin(-t * Mathf.PI * 2 * turns) * (growth + scale) * t,
            buttonHeight,
            Mathf.Cos(-t * Mathf.PI * 2 * turns) * (growth + scale) * t
        ) + transform.position;
    }
    float GetRotationAt(float time)
    {
        float t = Utils.Map(time, 0, 1, offset_start, offset_end);
        float offset = .001f;

        Vector3 p = GetPositionAt(t).normalized;
        Vector3 p1 = GetPositionAt(t + offset).normalized;
        Vector3 p2 = GetPositionAt(t - offset).normalized;

        return Mathf.Asin(Mathf.Sin(
            Vector3.Distance(p1, p2) / 2
            /
            Vector3.Distance(p, p1)
        ));

    }

    float findNearestLocation(Vector3 loc)
    {
        float t_loc = 0;
        float min_dist = 100000;

        // Coarse first pass
        for (float t = 0; t <= 1; t += .01f)
        {
            float dist = Vector3.Distance(GetPositionAt(t), loc);
            if (dist < min_dist)
            {
                t_loc = t;
                min_dist = dist;
            }
        }
        /* if (debug_draw) print("first-pass nearest " + t_loc); */
        float offset = .01f;
        int steps = 10;

        // Forward tracking
        for (float t = t_loc; t < t_loc + offset; t += offset / steps)
        {
            float dist = Vector3.Distance(GetPositionAt(t), loc);
            if (dist < min_dist)
            {
                t_loc = t;
                min_dist = dist;
            }
        }
        /* if (debug_draw) print("forward nearest " + t_loc); */

        // Backwards tracking
        for (float t = t_loc; t > t_loc - offset; t -= offset / steps)
        {
            float dist = Vector3.Distance(GetPositionAt(t), loc);
            if (dist < min_dist)
            {
                t_loc = t;
                min_dist = dist;
            }
        }
        /* if (debug_draw) print("backwards nearest " + t_loc); */

        return t_loc;

    }
}