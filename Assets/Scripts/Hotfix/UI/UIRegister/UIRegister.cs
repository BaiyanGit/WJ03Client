using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using RainbowArt.CleanFlatUI;
using Wx.Runtime.Event;


namespace Hotfix.UI
{
    /// <summary>
    /// 注册界面
    /// </summary>
    public class UIRegister : UIForm<UIViewRegister, UIModelRegister>
    {
        private UGuiForm _uGuiForm;
        
        #region Component

        private readonly EventGroup _eventGroup = new();
        private UIViewRegister _view;
        private UIModelRegister _model;
        
        #endregion

        #region Private

        private Notification _errorNotification;
        private NotificationWithButton _successNotification;

        #endregion

         public override UIGroupInfo SetUIGroupInfo()
        {
            return base.SetUIGroupInfo();
        }

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();
            
            InitUIComponent();
            InitUIListener();
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userDatas)
        {
            base.OnOpen(userDatas);

            InitEvent();
            ResetUIComponent();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnCover()
        {
            base.OnCover();
        }

        public override void OnReveal()
        {
            base.OnReveal();
        }

        public override void OnRefocus(object userData = null)
        {
            base.OnRefocus(userData);
        }

        public override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);
            
            _eventGroup.RemoveAllListener();
        }

        #region UI

        private void InitUIComponent()
        {
            _errorNotification = _view.tsErrorNotification.GetComponent<Notification>();
            _successNotification = _view.tsSuccessNotification.GetComponent<NotificationWithButton>();
        }

        private void ResetUIComponent()
        {
            _errorNotification.gameObject.SetActive(false);
            _successNotification.gameObject.SetActive(false);
            
            _view.tmpinputAccount.text = string.Empty;
            _view.tmpinputPassword.text = string.Empty;
            _view.tmpinputConfirm.text = string.Empty;

            _view.tmpinputAccount.GetComponent<InputFieldTransition>().UpdateGUI(false);
            _view.tmpinputPassword.GetComponent<InputFieldTransition>().UpdateGUI(false);
            _view.tmpinputConfirm.GetComponent<InputFieldTransition>().UpdateGUI(false);
        }

        private void InitUIListener()
        {
            _view.btnClose.onClick.AddListener(OnCloseHandle);
            _view.btnRegister.onClick.AddListener(OnRegisterHandle);
        }

        private void OnCloseHandle()
        {
            _uGuiForm.Close();
            GameEntry.UI.OpenUIFormAsync<UILogin>().Forget();
        }

        private void OnRegisterHandle()
        {
            var account = _view.tmpinputAccount.text;
            var password = _view.tmpinputPassword.text;
            var confirm = _view.tmpinputConfirm.text;

            if (CheckEmpty(account,password,confirm))
            {
                _errorNotification.TitleValue = @"注册失败";
                _errorNotification.DescriptionValue = @"请正确填写用户名及密码，不可为空!";
                _errorNotification.ShowTime = 2f;
                _errorNotification.ShowNotification();
                return;
            }

            if (!CheckPassword(password, confirm))
            {
                _errorNotification.TitleValue = @"注册失败";
                _errorNotification.DescriptionValue = @"密码不一致，请重新输入!";
                _errorNotification.ShowTime = 2f;
                _errorNotification.ShowNotification();
                return;
            }
            UserManager.Instance.Register(account,password);
        }

        #endregion

        #region Event

        private void InitEvent()
        {
            _eventGroup.AddListener<ProcessEventDefine.RegisterCall>(OnRegisterHandle);
        }

        private void OnRegisterHandle(IEventMessage msg)
        {
            var message = (ProcessEventDefine.RegisterCall)msg;

            if (message.success)
            {
                _successNotification.TitleValue = @"注册成功";
                _successNotification.DescriptionValue = @"注册成功，是否使用该账号登录？";
                _successNotification.OnFirst.RemoveAllListeners();
                _successNotification.OnFirst.AddListener(() =>
                {
                    OnSuccessFirstHandle(message.account, message.password);
                });
                _successNotification.ShowNotification();
            }
            else
            {
                _errorNotification.TitleValue = @"注册失败";
                _errorNotification.DescriptionValue = @$"账号：{message.account},{message.callBack}";
                _errorNotification.ShowTime = 5f;
                _errorNotification.ShowNotification();
            }
        }

        private void OnSuccessFirstHandle(string account,string password)
        {
            UserManager.Instance.Login(account, password);
        }

        #endregion

        #region InternalLogic

        private bool CheckEmpty(string account,string password,string confirm)
        {
            return string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm);
        }

        private bool CheckPassword(string password, string confirm)
        {
            return password.Equals(confirm);
        }

        #endregion
        
        
        
    }
}
