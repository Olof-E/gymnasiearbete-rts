using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    private Dictionary<ServerPacketId, Action<Packet>> packetHandlers = new Dictionary<ServerPacketId, Action<Packet>>()
    {
        {ServerPacketId.FIRSTCONNECTION, (Packet packet) => FirstConnectionPacket(packet)},
    };

    //Create a singelton of the network manager
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Network Manager already exists...");
            Destroy(this);
        }
    }

    #region Recieved Functions
    public static void FirstConnectionPacket(Packet packet)
    {
        string receivedMsg = packet.DecodeData<string>();
        int mapSeed = packet.DecodeData<int>();
        MapManager.instance.mapSeed = mapSeed;
        Debug.Log(receivedMsg);
    }

    #endregion

    #region Send Fucntions
    public static void SuccessfulConnectionPacket(int playerId, string connMsg)
    {
        Packet packet = new Packet();
        packet.EncodeData<int>((int)ClientPacketId.SUCCESFULCONNECTION);
        packet.EncodeData<string>(connMsg);
        //packet.EncodeData<int>(playerId);

        Player.instance.tcpClient.SendData(packet.addedData.ToArray()); //SendDataById(playerId, packet);
    }

    #endregion

    public void HandlePacket(Packet packet)
    {
        int packetId = packet.DecodeData<int>();
        Debug.Log($"Handling packet with packet id: {packetId}");
        packetHandlers[(ServerPacketId)packetId](packet);
    }
}
