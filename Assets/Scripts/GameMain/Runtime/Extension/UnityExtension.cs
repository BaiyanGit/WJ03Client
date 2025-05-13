using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Runtime
{
    public static class UnityExtension
    {
        public static List<T> GetComponentsInChildrenWithoutTx<T, TX>(this Transform transform) where T: Component where TX: Component
        {
            var components = new List<T>();
            InternalGetComponentsInChildrenWithoutTx<T, TX>(transform,ref components);
            return components;
        }

        private static void InternalGetComponentsInChildrenWithoutTx<T, TX>(Transform transform,ref List<T> components)
            where T : Component where TX : Component
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if(child.GetComponent<TX>() != null)continue;
                if (!child.TryGetComponent(out T component)) continue;
                components.Add(component);
                InternalGetComponentsInChildrenWithoutTx<T, TX>(child, ref components);
            }
        }
    }
}