
using UnityEditor;
using Wx.Runtime.Pool;

namespace Wx.Editor.OjectPool
{
    [CustomEditor(typeof(WPool))]
    public class ObjectPoolInspector : WInspector
    {
        private readonly HelperInfo<PoolHelperBase> _mPoolHelperInfo = new HelperInfo<PoolHelperBase>("Pool");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            var t = (WPool)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                _mPoolHelperInfo.Draw();
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
            _mPoolHelperInfo.Init(serializedObject);
            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            _mPoolHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
