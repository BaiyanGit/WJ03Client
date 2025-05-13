using System.Collections.Generic;

using UnityEngine;

public class FakeDataProcess 
{
    /// <summary>
    /// 获取唯一随机数（返回-1表示所有数字已用完）
    /// </summary>
    /// <param name="nums"></param>
    /// <returns></returns>
    public static int GetUniqueRandomNumber(List<int> nums)
    {
        if (nums.Count == 0)
        {
            Debug.LogWarning("所有数字已用完！");
            return -1;
        }

        // 随机选择一个索引
        int randomIndex = Random.Range(0, nums.Count);
        int selectedNumber = nums[randomIndex];

        // 移除已选数字
        nums.RemoveAt(randomIndex);

        return selectedNumber;
    }
}
