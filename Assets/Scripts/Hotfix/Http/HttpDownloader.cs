using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Hotfix.Event;

using Wx.Runtime.Http;

namespace Hotfix
{
    public static class HttpDownloader
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TM"></typeparam>
        /// <returns></returns>
        private static async UniTask<(bool, TM)> PostJsonAsync<T, TM>(string url, T data,
            float timeout = 5f) where T : RequestDataBase where TM : ResponseDataBase, new()
        {
            var responseCode = -1;
            TM response = null;
            while (responseCode == -1)
            {
                response = await NetworkHelper.PostJsonAsync<T, TM>(url, data, timeout);
                responseCode = response.code;
                if (responseCode != -1) continue;
                if (await ShowRetryDialog()) continue;
                return (false, new TM());
            }
            return (true, response);
        }

        private static async UniTask<bool> ShowRetryDialog()
        {
            var tcs = new UniTaskCompletionSource<bool>();
            UIEventDefine.UIPopTipCall.SendMessage(
                () => tcs.TrySetResult(true),
                "HTTP ERROR",
                "Get data from http server failed,Do you want to try again?",
                () => tcs.TrySetResult(false));
            return await tcs.Task;
        }

        /// <summary>
        /// 获取监控点数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async UniTask<(bool, List<MonitorData>)> GetMonitorPointData()
        {
            var request = new MonitorDataListRequest();
            var (success, response) =
                await PostJsonAsync<MonitorDataListRequest, MonitorDataListResponse>(AppConst.UrlConst.GetMonitorURL,
                    request);
            return (success, response?.dataList);
        }

        /// <summary>
        /// 获取监测项数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async UniTask<(bool, List<CheckItemData>)> GetCheckItemData(int id)
        {
            var request = new CheckItemRequest()
            {
                monitorDataId = id,
            };
            var (success, response) =
                await PostJsonAsync<CheckItemRequest, CheckItemResponse>(AppConst.UrlConst.GetCheckItemURL, request);
            return (success, response?.dataList);
        }

        /// <summary>
        /// 获取学科列表
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public static async UniTask<(bool, List<SubjectData>)> GetSubjectList(int modelId)
        {
            var request = new SubjectRequest()
            {
                modelId = modelId,
            };
            var (success, response) =
                await PostJsonAsync<SubjectRequest, SubjectListResponse>(AppConst.UrlConst.GetSubjectsURL, request);
            return (success, response?.dataList);
        }
    }
}