using Cysharp.Threading.Tasks;
using Hotfix.Event;
using Hotfix.UI;
using Wx.Runtime.Http;
using Wx.Runtime.Singleton;

namespace Hotfix
{
    public class UserManager : SingletonInstance<UserManager>, ISingleton
    {
        public string userAccount;
        public string userPassword;
        public string UserName;
        public int MainID;

        public void OnCreate(object createParam)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLateUpdate()
        {
        }

        public void OnDestroy()
        {
        }

        public void Login()
        {
            //登陆成功
            UserName = "离线模式";
            GameEntry.UI.CloseAllLoadUIForms();

            //TODO...进入游戏主模块
            ProcessEventDefine.ChangeGameMachineCall.SendMessage();
        }

        /// <summary>
        /// 处理用户登录事件
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public async void Login(string account, string password)
        {
            //var loginRequest = new LoginRequest()
            //{
            //    account = account,
            //    password = password
            //};

            //var loginResponse = await NetworkHelper.PostJsonAsync<LoginRequest, LoginResponse>(AppConst.UrlConst.LoginURL, loginRequest, 5f);
            //if (loginResponse.code == -1)
            //{
            //    //登陆失败
            //    ProcessEventDefine.LoginCall.SendMessage(false, loginResponse.msg);
            //    WLog.Log("登陆失败");
            //}
            //else
            //{
            //    //登陆成功
            //    userAccount = loginResponse.account;
            //    userPassword = loginResponse.password;
            //    GameEntry.UI.CloseAllLoadUIForms();
            //    //TODO...进入游戏主模块
            //    ProcessEventDefine.ChangeGameMachineCall.SendMessage();
            //}

            //ToDo 密码
            var loginPost = new LoginPost() { Account = account };
            HtttpWebRequestManager.HttpPost<ResponseLogin>(AppConst.Protocol.GetUserInfoByAccount, loginPost, (response) =>
            {
                if (response.code != 1000)
                {
                    //登陆失败
                    //ToDo 简化
                    var loginResponse = new LoginResponse() { code = response.code, msg = response.msg };
                    ProcessEventDefine.LoginCall.SendMessage(false, loginResponse.msg);
                    WLog.Log("登陆失败");
                }
                else
                {
                    if (response.infos.Password == password)
                    {
                        //登陆成功
                        userAccount = response.infos.Account;
                        userPassword = response.infos.Password;
                        UserName = response.infos.UserName;
                        MainID = response.infos.MainID;
                        GameEntry.UI.CloseAllLoadUIForms();
                        //TODO...进入游戏主模块
                       ProcessEventDefine.ChangeGameMachineCall.SendMessage();
                    }
                    else
                    {
                        var loginResponse = new LoginResponse() { msg = "密码错误!" };
                        ProcessEventDefine.LoginCall.SendMessage(false, loginResponse.msg);
                        WLog.Log("登陆失败");
                    }
                }
            });
        }

        /// <summary>
        /// 处理用户注册事件
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public async void Register(string account, string password)
        {
#if UNITY_EDITOR
            ProcessEventDefine.RegisterCall.SendMessage(true, null, account, password);
            WLog.Log("注册成功");
            return;
#endif
            var registerRequest = new RegisterRequest()
            {
                account = account,
                password = password
            };
            var registerResponse =
                await NetworkHelper.PostJsonAsync<RegisterRequest, RegisterResponse>(AppConst.UrlConst.RegisterURL,
                    registerRequest, 5f);
            if (registerResponse.code == -1)
            {
                //注册失败
                ProcessEventDefine.RegisterCall.SendMessage(false, registerResponse.msg, account, password);
                WLog.Log("注册失败");
            }
            else
            {
                //注册成功
                ProcessEventDefine.RegisterCall.SendMessage(true, registerResponse.msg, registerResponse.account, registerResponse.password);
                WLog.Log("注册成功");
            }
        }
    }
}