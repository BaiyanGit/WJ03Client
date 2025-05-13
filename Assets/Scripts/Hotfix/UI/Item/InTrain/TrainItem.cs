using Hotfix.ExcelData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hotfix.UI
{
    public class TrainItem : MonoBehaviour
    {
        Toggle toggle;
        GameObject _showItem;

        Text label;

        private FaultCheckConfig3rd config;
        private Action<int, FaultCheckConfig3rd, bool, bool> selectAction;

        private void Awake()
        {
            toggle ??= GetComponent<Toggle>();
            toggle.group ??= this.GetComponentInParent<ToggleGroup>();
            //_showItem = transform.Find("ShowItem").gameObject;
            label ??= transform.Find("Label").GetComponent<Text>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(ToggleValueChanged);
            ToggleValueChanged(toggle.isOn);
        }

        public void Init(FaultCheckConfig3rd config3rdData, Action<int, FaultCheckConfig3rd, bool, bool> selectAction = null /*, Action<FaultCheckConfig3rd> configAction = null*/)
        {
            Awake();
            config = config3rdData;
            label.text = config3rdData.Title;
            this.selectAction = selectAction;
        }

        private void ToggleValueChanged(bool value)
        {
            if (value)
                selectAction?.Invoke(transform.GetSiblingIndex(), config, value, true);
        }
    }
}