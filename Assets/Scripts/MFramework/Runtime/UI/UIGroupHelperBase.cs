using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.UI
{
    /// <summary>
    /// �����鸨�������ࡣ
    /// </summary>
    public abstract class UIGroupHelperBase : MonoBehaviour
    {
        /// <summary>
        /// ���ý�������ȡ�
        /// </summary>
        /// <param name="depth">��������ȡ�</param>
        public abstract void SetDepth(int depth);
    }
}
