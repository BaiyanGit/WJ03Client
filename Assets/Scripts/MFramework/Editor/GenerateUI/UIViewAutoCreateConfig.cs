using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Editor.UI
{
    [Serializable]
    public class UIViewAutoCreateInfo
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string propName;

        /// <summary>
        /// 组件名
        /// </summary>
        public string comName;
    }

    public class UIViewAutoCreateConfig : ScriptableObject
    {
        public List<UIViewAutoCreateInfo> uiInfoList;
    }
}
