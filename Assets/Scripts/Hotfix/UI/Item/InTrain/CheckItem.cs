using System;
using Hotfix.Event;
using Hotfix.ExcelData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace  Hotfix.UI
{
    public class CheckItem : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI checkTitle;
        
        private CheckItemData _data;
        private int _monitorDataIndex;
        private int _index;
        private bool _isShowed;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnToggleChangedHandle);
        }

        private void OnToggleChangedHandle(bool isOn)
        {
            if (isOn)
            {
                GameManager.Instance.ModelController.ShowValue(_data);
                ProcessEventDefine.CheckItemCall.SendMessage(_monitorDataIndex,_index);
            }
            else
            {
                GameManager.Instance.ModelController.HideValue(_data.id);
            }
        }
        
        private void Update()
        {
            if (_isShowed) return;
            if (!HardwareManager.Instance.GetHardwareState(_data.btnSerialId)) return;
            toggle.isOn = true;
            _isShowed = true;
        }

        public void InitData(int monitorDataIndex,int index,CheckItemData data,bool isChecked)
        {
            _monitorDataIndex = monitorDataIndex;
            _data = data;
            _index = index;
            checkTitle.text = _data.name;
            toggle.isOn = isChecked;
            _isShowed = false;
        }

        public bool GetIsChecked()
        {
            return toggle.isOn;
        }

        public void HideModelValue()
        {
            GameManager.Instance.ModelController.HideValue(_data.id);
            toggle.isOn = false;
        }

        
    }
}
