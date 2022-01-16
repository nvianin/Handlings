using UnityEngine;

public class CollisionListener : MonoBehaviour
{
    public void onEnter
    {

    }
    public void onExit
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        print("collision enter");
    }
    void OnCollisionExit(Collision other)
    {
        print("collision exit");
    }
}