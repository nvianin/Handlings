using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class TCPClient : MonoBehaviour
{
    // Start is called before the first frame update
    public String address = "127.0.0.1";
    public int port = 6969;
    public int retryCooldown = 2000;
    public string toSend = "lasagna";
    public bool dataToSend = true;
    #region private members
    private TcpClient socketConnection;
    private NetworkStream stream;
    public Thread clientThread;
    public CustomThreadHandler clientThreadHandler;
    public ClientController clientController;
    public List<String> MessageQueue = new List<string>();
    private NetArgParser parser;
    #endregion
    void Start()
    {
        clientThreadHandler = new CustomThreadHandler(clientThread);
        ConnectToServer();
        parser = new NetArgParser();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            /* ConnectToServer(); */
            SendCmd("Hello", "World");
        }
    }
    ///<summary>
    ///Setup socket connection
    ///</summary>
    ///
    IEnumerator RetryConnecting()
    {
        yield return new WaitForSeconds(2);
        /* ConnectToServer(); */
    }
    public void ConnectToServer()
    {
        try
        {
            CreateConnection();
        }
        catch (Exception e)
        {

            Debug.LogError("TCP connection exception: " + e);
            /* StartCoroutine(RetryConnecting()); */
        }
    }
    /// <summary> 	
	/// Runs in background clientThread; Listens for incomming data. 	
	/// </summary>  
    /// 
    private IEnumerator HandleData(Byte[] bytes, TcpClient socketConnection)
    {

        print("Handling TCP data in coroutine...");
        try
        {

            int length;
            print("Connected to logon server");
            if ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                print("Received data or whatever");
                var incomingData = new byte[length];
                Array.Copy(bytes, 0, incomingData, 0, length);
                string msg = Encoding.UTF8.GetString(incomingData);
                Debug.Log("tcp server message: " + msg);
                NetworkCommand cmd = parser.parse(msg);
                print("Command received from server: " + cmd.log());
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Stream error: " + e);
            /* System.Threading.Thread.Sleep(retryCooldown); */
        }
        yield return new WaitForSeconds(.1f);

    }

    private void CreateConnection()
    {
        try
        {
            clientThread = new Thread(() =>
            {
                print("Opening socket...");
                socketConnection = new TcpClient(address, port);
                stream = socketConnection.GetStream();
                Byte[] bytes = new Byte[1024];

                /* print("Handling socket in thread..."); */
                while (clientThreadHandler.threadRunning)
                {
                    try
                    {

                        int length;
                        print("Waiting for data from Server");
                        if ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            /* print("Received data or whatever"); */
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            string msg = Encoding.UTF8.GetString(incomingData);
                            Debug.Log("tcp server message: " + msg);
                            NetworkCommand cmd = parser.parse(msg);
                            print("Command received from server: " + cmd.log());
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
                        }
                        else if (dataToSend)
                        {
                            toSend = MessageQueue[0];
                            MessageQueue.Remove(MessageQueue[0]);
                            print("Sending data ! " + toSend);
                            Byte[] bytes_to_send = System.Text.Encoding.ASCII.GetBytes(toSend);
                            stream.Write(bytes_to_send, 0, bytes_to_send.Length);
                            toSend = "";
                            if (MessageQueue.Count == 0) dataToSend = false;
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Stream error: " + e);
                        /* System.Threading.Thread.Sleep(retryCooldown); */
                    }
                }
            });
            clientThread.IsBackground = true;
            clientThread.Name = "TCP Client";
            clientThread.Start();


            /* StartCoroutine(HandleData(bytes, socketConnection)); */
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket exception: " + e);
            /* Debug.Log("Retry coroutine started"); */
            /* StartCoroutine(RetryConnection()); */
            /* System.Threading.Thread.Sleep(retryCooldown); */
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    /// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 
    private void SendMessage()
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            /* stream = socketConnection.GetStream(); */
            if (stream.CanWrite)
            {
                string clientMessage = "This is a client message.";
                byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage);

                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent message");
            }
            else
            {
                Debug.LogWarning("Socket stream closed.");
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket exception: " + e);
        }
    }

    public void SendCmd(string command, string body)
    {


        Debug.Log("Sending command " + command + " from TCP client");
        /* MessageQueue.Add(command + ";" + body);
        dataToSend = true; */
        if (socketConnection == null)
        {
            Debug.LogError("Tried command " + command + " but not connected to server.");
            return;
        }
        try
        {
            /* stream = socketConnection.GetStream(); */
            if (stream.CanWrite)
            {
                string msg = command + ";" + body;
                byte[] msg_bytes = Encoding.UTF8.GetBytes(msg);

                stream.Write(msg_bytes, 0, msg_bytes.Length);
                Debug.Log("Sent command " + command);
            }
            else
            {
                Debug.LogError("Socket stream closed.");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void Close()
    {
        stream.Close();
    }

}

