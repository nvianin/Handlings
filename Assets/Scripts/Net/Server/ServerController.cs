using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ServerController : MonoBehaviour
{
    public List<GameObject> players;
    public GameObject EmulatedPlayerPrefab;
    private Thread TCPServerThread;
    private CustomThreadHandler TCPServerHandle;
    private TCPListener TCPServer;
    private UDPServer uDPServer;
    public string address = "127.0.0.1";
    public int tcp_port = 6969;
    public int udp_port = 6968;
    public Dictionary<String, Action<string[]>> binds;

    ServerController(string _address, int _tcp_port, int _udp_port)
    {
        address = _address;
        tcp_port = _tcp_port;
        udp_port = _udp_port;
    }
    void Awake()
    {
        players = new List<GameObject>();

        setupBinds();

        /* TCPServer.handleTCP(); */

    }

    public void Connect()
    {

        TCPServer = new TCPListener(address, tcp_port, this);
        uDPServer = new UDPServer(udp_port, this);
    }

    void Update()
    {

        foreach (GameObject player in players)
        {
            // Do stuff
        }


    }

    bool addPlayer(PlayerData playerData)
    {
        GameObject newPlayerObject = Instantiate(EmulatedPlayerPrefab);
        newPlayerObject.AddComponent<EmulatedPlayerBehaviour>();

        players.Add(newPlayerObject);

        return true;
    }

    void setupBinds()
    {
        binds = new Dictionary<string, Action<string[]>>();

        binds.Add("connect", (string[] args) =>
        {
            Debug.Log("Login with args:" + args);
        });
    }
}

public class PlayerData
{


}