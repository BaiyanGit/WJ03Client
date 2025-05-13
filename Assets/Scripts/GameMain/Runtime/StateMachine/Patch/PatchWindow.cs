using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.Event;

namespace GameMain.Runtime
{
    public class PatchWindow : MonoBehaviour
    {
        /// <summary>
        /// 对话框封装类
        /// </summary>
        private class MessageBox
        {
            private GameObject _cloneObject;
            private Text _content;
            private Button _btnOk;
            private Action _clickOk;

            public bool ActiveSelf
            {
                get { return _cloneObject.activeSelf; }
            }

            public void Create(GameObject cloneObject)
            {
                _cloneObject = cloneObject;
                _content = cloneObject.transform.Find("txt_content").GetComponent<Text>();
                _btnOk = cloneObject.transform.Find("btn_ok").GetComponent<Button>();
                _btnOk.onClick.AddListener(OnClickYes);
            }

            public void Show(string content, Action clickOK)
            {
                _content.text = content;
                _clickOk = clickOK;
                _cloneObject.SetActive(true);
                _cloneObject.transform.SetAsLastSibling();
            }

            public void Hide()
            {
                _content.text = string.Empty;
                _clickOk = null;
                _cloneObject.SetActive(false);
            }

            private void OnClickYes()
            {
                _clickOk?.Invoke();
                Hide();
            }
        }


        private readonly EventGroup _eventGroup = new EventGroup();
        private readonly List<MessageBox> _msgBoxList = new List<MessageBox>();

        // UGUI相关
        private GameObject _messageBoxObj;
        private Slider _slider;
        private Text _tips;


        private void Awake()
        {
            _slider = transform.Find("UIWindow/Slider").GetComponent<Slider>();
            _tips = transform.Find("UIWindow/Slider/txt_tips").GetComponent<Text>();
            _tips.text = "Initializing the game world !";
            _messageBoxObj = transform.Find("UIWindow/MessgeBox").gameObject;
            _messageBoxObj.SetActive(false);

            _eventGroup.AddListener<PatchEventDefine.ReRequestVersion>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.ReDownLoadApplication>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.InitializeFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.PatchStatesChange>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.FoundUpdateFiles>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.DownloadProgressUpdate>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.PackageVersionUpdateFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.PatchManifestUpdateFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.WebFileDownloadFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.DoneShow>(OnHandleEventMessage);
        }

        private void OnDestroy()
        {
            _eventGroup.RemoveAllListener();
        }
        
        

        /// <summary>
        /// 接收事件
        /// </summary>
        private void OnHandleEventMessage(IEventMessage message)
        {
            switch (message)
            {
                case PatchEventDefine.DoneShow msg:
                {
                    ShowMessageBox($"{msg.show}",Application.Quit);
                    break;
                }
                case PatchEventDefine.ReRequestVersion:
                {
                    ShowMessageBox($"Failed to get version ! Try to get again?",UserEventDefine.UserTryGetVersion.SendEventMessage);
                    break;
                }
                case PatchEventDefine.ReDownLoadApplication:
                {
                    ShowMessageBox($"Please go to store to download new version!", () =>
                    {
                        Application.OpenURL("www.baidu.com");
#if UNITY_EDITOR
                        EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                    });
                    break;
                }
                case PatchEventDefine.InitializeFailed:
                {
                    ShowMessageBox($"Failed to initialize package !", UserEventDefine.UserTryInitialize.SendEventMessage);
                    break;
                }
                case PatchEventDefine.PatchStatesChange change:
                {
                    _tips.text = change.tips;
                    break;
                }
                case PatchEventDefine.FoundUpdateFiles files:
                {
                    var sizeMb = files.totalSizeBytes / 1048576f;
                    sizeMb = Mathf.Clamp(sizeMb, 0.1f, float.MaxValue);
                    var totalSizeMb = sizeMb.ToString("f1");
                    ShowMessageBox($"Found update patch files, Total count {files.totalCount} Total size {totalSizeMb}MB",
                        UserEventDefine.UserBeginDownloadWebFiles.SendEventMessage);
                    break;
                }
                case PatchEventDefine.DownloadProgressUpdate update:
                {
                    _slider.value = (float)update.currentDownloadCount / update.totalDownloadCount;
                    var currentSizeMb = (update.currentDownloadSizeBytes / 1048576f).ToString("f1");
                    var totalSizeMb = (update.totalDownloadSizeBytes / 1048576f).ToString("f1");
                    _tips.text = $"{update.currentDownloadCount}/{update.totalDownloadCount} {currentSizeMb}MB/{totalSizeMb}MB";
                    break;
                }
                case PatchEventDefine.PackageVersionUpdateFailed:
                {
                    ShowMessageBox($"Failed to update static version, please check the network status.", UserEventDefine.UserTryUpdatePackageVersion.SendEventMessage);
                    break;
                }
                case PatchEventDefine.PatchManifestUpdateFailed:
                {
                    ShowMessageBox($"Failed to update patch manifest, please check the network status.", UserEventDefine.UserTryUpdatePatchManifest.SendEventMessage);
                    break;
                }
                case PatchEventDefine.WebFileDownloadFailed failed:
                {
                    ShowMessageBox($"Failed to download file : {failed.fileName}", UserEventDefine.UserTryDownloadWebFiles.SendEventMessage);
                    break;
                }
                default:
                    throw new NotImplementedException($"{message.GetType()}");
            }
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        private void ShowMessageBox(string content, Action ok)
        {
            // 尝试获取一个可用的对话框
            MessageBox msgBox = null;
            foreach (var item in _msgBoxList)
            {
                if (item.ActiveSelf) continue;
                msgBox = item;
                break;
            }

            // 如果没有可用的对话框，则创建一个新的对话框
            if (msgBox == null)
            {
                msgBox = new MessageBox();
                var cloneObject = Instantiate(_messageBoxObj, _messageBoxObj.transform.parent);
                msgBox.Create(cloneObject);
                _msgBoxList.Add(msgBox);
            }

            // 显示对话框
            msgBox.Show(content, ok);
        }
    }
}