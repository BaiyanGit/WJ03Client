using UnityEngine;

namespace Hotfix
{
    public struct SensorPost
    {
        /// <summary>
        /// ����
        /// </summary>
        public float TemperatureValue;
        /// <summary>
        /// ��ѹI
        /// </summary>
        public float KPAValueI;
        /// <summary>
        /// ��ѹII
        /// </summary>
        public float KPAValueII;
        /// <summary>
        /// ת��ѹ��
        /// </summary>
        public float ApsValue;
        /// <summary>
        /// ��ѹ
        /// </summary>
        public float OilKPAValue;
        /// <summary>
        /// ����
        /// </summary>
        public float OilTemperatureValue;
        /// <summary>
        /// ��ѹ
        /// </summary>
        public float ElectricityValue;
        /// <summary>
        /// ����
        /// </summary>
        public float OilValue;
        /// <summary>
        /// ���
        /// </summary>
        public Vector3 VibrationFrequency;
        /// <summary>
        /// ת��
        /// </summary>
        public float RPMValue;
        /// <summary>
        /// ����
        /// </summary>
        public float NoiseValue;
        /// <summary>
        /// ��¼ʱ��
        /// </summary>
        public string CurrentTime;
    }

    /// <summary>
    /// ���ؽṹ
    /// </summary>
    public struct ResponseSensor
    {
        /// <summary>
        /// �����룬1000Ϊ����������ȫ����Ϊ�д�
        /// </summary>
        public int code;
        /// <summary>
        /// ToDo:JsonString
        /// </summary>
        public int infos;
        /// <summary>
        /// ��code�����ֽ���
        /// </summary>
        public string msg;
    }
}