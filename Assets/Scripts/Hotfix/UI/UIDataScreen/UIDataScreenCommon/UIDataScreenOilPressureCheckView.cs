using UnityEngine;

/// <summary>
/// 油压监测子界面
/// </summary>
public class UIDataScreenOilPressureCheckView : UIDataScreenCommon
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    
    void Awake()
    {
        //添加按钮交互组件
        InteractionItemList.Add(gameObject.AddComponent<UIDataScreenInteractionItem>());
        InteractionItemList.Add(gameObject.AddComponent<UIDataScreenInteractionItem>());
        
        ApplyItemIndex();
    }
}
