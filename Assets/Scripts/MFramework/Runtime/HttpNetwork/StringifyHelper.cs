using System;
using System.IO;
using System.Text.RegularExpressions;
using LitJson;
using UnityEngine;

namespace Wx.Runtime.Http
{
    
    public abstract class StringifyHelper
    {
        /// <summary>
        /// 将类转换至JSON字符串
        /// Convert object to JSON string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string JsonSerialize(object value)
        {
            try
            {
                var json = JsonMapper.ToJson(value);
                //去转义
                json = ConvertTo(json);
                return json;
            }
            catch (IOException ex)
            {
                WLog.Error($"[StringifyHelper] 错误：{ex.Message}, {ex.Data["StackTrace"]}");
                return null;
            }
        }

        /// <summary>
        /// 将JSON字符串转类
        /// Convert JSON string to Class
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T JsonDeSerialize<T>(string value)
        {
            try
            {
                var jsonObj = JsonMapper.ToObject<T>(value);
                return jsonObj;
            }
            catch (IOException ex)
            {
                WLog.Error($"[StringifyHelper] 错误：{ex.Message}, {ex.Data["StackTrace"]}");
                return default(T);
            }
        }

        //汉字转义
        public static string ConvertTo(string json)
        {
            var zRegex = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
            var mJson = zRegex.Replace(json,
                match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString()) ?? throw new ArgumentNullException(nameof(json));
            return mJson;
        }
    }
}