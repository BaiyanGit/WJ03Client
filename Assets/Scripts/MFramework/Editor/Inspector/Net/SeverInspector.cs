
using UnityEditor;
using Wx.Runtime.Net;
using Wx.Runtime.Sever;

namespace Wx.Editor.Net
{
    [CustomEditor(typeof(WSever))]
    public class SeverInspector : WInspector
    {
        private readonly HelperInfo<MessageHelperBase> _mMessageHelperInfo = new HelperInfo<MessageHelperBase>("Message");

        private SerializedProperty _heartBeat;

        private SerializedProperty _heartBeatTime;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            var t = (WSever)target;

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
