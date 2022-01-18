using UnityEngine;
using System;

public class CollisionListener : MonoBehaviour
{
    public Action<Collision> onEnter;
    public Action<Collision> onExit;
    void OnCollisionEnter(Collision collision)
    {
        if (onEnter != null) onEnter(collision);
        print("collision enter");
    }
    void OnCollisionExit(Collision other)
    {
        if (onExit != null) onExit(other);
        print("collision exit");
    }
}