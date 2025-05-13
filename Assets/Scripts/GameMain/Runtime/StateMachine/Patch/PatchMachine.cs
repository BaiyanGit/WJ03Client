using Wx.Runtime.Event;
using Wx.Runtime.Machine;
using Wx.Runtime.Singleton;

namespace GameMain.Runtime
{
    public class PatchMachine : SingletonInstance<PatchMachine>,ISingleton
    {
        private readonly EventGroup _eventGroup = new();
        private WMachine _machine;
        public void OnCreate(object createParam)
        {
            // 注册监听事件
            _eventGroup.AddListener<UserEventDefine.UserTryInitialize>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);
            
            // 创建状态机
            _machine = new WMachine(this);
            _machine.AddNode<FsmInitializePackage>();
            _machine.AddNode<FsmUpdatePackageVersion>();
            _machine.AddNode<FsmUpdatePackageManifest>();
            _machine.AddNode<FsmCreatePackageDownloader>();
            _machine.AddNode<FsmDownloadPackageFiles>();
            _machine.AddNode<FsmDownloadPackageOver>();
            _machine.AddNode<FsmClearPackageCache>();
            _machine.AddNode<FsmUpdaterDone>();
            
            _machine.Run<FsmInitializePackage>();
        }

        public void OnUpdate()
        {
            _machine.Update();
        }

        public void OnFixedUpdate()
        {
            _machine.FixedUpdate();
        }

        public void OnLateUpdate()
        {
            _machine.LateUpdate();
        }

        public void OnDestroy()
        {
            _machine.Destroy();
            _eventGroup.RemoveAllListener();
        }
        
        /// <summary>
        /// 接收事件
        /// </summary>
        private void OnHandleEventMessage(IEventMessage message)
        {
            switch (message)
            {
                case UserEventDefine.UserTryInitialize:
                    _machine.ChangeState<FsmInitializePackage>();
                    break;
                case UserEventDefine.UserBeginDownloadWebFiles:
                    _machine.ChangeState<FsmDownloadPackageFiles>();
                    break;
                case UserEventDefine.UserTryUpdatePackageVersion:
                    _machine.ChangeState<FsmUpdatePackageVersion>();
                    break;
                case UserEventDefine.UserTryUpdatePatchManifest:
                    _machine.ChangeState<FsmUpdatePackageManifest>();
                    break;
                case UserEventDefine.UserTryDownloadWebFiles:
                    _machine.ChangeState<FsmCreatePackageDownloader>();
                    break;
                default:
                    throw new System.NotImplementedException($"{message.GetType()}");
            }
        }
    }
}
