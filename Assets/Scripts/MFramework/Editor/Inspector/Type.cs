using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Wx.Editor
{
    /// <summary>
    /// ������ص�ʵ�ú�����
    /// </summary>
    internal static class Type
    {
        private static readonly string[] AssemblyNames =
        {
            "Assembly-CSharp",
            "Wx.Runtime",
            "GameMain.Runtime",
        };

        private static readonly string[] EditorAssemblyNames =
        {
            "Assembly-CSharp-Editor",
            "Wx.Editor",
            "GameMain.Editor"
        };


        /// <summary>
        /// ��ȡָ�������������������ơ�
        /// </summary>
        /// <param name="typeBase">�������͡�</param>
        /// <returns>ָ�������������������ơ�</returns>
        internal static string[] GetTypeNames(System.Type typeBase)
        {
            return GetTypeNames(typeBase, AssemblyNames);
        }

        /// <summary>
        /// ��ȡָ�������������������ơ�
        /// </summary>
        /// <param name="typeBase">�������͡�</param>
        /// <returns>ָ�������������������ơ�</returns>
        internal static string[] GetEditorTypeNames(System.Type typeBase)
        {
            return GetTypeNames(typeBase, EditorAssemblyNames);
        }

        private static string[] GetTypeNames(System.Type typeBase, string[] assemblyNames)
        {
            List<string> typeNames = new List<string>();
            foreach (string assemblyName in assemblyNames)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch
                {
                    continue;
                }

                if (assembly == null)
                {
                    continue;
                }

                System.Type[] types = assembly.GetTypes();
                foreach (System.Type type in types)
                {
                    if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                    {
                        typeNames.Add(type.FullName);
                    }
                }
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }
    }
}
