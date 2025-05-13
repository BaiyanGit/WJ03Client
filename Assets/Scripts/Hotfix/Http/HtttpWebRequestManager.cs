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
        /// POST���󷽷�
        /// </summary>
        /// <param name="url">��ȡTokenֵ�ķ���URL��ַ������Ҫ��</param>
        /// <param name="jsonData">��������Ĳ������˴�����ΪJOSN��ʽ</param>
        public static void HttpPost<T>(string url, object data, Action<T> responseCallback = null)
        {
            string jsonData = JsonMapper.ToJson(data);
            Debug.Log(jsonData);
            url = string.Format("http://{0}:{1}{2}", AppConst.UrlConst.severIP, AppConst.UrlConst.severPort, url);
            Debug.Log(url);
            //����HttpWebRequest����https�а�ȫ֤������⣨ ������ֹ: δ�ܴ��� SSL/TLS ��ȫͨ������
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

            byte[] byteArray = Encoding.UTF8.GetBytes(jsonData);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            //httpWebRequest.ContentLength = byteArray.Length;  //�˴�ע�͵��������Ҫ������myResponseStream.Close();"ִ��˳��
            Stream myResponseStream = httpWebRequest.GetRequestStream();
            myResponseStream.Write(byteArray, 0, byteArray.Length);
            myResponseStream.Close(); //"httpWebRequest.ContentLength"δ��ֵ���������Ҫ�ڴ˴�ִ�йر���䣻����ֵ�ˣ����Է������ִ�йر����---
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
        /// GET����
        /// ContentType = "application/json";
        /// tokenΪAuthorization�е���Ȩ��֤��
        /// </summary>
        /// <param name="Url">�������ݵ�URL��ַ</param>
        /// <param name="token">tokenΪAuthorization�е���Ȩ��֤��</param>
        /// <returns></returns>
        public static void HttpGet(string url, string token)
        {
            //����HttpWebRequest����https�а�ȫ֤������⣨ ������ֹ: δ�ܴ��� SSL/TLS ��ȫͨ������
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/json";
            //if (authorization != null)
            {
                request.Headers.Add("authorization", token);//����ͷ�ļ�
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
        /// http�첽����
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="reqMethod">���󷽷� GET��POST</param>
        /// <param name="callback">�ص�����</param>
        /// <param name="ob">�ش�����</param>
        /// <param name="postData">post����</param>
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
                //��ʼ�����첽���� 
                //AsyResultTag ���Զ����� ���ڴ��ݵ���ʱ��Ϣ ����HttpWebRequest �Ǳ��봫�ݶ���
                //��Ϊ�ص���Ҫ��HttpWebRequest����ȡHttpWebResponse 
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
        /// �첽POST���󷽷�
        /// </summary>
        public static async Task HttpPostAsync<T>(string url, object data, Action<T> responseCallback = null, Action<Exception> errorCallback = null)
        {
            try
            {
                string jsonData = JsonMapper.ToJson(data);
                Debug.Log($"Request JSON: {jsonData}");

                // ��������URL
                url = $"http://{AppConst.UrlConst.severIP}:{AppConst.UrlConst.severPort}{url}";
                Debug.Log($"Request URL: {url}");

                // HTTPS֤����֤����
                ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

                // �����������
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";

                // �첽д��������
                byte[] payload = Encoding.UTF8.GetBytes(jsonData);
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(payload, 0, payload.Length);
                }

                // �첽��ȡ��Ӧ
                using (WebResponse response = await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string responseText = await reader.ReadToEndAsync();
                    Debug.Log($"Response: {responseText}");

                    // ������Ӧ����
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
        /// http����ص� ��.net�ڲ����� ��������ΪIAsyncResult
        /// </summary>
        /// <param name="asynchronousResult">http�ص�ʱ�ش�����</param>
        private static void HttpCallback(IAsyncResult asynchronousResult)
        {
            int statusCode = 0;
            string retString = "";
            AsyResultTag tag = new AsyResultTag();
            WebException webEx = null;
            try
            {
                //��ȡ����ʱ���ݵĶ���
                tag = asynchronousResult.AsyncState as AsyResultTag;
                HttpWebRequest req = tag.req;
                //��ȡ�첽���ص�http���
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
            //�����ⲿ�ص� �������Ļص�
            tag.callback(tag.obj, retString, statusCode, webEx);

        }

        /// <summary>
        /// �첽����ص�ί��
        /// </summary>
        /// <param name="asyObj">�ش�����</param>
        /// <param name="resStr">http��Ӧ���</param>
        /// <param name="statusCode">http״̬��</param>
        /// <param name="webEx">�쳣</param>
        public delegate void AsyRequetCallback(object asyObj, string respStr, int statusCode, WebException webEx);

        /// <summary>
        /// �첽���ض���
        /// </summary>
        class AsyResultTag
        {
            /// <summary>
            /// �ش�����
            /// </summary>
            public object obj { get; set; }
            /// <summary>
            /// ��ǰhttpRequest����ʵ��
            /// </summary>
            public HttpWebRequest req { get; set; }
            /// <summary>
            /// �ص�����ί��
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