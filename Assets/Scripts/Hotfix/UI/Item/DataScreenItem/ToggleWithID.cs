using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleWithID : MonoBehaviour
{
    // Toggle 组件
    private Toggle toggle;

    // 自定义 ID
    private int ID;

    private UnityAction<int, bool, bool> UnityAction;

    public void InitID(int id, UnityAction<int, bool, bool> action)
    {
        ID = id;
        UnityAction = action;
    }

    void Start()
    {
        // 确保 Toggle 组件存在
        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
        }

        // 监听 Toggle 的状态变化
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }
    }

    // Toggle 状态变化时的回调
    private void OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            UnityAction.Invoke(ID, isOn, true);
        }
    }
}