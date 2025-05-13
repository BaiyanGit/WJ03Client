using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.EventSystems;

/// <summary>
/// 相机控制器 3X Owner: 王柏雁 2025-4-9
/// </summary>
[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineCameraController3X : MonoBehaviour
{
    [Header("目标对象")] public Transform target;
    [Header("相机与目标实时距离")] public float distance = 10f; // 相机距离目标距离
    [Header("滚轮最小距离")] public float minDistance = 3f;
    [Header("滚轮最大距离")] public float maxDistance = 10f;
    [Header("滚轮缩放速度")] public float zoomSpeed = 5f;
    [Header("左键旋转速度")] public float rotateSpeed = 3f;
    [Header("右键平移速度")] public float panSpeed = 0.1f;
    [Header("平滑度")] public float smoothTime = 0.5f;
    [Header("边缘区域限制"), Range(0f, 0.5f)] public float margin = 0.1f;
    [Header("Y轴旋转角度"), SerializeField] private float _yRotation = 0f; // Y轴旋转角度
    [Header("X轴旋转角度"), SerializeField] private float _xRotation = 20f; // X轴旋转角度
    [Header("相机平移的偏移"), SerializeField] private Vector3 _panOffset; // 相机平移的偏移
    [Header("相机相对目标位置偏移"),] public Vector3 cameraPosOffset; // 相机相对于目标的位置偏移

    private CinemachineCamera _cinemachineCamera; // 相机组件
    private CinemachineCameraOffset _cinemachineOffset; // 相机偏移组件
    private Transform _lastTarget; // 上一次目标
    private Vector3 _initRotation; // 初始旋转角度
    private bool _mouseHasFocus; // 鼠标焦点状态


    public Vector3 tPosition;
    public Vector3 tRotation;

    public Vector3 GetPanOffset()
    {
        return _panOffset;
    }

    private void Start()
    {
        _cinemachineCamera = GetComponent<CinemachineCamera>();
        _cinemachineOffset = GetComponent<CinemachineCameraOffset>();
        Init();
    }

    private void Update()
    {
        if (!IsFindTarget()) return;
        MouseLeftPress();
        MouseRightPress();
        MouseMiddleScrollWheel();
        RealUpdateCameraPosition();

        // Tips: 调试视角专用
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            tPosition = transform.position;
            tRotation = transform.eulerAngles;
            Debug.Log($"[Pos] {tPosition} [Rot] {tRotation}");
        }
    }

    /// <summary>
    /// 鼠标焦点获得/丢失
    /// </summary>
    /// <param name="hasFocus"></param>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) _mouseHasFocus = true;
        // Debug.Log(hasFocus ? "鼠标焦点获得" : "鼠标焦点丢失");
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        _initRotation = transform.eulerAngles;
        _xRotation = _initRotation.x;
        _yRotation = _initRotation.y;

        _panOffset = Vector3.zero; // 相机平移的偏移
    }

    /// <summary>
    /// 重置相机位置(每当UI切换目标时调用)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="defaultDistance"></param>
    /// <param name="limitDistance"></param>
    /// <param name="panOffset"></param>
    public void ResetCameraPosition(Vector3 position, Vector3 rotation, float defaultDistance, Vector3 limitDistance, Vector3 panOffset)
    {
        transform.rotation = Quaternion.Euler(rotation);
        _initRotation = transform.eulerAngles;
        _xRotation = _initRotation.x;
        _yRotation = _initRotation.y;

        minDistance = limitDistance.x; // 推进最小距离
        maxDistance = limitDistance.y; // 拉远最大距离
        transform.localPosition = position; // 重置位置

        _panOffset = panOffset;
        _cinemachineOffset.Offset = _panOffset;
        distance = defaultDistance; // 重置距离
    }

    /// <summary>
    /// 是否查找到目标
    /// </summary>
    /// <returns></returns>
    private bool IsFindTarget()
    {
        target = _cinemachineCamera.Target.TrackingTarget;
        if (target == null) return false;

        if (target != null)
        {
            if (_lastTarget == null || _lastTarget != target)
            {
                _lastTarget = target; // 更新目标
            }
        }

        return true;
    }

    /// <summary>
    /// 鼠标左键按下
    /// </summary>
    private void MouseLeftPress()
    {
        // 鼠标左键旋转（绕目标旋转）
        if (Input.GetMouseButton(0))
        {
            // 鼠标失去焦点时，在获取焦点时，防止位置偏移
            if (_mouseHasFocus)
            {
                _mouseHasFocus = false;
                return;
            }

            // 检测指针是否悬停在UI元素上
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SetMouseCursor(false);
                var xDelta = Input.GetAxis("Mouse X") * rotateSpeed;
                var yDelta = Input.GetAxis("Mouse Y") * rotateSpeed;
                HandleRotation(xDelta, yDelta);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            SetMouseCursor(true);
        }
    }

    /// <summary>
    /// 鼠标右键按下
    /// </summary>
    private void MouseRightPress()
    {
        if (Input.GetMouseButton(1))
        {
            SetMouseCursor(false);

            // 鼠标失去焦点时，在获取焦点时，防止位置偏移
            if (_mouseHasFocus)
            {
                _mouseHasFocus = false;
                return;
            }

            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            HandlePan(mouseX, mouseY);
        }

        if (Input.GetMouseButtonUp(1))
        {
            SetMouseCursor(true);
        }
    }

    /// <summary>
    /// 鼠标滚轮滚动
    /// </summary>
    private void MouseMiddleScrollWheel()
    {
        // 鼠标失去焦点时，在获取焦点时，防止位置偏移
        if (_mouseHasFocus)
        {
            _mouseHasFocus = false;
            return;
        }

        var scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        HandleZoom(scrollDelta);
    }

    /// <summary>
    /// 设置鼠标光标可见性
    /// </summary>
    /// <param name="visible">是否可见</param>
    private static void SetMouseCursor(bool visible)
    {
        Cursor.visible = visible;
    }

    /// <summary>
    /// 鼠标左键旋转
    /// </summary>
    /// <param name="xDelta">鼠标左键左右偏移量</param>
    /// <param name="yxDelta">鼠标左键上下偏移量</param>
    public void HandleRotation(float xDelta, float yDelta)
    {
        _xRotation -= yDelta; // 绕X轴旋转
        _yRotation += xDelta; // 绕Y轴旋转
        _xRotation = Mathf.Clamp(_xRotation, 0f, 87f);
    }

    /// <summary>
    /// 鼠标滚轮视觉缩放
    /// </summary>
    /// <param name="scrollDelta">滚轮滚动距离偏移量</param>
    public void HandleZoom(float scrollDelta)
    {
        distance -= scrollDelta * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // 更新偏移
        var rotation = Quaternion.Euler(_xRotation, _yRotation, 0);

        cameraPosOffset = rotation * new Vector3(0, 0, -distance);
    }

    /// <summary>
    /// 鼠标右键平移
    /// </summary>
    /// <param name="mouseX">x轴平移距离偏移量 </param>
    /// <param name="mouseY">y轴平移距离偏移量 </param>
    public void HandlePan(float mouseX, float mouseY)
    {
        // 获取目标在视口中的位置状态
        var status = GetViewportPositionStatus(target.position);

        // 根据视口位置状态限制移动方向
        if (status.leftEdge && mouseX < 0) mouseX = 0; // 左边缘时禁止左移
        if (status.rightEdge && mouseX > 0) mouseX = 0; // 右边缘时禁止右移
        if (status.bottomEdge && mouseY < 0) mouseY = 0; // 下边缘时禁止下移
        if (status.topEdge && mouseY > 0) mouseY = 0; // 上边缘时禁止上移

        // 应用带阻尼系数的移动
        _panOffset.x += mouseX * -panSpeed * GetDampingFactor(status);
        _panOffset.y += mouseY * -panSpeed * GetDampingFactor(status);
    }

    /// <summary>
    /// 实时更新相机位置
    /// </summary>
    private void RealUpdateCameraPosition()
    {
        // 更新相机偏移
        // _cinemachineOffset.Offset = _offset;// 无阻尼效果
        _cinemachineOffset.Offset = Vector3.Lerp(_cinemachineOffset.Offset, _panOffset, smoothTime); // 带阻尼效果

        // 设置相机位置
        // transform.position = target.position + _cameraOffset;// 无阻尼效果
        transform.position = Vector3.Lerp(transform.position, target.position + cameraPosOffset, smoothTime);

        // 更新相机朝向
        transform.LookAt(target.position);
    }

    /// <summary>
    /// 视口位置状态结构体
    /// </summary>
    private struct ViewportPositionStatus
    {
        public bool leftEdge;
        public bool rightEdge;
        public bool topEdge;
        public bool bottomEdge;
        public bool isVisible;
    }

    /// <summary>
    /// 获取视口位置状态
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    private ViewportPositionStatus GetViewportPositionStatus(Vector3 targetPosition)
    {
        var status = new ViewportPositionStatus();
        var mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found.");
            return status;
        }

        var viewportPoint = mainCamera.WorldToViewportPoint(targetPosition);

        // 基础可见性检查
        status.isVisible = viewportPoint.z > mainCamera.nearClipPlane &&
                           viewportPoint.z < mainCamera.farClipPlane &&
                           viewportPoint.x > 0 && viewportPoint.x < 1 &&
                           viewportPoint.y > 0 && viewportPoint.y < 1;

        // 边缘检测（使用独立阈值）
        var horizontalMargin = margin * mainCamera.aspect; // 根据屏幕比例调整横向阈值
        status.leftEdge = viewportPoint.x <= horizontalMargin;
        status.rightEdge = viewportPoint.x >= 1 - horizontalMargin;
        status.bottomEdge = viewportPoint.y <= margin;
        status.topEdge = viewportPoint.y >= 1 - margin;

        return status;
    }

    /// <summary>
    /// 动态阻尼系数（基于边缘接近程度）
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    private static float GetDampingFactor(ViewportPositionStatus status)
    {
        var damping = 1f;

        // 如果任意方向到达边缘，应用阻尼
        if (status.leftEdge || status.rightEdge ||
            status.topEdge || status.bottomEdge)
        {
            damping = 0.3f; // 边缘区域移动速度降低70%
        }

        return Mathf.Clamp(damping, 0.1f, 1f);
    }
}