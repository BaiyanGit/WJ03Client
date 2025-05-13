using UnityEngine;
using System.Collections.Generic;
using Hotfix.ExcelData;
using System.Linq;
using System;

/// <summary>
/// 数据展示公用界面
/// </summary>
public class UIDataScreenCommon : MonoBehaviour
{
    /// <summary>
    /// 容器列表
    /// </summary>
    public List<UIDataScreenContainer> ContainerList;
    public GameObject ContainerPrefab;
    public List<UIDataScreenInteractionItem> InteractionItemList = new();

    void Start()
    {
        for (int i = 0; i < ContainerList.Count; i++)
        {
            var temp = Instantiate(ContainerPrefab);
            temp.transform.SetParent(transform, false);
            temp.transform.localScale = Vector3.one;
            var rt = temp.GetComponent<RectTransform>();
            rt.anchorMin = ContainerList[i].AnchorsMin;
            rt.anchorMax = ContainerList[i].AnchorsMax;
            rt.offsetMin = ContainerList[i].OffsetMin;
            rt.offsetMax = ContainerList[i].OffsetMax;
        }
    }

    public virtual void InitValue(DataCheckConfig config)
    {
        //List<int> values = DataCheckTopicManager.Instance.GetConfigItem(config.DefaultID);
        
    }

    /// <summary>
    /// 创造错误假数据
    /// </summary>
    /// <param name="values">正确数据的取值范围</param>
    /// <param name="isValues">是否可以两头取错误值</param>
    /// <returns></returns>
    public float CreateFakeValue(float[] values, bool isValues = false)
    {
        float tmpValue = 0;

        if (isValues)
        {
            //随机取值
            float tmpValue1 = UnityEngine.Random.Range(0, values[0]);
            float tmpValue2 = UnityEngine.Random.Range(values[1], values[1] + 10);
            List<float> tmpValues = new List<float>() { tmpValue1, tmpValue2 };
            var randomValue = tmpValues.OrderBy(x => Guid.NewGuid()).Take(1).ToList();
            tmpValue = randomValue[0];
        }
        else
        {
            tmpValue = UnityEngine.Random.Range(values[1], values[1] + (values[1] * .5f));
        }

        return tmpValue;
    }
    
    protected virtual void OnEnable()
    {
        Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>("PadOnSensorInteraction", OnPadOnSensorInteraction);
    }

    protected virtual void OnDisable()
    {
        Ctrl_MessageCenter.RemoveMsgListener<MsgUINavigationData>("PadOnSensorInteraction", OnPadOnSensorInteraction);
    }

    private void OnPadOnSensorInteraction(MsgUINavigationData msg)
    {
        if (InteractionItemList.Count > msg.optionIndex)
        {
            InteractionItemList[msg.optionIndex].OnInteraction();
        }
    }
    
    /// <summary>
    /// 应用索引
    /// </summary>
    protected virtual void ApplyItemIndex()
    {
        if (InteractionItemList == null || InteractionItemList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < InteractionItemList.Count; i++)
        {
            InteractionItemList[i].Index = i;
        }
    }
}