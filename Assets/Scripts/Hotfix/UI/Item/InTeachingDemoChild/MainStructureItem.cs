using Hotfix.ExcelData;
using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix.UI
{
    public class MainStructureItem : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI tmpText;

        private Action<bool, MainStructureItem, bool> _updateStructure;

        public CameraTarget CameraTarget = new();

        public MainStructureConfig StructureConfigData;

        private void Awake()
        {
            toggle.onValueChanged.AddListener((state) => { OnChangeHandle(state, true); });
        }

        public void InitData(MainStructureConfig structureConfig, ToggleGroup toggleGroup,
            Action<bool, MainStructureItem, bool> changeAction = null)
        {
            StructureConfigData = structureConfig;
            _updateStructure = changeAction;

            var path = $"{AppConst.AssetPathConst.ItemImage}{structureConfig.Image}";
            image.sprite = GameEntry.Resource.BuildInResource.Load<Sprite>(path);
            tmpText.text = StructureConfigData.Title;
            toggle.group = toggleGroup;

            var target = GameManager.Instance.ViewMainTarget.TrackingTarget.Find(structureConfig.ModelPath);
            CameraTarget.TrackingTarget = target;
            CameraTarget.LookAtTarget = target;
        }

        private void OnChangeHandle(bool isOn, bool isHand)
        {
            if (StructureConfigData == null) return;
            // Debug.Log($"[部件] 选择模型：{StructureConfigData.Title} {StructureConfigData.Description}");
            _updateStructure?.Invoke(isOn, this, isHand);
        }
    }
}