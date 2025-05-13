using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameMain.Runtime;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using UnityEngine.UI;
using Wx.Runtime.Event;
using Object = UnityEngine.Object;
using Hotfix.ExcelData;

namespace Hotfix.UI
{
    public struct UIEquipmentMonitoringData
    {
        public int checkPointId;
    }

    /// <summary>
    /// 设备监测界面
    /// </summary>
    public class UIEquipmentMonitoring : UIForm<UIViewEquipmentMonitoring, UIModelEquipmentMonitoring>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private readonly EventGroup _eventGroup = new();
        private UIViewEquipmentMonitoring _view;
        private UIModelEquipmentMonitoring _model;
        private ToggleGroup _toggleGroup;
        private Transform togItem;

        #endregion

        #region Private

        private List<CheckItemData> _checkItemDatas;
        private ListEx<TeachingCheckItem> _teachingCheckItems;
        private int _checkPointId;

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
            InitField();
            InitUIListener();
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            if (userData is UIEquipmentMonitoringData data)
            {
                _checkPointId = data.checkPointId;
            }

            InitEvent();

            UniTask.Void(async () =>
            {
                if (!await InitCheckItemData()) return;
                GenerateCheckItemData();
            });

            ShowCommonPage();
            GameManager.Instance.RestCinemachineCamera();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            //GameManager.Instance.ModelController.Control();
        }

        public override void OnPause()
        {
            base.OnPause();
            //GameManager.Instance.ModelController.ResetMouseControl();
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

            ShowCommonPage();
        }

        public override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);
            _eventGroup.RemoveAllListener();
            //GameManager.Instance.ModelController.ResetModel();
        }

        #region UI

        private void InitUIComponent()
        {
            _toggleGroup = _view.tsContentList.GetComponent<ToggleGroup>();
            //GameManager.Instance.ViewMainCamera.Target = GameManager.Instance.ViewMainTarget;
            GameManager.Instance.ViewMainCamera.Target.TrackingTarget = GameManager.Instance.MainTarget.Find("InitLookPos");

            _view.togItem.gameObject.SetActive(false);
        }

        private void InitUIListener()
        {

        }

        private void ShowCommonPage()
        {
            var userData = new UICommonPageUserData();
            userData.TabTitles = new string[] { "首页", "数字监测" };
            userData.BackLastPageAction = () => { _uGuiForm.Close(); GameEntry.UI.OpenUIFormSync<UIMainMenu>(); };
            userData.BackMainPageAction = () => { _uGuiForm.Close(); };
            userData.IsNotShowDownLine = true; userData.IsBackLastDialog = false; userData.IsBackMainDialog = false;
            userData.IsNotShowSetting = true;
            GameEntry.UI.OpenUIFormSync<UICommonPage>(false, userData);
        }

        private void OnCloseHandle()
        {
            //ResetToggleValue();
            UIEventDefine.UIPopTipCall.SendMessage(
                () => { _uGuiForm.Close(); });
        }

        #endregion

        #region Field

        private void InitField()
        {
            _checkItemDatas = new List<CheckItemData>();
            _teachingCheckItems = new ListEx<TeachingCheckItem>();
        }

        #endregion

        #region Event

        private void InitEvent()
        {
        }

        #endregion

        #region InternalLogic

        private async UniTask<bool> InitCheckItemData()
        {
            /*var success = false;
            (success, _checkItemDatas) = await HttpDownloader.GetCheckItemData(_checkPointId);
            return success;*/

            //TODO...测试数据
            //_checkItemDatas = new List<CheckItemData>
            //{
            //    new CheckItemData()
            //    {
            //        id = 1,
            //        serialId = 1,
            //        name = "检测项1",
            //        referenceValue = "50-60",
            //    },
            //    new CheckItemData()
            //    {
            //        id = 2,
            //        serialId = 2,
            //        name = "检测项2",
            //        referenceValue = "50-60",
            //    },
            //};

            _checkItemDatas = new();
            for (int i = 0; i < EquipmentCheckConfig1stTable.Instance.dataList.Count; i++)
            {
                var temp = new CheckItemData();
                temp.id = EquipmentCheckConfig1stTable.Instance.dataList[i].Id;
                temp.name = EquipmentCheckConfig1stTable.Instance.dataList[i].Title;
                //temp.serialId = string.IsNullOrEmpty(EquipmentCheckConfig1stTable.Instance.dataList[i].SensorTag) ? 0 : i;
                temp.btnSerialId = string.IsNullOrEmpty(EquipmentCheckConfig1stTable.Instance.dataList[i].Icon) ? 0 : i;
                //temp.referenceValue = EquipmentCheckConfig1stTable.Instance.dataList[i].TargetValues.Length == 0 ? string.Empty : "30-40";
                _checkItemDatas.Add(temp);
            }

            await UniTask.Yield();
            return true;
        }

        private void GenerateCheckItemData()
        {
            if (_checkItemDatas == null || _checkItemDatas.Count == 0) return;
            var checkItemCache = GameEntry.Resource.BuildInResource.Load<GameObject>(AppConst.AssetPathConst.TeachingCheckItem);

            _teachingCheckItems.UpdateItem(_checkItemDatas, checkItemCache, _view.tsContentList);

            for (var i = 0; i < _checkItemDatas.Count; i++)
            {
                _teachingCheckItems.self[i].InitData(_checkItemDatas[i], _toggleGroup, EquipmentCheckConfig1stTable.Instance.dataList[i], On1stSelectedAction);
                _teachingCheckItems.self[i].gameObject.SetActive(true);
            }

            _view.tsShowItem.gameObject.SetActive(false);
        }

        private void ResetToggleValue()
        {
            foreach (var checkItem in _teachingCheckItems.GetActiveList())
            {
                checkItem.ResetCheckItem();
            }
        }

        List<EquipmentCheckConfig2nd> equipmentCheckConfig2nds;
        List<GameObject> tempObject = new();
        private void On1stSelectedAction(EquipmentCheckConfig1st config,bool isShow = true)
        {
            _view.tsShowItem.gameObject.SetActive(isShow);

            equipmentCheckConfig2nds = _model.GetEquipmentCheckConfig2nds(config.Id);

            if (tempObject.Count > 0)
            {
                for (int i = 0; i < tempObject.Count; i++)
                {
                    tempObject[i]?.GetComponent<EquipmentCheckItem>()?.InitLabelTip();
                    Object.Destroy(tempObject[i]);
                }
                tempObject.Clear();
            }
            if (!isShow) return;

            for (int i = 0; i < equipmentCheckConfig2nds.Count; i++)
            {
                GameObject item = Object.Instantiate(_view.togItem.gameObject, _view.tsShowItem.GetChild(0), false);
                item.SetActive(true);

                item.GetComponent<EquipmentCheckItem>().Init(config, equipmentCheckConfig2nds[i], _view.tsShowTips);
                tempObject.Add(item);
            }
        }

        #endregion
    }
}