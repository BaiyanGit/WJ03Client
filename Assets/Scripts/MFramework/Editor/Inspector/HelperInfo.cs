using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Wx.Runtime;

namespace Wx.Editor
{
    internal sealed class HelperInfo<T> where T : MonoBehaviour
    {
        private const string CustomOptionName = "<Custom>";

        private readonly string _mName;

        private SerializedProperty _mHelperTypeName;
        private SerializedProperty _mCustomHelper;
        private string[] _mHelperTypeNames;
        private int _mHelperTypeNameIndex;

        public HelperInfo(string name)
        {
            _mName = name;

            _mHelperTypeName = null;
            _mCustomHelper = null;
            _mHelperTypeNames = null;
            _mHelperTypeNameIndex = 0;
        }

        public void Init(SerializedObject serializedObject)
        {
            _mHelperTypeName = serializedObject.FindProperty(Utility.Text.Format("m{0}HelperTypeName", _mName));
            _mCustomHelper = serializedObject.FindProperty(Utility.Text.Format("mCustom{0}Helper", _mName));
        }

        public void Draw()
        {
            var displayName = FieldNameForDisplay(_mName);
            var selectedIndex = EditorGUILayout.Popup(Utility.Text.Format("{0} Helper", displayName), _mHelperTypeNameIndex, _mHelperTypeNames);
            if (selectedIndex != _mHelperTypeNameIndex)
            {
                _mHelperTypeNameIndex = selectedIndex;
                _mHelperTypeName.stringValue = selectedIndex <= 0 ? null : _mHelperTypeNames[selectedIndex];
            }

            if (_mHelperTypeNameIndex > 0) return;
            EditorGUILayout.PropertyField(_mCustomHelper);
            if (_mCustomHelper.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(Utility.Text.Format("You must set Custom {0} Helper.", displayName), MessageType.Error);
            }
        }

        public void Refresh()
        {
            var helperTypeNameList = new List<string>
            {
                CustomOptionName
            };

            helperTypeNameList.AddRange(Type.GetTypeNames(typeof(T)));
            _mHelperTypeNames = helperTypeNameList.ToArray();

            _mHelperTypeNameIndex = 0;
            if (string.IsNullOrEmpty(_mHelperTypeName.stringValue)) return;
            _mHelperTypeNameIndex = helperTypeNameList.IndexOf(_mHelperTypeName.stringValue);
            if (_mHelperTypeNameIndex > 0) return;
            _mHelperTypeNameIndex = 0;
            _mHelperTypeName.stringValue = null;
        }

        private string FieldNameForDisplay(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return string.Empty;
            }

            var str = Regex.Replace(fieldName, @"^_m", string.Empty);
            str = Regex.Replace(str, @"((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", @" $1").TrimStart();
            return str;
        }
    }
}
