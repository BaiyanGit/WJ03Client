using UnityEditor;
using Wx.Runtime.Sound;

namespace Wx.Editor.Sound
{
    [CustomEditor(typeof(WSound))]
    public class SoundInspector : WInspector
    {
        private readonly HelperInfo<SoundHelperBase> _mSoundHelperInfo = new HelperInfo<SoundHelperBase>("Sound");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            var t = (WSound)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                _mSoundHelperInfo.Draw();
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
            _mSoundHelperInfo.Init(serializedObject);
            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            _mSoundHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

    }
}
