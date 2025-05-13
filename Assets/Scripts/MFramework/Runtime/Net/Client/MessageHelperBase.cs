using Google.Protobuf;
using UnityEngine;

namespace Wx.Runtime.Net
{
    public abstract class MessageHelperBase : MonoBehaviour
    {
        public abstract (ushort, byte[]) HandleReceivedData(byte[] data);

        public abstract byte[] HandleMessasge(ushort mainId, IMessage message);
    }
    
}
