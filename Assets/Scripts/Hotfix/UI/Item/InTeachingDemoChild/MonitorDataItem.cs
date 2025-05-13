using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameMain.Runtime;
using Hotfix.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.Event;

namespace Hotfix.UI
{
    public class MonitorDataItem : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI textOne;
        [SerializeField] private TextMeshProUGUI textTwo;
        [SerializeField] private Image swapOne;

        private readonly Color _normal = Color.white;
        private readonly Color _error = Color.red;
        
        private MonitorData _data;
        private EventGroup _eventGroup;
        
        private int _index;
        private int _tempCheckItemIndex;
        private bool _allChecked;
        private bool _isSendAllChecked;
        
        private ListEx<CheckItem> _checkItems;
        private List<CheckItemData> _checkItemDatas;
        private List<int> _checkItemIdList;
        private Dictionary<int,bool> _checkItemCheckedDict;
        private Dictionary<int,KeyValuePair<int,int>> _checkItemValueDict;
        
        private GameObject _checkItemCache;
        private Transform _checkItemParent;

        public bool AllChecked
        {
            get => _allChecked;
        }

        public bool IsOn
        {
            get => toggle.isOn;
            set => toggle.isOn = value;
        }

        private void Awake()
        {
            _eventGroup = new EventGroup();
            _checkItemCheckedDict = new Dictionary<int, bool>();
            _checkItemValueDict = new Dictionary<int, KeyValuePair<int, int>>();
            _checkItemIdList = new List<int>();
            
            toggle.onValueChanged.AddListener(OnToggleChangedHandle);
            _eventGroup.AddListener<ProcessEventDefine.CheckItemCall>(OnCheckItemCallHandle);
        }

        private void Update()
        {
            CheckCheckItemData();
        }

        private void OnCheckItemCallHandle(IEventMessage message)
        {
            var msg = (ProcessEventDefine.CheckItemCall)message;
            
            if (msg.monitorIndex != _index) return;

            _checkItemCheckedDict[msg.index] = true;
            var checkItemIndex = msg.index;

            if (checkItemIndex - _tempCheckItemIndex > 1)
            {
                var currentCheckItemTitle = _checkItemDatas[checkItemIndex].name;
                var errorList = new List<string>();
                for (var i = _tempCheckItemIndex < 0 ? 0 : _tempCheckItemIndex; i < checkItemIndex; i++)
                {
                    if(_checkItems.self[i].GetIsChecked())continue;
                    var unCheckItemName = _checkItemDatas[i].name;
                    errorList.Add($"before check {currentCheckItemTitle}, unCheck {unCheckItemName}");
                }
                ProcessEventDefine.CheckTipCall.SendMessage(errorList);
            }
            else
            {
                _tempCheckItemIndex = checkItemIndex;
                ProcessEventDefine.CheckTipCall.SendMessage(null);
                if (_isSendAllChecked) return;
                _allChecked = _checkItems.GetActiveList().All(checkItem => checkItem.GetIsChecked());
                if (!_allChecked) return;
                ProcessEventDefine.MonitorPointCall.SendMessage(_index);
                _isSendAllChecked = true;
            }
        }

        private void OnToggleChangedHandle(bool isOn)
        {
            if (isOn)
            {
                GenerateCheckItemData();
                GameManager.Instance.ModelController.HighlightModels(_checkItemIdList);
                
            }
            else
            {
                foreach (var checkItem in _checkItems.GetActiveList())
                {
                    checkItem.HideModelValue();
                    checkItem.gameObject.SetActive(false);
                    
                }
                ProcessEventDefine.CheckTipCall.SendMessage(null);
                GameManager.Instance.ModelController.UnHighlightModels(_checkItemIdList);
            }
        }

        public async void InitData(MonitorData data, ListEx<CheckItem> checkItems, Transform checkItemParent,
            ToggleGroup toggleGroup,int index)
        {
            _data = data;
            _checkItems = checkItems;
            _checkItemParent = checkItemParent;
            toggle.group = toggleGroup;
            _index = index;

            textOne.text = textTwo.text = _data.name;
            
            if (_checkItemCache == null)
            {
                _checkItemCache =
                    await GameEntry.Resource.BuildInResource.LoadAsync<GameObject>(AppConst.AssetPathConst.CheckItem,
                        this.GetCancellationTokenOnDestroy());
            }

            _tempCheckItemIndex = -1;
            _allChecked = false;

            await InitCheckItemData();

            ResetField();
        }

        private async UniTask<bool> InitCheckItemData()
        {
            /*var success = false;
            (success, _checkItemDatas) = await HttpDownloader.GetCheckItemData(_data.id);
            return success;*/
            
            //TODO...测试数据
            _checkItemDatas = new List<CheckItemData>()
            {
                new CheckItemData()
                {
                    id = 1,
                    name = "测试项1",
                    serialId = 1,
                    btnSerialId = 1,
                    referenceValue = "10-20",
                },
                new CheckItemData()
                {
                    id = 2,
                    name = "测试项2",
                    serialId = 2,
                    btnSerialId = 2,
                    referenceValue = "10-30",
                }
            };
            
            return true;
        }

        private void ResetField()
        {
            _checkItemCheckedDict.Clear();
            for (var i = 0; i < _checkItemDatas.Count; i++)
            {
                _checkItemCheckedDict.Add(i, false);
                _checkItemIdList.Add(_checkItemDatas[i].id);
            }
            
            _checkItemValueDict.Clear();
            foreach (var checkItemData in _checkItemDatas)
            {
                _checkItemValueDict.Add(checkItemData.serialId,
                    new KeyValuePair<int, int>(int.Parse(checkItemData.referenceValue.Split('-')[0]),
                        int.Parse(checkItemData.referenceValue.Split('-')[1])));
            }
        }

        private void GenerateCheckItemData()
        {
            _checkItems.UpdateItem(_checkItemDatas, _checkItemCache, _checkItemParent);
            for (var i = 0; i < _checkItemDatas.Count; i++)
            {
                _checkItems.self[i].InitData(_index, i, _checkItemDatas[i],_checkItemCheckedDict[i]);
                _checkItems.self[i].gameObject.SetActive(true);
            }
        }

        private void CheckCheckItemData()
        {
            if (_checkItemDatas == null || _checkItemDatas.Count == 0) return;
            foreach (var checkItemData in _checkItemDatas)
            {
                var hardwareValue = HardwareManager.Instance.GetHardwareValue(checkItemData.serialId);
                var (minReferenceValue, maxReferenceValue) = _checkItemValueDict[checkItemData.serialId];
                if (hardwareValue < minReferenceValue || hardwareValue > maxReferenceValue)
                {
                    swapOne.color = _error;
                    break;
                }
                else
                {
                    swapOne.color = _normal;
                }
            }
        }
    }
}