using System.Collections.Generic;
using Wx.Runtime.Event;

namespace Hotfix.Event
{
    public class ProcessEventDefine
    {
        /// <summary>
        /// 切换到LoginMachine
        /// </summary>
        public class ChangeLoginMachineCall : IEventMessage
        {
            public static void SendMessage()
            {
                var msg = new ChangeLoginMachineCall();
                GameEntry.Event.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// 登陆失败
        /// </summary>
        public class LoginCall : IEventMessage
        {
            public bool success;
            public string callBack;
            
            public static void SendMessage(bool success, string callBack)
            {
                
                var msg = new LoginCall()
                {
                    success = success,
                    callBack = callBack,
                };
                GameEntry.Event.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// 注册回调
        /// </summary>
        public class RegisterCall : IEventMessage
        {
            public bool success;
            public string callBack;

            public string account;
            public string password;

            public static void SendMessage(bool success, string callBack,string account,string password)
            {
                var msg = new RegisterCall()
                {
                    success = success,
                    callBack = callBack,
                    account = account,
                    password = password
                };
                GameEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 切换到GameMachine
        /// </summary>
        public class ChangeGameMachineCall : IEventMessage
        {
            public static void SendMessage()
            {
                var msg = new ChangeGameMachineCall();
                GameEntry.Event.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// 选择主课题
        /// 0：教学演示
        /// 1：故障训练
        /// 2：故障考核
        /// default:返回课题选择
        /// </summary>
        public class SelectTopicCall : IEventMessage
        {
            public int selectIndex;

            public static void SendMessage(int selectIndex)
            {
                var msg = new SelectTopicCall()
                {
                    selectIndex = selectIndex,
                };
                GameEntry.Event.SendMessage(msg);
            }
        }

        
        /// <summary>
        /// 监测项回调
        /// </summary>
        public class CheckItemCall : IEventMessage
        {
            public int monitorIndex;
            public int index;

            public static void SendMessage(int monitorIndex, int index)
            {
                var msg = new CheckItemCall()
                {
                    monitorIndex = monitorIndex,
                    index = index,
                };
                GameEntry.Event.SendMessage(msg);
            }
        }

        public class CheckPointItemCall: IEventMessage
        {
            public int modelIndex;
            public static void SendMessage(int modelIndex)
            {
                var msg = new CheckPointItemCall()
                {
                    modelIndex = modelIndex,
                };
                GameEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 检查提示
        /// </summary>
        public class CheckTipCall : IEventMessage
        {
            public List<string> tip;

            public static void SendMessage(List<string> tip)
            {
                var msg = new CheckTipCall()
                {
                    tip = tip,

                };
                GameEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 监测点回调
        /// </summary>
        public class MonitorPointCall : IEventMessage
        {
            public int index;

            public static void SendMessage(int index)
            {
                var msg = new MonitorPointCall()
                {
                    index = index,
                };
                GameEntry.Event.SendMessage(msg);
            }
        }
    }
}