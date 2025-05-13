using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Wx.Runtime.Net;

namespace Wx.Runtime.Sever
{
    public struct SocketTh
    {
        public Thread thread;
        public Socket socket;
    }

    public class WSever : WModule
    {
        private Socket _severSocket;

        private string _address;
        private int _port;

        private const int MaxListenerCount = 10;
        private const int BufferSize = 4096;

        private MemoryStream _memStream;
        private BinaryReader _reader;

        private Thread _waiter;

        private CancellationTokenSource _cancellationTokenSource;

        private Dictionary<System.Type, List<Action<IMessage>>> _cachedListener = new();

        private readonly Dictionary<ushort, LinkedList<Action<IMessage>>> _listeners = new(1000);

        private readonly Dictionary<ushort, Type> _typePair = new(1000);

        private readonly Queue<KeyValuePair<ushort, byte[]>> _receiveMessageCache = new();

        private readonly Queue<KeyValuePair<Socket, byte[]>> _sendMessageCache = new();


        [SerializeField] private string mMessageHelperTypeName = "Wx.Runtime.DefaultMessageHelper";

        [SerializeField] private MessageHelperBase mCustomMessageHelper = null;

        [SerializeField] private MessageHelperBase messageHelper = null;

        [SerializeField] private bool heartBeat = false;

        [SerializeField, Range(1, 10)] private int heartBeatTime = 1;

        public override int Priority => 8;

        protected override void Awake()
        {
            base.Awake();
            messageHelper = Helper.CreateHelper(mMessageHelperTypeName, mCustomMessageHelper);
            messageHelper.name = "Message Helper";

            var thisTransform = messageHelper.transform;
            thisTransform.SetParent(this.transform);
            thisTransform.localPosition = Vector3.zero;
            thisTransform.localRotation = Quaternion.identity;
            thisTransform.localScale = Vector3.one;

            WLog.Log($"{nameof(WSever)} Initialize");
        }


        private bool IsAvailable(Socket socket)
        {
            return socket is { Connected: true };
        }

        public void StartSever(string address, int port)
        {
            _memStream = new MemoryStream();
            _reader = new BinaryReader(_memStream);

            _cancellationTokenSource = new CancellationTokenSource();

            _address = address;
            _port = port;
            _waiter = new Thread(new ThreadStart(WaitClient));
            _waiter.Start();
            WLog.Log("Sever Start");
        }

        public void StopSever()
        {
            if (_severSocket == null) return;

            WLog.Log("Sever Stop");

            _cancellationTokenSource.Cancel();
            _waiter.Abort();
            _severSocket.Close();
            _severSocket = null;

            if (_memStream != null)
            {
                _memStream.Dispose();
                _memStream.Close();
                _memStream = null;
            }

            if (_reader == null) return;
            _reader.Dispose();
            _reader.Close();
            _reader = null;
        }

        private void WaitClient()
        {
            _severSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _severSocket.Bind(new IPEndPoint(IPAddress.Parse(_address), _port));

            _severSocket.Listen(MaxListenerCount);
            WLog.Log("\nServer is working..");
            WLog.Log(DateTime.Now.ToString(CultureInfo.InvariantCulture) + '\n');

            if (heartBeat)
            {
                HeartbeatPackage().Forget();
            }

            while (true)
            {
                var sclient = _severSocket.Accept();
                Thread newthd = new(new ParameterizedThreadStart(ListenClient));
                SocketTh socketTh = new()
                {
                    thread = newthd,
                    socket = sclient
                };
                newthd.Start(socketTh);
            }
        }


        private void ListenClient(object obj)
        {
            var t = (SocketTh)obj;

            var sclient = t.socket;

            if (!SeverSocketsDataHelper.connectSockets.Contains(sclient))
            {
                SeverSocketsDataHelper.connectSockets.Add(sclient);
            }

            var iPEndPoint = (IPEndPoint)sclient.RemoteEndPoint;
            WLog.Error($"Client Address :{iPEndPoint.Address} Port :{iPEndPoint.Port}");

            var buffer = new byte[BufferSize];


            while (IsAvailable(sclient))
            {
                try
                {
                    var length = sclient.Receive(buffer, 0, BufferSize, SocketFlags.None);
                    HandleData(buffer, length);
                }
                catch (Exception ex)
                {
                    t.thread.Abort();
                    if (SeverSocketsDataHelper.connectSockets.Contains(sclient))
                    {
                        SeverSocketsDataHelper.connectSockets.Remove(sclient);
                    }

                    WLog.Warning(ex.ToString());
                }
            }
        }

        private void HandleData(byte[] data, int length)
        {
            _memStream.Seek(0, SeekOrigin.End);
            _memStream.Write(data, 0, length);
            _memStream.Seek(0, SeekOrigin.Begin);

            while (_memStream.Length - _memStream.Position > 2)
            {
                int msgLength = _reader.ReadUInt16();
                if (_memStream.Length - _memStream.Position >= msgLength)
                {
                    HandleReceiveMessage(_reader.ReadBytes(msgLength));
                }
                else
                {
                    _memStream.Position -= 2;
                    break;
                }
            }

            var leftBytes = _reader.ReadBytes((int)(_memStream.Length - _memStream.Position));
            _memStream.SetLength(0);
            _memStream.Write(leftBytes, 0, leftBytes.Length);
        }

