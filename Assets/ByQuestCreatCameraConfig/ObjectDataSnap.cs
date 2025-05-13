namespace ByQuestCreatCameraConfig
{
    using Unity.Cinemachine;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class ObjectDataSnap : MonoBehaviour
    {
        [Header("属性类型")] public ObjectAttributeType attributeType;
        [Header("对象数据")] public BaseData baseData = null;
        private CinemachineCamera cinemachineCamera => GetComponent<CinemachineCamera>();

        private CinemachineCameraController3X cinemachineCameraController =>
            GetComponent<CinemachineCameraController3X>();

        private ObjectSnapManager _objectSnapManager;

        private Transform target => cinemachineCamera.Target.TrackingTarget;
        private Transform _lastTarget;

        private void Start()
        {
            _objectSnapManager = ObjectSnapManager.Instance;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
            {
                AddObjectAttributes();
            }

            if (target == null) return;

            if (_objectSnapManager.isReadFromFile)
            {
                if (_lastTarget == null || _lastTarget != target)
                {
                    _lastTarget = target;
                    GetLocalAttributes();
                }
            }
            else
            {
                if (target == null) return;
                GetObjectAttributes();
            }
        }

        /// <summary>
        /// 获取本地数据
        /// </summary>
        private void GetLocalAttributes()
        {
            if (target == null)
            {
                Debug.LogError("相机中暂时没有对象");
                return;
            }

            _objectSnapManager.GetSnapData(target.name, out var localBaseData);

            if (localBaseData == null)
            {
                Debug.LogError($"获取本地数据失败,未找到数据 {target.name}");
                return;
            }

            baseData ??= new BaseData();
            baseData = localBaseData;
        }

        /// <summary>
        /// 获取对象上的属性
        /// </summary>
        private void GetObjectAttributes()
        {
            if (target == null)
            {
                Debug.LogError("相机中暂时没有对象");
                return;
            }

            var position = Vector3.zero;
            var eulerAngles = Vector3.zero;

            switch (attributeType)
            {
                case ObjectAttributeType.Local:
                    position = transform.localPosition;
                    eulerAngles = transform.localEulerAngles;
                    break;
                case ObjectAttributeType.World:
                    position = transform.position;
                    eulerAngles = transform.eulerAngles;
                    break;
            }

            baseData ??= new BaseData();
            baseData.objectName = target.name;
            baseData.distance = cinemachineCameraController.distance;
            baseData.minDistance = cinemachineCameraController.minDistance;
            baseData.maxDistance = cinemachineCameraController.maxDistance;
            baseData.position = position.ToVector3();
            baseData.eulerAngles = eulerAngles.ToVector3();
        }

        /// <summary>
        /// 添加对象属性
        /// </summary>
        public void AddObjectAttributes()
        {
            if (string.IsNullOrEmpty(baseData.objectName))
            {
                Debug.LogError("数据为空");
                return;
            }

            Debug.LogError($"<color=green>添加对象属性：{baseData.objectName}</color>\n" +
                           $"默认缩放：{baseData.distance}\n" +
                           $"最小缩放：{baseData.minDistance}\n" +
                           $"最大缩放：{baseData.maxDistance}\n" +
                           $"对象位置：({string.Join(",", baseData.position)})\n" +
                           $"对象旋转：({string.Join(",", baseData.eulerAngles)})");

            _objectSnapManager.AddObjectData(baseData);
        }
    }

    public enum ObjectAttributeType
    {
        Local,
        World
    }

    [System.Serializable]
    public class BaseData
    {
        [Header("对象名称")] public string objectName;
        [Header("默认缩放")] public float distance;
        [Header("最小缩放")] public float minDistance;
        [Header("最大缩放")] public float maxDistance;
        [Header("对象位置")] public float[] position;
        [Header("对象旋转")] public float[] eulerAngles;
    }
}