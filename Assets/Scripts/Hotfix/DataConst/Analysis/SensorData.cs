using UnityEngine;

namespace Hotfix
{
    public struct SensorPost
    {
        /// <summary>
        /// 缸温
        /// </summary>
        public float TemperatureValue;
        /// <summary>
        /// 气压I
        /// </summary>
        public float KPAValueI;
        /// <summary>
        /// 气压II
        /// </summary>
        public float KPAValueII;
        /// <summary>
        /// 转速压力
        /// </summary>
        public float ApsValue;
        /// <summary>
        /// 油压
        /// </summary>
        public float OilKPAValue;
        /// <summary>
        /// 油温
        /// </summary>
        public float OilTemperatureValue;
        /// <summary>
        /// 电压
        /// </summary>
        public float ElectricityValue;
        /// <summary>
        /// 油量
        /// </summary>
        public float OilValue;
        /// <summary>
        /// 振幅
        /// </summary>
        public Vector3 VibrationFrequency;
        /// <summary>
        /// 转速
        /// </summary>
        public float RPMValue;
        /// <summary>
        /// 声音
        /// </summary>
        public float NoiseValue;
        /// <summary>
        /// 记录时间
        /// </summary>
        public string CurrentTime;
    }

    /// <summary>
    /// 返回结构
    /// </summary>
    public struct ResponseSensor
    {
        /// <summary>
        /// 错误码，1000为正常，其他全部视为有错
        /// </summary>
        public int code;
        /// <summary>
        /// ToDo:JsonString
        /// </summary>
        public int infos;
        /// <summary>
        /// 对code的文字解释
        /// </summary>
        public string msg;
    }
}