using UnityEngine;
using UnityEngine.UI;


namespace RainbowArt.CleanFlatUI
{
    public class TogglePullHorizontal : MonoBehaviour
    {
        [SerializeField] 
        private Toggle toggle;
        
        [SerializeField]
        private Animator animator;

        private void Start()
        {
            if (toggle == null)
            {
                toggle = GetComponent<Toggle>();
            }

            if (animator != null)
            {
                animator.enabled = false;
            }
            
            toggle.onValueChanged.AddListener(ToggleValueChanged);
        }

        private void ToggleValueChanged(bool isOn)
        {
            if (animator == null) return;
            if (animator.enabled == false)
            {
                animator.enabled = true;
            }

            animator.Play(isOn ? "TransitionOn" : "TransitionOff", 0, 0);
        }
    }
}

