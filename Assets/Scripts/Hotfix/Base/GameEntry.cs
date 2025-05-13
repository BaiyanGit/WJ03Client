
using GameMain.Runtime;
using Wx.Runtime.Event;
using Wx.Runtime.Excel;
using Wx.Runtime.Net;
using Wx.Runtime.Pool;
using Wx.Runtime.Resource;
using Wx.Runtime.Scene;
using Wx.Runtime.Sever;
using Wx.Runtime.Singleton;
using Wx.Runtime.Sound;
using Wx.Runtime.Timer;
using Wx.Runtime.UI;

namespace Hotfix
{
    public static class GameEntry
    {
        public static WResource Resource => AppEntry.Resource;
        public static WExcel Excel => AppEntry.Excel;
        public static WScene Scene => AppEntry.Scene;
        public static WSingleton Singleton => AppEntry.Singleton;
        public static WEvent Event => AppEntry.Event;
        public static WUI UI => AppEntry.UI;
        public static WSound Sound => AppEntry.Sound;
        public static WSever Sever => AppEntry.Sever;
        public static WNet Net => AppEntry.Net;
        public static WPool Pool => AppEntry.Pool;
        public static WTimer Timer => AppEntry.Timer;
        
        public static void Start()
        {
            WLog.Log($"<color=yellow>Hotfix Start</color>");
            Excel.UpdateAllAttributeTypes();
            Singleton.CreateSingleton<GameManager>();
            Singleton.CreateSingleton<ConfigManager>();
            Singleton.CreateSingleton<UserManager>();
            Singleton.CreateSingleton<HardwareManager>();
            Singleton.CreateSingleton<SettlementManager>();
            Singleton.CreateSingleton<TopicManager>();
            Singleton.CreateSingleton<SinglechipManager>();
            Singleton.CreateSingleton<DataCheckTopicManager>();
            Singleton.CreateSingleton<ModelLoaderManager>();
        }
    }
}
