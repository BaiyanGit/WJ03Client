using Hotfix.ExcelData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix.UI
{
    public class TopicItem : MonoBehaviour
    {
        Button toggle;
        GameObject _showItem;

        Text label;

        private FaultCheckConfig2nd config;
        private Action<int, FaultCheckConfig2nd, bool> selectAction;
        
        private int theIndex;

        private void Awake()
        {
            toggle = GetComponent<Button>();
            //_showItem = transform.Find("ShowItem").gameObject;
            label = transform.Find("Label").GetComponent<Text>();
        }

        private void Start()
        {
            toggle.onClick.AddListener(ToggleValueChanged);
        }

        public void Init(int index, FaultCheckConfig2nd config2ndData, Action<int, FaultCheckConfig2nd, bool> selectAction = null, Action<FaultCheckConfig3rd> configAction = null)
        {
            theIndex = index;
            config = config2ndData;
            label.text = config2ndData.Title;
            this.selectAction = selectAction;
        }

        private void ToggleValueChanged()
        {
            selectAction?.Invoke(theIndex, config, true);
        }
    }
}