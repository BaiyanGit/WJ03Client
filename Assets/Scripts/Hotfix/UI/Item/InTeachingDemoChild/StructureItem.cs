using Hotfix.ExcelData;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime;

namespace Hotfix.UI
{
    public class StructureItem : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI tmpText;

        private Action<bool, StructureItem, bool> _updateStructure;

        public StructureConfig StructureConfigData;
        public List<CameraTarget> CameraTargets = new();
        public List<Transform> IgnoreList;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnChangeHandle);
        }

        public void InitData(StructureConfig structureConfig, ToggleGroup toggleGroup,
            Action<bool, StructureItem, bool> changeAction = null, Transform parent = null)
        {
            if (IgnoreList != null && IgnoreList.Count > 0)
            {
                IgnoreList.Clear();
                CameraTargets.Clear();
            }

            IgnoreList = new List<Transform>() { parent };

            StructureConfigData = structureConfig;
            _updateStructure = changeAction;

            tmpText.text = StructureConfigData.Title;
            toggle.group = toggleGroup;

            toggle.isOn = false;
            gameObject.SetActive(true);

            for (int i = 0; i < structureConfig.ModelPaths.Length; i++)
            {
                var temp = new CameraTarget();
                var target =
                    GameManager.Instance.ViewMainTarget.TrackingTarget.FindTheChildNode(structureConfig.ModelPaths[i]);
                temp.TrackingTarget = target;
                temp.LookAtTarget = target;
                CameraTargets.Add(temp);
            }

            if (structureConfig.IgnoreModelPaths != null && structureConfig.IgnoreModelPaths.Length != 0)
            {
                for (int i = 0; i < structureConfig.IgnoreModelPaths.Length; i++)
                {
                    var target =
                        GameManager.Instance.ViewMainTarget.TrackingTarget.FindTheChildNode(
                            structureConfig.ModelPaths[i]);
                    IgnoreList.Add(target);
                }
            }
        }

        private void OnChangeHandle(bool isOn)
        {
            if (StructureConfigData == null) return;
            // Debug.Log($"[认知] 选择模型：{StructureConfigData.Title}");
            _updateStructure?.Invoke(isOn, this, true);
        }
    }
}