using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Tool.Extend
{
    public class CustomCinemachineInputAxisControllerProvider : MonoBehaviour
    {
        [SerializeField]
        private CinemachineInputAxisController cinemachineInputProvider;

        private void Update()
        {
            if (!IsPointerOverUIElement())
            {
                if (Input.GetMouseButton(0))
                {
                    cinemachineInputProvider.Controllers[0].Enabled = true;
                    cinemachineInputProvider.Controllers[1].Enabled = true;
                }
                else
                {
                    cinemachineInputProvider.Controllers[0].Enabled = false;
                    cinemachineInputProvider.Controllers[1].Enabled = false;
                }

                cinemachineInputProvider.Controllers[2].Enabled = true;
            }
            else
            {
                cinemachineInputProvider.Controllers[0].Enabled = false;
                cinemachineInputProvider.Controllers[1].Enabled = false;
                cinemachineInputProvider.Controllers[2].Enabled = false;
            }
        }

        public static bool IsPointerOverUIElement()
        {
            return GetEventSystemRaycastResults() != null && GetEventSystemRaycastResults().Count > 0;
        }

        static List<RaycastResult> GetEventSystemRaycastResults()
        {
            if (EventSystem.current == null) return null;
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            if (Pointer.current == null) return null;
            eventData.position = Pointer.current.position.ReadValue();
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            raycastResults.RemoveAll(x => x.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"));
            return raycastResults;
        }
    }
}