using Wx.Runtime.UI;
using Cysharp.Threading.Tasks;

namespace Hotfix
{
    public static class UIExtension
    {
        public static IUIForm GetUIForm<T>(this WUI wui) where T : class, IUIForm
        {
            return wui.GetUIForm(typeof(T).Name);
        }

        public static T OpenUIFormSync<T>(this WUI wui, bool pauseCoverUIForm = false, object userData = null) where T : class, IUIForm
        {   
            //扩展界面打开广播事件
            Ctrl_MessageCenter.SendMessage("UIFormOpen", typeof(T).Name, userData);
            return wui.OpenUIFormSync<T>(typeof(T).Name, pauseCoverUIForm, userData);
        }

        public static async UniTask<T> OpenUIFormAsync<T>(this WUI wui, bool pauseCoverUIForm = true, object userData = null) where T : class, IUIForm
        {
            //扩展界面异步打开广播事件
            Ctrl_MessageCenter.SendMessage("UIFormOpenAsync", typeof(T).Name, userData);
            return await wui.OpenUIFormAsync<T>(typeof(T).Name, pauseCoverUIForm, userData);
        }

        public static void CloseUIForm<T>(this WUI wui, bool isRecycle = false, object userData = null) where T : class, IUIForm
        {
            //扩展界面关闭广播事件
            Ctrl_MessageCenter.SendMessage("UIFormClose", typeof(T).Name, userData);
            wui.CloseUIForm(wui.GetUIForm<T>(), isRecycle, userData);
        }
    }
}