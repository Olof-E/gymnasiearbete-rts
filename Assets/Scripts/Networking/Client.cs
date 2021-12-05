using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    public TcpClient tcpClient { get; private set; }
    public NetworkStream stream { get; private set; }

    private byte[] recievedBuffer;
    private int maxBufferSize = 8192;

    public bool IsConnected
    {
        get
        {
            try
            {
                if (tcpClient?.Client != null)
                {
                    return !(tcpClient.Client.Poll(1, SelectMode.SelectRead) && tcpClient.Client.Available == 0);
                }
                return false;
            }
            catch (SocketException) { return false; }
        }
    }

    //Attempt to connect to server
    public void StartConnection()
    {
        tcpClient = new TcpClient();

        recievedBuffer = new byte[maxBufferSize];

        tcpClient.BeginConnect("127.0.0.1", 25687, ClientConnected, null);

    }

    //Executes when client has succesfully connected to server
    public void ClientConnected(IAsyncResult ar)
    {
        tcpClient.EndConnect(ar);

        if (!tcpClient.Connected)
            return;

        Debug.Log($"Connected to server {tcpClient.Client.RemoteEndPoint}");
        stream = tcpClient.GetStream();
        stream.ReadTimeout = 30;
        stream.WriteTimeout = 30;



        stream.BeginRead(recievedBuffer, 0, maxBufferSize, EndReadDataCb, null);
    }

    //Executed everytime data is recieved by this tcp client
    public void EndReadDataCb(IAsyncResult ar)
    {
        try
        {
            if (!IsConnected)
            {
                Disconnect();
                return;
            }
            int dataSize = stream.EndRead(ar);
            Debug.Log($"{dataSize} bytes was recieved");

            //Keep handling incoming packets if nothing went wrong
            if (dataSize <= 0)
            {
                Disconnect();
                return;
            }

            byte[] recivedData = new byte[dataSize];
            Array.Copy(recievedBuffer, recivedData, dataSize);

            NetworkManager.instance.HandlePacket(new Packet(recivedData));
            stream.BeginRead(recievedBuffer, 0, maxBufferSize, EndReadDataCb, null);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occured on stream read: {e}");
            Disconnect();
        }
    }

    //Send data to the client
    public void SendData(byte[] byteBuffer)
    {
        try
        {
            stream.BeginWrite(byteBuffer, 0, byteBuffer.Length, null, null);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occured on stream write: {e}");
            Disconnect();
        }
    }

    //Handle the cleanup of the client stream and socket
    public void Disconnect()
    {
        if (tcpClient == null)
            return;
        Debug.Log($"Connection to server has been lost");
        tcpClient.Close();
        tcpClient.Dispose();
        stream.Close();
        stream.Dispose();

        tcpClient = null;
        stream = null;
    }
}
