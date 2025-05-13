using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Wx.Runtime.UI
{
    public class WUI : WModule
    {
        [SerializeField] private string mUIGroupHelperTypeName = "Wx.Runtime.DefaultUIGroupHelper";
        [SerializeField] private string mUIFormHelperTypeName = "Wx.Runtime.DefaultUIFormHelper";

        [SerializeField] private UIGroupHelperBase mCustomUIGroupHelper = null;

        [SerializeField] private UIFormHelperBase mCustomUIFormHelper = null;

        private UIFormHelperBase _uiFormHelper;

        private Dictionary<string, UIGroup> _uiGroups;

        private Dictionary<string, IUIForm> _uiLoadedForms;

        private Dictionary<string, IUIForm> _uiShowingForms;

        [SerializeField] private Camera uiCamera;

        [SerializeField] private Transform instanceRoot = null;

        public override int Priority => 6;

        protected override void Awake()
        {
            base.Awake();
            WLog.Log($"{nameof(WUI)} Initialize");

            _uiGroups = new Dictionary<string, UIGroup>();
            _uiLoadedForms = new Dictionary<string, IUIForm>();
            _uiShowingForms = new Dictionary<string, IUIForm>();

            _uiFormHelper = Helper.CreateHelper(mUIFormHelperTypeName, mCustomUIFormHelper);

            if (_uiFormHelper == null)
            {
                WLog.Error("Can not create UI Form Helper");
                return;
            }

            _uiFormHelper.name = "UI Form Helper";
            var thisTransform = _uiFormHelper.transform;
            thisTransform.SetParent(gameObject.transform);
            thisTransform.localScale = Vector3.one;

            if (uiCamera == null)
            {
                uiCamera = new GameObject("UI Camera").AddComponent<Camera>();
                uiCamera.transform.localPosition = Vector3.zero;
                uiCamera.transform.localRotation = Quaternion.identity;
                uiCamera.transform.localScale = Vector3.one;
                uiCamera.clearFlags = CameraClearFlags.SolidColor;
                uiCamera.backgroundColor = Color.black;
                uiCamera.orthographic = true;
            }

            if (instanceRoot == null)
            {
                instanceRoot = new GameObject("UI Form Instance").transform;
                instanceRoot.SetParent(gameObject.transform);
                instanceRoot.localScale = Vector3.one;

                var canvas = instanceRoot.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = uiCamera;

                var canvasScaler = instanceRoot.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);

                instanceRoot.AddComponent<GraphicRaycaster>();
            }

            instanceRoot.gameObject.layer = LayerMask.NameToLayer("UI");

            foreach (var value in Enum.GetValues(typeof(UIGroupInfo)))
            {
                if (AddUIGroup(value.ToString(), (int)value)) continue;
                WLog.Warning($"Add UI group '{value}' failure.");
                continue;
            }
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var uiGroup in _uiGroups)
            {
                uiGroup.Value.Update(deltaTime, unscaledDeltaTime);
            }
        }

        public override void OnFixedUpdate(float fixedDeltaTime, float unscaledFixedDeltaTime)
        {
            foreach (var uiGroup in _uiGroups)
            {
                uiGroup.Value.FixedUpdate();
            }
        }

        public bool HasUIGroup(string uiGroupName)
        {
            if (string.IsNullOrEmpty(uiGroupName))
            {
                throw new Exception("UI Group name is invalid");
            }

            return _uiGroups.ContainsKey(uiGroupName);
        }

        public UIGroup GetUIGroup(string uiGroupName)
        {
            if (string.IsNullOrEmpty(uiGroupName))
            {
                throw new Exception("UI Group name is invalid");
            }

            return _uiGroups.GetValueOrDefault(uiGroupName);
        }

        public bool AddUIGroup(string uiGroupName, int uiGroupDepth)
        {
            if (HasUIGroup(uiGroupName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(uiGroupName))
            {
                throw new Exception("UI Group name is invalid");
            }

            var uiGroupHelper = Helper.CreateHelper(mUIGroupHelperTypeName, mCustomUIGroupHelper, _uiGroups.Count);

            if (uiGroupHelper == null)
            {
                throw new Exception("UI Group helper is invalid");
            }

            uiGroupHelper.name = Utility.Text.Format("UI Group - {0}", uiGroupName);
            uiGroupHelper.gameObject.layer = LayerMask.NameToLayer("UI");
            var groupTransform = uiGroupHelper.transform;
            groupTransform.SetParent(instanceRoot);
            groupTransform.localPosition = Vector3.zero;
            groupTransform.localRotation = Quaternion.identity;
            groupTransform.localScale = Vector3.one;

            _uiGroups.Add(uiGroupName, new UIGroup(uiGroupName, uiGroupDepth, uiGroupHelper));
            return true;
        }

        public bool HasUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UI form asset name is invalid.");
            }

            return _uiGroups.Any(uiGroup => uiGroup.Value.HasUIForm(uiFormAssetName));
        }

        public IUIForm GetUIForm(string uiFormAssetName)
        {
            return (from uiGroup in _uiGroups where uiGroup.Value.HasUIForm(uiFormAssetName) select uiGroup.Value.GetUIForm(uiFormAssetName)).FirstOrDefault();
        }


        public T OpenUIFormSync<T>(string uiFormAssetName, bool pauseCoverUIForm = false, object userData = null) where T : class, IUIForm
        {
            if (_uiFormHelper == null)
            {
                throw new Exception("UIForm Helper is invalid");
            }

            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UIForm asset name is invalid");
            }

            UIGroup uiGroup;
            if (!_uiLoadedForms.TryGetValue(uiFormAssetName, out var uiForm))
            {
                uiForm = CreateClass(typeof(T).Name, typeof(T).Namespace);

                uiGroup = GetUIGroup(uiForm.SetUIGroupInfo().ToString());
                if ((uiGroup == null))
                {
                    throw new Exception($"UIGroup is invalid.{uiForm.SetUIGroupInfo()}");
                }

                var handle = _uiFormHelper.InstantiateUIForm(uiFormAssetName, uiGroup) as UIFormLogic;
                _uiLoadedForms.Add(uiFormAssetName, uiForm);

                uiForm.OnInit(uiFormAssetName, uiGroup, pauseCoverUIForm, handle);
            }

            uiGroup = GetUIGroup(uiForm.SetUIGroupInfo().ToString());
            if ((uiGroup == null))
            {
                throw new Exception($"UIGroup is invalid.{uiForm.SetUIGroupInfo()}");
            }

            if (_uiShowingForms.TryAdd(uiFormAssetName, uiForm))
            {
                uiGroup.AddUIForm(uiForm);
                uiForm.OnOpen(userData);
                uiGroup.Refresh();
            }
            else
            {
                RefocusUIForm(uiForm, userData);
            }

            return uiForm as T;
        }

        public async UniTask<T> OpenUIFormAsync<T>(string uiFormAssetName, bool pauseCoverUIForm = false, object userData = null) where T : class, IUIForm
        {
            if (_uiFormHelper == null)
            {
                throw new Exception("UIForm Helper is invalid");
            }

            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UIForm asset name is invalid");
            }

            UIGroup uiGroup;
            if (!_uiLoadedForms.TryGetValue(uiFormAssetName, out var uiForm))
            {
                uiForm = CreateClass(typeof(T).Name, typeof(T).Namespace);

                uiGroup = GetUIGroup(uiForm.SetUIGroupInfo().ToString());
                if ((uiGroup == null))
                {
                    throw new Exception($"UIGroup is invalid.{uiForm.SetUIGroupInfo()}");
                }

                var handle = await _uiFormHelper.InstantiateUIFormAsync(uiFormAssetName, uiGroup) as UIFormLogic;
                _uiLoadedForms.Add(uiFormAssetName, uiForm);

                uiForm.OnInit(uiFormAssetName, uiGroup, pauseCoverUIForm, handle);
            }

            uiGroup = GetUIGroup(uiForm.SetUIGroupInfo().ToString());
            if ((uiGroup == null))
            {
                throw new Exception($"UIGroup is invalid.{uiForm.SetUIGroupInfo()}");
            }

            if (_uiShowingForms.TryAdd(uiFormAssetName, uiForm))
            {
                uiGroup.AddUIForm(uiForm);
                uiForm.OnOpen(userData);
                uiGroup.Refresh();
            }
            else
            {
                RefocusUIForm(uiForm, userData);
            }

            return uiForm as T;
        }

        public bool CloseUIForm(IUIForm uiForm, bool isRecycle = false, object userData = null)
        {
            if (uiForm == null)
            {
                throw new Exception("UIForm is invalid");
            }

            var uiGroup = uiForm.UIGroup;
            if (uiGroup == null)
            {
                throw new Exception($"UIForm group is invalid {uiForm.UIFormAssetName}");
            }

            if (!_uiShowingForms.ContainsValue(uiForm))
            {
                return true;
            }

            _uiShowingForms.Remove(uiForm.UIFormAssetName);
            uiGroup.RemoveUIForm(uiForm);
            uiForm.Close(userData);
            uiGroup.Refresh();

            //扩展界面关闭广播事件
            Ctrl_MessageCenter.SendMessage("UIFormClose", uiForm.UIFormAssetName, userData);

            if (!isRecycle) return true;
            uiForm.OnRecycle();
            _uiLoadedForms.Remove(uiForm.UIFormAssetName);

            return true;
        }

        public void CloseAllLoadUIForms(bool isRecycle = false, object userData = null)
        {
            foreach (var uiForm in _uiLoadedForms)
            {
                CloseUIForm(uiForm.Value, isRecycle, userData);
            }
        }

        public void CloseUIFormWithout(IUIForm uiForm, bool isRecycle = false, object userData = null)
        {
            foreach (var form in _uiLoadedForms.Where(form => form.Value != uiForm))
            {
                CloseUIForm(form.Value, isRecycle, userData);
            }
        }

        public void RefocusUIForm(IUIForm uiForm, object userData = null)
        {
            if (uiForm == null)
            {
                throw new Exception("UI form is invalid.");
            }

            var uiGroup = uiForm.UIGroup;
            if (uiGroup == null)
            {
                throw new Exception("UI group is invalid.");
            }

            uiGroup.RefocusUIForm(uiForm, userData);
            uiGroup.Refresh();
            uiForm.OnRefocus(userData);
        }


        private IUIForm CreateClass(string uiName, string uiNamespace)
        {
            var uiOriginName = uiName.Replace("UI", "");
            var uiModelName = "UIModel" + uiOriginName;
            var uiViewName = "UIView" + uiOriginName;

            var uiType = Utility.Assembly.GetType($"{uiNamespace}.{uiName}");
            var uiModelType = Utility.Assembly.GetType($"{uiNamespace}.{uiModelName}");
            var uiViewType = Utility.Assembly.GetType($"{uiNamespace}.{uiViewName}");

            var uiForm = Activator.CreateInstance(uiType) as IUIForm;
            var uiModel = Activator.CreateInstance(uiModelType) as IUIModel;
            var uiView = Activator.CreateInstance(uiViewType) as IUIView;

            if (uiForm == null) return null;
            uiForm.UIView = uiView;
            uiForm.UIModel = uiModel;

            return uiForm;
        }
    }
}