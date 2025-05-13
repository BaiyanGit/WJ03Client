using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Wx.Runtime.Sever
{
    public static class SeverSocketsDataHelper
    {
        public static List<Socket> connectSockets = new();
    }
}
