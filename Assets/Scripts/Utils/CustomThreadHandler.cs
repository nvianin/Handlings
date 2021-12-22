using System.Collections;
using System.Threading;

using UnityEngine;
using UnityEditor;

public class CustomThreadHandler : MonoBehaviour
{
    public Thread thread;
    public bool threadRunning = true;
    public CustomThreadHandler(Thread _thread)
    {
        thread = _thread;
    }

    public void StopThread()
    {
        threadRunning = false;
    }

#if UNITY_EDITOR
    private void Start()
    {
        AssemblyReloadEvents.beforeAssemblyReload += StopThread;
        EditorApplication.playModeStateChanged += StopThreadPlayMode;
    }
    private void StopThreadPlayMode(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            StopThread();
        }
    }

#endif

}