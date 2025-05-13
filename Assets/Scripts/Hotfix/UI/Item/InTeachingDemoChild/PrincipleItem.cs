using Hotfix.ExcelData;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix.UI
{
    public class PrincipleItem : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI tmpText;

        private Action<bool, PrincipleItem, bool> _updatePrinciple;

        public PrincipleConfig PrincipleConfigData;
        public string videoName;
        public string modelPath;
        public string text => tmpText.text;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnChangeHandle);
        }

        public void InitData(PrincipleConfig structureConfig, ToggleGroup toggleGroup,
            Action<bool, PrincipleItem, bool> changeAction, string video = null, string model = null)
        {
            PrincipleConfigData = structureConfig;
            _updatePrinciple = changeAction;

            //tmpText.text = PrincipleConfigData.Title;
            tmpText.text = video != null ? video : PrincipleConfigData.Title;
            toggle.group = toggleGroup;

            videoName = video;
            modelPath = model;

            toggle.isOn = false;
            gameObject.SetActive(true);
        }

        private void OnChangeHandle(bool isOn)
        {
            if (PrincipleConfigData == null) return;
            // Debug.Log($"[原理] 选择模型：{PrincipleConfigData.Title}");
            _updatePrinciple?.Invoke(isOn, this, true);
        }
    }
}