using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEditor;

public class UDPClient
{
    public UdpClient udpClient;
    public int port;
    public string address;
    public CustomThreadHandler UDPClientThreadHandler;
    public ClientController clientController;
    private NetArgParser parser = new NetArgParser();
    public UDPClient(string _address, int _port, ClientController _clientController)
    {
        clientController = _clientController;
        port = _port;
        address = _address;
        udpClient = new UdpClient();
        udpClient.Connect(address, port);

        UDPClientThreadHandler = new CustomThreadHandler(new Thread(() =>
        {
            IPEndPoint receiver = new IPEndPoint(IPAddress.Any, port);
            Byte[] received = udpClient.Receive(ref receiver);
            string data = System.Text.Encoding.ASCII.GetString(received);
            NetworkCommand cmd = parser.parse(data);
            if (!cmd.error)
            {
                try
                {
                    clientController.binds[cmd.command](cmd.args);
                }
                catch (Exception e)
                {
                    Debug.Log("Command " + cmd.command + " failed; " + e);
                }
            }
            Debug.Log("UDP Client received data: " + data);
        }));

        UDPClientThreadHandler.thread.Name = "UDP Client";
        UDPClientThreadHandler.thread.IsBackground = true;
        UDPClientThreadHandler.thread.Start();
        Send("test");
    }

    public void Send(string message)
    {
        Byte[] message_bytes = System.Text.Encoding.ASCII.GetBytes(message);
        udpClient.Send(message_bytes, message_bytes.Length);
    }
}