using Hotfix.ExcelData;
using UnityEngine;

namespace Hotfix.Runtime
{
    /// <summary>
    /// 用于保存模型相机属性的工具类
    /// </summary>
    public class ModelCameraPropertyTools : MonoBehaviour
    {
        private bool m_IsEditing;
        private bool m_IsSaving;

        private CinemachineCameraController3X m_CameraController
        {
            get
            {
                return GameManager.Instance.CinemachineCameraController;
            }
        }

        private void Update()
        {
            
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
            {
                m_IsEditing = !m_IsEditing;
            }

            if (m_IsEditing)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
                {
                    m_IsSaving = true;
                    EditorModelCameraProperty();
                    m_IsSaving = false;
                }
            }
        }

        private void OnGUI()
        {
            if (m_IsEditing)
            {
                GUILayout.Label("编辑模式", GUILayout.Width(100f), GUILayout.Height(20f));

                if (m_IsSaving)
                {
                    GUILayout.Label("修改中...", GUILayout.Width(100f), GUILayout.Height(20f));
                }
            }
        }

        private void EditorModelCameraProperty()
        {
            if (m_CameraController == null) return;
            if (m_CameraController.target == null) return;
            WLog.Log("编辑相机属性");
            
            int currentLabelEntry = GameManager.Instance.GetCurrentLabelEntry();
            string currentType = GameManager.Instance.GetCurrentType();

            if (currentLabelEntry == -1) return;
            if (string.IsNullOrEmpty(currentType)) return;
            
            ModelCameraPosition cameraPosConfig = ModelCameraPositionTable.Instance.GetCameraPositionByLabelAndType(currentLabelEntry, currentType);
            if (cameraPosConfig == null) return;

            Vector3 cameraPosition = m_CameraController.transform.localPosition;
            float[] position = new float[]
            {
                cameraPosition.x,
                cameraPosition.y,
                cameraPosition.z
            };
            
            Vector3 cameraRotation = m_CameraController.transform.eulerAngles;
            float[] rotation = new float[]
            {
                cameraRotation.x,
                cameraRotation.y,
                cameraRotation.z
            };
            
            float defaultDistance = m_CameraController.distance;

            Vector3 panOffset = m_CameraController.GetPanOffset();
            float[] pan = new float[]
            {
                panOffset.x,
                panOffset.y,
                panOffset.z
            };

            cameraPosConfig.CPosition = position;
            cameraPosConfig.CRotation = rotation;
            cameraPosConfig.DefDistance = defaultDistance;
            cameraPosConfig.PanOffset = pan;

            ModelCameraPositionTable.Instance.isChanged = true;
        }
    }
}
