using GameMain.Runtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Wx.Runtime.Event;

namespace Hotfix.UI
{
    public class ToggleExtend : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Toggle toggle;
        private Text[] toggleTexts;
        bool isPointer;

        public Action<Toggle> OnPointerEnterAction;
        public Action<Toggle> OnPointerExitAction;
        private EventGroup _eventGroup = new();
        public EnumAssessmentMode assessmentModel;
        public TrainType trainType;

        /// <summary>
        /// 是否自动初始化状态
        /// </summary>
        public bool _isInitState;

        private void Awake()
        {
            InitComponents();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(OnToggleOnClickHandle);
            //toggle.onValueChanged.AddListener(ToggleValueChanged);
            ToggleValueChanged(toggle.isOn);
        }

        /// <summary>
        /// 初始化设置Text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isValue">设置是否鼠标进入现实</param>
        public void Init(string value, bool isValue = true, bool isUse = true, TrainType trainTy = TrainType.None)
        {
            InitComponents();

            isPointer = isValue;
            //assessmentModel = model;
            toggle.interactable = isUse;
            trainType = trainTy;

            for (int i = 0; i < toggleTexts.Length; i++)
            {
                toggleTexts[i].text = value;
            }

            _isInitState = true;
        }

        public void SetToggleInitState()
        {
            _isInitState = false;
        }

        private void InitComponents()
        {
            if (!toggle) toggle = GetComponent<Toggle>();
            if (toggleTexts == null) toggleTexts = toggle.transform.GetComponentsInChildren<Text>();
        }

        public void ToggleValueChanged(bool value)
        {
            transform.Find("IsOn").gameObject.SetActive(value);
            transform.Find("IsOff").gameObject.SetActive(!value);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isPointer) return;
            //TODO:AssessmentMode需要换位置进行赋值
            //GameManager.Instance.AssessmentMode = assessmentModel;
            GameManager.Instance.TrainType = trainType;

            toggle.Select();
            ToggleValueChanged(true);
            OnPointerEnterAction?.Invoke(toggle);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isPointer) return;

            ToggleValueChanged(false);
            OnPointerExitAction?.Invoke(toggle);
        }

        void OnToggleOnClickHandle(bool value)
        {
            if (!_isInitState) { ToggleValueChanged(value); return; }

            Invoke("InitToggleState", .5f);
        }

        /// <summary>
        /// 初始化按钮状态
        /// </summary>
        public void InitToggleState()
        {
            this.toggle.isOn = false;
            transform.Find("IsOn").gameObject.SetActive(false);
            transform.Find("IsOff").gameObject.SetActive(true);
        }

    }
}