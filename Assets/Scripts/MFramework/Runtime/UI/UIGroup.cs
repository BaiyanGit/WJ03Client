using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wx.Runtime.UI
{
    public class UIGroup
    {
        private readonly string _mName;
        private int _mDepth;
        private bool _mPause;
        private readonly UIGroupHelperBase _mUIGroupHelper;
        private readonly LinkedList<IUIForm> _mUIForms;
        private LinkedListNode<IUIForm> _mCachedNode;

        public UIGroup(string name,int depth, UIGroupHelperBase uiGroupHelper)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("UI Group name is invalid");
            }

            if(uiGroupHelper == null)
            {
                throw new Exception("UI Group helper is invalid");
            }

            _mName = name;
            _mPause = false;
            _mUIGroupHelper = uiGroupHelper;
            _mUIForms = new LinkedList<IUIForm>();
            _mCachedNode = null;
            Depth = depth;
        }
        public string Name
        {
            get
            {
                return _mName;
            }
        }

        public int Depth
        {
            get
            {
                return _mDepth;
            }
            set
            {
                if (_mDepth == value)
                {
                    return;
                }

                _mDepth = value;
                _mUIGroupHelper.SetDepth(_mDepth);
                Refresh();
            }
        }

        public bool Pause
        {
            get
            {
                return _mPause;
            }
            set
            {
                if (_mPause == value)
                {
                    return;
                }

                _mPause = value;
                Refresh();
            }
        }

        public int UIFormCount
        {
            get
            {
                return _mUIForms.Count;
            }
        }

        public IUIForm CurrentUIForm
        {
            get => _mUIForms.First.Value ?? null;
        }

        public Transform Handle
        {
            get => _mUIGroupHelper.transform;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            var current = _mUIForms.First;
            while (current != null)
            {
                if (current.Value.Paused)
                {
                    break;
                }

                _mCachedNode = current.Next;
                current.Value.OnUpdate(elapseSeconds, realElapseSeconds);
                current = _mCachedNode;
                _mCachedNode = null;
            }
        }

        public void FixedUpdate()
        {
            var current = _mUIForms.First;
            while (current != null)
            {
                if (current.Value.Paused)
                {
                    break;
                }

                _mCachedNode = current.Next;
                current.Value.OnFixedUpdate();
                current = _mCachedNode;
                _mCachedNode = null;
            }
        }
        

        public bool HasUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UIForm name is invalid");
            }

            return _mUIForms.Any(uiForm => uiForm.UIFormAssetName == uiFormAssetName);
        }

        public IUIForm GetUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UIForm name is invalid");
            }

            return _mUIForms.FirstOrDefault(uiForm => uiForm.UIFormAssetName == uiFormAssetName);
        }

        public IUIForm[] GetAllUIForms()
        {
            return _mUIForms.ToArray();
        }

        public void AddUIForm(IUIForm uiForm)
        {
            _mUIForms.AddFirst(uiForm);
        }

        public void RemoveUIForm(IUIForm uiForm)
        {
            if(uiForm == null)
            {
                throw new Exception("UIForm is invalid");
            }

            /*if (!uiForm.Covered)
            {
                uiForm.Covered = true;
                uiForm.OnCover();
            }
            if (!uiForm.Paused)
            {
                uiForm.Paused = true;
                uiForm.OnPause();
            }*/

            if(_mCachedNode != null && _mCachedNode.Value == uiForm)
            {
                _mCachedNode = _mCachedNode.Next;
            }

            if (!_mUIForms.Remove(uiForm))
            {
                throw new Exception($"Remove uiForm Error. UIGroup :{Name} UIForm :{uiForm.UIFormAssetName}");
            }
        }

        public void RefocusUIForm(IUIForm uiForm,object userData)
        {
            if(uiForm == null)
            {
                throw new Exception("UIForm is invalid");
            }
            _mUIForms.Remove(uiForm);
            _mUIForms.AddFirst(uiForm);
        }

        /// <summary>
        /// 刷新界面组。
        /// </summary>
        public void Refresh()
        {
            var current = _mUIForms.First;
            var pause = _mPause;
            var cover = false;
            var depth = UIFormCount;
            while (current is { Value: not null })
            {
                var next = current.Next;
                current.Value.OnDepthChanged(Depth, depth--);
                if (current.Value == null)
                {
                    return;
                }

                if (pause)
                {
                    if (!current.Value.Covered)
                    {
                        current.Value.Covered = true;
                        current.Value.OnCover();
                        if (current.Value == null)
                        {
                            return;
                        }
                    }

                    if (!current.Value.Paused)
                    {
                        current.Value.Paused = true;
                        current.Value.OnPause();
                        if (current.Value == null)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    if (current.Value.Paused)
                    {
                        current.Value.Paused = false;
                        current.Value.OnResume();
                        if (current.Value == null)
                        {
                            return;
                        }
                    }

                    if (current.Value.PauseCoveredUIForm)
                    {
                        pause = true;
                    }

                    if (cover)
                    {
                        if (!current.Value.Covered)
                        {
                            current.Value.Covered = true;
                            current.Value.OnCover();
                            if (current.Value == null)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (current.Value.Covered)
                        {
                            current.Value.Covered = false;
                            current.Value.OnReveal();
                            if (current.Value == null)
                            {
                                return;
                            }
                        }

                        cover = true;
                    }
                }

                current = next;
            }
        }

    }
}
