using GameMain.Runtime;
using UnityEditor;
using Wx.Editor;

namespace GameMain.Editor
{
    [CustomEditor(typeof(AppEntry))]
    public class AppEntryInspector : WInspector
    {
        private SerializedProperty _playMode;
        private SerializedProperty _ePlayMode;
        private SerializedProperty _packageName;
        private SerializedProperty _buildPipeline;
        private SerializedProperty _assetsSever;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(_playMode);
                
                var playMode = (EnumPlayMode)_playMode.enumValueIndex;
                if (playMode == EnumPlayMode.YooAsset)
                {
                    EditorGUILayout.PropertyField(_ePlayMode);
                    EditorGUILayout.PropertyField(_packageName);
                    EditorGUILayout.PropertyField(_buildPipeline);
                    EditorGUILayout.PropertyField(_assetsSever);
                }
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        private void OnEnable()
        {
            _playMode = serializedObject.FindProperty("playMode");
            _ePlayMode = serializedObject.FindProperty("ePlayMode");
            _packageName = serializedObject.FindProperty("packageName");
            _buildPipeline = serializedObject.FindProperty("buildPipeline");
            _assetsSever = serializedObject.FindProperty("assetsSever");
        }
    }
}
