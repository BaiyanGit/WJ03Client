using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Hotfix.ExcelData;

/// <summary>
/// ���ü������Ϣ
/// </summary>
public class TipItemExtend : MonoBehaviour
{
    Action<DataCheckConfig> clickAction;
    DataCheckConfig dataConfig;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void Init(Action<DataCheckConfig> action, DataCheckConfig uiName)
    {
        clickAction = action;
        dataConfig = uiName;
        //Debug.Log(uiName.Id);
    }

    private void OnMouseDown()
    {
        clickAction?.Invoke(dataConfig);
        Ctrl_MessageCenter.SendMessage("OnDataCheckPointClick", dataConfig, true);
    }
}