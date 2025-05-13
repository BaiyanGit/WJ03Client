using Hotfix.Event;
using Hotfix.ExcelData;
using UnityEngine;
using Wx.Runtime.Event;


namespace Hotfix
{
    public class ModelParamControl : MonoBehaviour
    {
        FaultCheckConfig4th config;
        /// <summary>
        /// 初始化Model
        /// </summary>
        public void InitModel(FaultCheckConfig4th config)
        {
            this.config = config;
        }

        private void OnMouseDown()
        {
            ProcessEventDefine.CheckPointItemCall.SendMessage(config.Id);
        }
    }
}

