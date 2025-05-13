using UnityEngine;
using System.Net;
using LitJson;
using System.IO;
using System.Text;
using System;
using Wx.Runtime.Singleton;
using System.Threading.Tasks;

namespace Hotfix
{
    public class HtttpWebRequestManager : SingletonInstance<HtttpWebRequestManager>, ISingleton
    {
        /// <summary>
        /// POST请求方法
        /// </summary>
        /// <param name="url">获取Token值的服务URL地址（很重要）</param>
        /// <param name="jsonData">传入请求的参数，此处参数为JOSN格式</param>
        public static void HttpPost<T>(string url, object data, Action<T> responseCallback = null)
        {
            string jsonData = JsonMapper.ToJson(data);
            Debug.Log(jsonData);
            url = string.Format("http://{0}:{1}{2}", AppConst.UrlConst.severIP, AppConst.UrlConst.severPort, url);
            Debug.Log(url);
            //处理HttpWebRequest访问https有安全证书的问题（ 请求被中止: 未能创建 SSL/TLS 安全通道。）
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

            byte[] byteArray = Encoding.UTF8.GetBytes(jsonData);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            //httpWebRequest.ContentLength = byteArray.Length;  //此处注释的情况下需要调整“myResponseStream.Close();"执行顺序
            Stream myResponseStream = httpWebRequest.GetRequestStream();
            myResponseStream.Write(byteArray, 0, byteArray.Length);
            myResponseStream.Close(); //"httpWebRequest.ContentLength"未赋值的情况下需要在此处执行关闭语句；若赋值了，可以放在最后执行关闭语句---
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            response.Close();
            if (!string.IsNullOrEmpty(retString))
            {
                Debug.Log(retString);
                responseCallback?.Invoke(JsonMapper.ToObject<T>(retString));
            }
        }

        /// <summary>
        /// GET请求
        /// ContentType = "application/json";
        /// token为Authorization中的授权验证码
        /// </summary>
        /// <param name="Url">请求数据的URL地址</param>
        /// <param name="token">token为Authorization中的授权验证码</param>
        /// <returns></returns>
        public static void HttpGet(string url, string token)
        {
            //处理HttpWebRequest访问https有安全证书的问题（ 请求被中止: 未能创建 SSL/TLS 安全通道。）
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/json";
            //if (authorization != null)
            {
                request.Headers.Add("authorization", token);//请求头文件
                                                            //Debug.LogError(authorization);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            response.Close();
            if (!string.IsNullOrEmpty(retString))
            {
                Debug.LogError(retString);
                //GetDataInfo getDataInfo = JsonMapper.ToObject<GetDataInfo>(retString);
                //Debug.LogError(getDataInfo.address["name"]);
                //Debug.LogError(getDataInfo.companyInfo["companyName"]);
                //Debug.LogError(getDataInfo.alarmCount);
                //Debug.LogError(getDataInfo.faultCount);
            }
        }

        /// <summary>
        /// http异步请求
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="reqMethod">请求方法 GET、POST</param>
        /// <param name="callback">回调函数</param>
        /// <param name="ob">回传对象</param>
        /// <param name="postData">post数据</param>
        public static void HttpAsyncRequest(string url, string reqMethod, AsyRequetCallback callback, object ob = null, string postData = "")
        {
            Stream requestStream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = reqMethod;
                if (reqMethod.ToUpper() == "POST")
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = bytes.Length;
                    requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                //开始调用异步请求 
                //AsyResultTag 是自定义类 用于传递调用时信息 其中HttpWebRequest 是必须传递对象。
                //因为回调需要用HttpWebRequest来获取HttpWebResponse 
                request.BeginGetResponse(new AsyncCallback(HttpCallback), new AsyResultTag() { obj = ob, callback = callback, req = request });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                }
            }
        }

        /// <summary>
        /// 异步POST请求方法
        /// </summary>
        public static async Task HttpPostAsync<T>(string url, object data, Action<T> responseCallback = null, Action<Exception> errorCallback = null)
        {
            try
            {
                string jsonData = JsonMapper.ToJson(data);
                Debug.Log($"Request JSON: {jsonData}");

                // 构建完整URL
                url = $"http://{AppConst.UrlConst.severIP}:{AppConst.UrlConst.severPort}{url}";
                Debug.Log($"Request URL: {url}");

                // HTTPS证书验证设置
                ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

                // 创建请求对象
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";

                // 异步写入请求体
                byte[] payload = Encoding.UTF8.GetBytes(jsonData);
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(payload, 0, payload.Length);
                }

                // 异步获取响应
                using (WebResponse response = await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string responseText = await reader.ReadToEndAsync();
                    Debug.Log($"Response: {responseText}");

                    // 处理响应数据
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        T result = JsonMapper.ToObject<T>(responseText);
                        responseCallback?.Invoke(result);
                    }
                }
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                string errorResponse = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                Debug.LogError($"HTTP Error {(int)response.StatusCode}: {errorResponse}");
                errorCallback?.Invoke(new Exception($"HTTP Error: {response.StatusCode}"));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Request Failed: {ex}");
                errorCallback?.Invoke(ex);
            }
        }

        /// <summary>
        /// http请求回调 由.net内部调用 参数必须为IAsyncResult
        /// </summary>
        /// <param name="asynchronousResult">http回调时回传对象</param>
        private static void HttpCallback(IAsyncResult asynchronousResult)
        {
            int statusCode = 0;
            string retString = "";
            AsyResultTag tag = new AsyResultTag();
            WebException webEx = null;
            try
            {
                //获取请求时传递的对象
                tag = asynchronousResult.AsyncState as AsyResultTag;
                HttpWebRequest req = tag.req;
                //获取异步返回的http结果
                HttpWebResponse response = req.EndGetResponse(asynchronousResult) as HttpWebResponse;
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                statusCode = ((int)response.StatusCode);

            }
            catch (WebException ex)
            {
                if ((HttpWebResponse)ex.Response != null)
                {
                    statusCode = ((int)((HttpWebResponse)ex.Response).StatusCode);
                }

                webEx = ex;
            }
            //调用外部回调 即最外层的回调
            tag.callback(tag.obj, retString, statusCode, webEx);

        }

        /// <summary>
        /// 异步请求回调委托
        /// </summary>
        /// <param name="asyObj">回传对象</param>
        /// <param name="resStr">http响应结果</param>
        /// <param name="statusCode">http状态码</param>
        /// <param name="webEx">异常</param>
        public delegate void AsyRequetCallback(object asyObj, string respStr, int statusCode, WebException webEx);

        /// <summary>
        /// 异步返回对象
        /// </summary>
        class AsyResultTag
        {
            /// <summary>
            /// 回传对象
            /// </summary>
            public object obj { get; set; }
            /// <summary>
            /// 当前httpRequest请求实例
            /// </summary>
            public HttpWebRequest req { get; set; }
            /// <summary>
            /// 回调函数委托
            /// </summary>
            public AsyRequetCallback callback { get; set; }
        }

        public void OnCreate(object createParam)
        {
            throw new NotImplementedException();
        }

        public void OnUpdate()
        {
            throw new NotImplementedException();
        }

        public void OnFixedUpdate()
        {
            throw new NotImplementedException();
        }

        public void OnLateUpdate()
        {
            throw new NotImplementedException();
        }

        public void OnDestroy()
        {
            throw new NotImplementedException();
        }
    }
}