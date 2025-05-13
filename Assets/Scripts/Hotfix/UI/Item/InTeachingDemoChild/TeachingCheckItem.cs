using Hotfix.ExcelData;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix.UI
{
    public class TeachingCheckItem : MonoBehaviour
    {
        public TextMeshProUGUI titleOne;
        public TextMeshProUGUI titleTwo;
        public Image bg;
        public Toggle toggle;
        public GameObject error;

        private GameObject _tsShowItem;

        private readonly Color _defaultColor = Color.white;
        private readonly Color _errorColor = Color.red;

        private CheckItemData _data;
        private int _minReferenceValue;
        private int _maxReferenceValue;

        private EquipmentCheckConfig1st config;
        private Action<EquipmentCheckConfig1st,bool> selectAction;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnToggleChangedHandle);
        }

        private void Start()
        {
            InvokeRepeating("CheckCheckItem", 0.1f, 0.23f);
        }

        private void Update()
        {
            //TODO...获取信号是否有误并改变背景颜色
            //注释掉，太浪费，LateUpdate或者InvokeRepeat
            //CheckCheckItem();
        }

        private void OnToggleChangedHandle(bool isOn)
        {
            if (isOn)
            {
                //TODO...显示信号数值提示框
                GameManager.Instance.ModelController?.ShowValue(_data);
            }
            else
            {
                GameManager.Instance.ModelController?.HideValue(_data.id);
            }

            this.selectAction?.Invoke(config,isOn);
        }

        public void InitData(CheckItemData data, ToggleGroup toggleGroup,EquipmentCheckConfig1st config = null, Action<EquipmentCheckConfig1st,bool> selectAction = null)
        {
            _data = data;
            titleOne.text = data.name;
            titleTwo.text = data.name;

            toggle.group = toggleGroup;
            toggle.isOn = false;

            var referenceValue = data.referenceValue.Split('-');
            _minReferenceValue = int.Parse(referenceValue[0]);
            _maxReferenceValue = int.Parse(referenceValue[1]);

            this.config = config;
            this.selectAction = selectAction;
        }

        public void ResetCheckItem()
        {
            toggle.isOn = false;
        }

        private void CheckCheckItem()
        {
            var hardwareValue = HardwareManager.Instance.GetHardwareValue(_data.serialId);
            error.SetActive(hardwareValue < _minReferenceValue || hardwareValue > _maxReferenceValue);

            //if (hardwareValue < _minReferenceValue || hardwareValue > _maxReferenceValue)
            //{
            //    bg.color = _errorColor;
            //}
            //else
            //{
            //    bg.color = _defaultColor;
            //}
        }
    }
}