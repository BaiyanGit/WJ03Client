using System.Collections.Generic;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.ExcelData;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;

namespace Hotfix.UI
{
    /// <summary>
    /// 提交实训结果界面
    /// </summary>
    public class UIStudentInfoRecord : UIForm<UIViewStudentInfoRecord, UIModelStudentInfoRecord>
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

        private UIViewStudentInfoRecord _view;
        private UIModelStudentInfoRecord _model;

        #endregion

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            _view.btnSure.onClick.AddListener(() => { OnButtonClickHandle(true); });

            #region 控制和被控消息扩展

            moduleConfig = ModuleConfigTable.Instance.Get(21);
            HandleMsg.Instance.PadAdpaterParmsToUILevel(ref uiLevel, moduleConfig.AdapterParam);
            Ctrl_MessageCenter.AddMsgListener<MsgBase>("PadInputUserInfo", (value) =>
            {
                OnPadInputUserInfo(value);
                ServNet.Instance.SetCacheMsgBaseState(value);
            });
            Ctrl_MessageCenter.AddMsgListener<MsgBase>("PadConfirmInputUserInfo", (value) =>
            {
                OnButtonClickHandle(false);
                ServNet.Instance.SetCacheMsgBaseState(value);
            });
            
            //缓存并发送用户输入的变化
            _view.tmpinputName.onValueChanged.AddListener((string value) => { SendMsgManager.SendUIInputUserInfoMsg(value, _view.tmpinputID.text, _view.tmpinputEvaluation.text, _view.tmpinputScore.text); });
            _view.tmpinputID.onValueChanged.AddListener((string value) => { SendMsgManager.SendUIInputUserInfoMsg(_view.tmpinputName.text, value, _view.tmpinputEvaluation.text, _view.tmpinputScore.text); });
            _view.tmpinputEvaluation.onValueChanged.AddListener((string value) => { SendMsgManager.SendUIInputUserInfoMsg(_view.tmpinputName.text, _view.tmpinputID.text, value, _view.tmpinputScore.text); });
            _view.tmpinputScore.onValueChanged.AddListener((string value) => { SendMsgManager.SendUIInputUserInfoMsg(_view.tmpinputName.text, _view.tmpinputID.text, _view.tmpinputEvaluation.text, value); });

            #endregion
        }

        #region 被控消息扩展

        void OnPadInputUserInfo(MsgBase msg)
        {
            var temp = (MsgInputUserInfoData)msg;
            _view.tmpinputName.SetTextWithoutNotify(temp.userName);
            _view.tmpinputID.SetTextWithoutNotify(temp.userNum);
            _view.tmpinputEvaluation.SetTextWithoutNotify(temp.userEvaluation);
            _view.tmpinputScore.SetTextWithoutNotify(temp.userScore);

            Debug.LogFormat("MsgInputUserInfoData: userName={0}.userNum={1}.userEvaluation={2}.userScore={3}", temp.userName, temp.userNum, temp.userEvaluation, temp.userScore);

            Debug.LogFormat("MsgInputUserInfoData: userName={0}.userNum={1}.userEvaluation={2}.userScore={3}", _view.tmpinputName.text, _view.tmpinputID.text, _view.tmpinputEvaluation.text, _view.tmpinputScore.text);

        }

        #endregion

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        void OnButtonClickHandle(bool isHand)
        {
            //_view.tmpinputName;
            TopicManager.Instance.SubmitStudentInfo(_view.tmpinputName.text, _view.tmpinputID.text, _view.tmpinputEvaluation.text, _view.tmpinputScore.text);
            _uGuiForm.Close();

            if (isHand)
            {
                SendMsgManager.SendCommonBtnMsg(CommonBtnMsgType.ConfirmInputUserInfo);
            }

            GameEntry.UI.OpenUIFormSync<UISettlement>();
        }
    }
}