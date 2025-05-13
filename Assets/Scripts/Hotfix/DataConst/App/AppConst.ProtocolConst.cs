namespace Hotfix
{
    public partial class AppConst
    {
        public class Protocol
        {
            public static string GetUserInfoByAccount = $"/GetUserInfoByAccount"; 
            public static string GetTopicInfoByAccount = $"/GetTeacherExamTask";
            public static string AddExamRecord = $"/AddExamRecord";
            public static string AddEvaluation = $"/AddEvaluation";
            public static string AddSensor = $"/AddSensor";
            //不需要参数,返回"ID"用于接下来进一步请求
            public static string GetRepairmalFunction = $"/GetRepairmalfunction";
            public static string GetRepairTopic = $"/GetRepairTopic";
        }
    }
}