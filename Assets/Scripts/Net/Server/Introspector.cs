using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Introspector : MonoBehaviour
{

    public bool isServer = false;
    public bool debug_spawn_both = false;
    public string server_address = "127.0.0.1";
    public int tcp_port = 4194;
    public int udp_port = 4196;

    // Start is called before the first frame update

    void Awake()
    {
        if (gameObject.activeSelf)
        {
            print(gameObject.activeSelf);
            string isServerArg = GetArg("-server");
            switch (isServerArg)
            {
                case "true":
                    isServer = true;
                    break;
                case "false":
                    isServer = false;
                    break;
            }

            if (isServer)
            {
                SpawnServerScene();
                if (debug_spawn_both) SpawnClientScene();
            }
            else
            {
                SpawnClientScene();
                if (debug_spawn_both) SpawnServerScene();
            }
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnServerScene()
    {
        var s = gameObject.AddComponent<ServerController>();
        s.address = server_address;
        s.tcp_port = tcp_port;
        s.udp_port = udp_port;
        s.Connect();
    }

    void SpawnClientScene()
    {
        var c = gameObject.AddComponent<ClientController>();
        c.address = server_address;
        c.tcp_port = tcp_port;
        c.udp_port = udp_port;
        c.Connect();
    }




    // Helper function for getting the command line arguments
    public static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
