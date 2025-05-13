using System.Collections.Generic;
using GameMain.Runtime;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.ExcelData;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;

namespace Hotfix.UI
{
    /// <summary>
    /// 结算界面
    /// </summary>
    public class UISettlement : UIForm<UIViewSettlement, UIModelSettlement>
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

        private UIViewSettlement _view;
        private UIModelSettlement _model;

        #endregion

        #region Private

        private ListEx<SettlementItem> _settlementItems;
        private GameObject _goCache;

        #endregion

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm,
            UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            InitField();
            InitUIListener();

            #region 控制和被控消息扩展

            moduleConfig = ModuleConfigTable.Instance.Get(22);
            HandleMsg.Instance.PadAdpaterParmsToUILevel(ref uiLevel, moduleConfig.AdapterParam);
            Ctrl_MessageCenter.AddMsgListener<MsgBase>("PadExitAssess", (value) =>
            {
                OnCloseBtnHandle(false);
                ServNet.Instance.SetCacheMsgBaseState(value);
            });

            #endregion
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //ResetUI();
            GenerateSettlement();

            PlayUIEffect(AppConst.AssetPathConst.SettlementSound);
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);

            if (SettlementManager.Instance.ErrorList.Count <= 0) return;

            for (var i = 0; i < SettlementManager.Instance.ErrorList.Count; i++)
            {
                Object.Destroy(_settlementItems.self[i].gameObject);
            }
            _settlementItems.self.Clear();

            SettlementManager.Instance.ErrorList.Clear();
        }

        #region Field

        private void InitField()
        {
            _settlementItems = new ListEx<SettlementItem>();
            _goCache = GameEntry.Resource.BuildInResource.Load<GameObject>(AppConst.AssetPathConst.SettlementItem);
        }

        #endregion

        #region UI

        private void ResetUI()
        {
            _view.tmptxtModelName.text = $"ModelName:{SettlementManager.Instance.ModelName}";
            _view.tmptxtErrorCount.text = $"ErrorCount:{SettlementManager.Instance.ErrorCount}";
            _view.tmptxtTime.text = $"{SettlementManager.Instance.Time}";
        }

        private void InitUIListener()
        {
            _view.btnClose.onClick.AddListener(() => { OnCloseBtnHandle(true); });
        }

        private void OnCloseBtnHandle(bool isHand)
        {
            if (GameManager.Instance.AssessmentMode != EnumAssessmentMode.Train)
                SettlementManager.Instance.UploadData();

            //关闭声音
            GameEntry.Sound.StopSound(AppConst.AssetPathConst.SettlementSound);
            //GameEntry.Sound.StopSound(4);
            _uGuiForm.Close();
            GameEntry.UI.OpenUIFormSync<UITopicList>();

            if (isHand)
            {
                SendMsgManager.SendCommonBtnMsg(CommonBtnMsgType.ExitAssess);
                HandleMsg.Instance.ForceSyncAfterSettlementUI();
            }
        }

        #endregion

        #region InternalLogic

        private void GenerateSettlement()
        {
            _view.tmptxtTime.text = TopicManager.Instance.UseTime;
            _view.tmptxtScore.text = /*TopicManager.Instance.GetIsComplete() ?*/ TopicManager.Instance.Score.ToString() /*: 0.ToString()*/;
            _settlementItems.UpdateItem(SettlementManager.Instance.ErrorList, _goCache, _view.tsSettlementContent);
            for (var i = 0; i < SettlementManager.Instance.ErrorList.Count; i++)
            {
                _settlementItems.self[i].InitData(SettlementManager.Instance.ErrorList[i]);
                _settlementItems.self[i].gameObject.SetActive(true);
            }

            SendMsgManager.SendSettlementInfo(_view.tmptxtTime.text, _view.tmptxtScore.text, SettlementManager.Instance.ErrorList);
        }

        #endregion
    }
}