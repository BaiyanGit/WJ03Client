using Wx.Runtime.Machine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hotfix.Event;
using Hotfix.UI;
using Wx.Runtime.Event;
using System.Diagnostics;
using UnityEngine;

namespace Hotfix
{
    using Debug = UnityEngine.Debug;

    public class GameMachineState : IStateNode
    {
        private WMachine _machine;
        private CancellationTokenSource _cancellationTokenSource = new();

        private WMachine _subMachine;
        private readonly EventGroup _eventGroup = new();

        void IStateNode.OnCreate(WMachine machine)
        {
            _machine = machine;

            _subMachine = new WMachine(this);
            _subMachine.AddNode<StateLoadMainScene>();
            _subMachine.AddNode<StateSelectTopic>();
            _subMachine.AddNode<StateTeachingDemo>();
            _subMachine.AddNode<StateTrain>();
            _subMachine.AddNode<StateAssessment>();
        }

        void IStateNode.OnEnter(params object[] data)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _eventGroup.AddListener<ProcessEventDefine.SelectTopicCall>(OnSelectTopicHandle);

            _subMachine.Run<StateLoadMainScene>();
        }

        void IStateNode.OnExit()
        {
            _eventGroup.RemoveAllListener();
            _cancellationTokenSource.Cancel();
        }

        void IStateNode.OnUpdate()
        {
            _subMachine.Update();
        }

        void IStateNode.OnFixedUpdate()
        {
            _subMachine.FixedUpdate();
        }

        void IStateNode.OnLateUpdate()
        {
            _subMachine.LateUpdate();
        }

        void IStateNode.OnDestroy()
        {
            _subMachine.Destroy();
            _cancellationTokenSource.Cancel();
        }

        private void OnSelectTopicHandle(IEventMessage msg)
        {
            var message = (ProcessEventDefine.SelectTopicCall)msg;

            // Debug.Log($"进入课题：{message.selectIndex}");
            switch (message.selectIndex)
            {
                case 0:
                    GameEntry.UI.OpenUIFormAsync<UIStructuralCognition>().Forget();
                    //_subMachine.ChangeState<StateTeachingDemo>();
                    break;
                case 1:
                    GameEntry.UI.OpenUIFormAsync<UIPrincipleLearning>().Forget();
                    //GameEntry.UI.OpenUIFormSync<UITopicList>();
                    //GameManager.Instance.MonitorMode = EnumMonitorMode.Train;
                    break;
                case 2:
                    //_subMachine.ChangeState<StateAssessment>();
                    GameEntry.UI.OpenUIFormAsync<UIEquipmentMonitoring>().Forget();
                    GameManager.Instance.MonitorMode = EnumMonitorMode.Assessment;
                    break;
                case 3:
                    //_subMachine.ChangeState<StateAssessment>();
                    GameEntry.UI.OpenUIFormAsync<UIAssessment>().Forget();
                    GameManager.Instance.MonitorMode = EnumMonitorMode.Assessment;
                    break;
                case 4:
                    if (GameEntry.UI.HasUIForm("UICommonPage"))
                    {
                        GameEntry.UI.CloseUIForm<UICommonPage>();
                    }

                    GameEntry.UI.OpenUIFormAsync<UIDataScreen>().Forget();
                    //显示交互Tip
                    DataCheckTopicManager.Instance.ShowAllTip();
                    break;
                case 5:
                    //GameEntry.UI.CloseUIForm<UICommonPage>();
                    GameEntry.UI.OpenUIFormAsync<UITopicList>().Forget();
                    break;
                default:
                    _subMachine.ChangeState<StateSelectTopic>();
                    break;
            }
        }
    }
}