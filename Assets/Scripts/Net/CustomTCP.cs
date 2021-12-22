using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class CustomTCP : MonoBehaviour
{
    public String address = "127.0.0.1";
    public int port = 3333;
    public int coolDown = 100;
    public LoginController login_controller;
    public Action<string, string[]> bind_handler;
    private TcpClient socket;
    private NetworkStream stream;
    private NetworkStream writeStream;
    private Thread thread;
    private CustomThreadHandler threadHandler;
    /* private bool threadRunning = true; */
    private NetArgParser parser;
    private Dictionary<string, Action<string[]>> binds;


    void Start()
    {
        parser = new NetArgParser();
        /* socket = new TcpClient(); */
        threadHandler = new CustomThreadHandler(thread);
        binds = new Dictionary<string, Action<string[]>>();
        binds.Add("test", (string[] args) => { print(args); });

        if (login_controller != null)
        {
            bind_handler = login_controller.HandleLoginResponse;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            Debug.Log(thread.IsAlive);
        }
        if (Input.GetKeyDown("p"))
        {
            Disconnect();
            Debug.Log("Thread run condition: " + threadHandler.threadRunning);
        }
    }
    public void Disconnect()
    {
        /* print("Flagging thread for join");
        threadHandler.thread.Join();
        print("Thread joined"); */
        threadHandler.StopThread();
        try
        {
            stream.Dispose();
            writeStream.Dispose();
            socket.Dispose();
            print("Shutdown TCP connection");
            print("write: " + writeStream.CanWrite + " read: " + stream.CanRead + " socket: " + socket.Connected);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void Connect()
    {
        socket = new TcpClient();
        socket.Connect(address, port);
        stream = socket.GetStream();
        writeStream = socket.GetStream();
        try
        {
            thread = new Thread(() =>
               {
                   // THREADED NETCODE
                   Debug.Log(socket);
                   Byte[] bytes = new Byte[1024];
                   while (threadHandler.threadRunning)
                   {
                       Debug.Log("Thread loop running");
                       int length;
                       if ((length = stream.Read(bytes, 0, bytes.Length)) != 0 && threadHandler.threadRunning)
                       {
                           Debug.Log("data incoming, length: " + length);
                           var incomingData = new byte[length];
                           Array.Copy(bytes, 0, incomingData, 0, length);
                           string msg = Encoding.UTF8.GetString(incomingData);
                           print("TCP message: " + msg);
                           // Handle commands
                           if (msg.Contains(";"))
                           {
                               NetworkCommand command = parser.parse(msg);
                               try
                               {
                                   if (bind_handler == null)
                                   {
                                       binds[command.command](command.args);
                                   }
                                   else
                                   {
                                       bind_handler(command.command, command.args);
                                   }
                               }
                               catch (Exception e)
                               {
                                   print("Server command bind error: " + e);
                               }
                           }
                       }
                       /* Debug.Log("Thread loop finished"); */
                       Thread.Sleep(coolDown);
                   }

               });
            thread.IsBackground = true;
            thread.Name = "TCPClient";
            threadHandler.thread = thread;
            thread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("Thread creation exception: " + e);
        }
    }

    public void Send(string msg)
    {
        if (writeStream.CanWrite)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            writeStream.Write(bytes, 0, bytes.Length);
        }
    }
    public void SendCmd(string command, string body)
    {
        try
        {
            if (writeStream.CanWrite && socket.Connected)
            {
                string msg = command + ";" + body;
                byte[] msg_bytes = Encoding.UTF8.GetBytes(msg);

                writeStream.Write(msg_bytes, 0, msg_bytes.Length);
                Debug.Log("Sent command " + command);
                print("socket: " + socket.Connected + " writeStream: " + writeStream.CanWrite + " stream: " + stream.CanRead);
            }
            else
            {
                Debug.LogWarning("Couldn't send command, socket: " + socket.Connected + " writeStream: " + writeStream.CanWrite);
                /* Thread.Sleep(coolDown); */
                Connect();
                SendCmd(command, body);
            }
        }
        catch (Exception e)
        {
            /* Debug.Log("Couldn't send command, socket: " + socket.Connected + " writeStream: " + writeStream.CanWrite + " exception: " + e); */
            /* Debug.Log(e); */
            /* Thread.Sleep(coolDown); */
            print("Reconnecting");
            Connect();
            SendCmd(command, body);
        }
    }
    public void bind(string e, Action<string[]> cb)
    {
        binds[e] = cb;
    }
}