using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using Wx.Runtime;

namespace Wx.Editor
{
    /// <summary>
    /// 长按触发 自定义按钮的 inspector GUI
    /// </summary>
    [CustomEditor(typeof(LongClickButton), true)]
    [CanEditMultipleObjects]
    public class LongButtonInspector : ButtonEditor
    {
        private SerializedProperty _longClick;
        private SerializedProperty _pointUp;
        private SerializedProperty _longPressTime;
        private SerializedProperty _btnType;

        protected override void OnEnable()
        {
            base.OnEnable();
            _longClick = serializedObject.FindProperty("mOnLongClick");
            _pointUp = serializedObject.FindProperty("mOnPointerUp");
            _longPressTime = serializedObject.FindProperty("myLongPressTime");
            _btnType = serializedObject.FindProperty("btnType");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(_longClick, new GUIContent("On Long Click"));
            EditorGUILayout.PropertyField(_pointUp, new GUIContent("On Point Up"));
            EditorGUILayout.PropertyField(_longPressTime, new GUIContent("长按时间"));
            EditorGUILayout.PropertyField(_btnType, new GUIContent("按钮类型"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}