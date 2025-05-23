﻿
using Wx.Runtime.Event;

namespace GameMain.Runtime
{
    public class UserEventDefine
    {
        /// <summary>
        /// 用户尝试获取版本信息
        /// </summary>
        public class UserTryGetVersion : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserTryGetVersion();
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 用户尝试再次初始化资源包
        /// </summary>
        public class UserTryInitialize : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserTryInitialize();
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 用户开始下载网络文件
        /// </summary>
        public class UserBeginDownloadWebFiles : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserBeginDownloadWebFiles();
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 用户尝试再次更新静态版本
        /// </summary>
        public class UserTryUpdatePackageVersion : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserTryUpdatePackageVersion();
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 用户尝试再次更新补丁清单
        /// </summary>
        public class UserTryUpdatePatchManifest : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserTryUpdatePatchManifest();
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 用户尝试再次下载网络文件
        /// </summary>
        public class UserTryDownloadWebFiles : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserTryDownloadWebFiles();
                AppEntry.Event.SendMessage(msg);
            }
        }

    }
}