using System;
using System.Collections.Generic;

namespace Wx.Runtime.Machine
{
    public class WMachine
    {
        private readonly Dictionary<string, object> _blackboard = new Dictionary<string, object>(100);
        private readonly Dictionary<string, IStateNode> _nodes = new Dictionary<string, IStateNode>(100);
        private IStateNode _curNode;
        private IStateNode _preNode;

        /// <summary>
        /// 状态机持有者
        /// </summary>
        public object Owner { private set; get; }

        /// <summary>
        /// 当前运行的节点名称
        /// </summary>
        public string CurrentNode
        {
            get { return _curNode != null ? _curNode.GetType().FullName : string.Empty; }
        }

        /// <summary>
        /// 之前运行的节点名称
        /// </summary>
        public string PreviousNode
        {
            get { return _preNode != null ? _preNode.GetType().FullName : string.Empty; }
        }


        private WMachine() { }
        public WMachine(object owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// 更新状态机
        /// </summary>
        public void Update()
        {
            _curNode?.OnUpdate();
        }

        public void FixedUpdate()
        {
            _curNode?.OnFixedUpdate();
        }

        public void LateUpdate()
        {
            _curNode?.OnLateUpdate();
        }

        public void Destroy()
        {
            _curNode?.OnDestroy();
        }

        /// <summary>
        /// 启动状态机
        /// </summary>
        public void Run<TNode>(params object[] datas) where TNode : IStateNode
        {
            var nodeType = typeof(TNode);
            var nodeName = nodeType.FullName;
            Run(nodeName,datas);
        }
        public void Run(Type entryNode,params object[] datas)
        {
            var nodeName = entryNode.FullName;
            Run(nodeName,datas);
        }
        public void Run(string entryNode,params object[] datas)
        {
            _curNode = TryGetNode(entryNode);
            _preNode = _curNode;

            if (_curNode == null)
                throw new Exception($"Not found entry node: {entryNode}");

            _curNode.OnEnter(datas);
        }

        /// <summary>
        /// 加入一个节点
        /// </summary>
        public void AddNode<TNode>() where TNode : IStateNode
        {
            var nodeType = typeof(TNode);
            var stateNode = Activator.CreateInstance(nodeType) as IStateNode;
            AddNode(stateNode);
        }
        public void AddNode(IStateNode stateNode)
        {
            if (stateNode == null)
                throw new ArgumentNullException();

            var nodeType = stateNode.GetType();
            var nodeName = nodeType.FullName;

            if (_nodes.ContainsKey(nodeName) == false)
            {
                stateNode.OnCreate(this);
                _nodes.Add(nodeName, stateNode);
            }
            else
            {
                WLog.Error($"State node already existed : {nodeName}");
            }
        }

        /// <summary>
        /// 转换状态节点
        /// </summary>
        public void ChangeState<TNode>(params object[] datas) where TNode : IStateNode
        {
            var nodeType = typeof(TNode);
            var nodeName = nodeType.FullName;
            ChangeState(nodeName,datas);
        }
        public void ChangeState(Type nodeType,params object[] datas)
        {
            var nodeName = nodeType.FullName;
            ChangeState(nodeName,datas);
        }
        public void ChangeState(string nodeName,params object[] datas)
        {
            if (string.IsNullOrEmpty(nodeName))
                throw new ArgumentNullException();

            var node = TryGetNode(nodeName);
            if (node == null)
            {
                WLog.Error($"Can not found state node : {nodeName}");
                return;
            }

            // WLog.Log($"{_curNode.GetType().FullName} --> {node.GetType().FullName}");
            _preNode = _curNode;
            _curNode.OnExit();
            _curNode = node;
            _curNode.OnEnter(datas);
        }
        
        /// <summary>
        /// 设置黑板数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBlackboardValue(string key, object value)
        {
            if (_blackboard.ContainsKey(key) == false)
                _blackboard.Add(key, value);
            else
                _blackboard[key] = value;
        }

        /// <summary>
        /// 获取黑板数据
        /// </summary>
        public object GetBlackboardValue(string key)
        {
            if (_blackboard.TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                WLog.Warning($"Not found blackboard value : {key}");
                return null;
            }
        }

        private IStateNode TryGetNode(string nodeName)
        {
            _nodes.TryGetValue(nodeName, out var result);
            return result;
        }
    }
}