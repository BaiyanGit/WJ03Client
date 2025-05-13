namespace Encryption
{
    /// <summary>
    /// 加解密数据类
    /// </summary>
    public class CryptoData
    {
        //设备名称
        public string deviceName;

        //设备唯一标识
        public string deviceUniqueID;

        //操作系统
        public string operatingSystem;

        //图形设备名称
        public string graphicsDeviceName;

        //图形设备类型
        public string graphicsDeviceType;

        //图形设备版本
        public string graphicsDeviceVersion;

        //处理器类型
        public string processorType;

        //有效期
        public string expirationDate;

        protected bool Equals(CryptoData other)
        {
            return deviceName.Equals(other.deviceName)
                   && deviceUniqueID.Equals(other.deviceUniqueID)
                   && operatingSystem.Equals(other.operatingSystem)
                   && graphicsDeviceName.Equals(other.graphicsDeviceName)
                   && graphicsDeviceType.Equals(other.graphicsDeviceType)
                   && graphicsDeviceVersion.Equals(other.graphicsDeviceVersion)
                   && processorType.Equals(other.processorType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((CryptoData)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (!string.IsNullOrEmpty(deviceName) ? deviceName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(deviceUniqueID) ? deviceUniqueID.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(operatingSystem) ? operatingSystem.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(graphicsDeviceName) ? graphicsDeviceName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(graphicsDeviceType) ? graphicsDeviceType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(graphicsDeviceVersion) ? graphicsDeviceVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(processorType) ? processorType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"deviceName: {deviceName}, deviceUniqueID: {deviceUniqueID}, operatingSystem: {operatingSystem}, graphicsDeviceName: {graphicsDeviceName}, graphicsDeviceType: {graphicsDeviceType}, graphicsDeviceVersion: {graphicsDeviceVersion}, processorType: {processorType}, expirationDate: {expirationDate}";
        }
    }
}