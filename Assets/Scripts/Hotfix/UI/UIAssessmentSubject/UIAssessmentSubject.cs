using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameMain.Runtime;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using TMPro;

namespace Hotfix.UI
{
    public struct UIAssessmentSubjectData
    {
        public int modelId;
    }
    
    /// <summary>
    /// 考核课题选择界面
    /// </summary>
    public class UIAssessmentSubject : UIForm<UIViewAssessmentSubject, UIModelAssessmentSubject>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private UIViewAssessmentSubject _view;
        private UIModelAssessmentSubject _model;
        private ScrollViewEx _scrollViewEx;

        #endregion

        #region Private

        private List<SubjectData> _subjectData;
        private ListEx<SubjectItem> _subjectItems;
        private int _modelId;

        #endregion

        public override UIGroupInfo SetUIGroupInfo()
        {
            return base.SetUIGroupInfo();
        }

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm,
            UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            InitField();
            InitUIComponent();
            InitUIListener();

            showPauseAndResumeAction = true;
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            if (userData is UIAssessmentSubjectData data)
            {
                _modelId = data.modelId;
                _view.tmptxtTitle.text = _modelId switch
                {
                    AppConst.IDConst.SelfExamSubjectId => $"SelfExam",
                    AppConst.IDConst.AssessmentSubjectId => $"Assessment",
                    _ => _view.tmptxtTitle.text
                };
            }
            
            UniTask.Void(async () =>
            {
                await GetSubjectData();
                GenerateSubject();
                ResetUIComponent();
            });

        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnCover()
        {
            base.OnCover();
        }

        public override void OnReveal()
        {
            base.OnReveal();
        }

        public override void OnRefocus(object userData = null)
        {
            base.OnRefocus(userData);
        }

        public override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);
        }

        #region UI

        private void InitUIComponent()
        {
            _scrollViewEx = _view.tsScrollView.GetComponent<ScrollViewEx>();
        }

        private void ResetUIComponent()
        {
            _scrollViewEx.ResetScrollEx();
        }

        private void InitUIListener()
        {
            _view.btnClose.onClick.AddListener(OnCloseHandle);
            _scrollViewEx.onIndexChanged += OnSubjectChangedHandle;
        }

        private void OnCloseHandle()
        {
            //GameEntry.UI.OpenUIFormAsync<UIAssessment>().Forget();
            _uGuiForm.Close();
            
        }
        
        private void OnSubjectChangedHandle(int index)
        {
            if (_subjectData == null || _subjectData.Count == 0) return;
            _view.tmptxtDescribe.text = _subjectData[index].describe;
        }

        #endregion

        #region  Field

        private void InitField()
        {
            _subjectItems = new ListEx<SubjectItem>();
        }

        #endregion

        #region InternalLogic

        private async UniTask GetSubjectData()
        {
            /*_subjectData = await HttpDownloader.GetSubjectList(_modelId,() =>
            {
                UniTask.Void(async () => { await GetSubjectData(); });
            });*/

            _subjectData = new List<SubjectData>()
            {
                new SubjectData()
                {
                    name = "故障1",
                    describe = "故障描述1"
                },
                new SubjectData()
                {
                    name = "故障2",
                    describe = "故障描述2"
                }
            };
        }

        private void GenerateSubject()
        {
            if (_subjectData == null || _subjectData.Count == 0) return;
            var goCache = GameEntry.Resource.BuildInResource.Load<GameObject>(AppConst.AssetPathConst.SubjectItem);
            
            _subjectItems.UpdateItem(_subjectData, goCache,_view.tsContent);
            
            for (var i = 0; i < _subjectData.Count; i++)
            {
                _subjectItems.self[i].InitData(_subjectData[i],EnumMonitorMode.Assessment);
                _subjectItems.self[i].gameObject.SetActive(true);
            }
            _scrollViewEx.Init();
            _scrollViewEx.ResetScrollEx();
        }
        

        #endregion


    }
}
