using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleWithID : MonoBehaviour
{
    // Toggle ���
    private Toggle toggle;

    // �Զ��� ID
    private int ID;

    private UnityAction<int, bool, bool> UnityAction;

    public void InitID(int id, UnityAction<int, bool, bool> action)
    {
        ID = id;
        UnityAction = action;
    }

    void Start()
    {
        // ȷ�� Toggle �������
        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
        }

        // ���� Toggle ��״̬�仯
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }
    }

    // Toggle ״̬�仯ʱ�Ļص�
    private void OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            UnityAction.Invoke(ID, isOn, true);
        }
    }
}