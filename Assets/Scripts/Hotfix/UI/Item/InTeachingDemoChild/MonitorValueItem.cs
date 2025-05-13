using System;
using TMPro;
using UnityEngine;

namespace Hotfix.UI
{
    public class MonitorValueItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI referenceValue;
        [SerializeField]
        private TextMeshProUGUI hardwareValue;

        private CheckItemData _data;

        public void InitData(CheckItemData data)
        {
            _data = data;
            referenceValue.text = _data.referenceValue;
        }

        private void Update()
        {
            if (_data == null) return;
            hardwareValue.text = $"{HardwareManager.Instance.GetHardwareValue(_data.serialId)}";

        }
    }
}
