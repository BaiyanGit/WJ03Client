using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime
{
    public static class Helper
    {
        public static T CreateHelper<T>(string helperTypeName,T customHelper) where T : MonoBehaviour
        {
            return CreateHelper(helperTypeName, customHelper, 0);
        }

        public static T CreateHelper<T>(string helperTypeName,T customHelper,int index)where T : MonoBehaviour
        {
            T helper = null;
            if (!string.IsNullOrEmpty(helperTypeName))
            {
                System.Type helperType = Utility.Assembly.GetType(helperTypeName);
                if(helperType == null)
                {
                    WLog.Warning(Utility.Text.Format("Can not find helper type {0}", helperType));
                    return null;
                }

                if (!typeof(T).IsAssignableFrom(helperType))
                {
                    WLog.Warning(Utility.Text.Format("Type '{0}' is not assignable from '{1}'.", typeof(T).FullName, helperType.FullName));
                    return null;
                }
                helper = (T)new GameObject(helperTypeName).AddComponent(helperType);
            }
            else if(customHelper == null)
            {
                WLog.Warning(Utility.Text.Format("You must set custom helper with '{0}' type first.", typeof(T).FullName));
                return null;
            }
            else if (customHelper.gameObject.InScene())
            {
                helper = index > 0 ? Object.Instantiate(customHelper) : customHelper;
            }
            else
            {
                helper = Object.Instantiate(customHelper);
            }
            return helper;
        }
    }
}
