using Wx.Runtime.Event;

namespace GameMain.Runtime
{
    public class PatchEventDefine
    {
        /// <summary>
        /// 补丁包初始化失败
        /// </summary>
        public class InitializeFailed : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new InitializeFailed();
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 补丁流程步骤改变
        /// </summary>
        public class PatchStatesChange : IEventMessage
        {
            public string tips;

            public static void SendEventMessage(string tips)
            {
                var msg = new PatchStatesChange
                {
                    tips = tips
                };
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 发现更新文件
        /// </summary>
        public class FoundUpdateFiles : IEventMessage
        {
            public int totalCount;
            public long totalSizeBytes;

            public static void SendEventMessage(int totalCount, long totalSizeBytes)
            {
                var msg = new FoundUpdateFiles
                {
                    totalCount = totalCount,
                    totalSizeBytes = totalSizeBytes
                };
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 下载进度更新
        /// </summary>
        public class DownloadProgressUpdate : IEventMessage
        {
            public int totalDownloadCount;
            public int currentDownloadCount;
            public long totalDownloadSizeBytes;
            public long currentDownloadSizeBytes;

            public static void SendEventMessage(int totalDownloadCount, int currentDownloadCount,
                long totalDownloadSizeBytes, long currentDownloadSizeBytes)
            {
                var msg = new DownloadProgressUpdate
                {
                    totalDownloadCount = totalDownloadCount,
                    currentDownloadCount = currentDownloadCount,
                    totalDownloadSizeBytes = totalDownloadSizeBytes,
                    currentDownloadSizeBytes = currentDownloadSizeBytes
                };
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 资源版本号更新失败
        /// </summary>
        public class PackageVersionUpdateFailed : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new PackageVersionUpdateFailed();
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 补丁清单更新失败
        /// </summary>
        public class PatchManifestUpdateFailed : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new PatchManifestUpdateFailed();
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 网络文件下载失败
        /// </summary>
        public class WebFileDownloadFailed : IEventMessage
        {
            public string fileName;
            public string error;

            public static void SendEventMessage(string fileName, string error)
            {
                var msg = new WebFileDownloadFailed
                {
                    fileName = fileName,
                    error = error
                };
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 重新获取版本信息
        /// </summary>
        public class ReRequestVersion : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new ReRequestVersion();
                AppEntry.Event.SendMessage(msg);
            }
        }

        /// <summary>
        /// 重新下载程序
        /// </summary>
        public class ReDownLoadApplication : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new ReDownLoadApplication();
                AppEntry.Event.SendMessage(msg);
            }
        }

        public class DoneShow : IEventMessage
        {
            public string show;

            public static void SendEventMessage(string show)
            {
                var msg = new DoneShow()
                {
                    show = show
                };
                AppEntry.Event.SendMessage(msg);
            }
        }
    }
}