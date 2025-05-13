using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ServNet : MonoBehaviour
{
    /// <summary>
    /// 监听嵌套字
    /// </summary>
    public Socket listenfd;

    /// <summary>
    /// 客户端连接
    /// </summary>
    public Conn[] conns;

    /// <summary>
    /// 最大连接数
    /// </summary>
    public int maxConn = 50;

    /// <summary>
    /// 单例
    /// </summary>
    public static ServNet Instance;

    //消息委托类型
    public delegate void MsgListener(Conn conn, MsgBase msgBase);

    //消息监听列表
    private Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

    // 信息队列，用于在附加线程与主线程之间共享数据
    private Queue<SwapDataStruct> msgList = new Queue<SwapDataStruct>();

    private Dictionary<SwapDataStruct, bool> msgDIC = new Dictionary<SwapDataStruct, bool>();

    // 兼容原有功能的数据结构
    struct SwapDataStruct
    {
        public MsgBase msg;
        public Conn conn;
    }
    
    /// <summary>
    /// 当有连接时
    /// </summary>
    public Action<Conn> OnSomeOneConnect;
    
    /// <summary>
    /// 设置消息缓存字典的消息状态
    /// </summary>
    /// <param name="msgBase"></param>
    /// <param name="state">默认为true</param>
    public void SetCacheMsgBaseState(MsgBase msgBase, bool state = true)
    {
        foreach (var item in msgDIC)
        {
            if (item.Key.msg.Equals(msgBase))
            {
                msgDIC[item.Key] = state;
                break;
            }
        }
    }

    /// <summary>
    /// 主线程中分发消息，避免错误
    /// </summary>
    private void Update()
    {
        if (msgList.Count > 0)
        {
            var msgBase = msgList.Peek();

            if (msgDIC.ContainsKey(msgBase) && msgDIC[msgBase])
            {
                msgDIC.Remove(msgBase);
                msgList.Dequeue();
                
                return;
            }

            FireMsg(msgBase.msg.protoName, msgBase.conn, msgBase.msg);
        }
    }

    //添加消息监听
    public void AddMsgListener(string msgName, MsgListener listener)
    {
        //添加
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        }
        //新增
        else
        {
            msgListeners[msgName] = listener;
        }
    }

    //删除消息监听
    public void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;
        }
    }

    //分发消息
    private void FireMsg(string msgName, Conn conn, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](conn, msgBase);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 获取连接池索引，返回负数表示获取失败
    /// </summary>
    public int NewIndex()
    {
        if (conns == null)
        {
            return -1;
        }

        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i].isUse == false)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 开启服务器
    /// </summary>
    public void StartServer(string host, int port)
    {
        //连接池
        conns = new Conn[maxConn];
        for (int i = 0; i < maxConn; i++)
        {
            conns[i] = new Conn();
        }

        //Socket
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Bind
        IPAddress ipAdr = IPAddress.Parse(host);
        IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
        listenfd.Bind(ipEp);
        //Listen
        listenfd.Listen(maxConn);
        //Accept
        listenfd.BeginAccept(AcceptCb, null);

        Debug.LogErrorFormat("服务器开启成功 @ {0}.{1}", host, port);
    }

    /// <summary>
    /// Accept 回调
    /// </summary>
    private void AcceptCb(IAsyncResult ar)
    {
        try
        {
            if (listenfd == null)
            {
                return;
            }

            Socket socket = listenfd.EndAccept(ar);

            int index = NewIndex();

            if (index < 0)
            {
                socket.Close();
                Debug.Log("连接已满");
            }
            else
            {
                Conn conn = conns[index];
                conn.Init(socket);
                string adr = conn.GetAdress();
                foreach (var c in conns)
                {
                    if (c.player != null)
                        Debug.Log("=====> " + c.player.id);
                }

                Debug.LogError($"客户端<color=green>[{adr}]</color>已连接，池ID:{index}");
                OnSomeOneConnect?.Invoke(conn);
                
                conn.socket.BeginReceive(conn.readBuff.bytes, conn.readBuff.writeIdx, conn.readBuff.remain,
                    SocketFlags.None, ReceiveCb, conn);
                listenfd.BeginAccept(AcceptCb, null);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Accept失败：" + e.Message);
        }
    }

    /// <summary>
    /// Receive 回调
    /// </summary>
    private void ReceiveCb(IAsyncResult ar)
    {
        Conn conn = (Conn)ar.AsyncState;

        //try
        {
            int count = conn.socket.EndReceive(ar);
            conn.readBuff.writeIdx += count;
            //关闭
            if (count <= 0)
            {
                Debug.LogError($"客户端<color=red> [{conn.GetAdress()}] </color>断开连接 PlayerId={conn.player?.id}" + "  count:" + count);
                conn.Close();
                return;
            }

            //处理数据
            ProcessData(conn);
            //继续接收
            if (conn.readBuff.remain < 8)
            {
                conn.readBuff.MoveBytes();
                conn.readBuff.ReSize(conn.readBuff.length * 2);
            }

            conn.socket.BeginReceive(conn.readBuff.bytes, conn.readBuff.writeIdx, conn.readBuff.remain,
                SocketFlags.None, ReceiveCb, conn);
        }
        // catch (Exception e)
        // {
        //     Debug.LogError($"客户端<color=yellow> [{conn.GetAdress()}] </color>异常断开连接\n{e.Message}\n{e.StackTrace}");
        //     conn.Close();
        // }
    }

    /// <summary>
    /// 处理一个客户端发送来的数据
    /// </summary>
    private void ProcessData(Conn conn)
    {
        //小于长度字节
        if (conn.readBuff.length <= 3)
        {
            return;
        }

        //获取消息体长度
        int readIdx = conn.readBuff.readIdx;

        byte[] bytes = conn.readBuff.bytes;

        //Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);

        int bodyLength = bytes[readIdx] + bytes[readIdx + 1] * 256 + bytes[readIdx + 2] * 65536;

        if (conn.readBuff.length < bodyLength)
        {
            return;
        }

        conn.readBuff.readIdx += 3;

        //解析协议名
        int nameCount = 0;

        string protoName = MsgBase.DecodeName(conn.readBuff.bytes, conn.readBuff.readIdx, out nameCount);

        if (protoName == "")
        {
            Debug.LogError("OnReceiveData MsgBase.DecodeName fail");
            return;
        }

        Debug.LogWarning(protoName);

        conn.readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;

        MsgBase msgBase = MsgBase.Decode(protoName, conn.readBuff.bytes, conn.readBuff.readIdx, bodyCount);

        conn.readBuff.readIdx += bodyCount;

        conn.readBuff.CheckAndMoveBytes();

        //处理消息 msgBase
        //分发消息
        if (msgBase != null)
        {
            // FireMsg(msgBase.protoName, conn, msgBase);
            SwapDataStruct data = new SwapDataStruct();
            data.msg = msgBase;
            data.conn = conn;
            //添加进缓存
            msgList.Enqueue(data);
            msgDIC.Add(data, false);
        }
        else
        {
            Debug.LogWarning("OnReceiveData MsgBase.Decode fail");
        }

        //继续读取消息
        if (conn.readBuff.length > 3)
        {
            ProcessData(conn);
        }
    }


    //发送
    public void Send(Conn conn, MsgBase msg)
    {
        //状态判断
        if (conn == null)
        {
            return;
        }

        if (!conn.socket.Connected)
        {
            return;
        }

        //数据编码
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[3 + len];
        //组装长度
        //组装长度
        sendBytes[0] = (byte)((len % 65536) % 256);
        sendBytes[1] = (byte)((len % 65536) / 256);
        sendBytes[2] = (byte)(len / 65536);

        //组装名字
        Array.Copy(nameBytes, 0, sendBytes, 3, nameBytes.Length);
        //组装消息体
        Array.Copy(bodyBytes, 0, sendBytes, 3 + nameBytes.Length, bodyBytes.Length);
        //为简化代码，不设置回调
        try
        {
            conn.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Socket Close on BeginSend" + ex.ToString());
        }
    }

    //关闭
    public void Close()
    {
        if (conns == null)
        {
            return;
        }

        for (int i = 0; i < conns.Length; i++)
        {
            Conn conn = conns[i];
            if (conn == null)
            {
                continue;
            }

            if (!conn.isUse)
            {
                continue;
            }

            lock (conn)
            {
                conn.Close();
            }
        }

        listenfd.Close();

        listenfd = null;
    }

    /// <summary>
    /// 向所有连接广播
    /// </summary>
    /// <param name="msg"></param>
    public void Broadcast(MsgBase msg)
    {
        foreach (Conn c in conns)
        {
            if (c.isUse)
            {
                Send(c, msg);
            }
        }
    }
    
    /// <summary>
    /// 定向广播消息
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="conn"></param>
    public void Broadcast(MsgBase msg, Conn conn)
    {
        if (conn != null)
        {
            Send(conn, msg);
        }
    }
}