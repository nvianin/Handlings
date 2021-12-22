using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TCPListener
{
    public string address;
    public Int32 port;
    public CustomThreadHandler TCPListenerThreadHandler;
    public Thread thread;
    public int ClientThreadCount = -1;
    public List<CustomThreadHandler> ClientThreads;
    private NetArgParser parser;
    private ServerController serverController;
    public TCPListener(string _address, Int32 _port, ServerController _serverController)
    {
        serverController = _serverController;

        parser = new NetArgParser();

        address = _address;
        port = _port;

        ClientThreads = new List<CustomThreadHandler>();

        TCPListenerThreadHandler = new CustomThreadHandler(thread);
        Debug.Log("Launching server thread...");
        thread = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                IPAddress localAddr = IPAddress.Parse(address);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(IPAddress.Any, port);

                // Start listening for client requests.
                server.Start();

                Debug.Log("Server started.");

                // Buffer for reading data
                Byte[] bytes = new Byte[1024];
                String data = null;

                // Enter the listening loop.
                while (TCPListenerThreadHandler.threadRunning)
                {

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    Debug.Log("Waiting for a connection... ");
                    TcpClient client = server.AcceptTcpClient();
                    Debug.Log("Client connected at " + client.Client.RemoteEndPoint + " ! Spinning up thread n°" + (ClientThreadCount + 1) + "...");

                    ClientThreadCount++;

                    thread = new Thread(new ParameterizedThreadStart((object o) =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        int identity = ClientThreadCount;

                        try
                        {
                            TcpClient client = (TcpClient)o;
                            NetworkStream stream = client.GetStream();
                            string imei = String.Empty;
                            string data = null;
                            Byte[] bytes = new byte[1024];
                            int i;

                            Byte[] hello_world_msg = System.Text.Encoding.ASCII.GetBytes("Hello");
                            stream.Write(hello_world_msg, 0, hello_world_msg.Length);

                            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                string hex = BitConverter.ToString(bytes);
                                data = Encoding.ASCII.GetString(bytes, 0, i);
                                Debug.Log("Client thread n°" + identity + " received: " + data);
                                NetworkCommand cmd = parser.parse(data);
                                if (!cmd.error)
                                {
                                    try
                                    {
                                        serverController.binds[cmd.command](cmd.args);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.Log("Command " + cmd.command + ";" + cmd.args + " failed; " + e);
                                    }
                                }


                                string str = "Hey !";
                                Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                                stream.Write(reply, 0, reply.Length);
                                Debug.Log("Client thread n°" + identity + " sent: " + str);
                            }
                            Debug.Log("Client thread n°" + identity + " shutting down");
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Connection handling exception:" + e);
                            client.Close();
                            ClientThreads.Remove(ClientThreads[identity]);
                        }

                    }));
                    thread.IsBackground = true;
                    thread.Name = "TCP Server Connection n°" + ClientThreadCount;
                    thread.Start(client);

                    CustomThreadHandler c = new CustomThreadHandler(thread);
                    ClientThreads.Add(c);

                    /* data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Debug.Log("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Debug.Log("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close(); */
                }
            }
            catch (SocketException e)
            {
                Debug.LogError("SocketException: " + e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Debug.LogError("TCP server stopped !");
        });
        thread.Name = "TCP Server";
        thread.Start();
    }
    public void handleTCP()
    {
        TcpListener server = null;
        try
        {
            // Set the TcpListener on port 13000.
            IPAddress localAddr = IPAddress.Parse(address);

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[1024];
            String data = null;

            // Enter the listening loop.
            while (true)
            {
                Debug.Log("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Connected!");

                thread = new Thread(new ParameterizedThreadStart((object o) =>
                {

                    TcpClient client = (TcpClient)o;
                    NetworkStream stream = client.GetStream();
                    string imei = String.Empty;
                    string data = null;
                    Byte[] bytes = new byte[1024];
                    int i;
                    try
                    {
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            string hex = BitConverter.ToString(bytes);
                            data = Encoding.ASCII.GetString(bytes, 0, i);
                            Debug.Log(Thread.CurrentThread.ManagedThreadId + " received " + data);

                            string str = "Hey !";
                            Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                            stream.Write(reply, 0, reply.Length);
                            Debug.Log(Thread.CurrentThread.ManagedThreadId + " sent " + str);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Connection handling exception:" + e);
                        client.Close();
                    }

                }));
                thread.Name = "TCP Server";
                thread.Start(client);

                /* data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Debug.Log("Received: {0}", data);

                    // Process the data sent by the client.
                    data = data.ToUpper();

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Debug.Log("Sent: {0}", data);
                }

                // Shutdown and end connection
                client.Close(); */
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketException: " + e);
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }

        Debug.LogError("TCP server stopped !");
    }
}

