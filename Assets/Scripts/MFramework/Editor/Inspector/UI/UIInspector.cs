using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Wx.Runtime;
using Wx.Runtime.UI;

namespace Wx.Editor.UI
{
    [CustomEditor(typeof(WUI))]
    internal class UIInspector : WInspector
    {
        private readonly HelperInfo<UIGroupHelperBase> _mUIGroupHelperInfo = new HelperInfo<UIGroupHelperBase>("UIGroup");
        private readonly HelperInfo<UIFormHelperBase> _mUIFormHelperInfo = new HelperInfo<UIFormHelperBase>("UIForm");
        private SerializedProperty _mUICamera = null;
        private SerializedProperty _mUIFormInstance = null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            var t = (WUI)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                _mUIGroupHelperInfo.Draw();
                _mUIFormHelperInfo.Draw();
                EditorGUILayout.PropertyField(_mUICamera);
                EditorGUILayout.PropertyField(_mUIFormInstance);
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();
            RefreshTypeNames();
        }

        private void OnEnable()
        {
            _mUICamera = serializedObject.FindProperty("uiCamera");
            _mUIFormInstance = serializedObject.FindProperty("instanceRoot");

            _mUIGroupHelperInfo.Init(serializedObject);
            _mUIFormHelperInfo.Init(serializedObject);
            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            _mUIGroupHelperInfo.Refresh();
            _mUIFormHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
