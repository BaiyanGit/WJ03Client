using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Wx.Runtime.Net
{
    public class WNet: WModule
    {
        [SerializeField]
        private string mMessageHelperTypeName = "Wx.Runtime.DefaultMessageHelper";

        [SerializeField]
        private MessageHelperBase mCustomMessageHelper = null;

        [SerializeField]
        private MessageHelperBase messageHelper = null;

        [SerializeField]
        private bool heartBeat = true;

        [SerializeField, Range(1,10)]
        private int heartBeatTime = 1;

        public override int Priority => 9;

        private ClientSession _clientListener;

        private readonly Dictionary<ushort, LinkedList<Action<IMessage>>> _listeners = new(1000);

        private readonly Dictionary<ushort, Type> _typePair = new(1000);

        private readonly Queue<KeyValuePair<ushort, byte[]>> _receiveMessageCache = new();

        private readonly Queue<byte[]> _sendMessageCache = new();

        private CancellationTokenSource _cancellationTokenSource;


        protected override void Awake()
        {
            base.Awake();

            messageHelper = Helper.CreateHelper(mMessageHelperTypeName, mCustomMessageHelper);
            if(messageHelper == null)
            {
                WLog.Log("Can not create message helper");
                return;
            }

            messageHelper.name = "Message Helper";
            var thisTransform = messageHelper.transform;
            thisTransform.SetParent(this.transform);
            thisTransform.localPosition = Vector3.zero;
            thisTransform.localRotation = Quaternion.identity;
            thisTransform.localScale = Vector3.one;

            _clientListener = ClientSession.Instance;

            WLog.Log($"{nameof(WNet)} Initialize");
        }

        public async UniTask<(bool,string)> CreateClient(string address, int port)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var (connect,connectCall)  = await _clientListener.Connect(address, port, _cancellationTokenSource);

            if (connect)
            {
                _clientListener.handleReceivedDataCall += HandleReceivedData;

                if (heartBeat)
                {
                    SendHeartbeatPacket().Forget();
                }
            }
            return (connect,connectCall);
        }

        public void CloseClient()
        {
            _clientListener.Release();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            CloseClient();
        }

        private void HandleReceivedData(byte[] msg)
        {
            var (mainId, data) = messageHelper.HandleReceivedData(msg);
            lock (_receiveMessageCache)
            {
                _receiveMessageCache.Enqueue(new KeyValuePair<ushort, byte[]>(mainId, data));
            }
        }

        public void SendMessage(ushort mainId, IMessage msg)
        {
            var data = messageHelper.HandleMessasge(mainId, msg);
            lock (_sendMessageCache)
            {
                _sendMessageCache.Enqueue(data);
            }
        }


        private async UniTask SendHeartbeatPacket()
        {
            while (_clientListener.IsAvailable)
            {
                Heartbeat heartbeat = new()
                {
                    Time = DateTime.Now.ToString("yyyy MM dd HH mm ss")
                };
                var data = messageHelper.HandleMessasge(3, heartbeat);
                lock (_sendMessageCache)
                {
                    _sendMessageCache.Enqueue(data);
                }
                await UniTask.Delay(heartBeatTime * 1000, ignoreTimeScale: false, PlayerLoopTiming.Update, _cancellationTokenSource.Token);
            }
        }

        public void AddNetListener(ushort mainId, Type eventType, Action<IMessage> listener)
        {
            _typePair.TryAdd(mainId, eventType);

            if (!_listeners.ContainsKey(mainId))
                _listeners.Add(mainId, new LinkedList<Action<IMessage>>());
            if (!_listeners[mainId].Contains(listener))
                _listeners[mainId].AddLast(listener);
        }

        /// <summary>
        /// ÒÆ³ý¼àÌý
        /// </summary>
        public void RemoveNetListener(ushort mainId, System.Action<IMessage> listener)
        {
            if (_typePair.ContainsKey(mainId))
            {
                _typePair.Remove(mainId);
            }

            if (!_listeners.ContainsKey(mainId)) return;
            if (_listeners[mainId].Contains(listener))
                _listeners[mainId].Remove(listener);
        }

        private void BroadcastNetEvent(ushort mainId,System.Type eventType, IMessage msg)
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
                    var instance = Activator.CreateInstance(netEventType) as IMessage;
                    BroadcastNetEvent(cachePair.Key,netEventType, instance?.Descriptor.Parser.ParseFrom(cachePair.Value));
                }
            }
        }

        private void SendCacheMessage()
        {
            lock (_sendMessageCache)
            {
                if (_sendMessageCache.Count <= 0) return;
                lock (_sendMessageCache)
                {
                    var data = _sendMessageCache.Dequeue();
                    _clientListener.Send(data);
                }
            }
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            SendCacheMessage();
            BroadcastCacheMessage();
        }

    }
}
