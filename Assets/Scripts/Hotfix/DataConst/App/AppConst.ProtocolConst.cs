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
            //����Ҫ����,����"ID"���ڽ�������һ������
            public static string GetRepairmalFunction = $"/GetRepairmalfunction";
            public static string GetRepairTopic = $"/GetRepairTopic";
        }
    }
}