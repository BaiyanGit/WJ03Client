using GameMain.Runtime;
using Hotfix.Event;
using Hotfix.ExcelData;
using System;
using System.Collections.Generic;
using UI.NetworkUI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime;
using Wx.Runtime.Event;

namespace Hotfix.UI
{
    public class CheckPointItem : MonoBehaviour
    {
        public FaultCheckConfig4th config;

        Action<CheckPointItem, bool, bool> action;

        GameObject label;

        Animator anim;

        public GameObject Label
        {
            get
            {
                if (label == null)
                {
                    label = Resources.Load<GameObject>("Prefab/OtherModels/Label");
                }

                return label;
            }
        }

        GameObject currentLabel;

        /// <summary>
        /// 是否是考核模式
        /// </summary>
        bool _isMonitor;

        public List<Transform> ignoreList = new();

        private EventGroup _eventGroup = new();

        private int _index;

        private int _singlePort;
        
        /// <summary>
        /// 带动画和可点击的提示点
        /// </summary>
        Transform tmpTip = null;

        public void Init(FaultCheckConfig4th config, Transform tip, Action<CheckPointItem, bool, bool> action = null, bool isMonitor = false)
        {
            this.config = config;

            _isMonitor = isMonitor;
            _index = config.Id;

            _singlePort = config.SinglechipPort;
            //隐藏非必要模型
            this.action = action;
            for (int i = 0; i < config.TargetModel.Length; i++)
            {
                Transform target = GameManager.Instance.ViewMainTarget.TrackingTarget.FindTheChildNode(config.TargetModel[i]);
                //target.gameObject.AddComponent<>();
                ignoreList.Add(target);
            }

            if (config.TargetModel[0] != null && config.TargetModel[0] != string.Empty && config.TargetModel[0] != "")
            {
                tmpTip = UnityEngine.Object.Instantiate(tip, GameManager.Instance.ViewMainTarget.TrackingTarget.FindTheChildNode(config.TargetModel[0]));
            }
            else tmpTip = UnityEngine.Object.Instantiate(tip, GameManager.Instance.MainTarget);

            Vector3 offsetVector = new Vector3();
            switch (config.TipPosOffsetAxis)
            {
                case "x":
                    offsetVector = new Vector3((float)(1080 * 0.001), 0, 0);
                    break;
                case "y":
                    offsetVector = new Vector3(0, config.TipPosOffsetValue, 0);
                    break;
                case "z":
                    offsetVector = new Vector3(0, 0, config.TipPosOffsetValue);
                    break;
            }

            if (tmpTip != null)
            {
                //tmpTip.localPosition = Vector3.zero + offsetVector;
                tmpTip.localPosition = new Vector3(config.CenterPos[0], config.CenterPos[1], config.CenterPos[2]);

                tmpTip.gameObject.AddComponent<YAxisFaceCamera>();
                anim = tmpTip.GetComponentInChildren<Animator>();
                anim.Play("Wrong");
            }

            CheckItemPoint();

            Debug.Log(_isMonitor);
            GetComponent<Toggle>().onValueChanged.AddListener((value) => OnToggleValueChange(value, true));
            //TODO:初始化监听，点击模型需要进行IsOn
            //if (_isMonitor) { _eventGroup.AddListener<ProcessEventDefine.CheckPointItemCall>(OnCheckPointItemCallHandle); return; }
            if (_isMonitor)
            {
                Ctrl_MessageCenter.AddMsgListener<bool, int>("Comparison", ComparisonSingle);
                return;
            }

            //--不是点击模型，需要添加监听硬件信息

            ////非考核模式需要显示添加的内容
            //currentLabel = Instantiate(Label, GameManager.Instance.ViewMainTarget.TrackingTarget.FindTheChildNode(config.TargetModel[0]));
            //Vector3 pos;
            //if (config.CenterPos.Length >= 3)
            //{
            //    pos = new Vector3(config.CenterPos[0], config.CenterPos[1], config.CenterPos[2]);
            //}
            //else if (config.CenterPos.Length >= 2)
            //{ pos = new Vector3(config.CenterPos[0], config.CenterPos[1], 0); }
            //else pos = new Vector3(config.CenterPos[0], 0, 0);
            //currentLabel.transform.position = pos;
            //currentLabel.SetActive(false);
        }

        public void OnToggleValueChange(bool isOn, bool isHand)
        {
            //this.action?.Invoke(this, isOn);

            anim.Play("Right");
            //currentLabel.SetActive(_isMonitor);
            //Debug.Log("111111");
            //TODO:需要进行检测完成鉴定
            if (!_isMonitor)
            {
                //TopicManager.Instance.TrainCheckIsComplete(config.Id);
                //回调父类里的点击
                action?.Invoke(this, isOn, isHand);
            }
        }

        private void CheckItemPoint()
        {
            CheckPointState checkPointState = TopicManager.Instance.CheckIsComplete(config.Id);
            if (checkPointState.isCompleted)
            {
                Debug.Log("进行了检测");
                GetComponent<Toggle>().isOn = true;
                //TODO:处理对错的UI显示
                anim?.Play("Right");
            }
        }

        /// <summary>
        /// 需要比对是否是该Item
        /// </summary>
        /// <param name="isWin"></param>
        /// <param name="port"></param>
        private void ComparisonSingle(bool isWin, int port)
        {
            Debug.Log(port);
            //如果当前进行的是该Item则进行比对
            if (port == config.Id)
            {
                this.GetComponent<Toggle>().isOn = isWin;
                //TODO:处理错误（已经在TopicManager中进行了处理）
            }
        }

        private void OnCheckPointItemCallHandle(IEventMessage message)
        {
            var msg = (ProcessEventDefine.CheckPointItemCall)message;
            if (msg.modelIndex != _index) return;

            this.GetComponent<Toggle>().isOn = true;
        }

        private void OnDestroy()
        {
            if (tmpTip != null)
            {
                Destroy(tmpTip.gameObject);
            }

            Ctrl_MessageCenter.RemoveMsgListener<bool, int>("Comparison", ComparisonSingle);
        }
        
        /// <summary>
        /// 设置提示点是否可点击
        /// </summary>
        /// <param name="mayClick"></param>
        public void SetTipClcikableState(bool mayClick)
        {
            if (tmpTip != null)
            {
                //var component = tmpTip.GetComponentInChildren<TipItemExtend>();
                //if (component != null) component.enabled = mayClick;
                var collider = tmpTip.GetComponentInChildren<Collider>();
                if (collider != null) collider.enabled = mayClick;
            }
        }
    }
}