using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameMain.Runtime;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using TMPro;
using UnityEngine.UI;
using Wx.Runtime.Event;
using Timer = Wx.Runtime.Timer.Timer;


namespace Hotfix.UI
{
    public struct UIAssessmentMonitoringData
    {
        public int subjectId;
        public string subjectName;
    }

    /// <summary>
    /// 考核监测界面
    /// </summary>
    public class UIAssessmentMonitoring : UIForm<UIViewAssessmentMonitoring, UIModelAssessmentMonitoring>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private readonly EventGroup _eventGroup = new();
        private UIViewAssessmentMonitoring _view;
        private UIModelAssessmentMonitoring _model;
        private ToggleGroup _toggleGroup;

        #endregion

        #region Private

        private CancellationTokenSource _tokenSource;
        private UIAssessmentMonitoringData _data;
        private Timer _timer;

        private GameObject _monitorPointCache;
        private GameObject _checkTipCache;
        
        private List<MonitorData> _monitorPointData;
        private ListEx<MonitorDataItem> _monitorDataItems;
        private ListEx<CheckItem> _checkItems;
        private ListEx<CheckTipItem> _checkTipItems;

        #endregion

        public override UIGroupInfo SetUIGroupInfo()
        {
            return base.SetUIGroupInfo();
        }

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm,
            UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();
            
            InitUIComponent();
            InitUIListener();
            InitField();
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            if (userData is UIAssessmentMonitoringData data)
            {
                _data = data;
            }

            UniTask.Void(async () =>
            {
                ResetField();
                InitEvent();
                await InitCache();
                await InitMonitorPointData();
                GenerateMonitorPoint();
                SettlementManager.Instance.ModelName = _data.subjectName;
                //TODO。。。开始计时
                _timer = GameEntry.Timer.CreateTimer(-1, true, null);
            });
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            GameManager.Instance.ModelController.Control();
        }

        public override void OnPause()
        {
            base.OnPause();
            GameManager.Instance.ModelController.ResetMouseControl();
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
            SettlementManager.Instance.Time = _timer.GetTimeString();
            _timer.ShutDown();
            GameManager.Instance.ModelController.ResetModel();
            GameManager.Instance.ModelController.ResetTargetTran();
            GameManager.Instance.ModelController.ResetMouseControl();
        }

        #region UI

        private void InitUIComponent()
        {
            _toggleGroup = _view.tsLeftToggleList.GetComponent<ToggleGroup>();
        }

        private void InitUIListener()
        {
            _view.btnClose.onClick.AddListener(OnCloseHandle);
        }

        private void OnCloseHandle()
        {
            foreach (var monitorDataItem in _monitorDataItems.GetActiveList())
            {
                monitorDataItem.IsOn = false;
            }

            UIEventDefine.UIPopTipCall.SendMessage(() =>
            {
                _uGuiForm.Close();
            });
        }

        #endregion

        #region Field

        private void InitField()
        {
            _monitorPointData = new List<MonitorData>();
            _monitorDataItems = new ListEx<MonitorDataItem>();
            _checkItems = new ListEx<CheckItem>();
            _checkTipItems = new ListEx<CheckTipItem>();
        }

        private void ResetField()
        {
            _tokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Cache

        private async UniTask InitCache()
        {
            if (null == _monitorPointCache)
            {
                _monitorPointCache = await GameEntry.Resource.BuildInResource.LoadAsync<GameObject>(
                    AppConst.AssetPathConst.MonitorDataItem,
                    _tokenSource.Token);
            }

            if (null == _checkTipCache)
            {
                _checkTipCache =
                    await GameEntry.Resource.BuildInResource.LoadAsync<GameObject>(AppConst.AssetPathConst.CheckTipItem,
                        _tokenSource.Token);
            }
            
        }

        #endregion
        
        #region Event

        private void InitEvent()
        {
            _eventGroup.AddListener<ProcessEventDefine.CheckTipCall>(OnCheckTipCallHandle);
            _eventGroup.AddListener<ProcessEventDefine.MonitorPointCall>(OnMonitorPointCallHandle);
        }

        private void OnCheckTipCallHandle(IEventMessage message)
        {
            var msg = (ProcessEventDefine.CheckTipCall) message;
            if (msg.tip == null)
            {
                foreach (var checkTipItem in _checkTipItems.GetActiveList())
                {
                    checkTipItem.HideTip();
                }
            }
            else
            {
                //SettlementManager.Instance.ErrorList.AddRange(msg.tip);
            }
        }

        private void OnMonitorPointCallHandle(IEventMessage message)
        {
            var allChecked = true;
            foreach (var monitorDataItem in _monitorDataItems.GetActiveList().Where(monitorDataItem => !monitorDataItem.AllChecked))
            {
                monitorDataItem.IsOn = true;
                allChecked = false;
                break;
            }

            if (!allChecked) return;
            UniTask.Void(async () =>
            {
                foreach (var monitorDataItem in _monitorDataItems.GetActiveList())
                {
                    monitorDataItem.IsOn = false;
                }
                
                //等待动画完成
                await UniTask.Delay(300);
                _uGuiForm.Close();
                await GameEntry.UI.OpenUIFormAsync<UISettlement>();
            });
        }

        #endregion

        #region InternalLogic

        private async UniTask<bool> InitMonitorPointData()
        {
            /*var success = false;
            (success, _monitorPointData) = await HttpDownloader.GetMonitorPointData(_data.subjectId);
            return success;*/

            //TODO...测试数据

            _monitorPointData = new List<MonitorData>()
            {
                new MonitorData()
                {
                    id = 1,
                    name = "监测点1",
                },
                new MonitorData()
                {
                    id = 2,
                    name = "监测点2",
                }
            };
            
            return true;
        }

        private void GenerateMonitorPoint()
        {
            _monitorDataItems.UpdateItem(_monitorPointData,_monitorPointCache,_view.tsLeftToggleList);
            for (var i = 0; i < _monitorPointData.Count; i++)
            {
                _monitorDataItems.self[i]
                    .InitData(_monitorPointData[i], _checkItems, _view.tsCheckItemList, _toggleGroup, i);
                _monitorDataItems.self[i].gameObject.SetActive(true);
            }
        }
            
        #endregion
    }
}