using System.Collections.Generic;

using UnityEngine;

public class FakeDataProcess 
{
    /// <summary>
    /// ��ȡΨһ�����������-1��ʾ�������������꣩
    /// </summary>
    /// <param name="nums"></param>
    /// <returns></returns>
    public static int GetUniqueRandomNumber(List<int> nums)
    {
        if (nums.Count == 0)
        {
            Debug.LogWarning("�������������꣡");
            return -1;
        }

        // ���ѡ��һ������
        int randomIndex = Random.Range(0, nums.Count);
        int selectedNumber = nums[randomIndex];

        // �Ƴ���ѡ����
        nums.RemoveAt(randomIndex);

        return selectedNumber;
    }
}
