using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    public Vector3 playerStartPos = Vector3.zero;
    public GameObject ClientPlayerControllerPrefab;
    public List<GameObject> players;
    public GameObject clientPlayer;
    public TCPClient TCPClient;
    public UDPClient uDPClient;
    public Dictionary<String, Action<String[]>> binds;

    public string address = "127.0.0.1";
    public int tcp_port = 6969;
    public int udp_port;

    public void Connect()
    {
        TCPClient = gameObject.AddComponent<TCPClient>();
        TCPClient.address = address;
        TCPClient.port = tcp_port;
        TCPClient.clientController = this;
        /* TCPClient.ConnectToServer(); */
        TCPClient.SendCmd("connect", System.Guid.NewGuid().ToString());

        uDPClient = new UDPClient(address, udp_port, this);

        clientPlayer = Instantiate(ClientPlayerControllerPrefab, playerStartPos, new Quaternion());

    }

    void Start()
    {
        binds = new Dictionary<string, Action<string[]>>();
        SetupBinds();
    }

    void Update()
    {

    }

    void SetupBinds()
    {

    }

}