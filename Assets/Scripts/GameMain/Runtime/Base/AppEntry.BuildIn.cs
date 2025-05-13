using UnityEngine;
using Wx.Runtime.Event;
using Wx.Runtime.Excel;
using Wx.Runtime.Machine;
using Wx.Runtime.Net;
using Wx.Runtime.Pool;
using Wx.Runtime.Resource;
using Wx.Runtime.Scene;
using Wx.Runtime.Sever;
using Wx.Runtime.Singleton;
using Wx.Runtime.Sound;
using Wx.Runtime.Timer;
using Wx.Runtime.UI;

namespace GameMain.Runtime
{
    public partial class AppEntry : MonoBehaviour
    {
        public static WResource Resource
        {
            get;
            private set;
        }

        public static WExcel Excel
        {
            get;
            private set;
        }

        public static WScene Scene
        {
            get;
            private set;
        }

        public static WSingleton Singleton
        {
            get;
            private set;
        }

        public static WEvent Event
        {
            get;
            private set;
        }

        public static WNet Net
        {
            get;
            private set;
        }

        public static WUI UI
        {
            get;
            private set;
        }

        public static WSound Sound
        {
            get;
            private set;
        }


        public static WSever Sever
        {
            get;
            private set;
        }

        public static WPool Pool
        {
            get;
            private set;
        }

        public static WTimer Timer
        {
            get;
            private set;
        }
        private static void InitBuildInModules()
        {
            Resource = Wx.Runtime.AppEntry.GetModule<WResource>();
            Excel = Wx.Runtime.AppEntry.GetModule<WExcel>();
            Scene = Wx.Runtime.AppEntry.GetModule<WScene>();
            Singleton = Wx.Runtime.AppEntry.GetModule<WSingleton>();
            Event = Wx.Runtime.AppEntry.GetModule<WEvent>();
            Net = Wx.Runtime.AppEntry.GetModule<WNet>();
            UI = Wx.Runtime.AppEntry.GetModule<WUI>();
            Sound = Wx.Runtime.AppEntry.GetModule<WSound>();
            Sever = Wx.Runtime.AppEntry.GetModule<WSever>();
            Pool = Wx.Runtime.AppEntry.GetModule<WPool>();
            Timer = Wx.Runtime.AppEntry.GetModule<WTimer>();
        }


    }
}
