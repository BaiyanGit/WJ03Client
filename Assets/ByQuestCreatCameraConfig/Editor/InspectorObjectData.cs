namespace ByQuestCreatCameraConfig.Editor
{
    using UnityEditor;
    using UnityEngine;
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(ObjectDataSnap))]
    public class InspectorObjectData : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var saver = (ObjectDataSnap)target;

            if (!ObjectSnapManager.Instance.isReadFromFile)
            {
                if (GUILayout.Button("记录属性"))
                {
                    saver.AddObjectAttributes();
                }
            }
        }
    }
}