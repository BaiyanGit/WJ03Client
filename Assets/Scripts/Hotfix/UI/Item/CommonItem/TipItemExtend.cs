using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Hotfix.ExcelData;

/// <summary>
/// 设置检查点的信息
/// </summary>
public class TipItemExtend : MonoBehaviour
{
    Action<DataCheckConfig> clickAction;
    DataCheckConfig dataConfig;

    /// <summary>
    /// 初始化
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