        private void HandleReceiveMessage(byte[] msg)
        {
            var (mainId, data) = messageHelper.HandleReceivedData(msg);

            lock (_receiveMessageCache)
            {
                _receiveMessageCache.Enqueue(new KeyValuePair<ushort, byte[]>(mainId, data));
            }
        }

        public void SendToClient(Socket client, ushort mainId, IMessage message)
        {
            var data = messageHelper.HandleMessasge(mainId, message);
            lock (_sendMessageCache)
            {
                _sendMessageCache.Enqueue(new KeyValuePair<Socket, byte[]>(client, data));
            }
        }

        public void SendToAllClient(ushort mainId, IMessage message)
        {
            var data = messageHelper.HandleMessasge(mainId, message);
            lock (_sendMessageCache)
            {
                _sendMessageCache.Enqueue(new KeyValuePair<Socket, byte[]>(null, data));
            }
        }

        private async UniTask HeartbeatPackage()
        {
            while (true)
            {
                lock (_sendMessageCache)
                {
                    Heartbeat heartbeat = new()
                    {
                        Time = DateTime.Now.ToString("yyyy MM dd HH mm ss")
                    };
                    var data = messageHelper.HandleMessasge(3, heartbeat);

                    _sendMessageCache.Enqueue(new KeyValuePair<Socket, byte[]>(null, data));
                }

                await UniTask.Delay(heartBeatTime * 1000, ignoreTimeScale: false, PlayerLoopTiming.Update, _cancellationTokenSource.Token);
            }
        }

        private void SendCacheMessasge()
        {
            if (_sendMessageCache == null) return;
            lock (_sendMessageCache)
            {
                if (_sendMessageCache.Count <= 0) return;
                lock (_sendMessageCache)
                {
                    try
                    {
                        var (socket, data) = _sendMessageCache.Dequeue();
                        if (socket == null)
                        {
                            foreach (var caCheSocket in SeverSocketsDataHelper.connectSockets.Where(caCheSocket => IsAvailable(caCheSocket)))
                            {
                                caCheSocket.Send(data);
                            }
                        }
                        else
                        {
                            socket.Send(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        WLog.Warning(ex.ToString());
                    }
                }
            }
        }

        private void BroadcastSeverEvent(ushort mainId, System.Type eventType, IMessage msg)
        {
            if (!_listeners.ContainsKey(mainId) || !_typePair.ContainsKey(mainId))
            {
                return;
            }

            var listeners = _listeners[mainId];
            if (listeners.Count <= 0) return;
            var currentNode = listeners.Last;
            while (currentNode != null)
            {
                currentNode.Value.Invoke(msg);
                currentNode = currentNode.Previous;
            }
        }

        private void BroadcastCacheMessage()
        {
            if (_receiveMessageCache == null) return;
            lock (_receiveMessageCache)
            {
                if (_receiveMessageCache.Count <= 0) return;
                lock (_receiveMessageCache)
                {
                    var cachePair = _receiveMessageCache.Dequeue();
                    if (!_typePair.ContainsKey(cachePair.Key) || !_listeners.ContainsKey(cachePair.Key))
                    {
                        WLog.Warning($"Net Message dont hava listener. mainId:{cachePair.Key}");
                        return;
                    }

                    var netEventType = _typePair[cachePair.Key];
                    if (Activator.CreateInstance(netEventType) is IMessage instance)
                        BroadcastSeverEvent(cachePair.Key, netEventType,
                            instance.Descriptor.Parser.ParseFrom(cachePair.Value));
                }
            }
        }


        public void AddSeverListener(ushort mainId, Type eventType, Action<IMessage> listener)
        {
            _typePair.TryAdd(mainId, eventType);

            if (!_listeners.ContainsKey(mainId))
                _listeners.Add(mainId, new LinkedList<Action<IMessage>>());
            if (!_listeners[mainId].Contains(listener))
                _listeners[mainId].AddLast(listener);
        }

        public void AddSeverListener<T>(ushort mainId, Action<IMessage> listener) where T : IMessage
        {
            var eventType = typeof(T);
            _typePair.TryAdd(mainId, eventType);

            if (!_listeners.ContainsKey(mainId))
                _listeners.Add(mainId, new LinkedList<Action<IMessage>>());
            if (!_listeners[mainId].Contains(listener))
                _listeners[mainId].AddLast(listener);
        }


        private void RemoveSeverListener(ushort mainId, System.Action<IMessage> listener)
        {
            if (_typePair.ContainsKey(mainId))
            {
                _typePair.Remove(mainId);
            }

            if (!_listeners.ContainsKey(mainId)) return;
            if (_listeners[mainId].Contains(listener))
                _listeners[mainId].Remove(listener);
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            BroadcastCacheMessage();
            SendCacheMessasge();
        }

        public override void OnDestroy()
        {
            StopSever();
        }
    }
}