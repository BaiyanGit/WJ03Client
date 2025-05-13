using System;
using UnityEngine.Events;
using Wx.Runtime.Event;

namespace Hotfix.Event
{
    public class UIEventDefine
    {
        /// <summary>
        /// UILoading显示进度
        /// 0：循环进度
        /// 1：进度条
        /// </summary>
        public class UILoadingShowPro : IEventMessage
        {
            public int showIndex;

            public static void SendMessage(int showIndex)
            {
                var msg = new UILoadingShowPro()
                {
                    showIndex = showIndex,
                };
                GameEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// UILoading更新进度
        /// </summary>
        public class UILoadingUpdatePro : IEventMessage
        {
            public float progress;

            public static void SendMessage(float progress)
            {
                var msg = new UILoadingUpdatePro()
                {
                    progress = progress,
                };
                GameEntry.Event.SendMessage(msg);
            }
        }
        
        /// <summary>
        /// UILoading显示提示框
        /// </summary>
        public class UILoadingShowWindow : IEventMessage
        {
            public string title;
            public string content;
            public Action cancel;
            public Action confirm;
            public string cancelText;
            public string confirmText;
            
            public static void SendMessage(string title,string content,Action cancel,Action confirm,string cancelText,string confirmText)
            {
                var msg = new UILoadingShowWindow()
                {
                    title = title,
                    content = content,
                    cancel = cancel,
                    confirm = confirm,
                    cancelText = cancelText,
                    confirmText = confirmText
                };
                GameEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// UIEntry界面选择游戏模式
        /// 0：单机
        /// 1：联网
        /// </summary>
        public class UIEntrySelectModel : IEventMessage
        {
            public int modelIndex;

            public static void SendMessage(int modelIndex)
            {
                var msg = new UIEntrySelectModel()
                {
                    modelIndex = modelIndex,
                };
                GameEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 弹出返回界面
        /// </summary>
        public class UIPopTipCall : IEventMessage
        {
            public string title;
            public string content;
            public UnityAction confirm;
            public UnityAction cancel;

            public static void SendMessage(UnityAction confirm,string title = @"BACK", string content = @"Please confirm to back!",UnityAction cancel = null)
            {
                var msg = new UIPopTipCall()
                {
                    title = title,
                    content = content,
                    confirm = confirm,
                    cancel = cancel,
                };
                GameEntry.Event.SendMessage(msg);
            }
        }
    }
}
