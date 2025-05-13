using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime
{
    public static partial class AppEntry
    {
        private static readonly LinkedList<WModule> WModules = new LinkedList<WModule>();

        public static void RegisterModule(WModule wModule)
        {
            if(wModule == null)
            {
                throw new Exception("WComponent is invalid");
            }

            Type type = wModule.GetType();

            LinkedListNode<WModule> current = WModules.First;
            while(current != null)
            {
                if(current.Value.GetType() == type)
                {
                    WLog.Error($"WComponent :{type.FullName} is already exist.");
                    return;
                }

                if(wModule.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if(current != null)
            {
                WModules.AddBefore(current, wModule);
            }
            else
            {
                WModules.AddLast(wModule);
            }
        }

        public static T GetModule<T>() where T : WModule
        {
            return (T)GetModule(typeof(T));
        }

        public static WModule GetModule(Type type)
        {
            LinkedListNode<WModule> current = WModules.First;
            while(current != null)
            {
                if(current.Value.GetType() == type)
                {
                    return current.Value;
                }
                current = current.Next;
            }

            return null;
        }

        public static void Update()
        {
            foreach(var wMoudle in WModules)
            {
                wMoudle.OnUpdate(Time.deltaTime,Time.unscaledDeltaTime);
            }
        }

        public static void FixedUpdate()
        {
            foreach (var wMoudle in WModules)
            {
                wMoudle.OnFixedUpdate(Time.fixedDeltaTime,Time.fixedUnscaledDeltaTime);
            }
        }

        public static void LateUpdate()
        {
            foreach (var wMoudle in WModules)
            {
                wMoudle.OnLateUpdate();
            }
        }

        public static void Destroy()
        {
            foreach (var wMoudle in WModules)
            {
                wMoudle.OnDestroy();
            }
        }
    }
}
