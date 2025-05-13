using System;
using System.Text;

namespace Wx.Runtime
{
    public static partial class Utility
    {
        /// <summary>
        /// �ַ���ص�ʵ�ú�����
        /// </summary>
        public static class Text
        {
            [ThreadStatic]
            private static StringBuilder s_CachedStringBuilder = null;

            /// <summary>
            /// ��ȡ��ʽ���ַ�����
            /// </summary>
            /// <param name="format">�ַ�����ʽ��</param>
            /// <param name="arg0">�ַ������� 0��</param>
            /// <returns>��ʽ������ַ�����</returns>
            public static string Format(string format, object arg0)
            {
                if (format == null)
                {
                    throw new Exception("Format is invalid.");
                }

                CheckCachedStringBuilder();
                s_CachedStringBuilder.Length = 0;
                s_CachedStringBuilder.AppendFormat(format, arg0);
                return s_CachedStringBuilder.ToString();
            }

            /// <summary>
            /// ��ȡ��ʽ���ַ�����
            /// </summary>
            /// <param name="format">�ַ�����ʽ��</param>
            /// <param name="arg0">�ַ������� 0��</param>
            /// <param name="arg1">�ַ������� 1��</param>
            /// <returns>��ʽ������ַ�����</returns>
            public static string Format(string format, object arg0, object arg1)
            {
                if (format == null)
                {
                    throw new Exception("Format is invalid.");
                }

                CheckCachedStringBuilder();
                s_CachedStringBuilder.Length = 0;
                s_CachedStringBuilder.AppendFormat(format, arg0, arg1);
                return s_CachedStringBuilder.ToString();
            }

            /// <summary>
            /// ��ȡ��ʽ���ַ�����
            /// </summary>
            /// <param name="format">�ַ�����ʽ��</param>
            /// <param name="arg0">�ַ������� 0��</param>
            /// <param name="arg1">�ַ������� 1��</param>
            /// <param name="arg2">�ַ������� 2��</param>
            /// <returns>��ʽ������ַ�����</returns>
            public static string Format(string format, object arg0, object arg1, object arg2)
            {
                if (format == null)
                {
                    throw new Exception("Format is invalid.");
                }

                CheckCachedStringBuilder();
                s_CachedStringBuilder.Length = 0;
                s_CachedStringBuilder.AppendFormat(format, arg0, arg1, arg2);
                return s_CachedStringBuilder.ToString();
            }

            /// <summary>
            /// ��ȡ��ʽ���ַ�����
            /// </summary>
            /// <param name="format">�ַ�����ʽ��</param>
            /// <param name="args">�ַ���������</param>
            /// <returns>��ʽ������ַ�����</returns>
            public static string Format(string format, params object[] args)
            {
                if (format == null)
                {
                    throw new Exception("Format is invalid.");
                }

                if (args == null)
                {
                    throw new Exception("Args is invalid.");
                }

                CheckCachedStringBuilder();
                s_CachedStringBuilder.Length = 0;
                s_CachedStringBuilder.AppendFormat(format, args);
                return s_CachedStringBuilder.ToString();
            }

            private static void CheckCachedStringBuilder()
            {
                if (s_CachedStringBuilder == null)
                {
                    s_CachedStringBuilder = new StringBuilder(1024);
                }
            }
        }
    }
}
