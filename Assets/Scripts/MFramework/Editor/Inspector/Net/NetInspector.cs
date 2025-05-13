using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Wx.Runtime.Net;

namespace Wx.Editor.Net
{
    [CustomEditor(typeof(WNet))]
    public class NetInspector : WInspector
    {
        private readonly HelperInfo<MessageHelperBase> _mMessageHelperInfo = new HelperInfo<MessageHelperBase>("Message");

        private SerializedProperty _heartBeat;

        private SerializedProperty _heartBeatTime;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            var t = (WNet)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                _mMessageHelperInfo.Draw();
                EditorGUILayout.PropertyField(_heartBeat);
                EditorGUILayout.PropertyField(_heartBeatTime);
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
            _mMessageHelperInfo.Init(serializedObject);
            _heartBeat = serializedObject.FindProperty("heartBeat");
            _heartBeatTime = serializedObject.FindProperty("heartBeatTime");
            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            _mMessageHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
    }

}