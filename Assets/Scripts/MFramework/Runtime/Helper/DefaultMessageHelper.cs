using Google.Protobuf;
using System;
using System.IO;
using Wx.Runtime.Net;
using UnityEngine;

namespace Wx.Runtime
{
    public class DefaultMessageHelper : MessageHelperBase
    {
        public override (ushort, byte[]) HandleReceivedData(byte[] data)
        {
            using MemoryStream memoryStream = new();
            BinaryWriter binaryWriter = new(memoryStream);
            binaryWriter.Write(data);
            memoryStream.Seek(0, SeekOrigin.Begin);
            BinaryReader binaryReader = new(memoryStream);
            ushort mainId = binaryReader.ReadUInt16();
            byte[] msgData = binaryReader.ReadBytes((int)(memoryStream.Length - memoryStream.Position));
            return (mainId, msgData);
        }

        public override byte[] HandleMessasge(ushort mainId,IMessage message)
        {
            byte[] msg = message.ToByteArray();
            ushort length = (ushort)(msg.Length + 2);
            using MemoryStream memoryStream = new();
            BinaryWriter writer = new(memoryStream);
            BinaryReader reader = new(memoryStream);
            writer.Write(BitConverter.GetBytes(length));
            writer.Write(BitConverter.GetBytes(mainId));
            writer.Write(msg);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return reader.ReadBytes((int)(memoryStream.Length));
        }
    }
}
