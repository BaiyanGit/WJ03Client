using System;
using Hotfix.ExcelData;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状态监测-可交互元素（采集）
/// </summary>
public class UIDataScreenInteractionItem : MonoBehaviour
{
    protected EquipmentCheckConfig2nd config;
    protected Button btn;

    public int Index;

    void Awake()
    {
        btn = GetComponent<Button>();
        if (btn)
        {
            btn.onClick.AddListener(OnCheckBtnClicked);
        }
    }

    private void InitComponent()
    {
        if (!btn)
        {
            btn = GetComponent<Button>();
        }
    }

    public virtual void OnInteraction()
    {
        InitComponent();
        if (btn)
        {
            //不给Pad同步数据
            btn.onClick.RemoveListener(OnCheckBtnClicked);
            btn.onClick.Invoke();
            //给pad同步数据
            btn.onClick.AddListener(OnCheckBtnClicked);
        }
    }

    private void OnCheckBtnClicked()
    {
        //Send Msg
        //先注释掉，得深入到具体按钮内部才能执行功能
        Ctrl_MessageCenter.SendMessage("OnAnyCheckItemChecked", Index);
    }
}