using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Wx.Runtime.Net
{
    public class ClientSession
    {
        private static ClientSession _instance;
        private static readonly object Syslock = new();

        public static ClientSession Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Syslock)
                {
                    _instance ??= new ClientSession();
                }
                return _instance;
            }
        }


        private TcpClient _client;

        private MemoryStream _memoryStream;

        private BinaryReader _binaryReader;

        private const int BufferSize = 4096;

        private CancellationTokenSource _cancellationTokenSource;

        private const int Timeout = 5000;

        public Action<byte[]> handleReceivedDataCall;

        public bool IsAvailable
        {
            get
            {
                return _client is { Connected: true };
            }
        }

        public async UniTask<(bool,string)> Connect(string address, int port,CancellationTokenSource cancellationTokenSource)
        {
            _memoryStream = new MemoryStream();
            _binaryReader = new BinaryReader(_memoryStream);

            _client = new TcpClient()
            {
                SendTimeout = 1000,
                ReceiveTimeout = 1000,
                NoDelay = true
            };
            _cancellationTokenSource = cancellationTokenSource;
            return await InternalConnect(address,port);
        }

        private async UniTask<(bool,string)> InternalConnect(string address,int port)
        {
            _cancellationTokenSource.CancelAfter(Timeout);
            var connectCall = string.Empty;
            try
            {
                var connectTask = _client.ConnectAsync(address, port).AsUniTask()
                    .AttachExternalCancellation(_cancellationTokenSource.Token);
                await connectTask;

                if (connectTask.Status == UniTaskStatus.Succeeded)
                {
                    connectCall = "Client Connect Success";
                    WLog.Log(connectCall);
                    ReceiveData().Forget();
                    return (true,connectCall);
                }
            }
            catch (OperationCanceledException) when (_cancellationTokenSource.IsCancellationRequested)
            {
                connectCall = "连接超时，或被取消";
                WLog.Log(connectCall);
                Release();
            }
            catch (Exception exception)
            {
                connectCall = "连接失败";
                WLog.Log($"连接失败:{exception}");
                Release();
            }

            return (false, connectCall);
        }

        private async UniTask ReceiveData()
        {
            try
            {
                while (IsAvailable)
                {
                    var byteBuffer = new byte[BufferSize];
                    var length = await _client.GetStream().ReadAsync(byteBuffer, 0, byteBuffer.Length, _cancellationTokenSource.Token);

                    if (length > 0)
                    {
                        HandleReceive(byteBuffer, length);
                    }
                    else
                    {
                        CloseConnect();
                    }
                }
            }
            catch(Exception ex)
            {
                WLog.Warning(ex.ToString());
            }
        }

        public void Release()
        {
            // 关闭网络连接
            CloseConnect();

            if (_binaryReader != null)
            {
                _binaryReader.Close();
                _binaryReader.Dispose();
                _binaryReader = null;
            }

            if (_memoryStream != null)
            {
                _memoryStream.Close();
                _memoryStream.Dispose();
                _memoryStream = null;
            }

            handleReceivedDataCall = null;

        }

        private void CloseConnect()
        {
            try
            {
                if(IsAvailable)
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        _cancellationTokenSource.Cancel();
                        _cancellationTokenSource.Dispose();
                    }
                    _client.Close();
                    WLog.Log("Client Close");
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                WLog.Warning(ex.ToString());
            }
            
        }

        //包体结构 (uint)length (uint)mainId (byte[])message
        //包体大小(2) 协议ID(2) 包体(byte[])
        private void HandleReceive(byte[] buffer,int length)
        {
            try
            {
                _memoryStream.Seek(0, SeekOrigin.End);
                _memoryStream.Write(buffer, 0, length);
                _memoryStream.Seek(0, SeekOrigin.Begin);

                while (_memoryStream.Length - _memoryStream.Position > 2)
                {
                    int msgLength = _binaryReader.ReadUInt16();
                    if (_memoryStream.Length - _memoryStream.Position >= msgLength)
                    {
                        HandleMessage(_binaryReader.ReadBytes(msgLength));
                    }
                    else
                    {
                        _memoryStream.Position -= 2;
                        break;
                    }
                }

                var leftBytes = _binaryReader.ReadBytes((int)(_memoryStream.Length - _memoryStream.Position));
                _memoryStream.SetLength(0);
                _memoryStream.Write(leftBytes, 0, leftBytes.Length);
            }
            catch (Exception ex)
            {
                WLog.Warning(ex.ToString());
            }
        }

        private void HandleMessage(byte[] msg)
        {
            handleReceivedDataCall?.Invoke(msg);
        }

        public void Send(byte[] data)
        {
            if (!IsAvailable)
            {
                WLog.Error("Client is onConnected");

                //TODO...发送断开连接事件
                Application.Quit();
            }
            try
            {
                InternalWrite(data).Forget();
            }
            catch (Exception ex)
            {
                WLog.Warning(ex);
            }
        }

        private async UniTask InternalWrite(byte[] data)
        {
            if (!IsAvailable)
            {
                WLog.Error("Client is onConnected");
            }
            try
            {
                await _client.GetStream().WriteAsync(data, _cancellationTokenSource.Token);
            }
            catch(Exception ex)
            {
                WLog.Warning(ex);
            }
        }
    }
}
