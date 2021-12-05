using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ServerPacketId
{
    FIRSTCONNECTION,
};

public enum ClientPacketId
{
    SUCCESFULCONNECTION
};

public class Packet
{

    public byte[] packetData { get; private set; }
    public List<byte> addedData { get; private set; }
    private int readBytes = 0;

    public Packet()
    {
        addedData = new List<byte>();
    }

    public Packet(byte[] _packetData)
    {
        packetData = _packetData;
        readBytes = 0;
    }


    private float DecodeFloat()
    {
        float decodedValue = BitConverter.ToSingle(packetData, readBytes);
        readBytes += sizeof(float);
        return decodedValue;
    }

    public T DecodeData<T>()
    {
        //Decode integer from packet
        if (typeof(T) == typeof(int))
        {
            T decodedValue = (T)(object)BitConverter.ToInt32(packetData, readBytes);
            readBytes += sizeof(int);
            return decodedValue;
        }
        //Decode float from packet
        else if (typeof(T) == typeof(float))
        {
            return (T)(object)DecodeFloat();
        }
        //Decode bool from packet
        else if (typeof(T) == typeof(bool))
        {
            T decodedValue = (T)(object)BitConverter.ToBoolean(packetData, readBytes);
            readBytes += sizeof(bool);
            return decodedValue;
        }
        //Decode string from packet
        else if (typeof(T) == typeof(string))
        {
            int stringByteLength = BitConverter.ToInt32(packetData, readBytes);
            readBytes += sizeof(int);
            T decodedValue = (T)(object)Encoding.ASCII.GetString(packetData, readBytes, stringByteLength);
            readBytes += stringByteLength;
            return decodedValue;
        }
        //Decode Vector2 from packet 
        else if (typeof(T) == typeof(Vector2))
        {
            float x = DecodeFloat();
            float y = DecodeFloat();
            return (T)(object)new Vector2(x, y);
        }
        //Decode Vector3 from packet 
        else if (typeof(T) == typeof(Vector3))
        {
            float x = DecodeFloat();
            float y = DecodeFloat();
            float z = DecodeFloat();
            return (T)(object)new Vector3(x, y, z);
        }
        //Decode Quaternion from packet 
        else if (typeof(T) == typeof(Quaternion))
        {
            float x = DecodeFloat();
            float y = DecodeFloat();
            float z = DecodeFloat();
            float w = DecodeFloat();
            return (T)(object)new Quaternion(x, y, z, w);
        }

        return default(T);
    }

    public void EncodeData<T>(object data)
    {
        //Decode integer from packet
        if (typeof(T) == typeof(int))
        {
            addedData.InsertRange(0, BitConverter.GetBytes((int)data));
        }
        //Decode float from packet
        else if (typeof(T) == typeof(float))
        {
            addedData.InsertRange(0, BitConverter.GetBytes((float)data));
        }
        //Decode bool from packet
        else if (typeof(T) == typeof(bool))
        {
            addedData.InsertRange(0, BitConverter.GetBytes((bool)data));
        }
        //Decode string from packet
        else if (typeof(T) == typeof(string))
        {
            byte[] stringLength = BitConverter.GetBytes(((string)data).Length);
            byte[] stringData = Encoding.ASCII.GetBytes((string)data);
            addedData.InsertRange(0, stringLength.Concat(stringData));
        }
        //Decode Vector2 from packet 
        else if (typeof(T) == typeof(Vector2))
        {
            Vector2 vectorData = (Vector2)data;
            byte[] packetData = new byte[sizeof(float) * 2];
            packetData = packetData
            .Concat(BitConverter.GetBytes(vectorData.x))
            .Concat(BitConverter.GetBytes(vectorData.y)).ToArray();
            addedData.InsertRange(0, packetData);
        }
        //Decode Vector3 from packet 
        else if (typeof(T) == typeof(Vector3))
        {
            Vector3 vectorData = (Vector3)data;
            byte[] packetData = new byte[sizeof(float) * 3];
            packetData = packetData
                .Concat(BitConverter.GetBytes(vectorData.x))
                .Concat(BitConverter.GetBytes(vectorData.y))
                .Concat(BitConverter.GetBytes(vectorData.z)).ToArray();
            addedData.InsertRange(0, packetData);
        }
        //Decode Quaternion from packet 
        else if (typeof(T) == typeof(Quaternion))
        {
            Quaternion rotationData = (Quaternion)data;
            byte[] packetData = new byte[sizeof(float) * 4];
            packetData = packetData
                .Concat(BitConverter.GetBytes(rotationData.x))
                .Concat(BitConverter.GetBytes(rotationData.y))
                .Concat(BitConverter.GetBytes(rotationData.z))
                .Concat(BitConverter.GetBytes(rotationData.w))
                .ToArray();
            addedData.InsertRange(0, packetData);
        }
    }
}
