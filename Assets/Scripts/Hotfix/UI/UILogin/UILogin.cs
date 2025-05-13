using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using RainbowArt.CleanFlatUI;
using Wx.Runtime.Event;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using System.Collections.Generic;
using Hotfix.ExcelData;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;

namespace Hotfix.UI
{
    /// <summary>
    /// 登陆界面
    /// </summary>
    public class UILogin : UIForm<UIViewLogin, UIModelLogin>
    {
        private UGuiForm _uGuiForm;

        #region 控制消息扩展

        private List<int> uiLevel;

        /// <summary>
        /// 本模块主配置
        /// </summary>
        private ModuleConfig moduleConfig;

        #endregion

        #region Component

        private readonly EventGroup _eventGroup = new();
        private UIViewLogin _view;
        private UIModelLogin _model;
        private int _currentIndex;
        private List<VideoClip> _videoClips = new();

        #endregion

        #region Private

        private Notification _notification;

        #endregion

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            InitUIListener();
            InitUIComponent();

            _videoClips.Add(_view.videoPlayer.clip);
            _videoClips.Add(_view.clip);

            _view.videoPlayer.loopPointReached += ChangeVideoClip;

            #region 控制和被控扩展

            moduleConfig = ModuleConfigTable.Instance.Get(20);
            HandleMsg.Instance.PadAdpaterParmsToUILevel(ref uiLevel, moduleConfig.AdapterParam);
            Ctrl_MessageCenter.AddMsgListener<MsgBase>("PadLogin", (msgBase) =>
            {
                OnLoginHandle(false);
                ServNet.Instance.SetCacheMsgBaseState(msgBase);
            });

            #endregion

            GameManager.Instance.IsLoginNormal = true;
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
            InitVideoClip();

            if (GameEntry.Sound.IsPaused(AppConst.AssetPathConst.LoginBackSound))
            {
                GameEntry.Sound.RecoverSound(AppConst.AssetPathConst.LoginBackSound);
            }
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);

            _eventGroup.RemoveAllListener();
            GameEntry.Sound.PauseSound(AppConst.AssetPathConst.LoginBackSound);
        }

        #region UI

        private void InitUIComponent()
        {
            _notification = _view.tsNotification.GetComponent<Notification>();
            EventSystem.current.SetSelectedGameObject(_view.tmpinputAccount.gameObject);
        }

        private void ResetUIComponent()
        {
            _notification.gameObject.SetActive(false);
            _view.tmpinputAccount.text = string.Empty;
            _view.tmpinputPassword.text = string.Empty;
            //_view.tmpinputAccount.GetComponent<InputFieldTransition>().UpdateGUI(false);
            //_view.tmpinputPassword.GetComponent<InputFieldTransition>().UpdateGUI(false);
        }

        private void InitUIListener()
        {
            //扩展区分手动还是被控的逻辑
            //_view.btnLogin.onClick.AddListener(OnLoginHandle);
            _view.btnLogin.onClick.AddListener(() => { OnLoginHandle(true); });
            _view.btnRegister.onClick.AddListener(OnRegisterHandle);
            _view.btnClose.onClick.AddListener(OnCloseHandle);
            _view.togGuest.onValueChanged.AddListener((state) => { PlayUIEffect(AppConst.AssetPathConst.ClickSound); });
        }

        private void OnCloseHandle()
        {
            //暂时注释，不知何意，2024.11.15
            //ProcessEventDefine.ChangeLoginMachineCall.SendMessage();

            UIEventDefine.UIPopTipCall.SendMessage(() => { GameManager.Instance.QuitApplication(); }, "提示", "确定关闭软件吗？");
        }

        private void OnRegisterHandle()
        {
            _uGuiForm.Close();
            GameEntry.UI.OpenUIFormAsync<UIRegister>().Forget();
        }

        private void OnLoginHandle(bool isHand)
        {
            if (_view.togGuest.isOn)
            {
                UserManager.Instance.Login();
                PlayUIEffect(AppConst.AssetPathConst.ClickSound);

                if (isHand)
                {
                    SendMsgManager.SendCommonBtnMsg(CommonBtnMsgType.Login);
                }

                return;
            }
            else
            {
                if (string.IsNullOrEmpty(_view.tmpinputAccount.text))
                {
                    ShowNotification(@"警告", @$"{"账户为空！"}");
                    return;
                }
                else if (string.IsNullOrEmpty(_view.tmpinputPassword.text))
                {
                    ShowNotification(@"警告", @$"{"密码为空！"}");
                    return;
                }

                PlayUIEffect(AppConst.AssetPathConst.ClickSound);
                Debug.Log(_view.tmpinputAccount.text);
                UserManager.Instance.Login(_view.tmpinputAccount.text, _view.tmpinputPassword.text);
            }
        }

        private void ShowNotification(string title, string content)
        {
            _notification.TitleValue = title;
            _notification.DescriptionValue = content;
            _notification.ShowNotification();
            PlayUIEffect(AppConst.AssetPathConst.WarningSound);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitVideoClip()
        {
            _currentIndex = 0;
            _view.videoPlayer.clip = _videoClips[_currentIndex];
        }

        private void ChangeVideoClip(VideoPlayer videoPlayer)
        {
            _currentIndex++;
            VideoClip videoClip = null;

            _currentIndex = _currentIndex < _videoClips.Count ? _currentIndex++ : 1;

            videoClip = _videoClips[_currentIndex];
            _view.videoPlayer.clip = videoClip;
        }

        #endregion

        #region Event

        private void InitEvent()
        {
            _eventGroup.AddListener<ProcessEventDefine.LoginCall>(OnLoginFailHandle);
        }

        private void OnLoginFailHandle(IEventMessage msg)
        {
            var message = (ProcessEventDefine.LoginCall)msg;
            if (message.success) return;

            ShowNotification(@"登录失败", @$"{message.callBack}");
        }

        #endregion
    }
}