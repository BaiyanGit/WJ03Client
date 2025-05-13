using System.Threading;
using Cysharp.Threading.Tasks;
using Hotfix.Event;
using Hotfix.UI;
using Wx.Runtime;
using Wx.Runtime.Event;
using Wx.Runtime.Machine;
using Wx.Runtime.Singleton;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Hotfix.ExcelData;
using System.Linq;
using UnityEngine.Playables;

namespace Hotfix
{
    public class GameManager : SingletonInstance<GameManager>, ISingleton
    {
        #region Component

        private WMachine _machine;
        private readonly EventGroup _eventGroup = new();

        #endregion

        #region Private

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region Public

        /// <summary>
        /// 游戏模式
        /// </summary>
        public EnumGameMode GameMode;

        /// <summary>
        /// 故障课题类型
        /// </summary>
        public EnumMonitorMode MonitorMode;

        /// <summary>
        /// 故障考核类型
        /// </summary>
        public EnumAssessmentMode AssessmentMode;

        /// <summary>
        /// 训练类型
        /// </summary>
        public TrainType TrainType;

        public List<Renderer> LastCachedRenders = new();

        private ModelController modelController;

        public ModelController ModelController
        {
            get
            {
                if (modelController == null)
                {
                    modelController = ReferenceCollector.Instance.GetComponent<ModelController>("ModelController");
                }

                return modelController;
            }
        }

        private CinemachineCamera viewMainCamera;

        public CinemachineCamera ViewMainCamera
        {
            get
            {
                if (viewMainCamera == null)
                {
                    viewMainCamera = Object.FindFirstObjectByType<CinemachineCamera>();
                }

                return viewMainCamera;
            }
        }

        private CameraTarget viewMainTarget;

        public CameraTarget ViewMainTarget
        {
            get
            {
                if (viewMainTarget.Equals(default(CameraTarget)))
                {
                    var model = ReferenceCollector.Instance.GetComponent<Transform>("ModelController");
                    viewMainTarget.TrackingTarget = model;
                    viewMainTarget.LookAtTarget = model;
                }

                return viewMainTarget;
            }
        }

        private Transform mainTarget;

        public Transform MainTarget
        {
            get
            {
                if (mainTarget == null)
                {
                    mainTarget = ReferenceCollector.Instance.GetComponent<Transform>("ModelController");
                }

                return mainTarget;
            }
        }

        private Dictionary<Renderer, Material[]> allTragetRenderers;

        public Dictionary<Renderer, Material[]> AllTragetRenderers
        {
            get
            {
                if (allTragetRenderers == null || allTragetRenderers.FirstOrDefault().Key == null)
                {
                    allTragetRenderers = new Dictionary<Renderer, Material[]>();
                    var temp = ViewMainTarget.TrackingTarget.GetComponentsInChildren<Renderer>();

                    for (int i = 0; i < temp.Length; i++)
                    {
                        allTragetRenderers.Add(temp[i], temp[i].materials);
                    }
                }

                return allTragetRenderers;
            }
        }

        private Material targetMaterial;

        public Material TargetMaterial
        {
            get
            {
                if (targetMaterial == null)
                {
                    //Runtime时会丢掉，已追加入Graphics的Always Include
                    targetMaterial = new Material(Shader.Find("Shader Graphs/banToua"));
                }

                return targetMaterial;
            }
        }


        /// <summary>
        /// 动画模型
        /// </summary>
        private Transform animModel;

        public Transform AnimModel
        {
            get
            {
                if (animModel == null)
                {
                    animModel = ReferenceCollector.Instance.GetComponent<Transform>("AnimModel");
                }

                return animModel;
            }
        }

        private CinemachineCameraController3X cinemachineCameraController;
        
        /// <summary>
        /// 相机控制脚本
        /// </summary>
        public CinemachineCameraController3X CinemachineCameraController
        {
            get
            {
                if (cinemachineCameraController == null)
                {
                    cinemachineCameraController = ViewMainCamera.GetComponent<CinemachineCameraController3X>();
                }

                return cinemachineCameraController;
            }
        }

        #endregion

        #region 涂装修改使用

        private Material camouflageMaterial;
        bool _isComplete = true;
        private Material[] _machineCoating;

        public Material[] MachineCoating
        {
            get
            {
                if (_machineCoating != null && _machineCoating.Length > 0)
                {
                    return _machineCoating;
                }
                else _machineCoating = Renderer.materials;

                return _machineCoating;
            }

            set { _machineCoating = value; }
        }

        private Renderer _renderer;

        public Renderer Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = ReferenceCollector.Instance.GetComponent<Renderer>("MeratialModel");
                }

                return _renderer;
            }
        }

        /// <summary>
        /// 默认渲染层级
        /// </summary>
        private int _rendererQueueDefult = 2000;


        private Material carMaterial;

        public Material CarMaterial
        {
            get
            {
                if (carMaterial == null)
                {
                    carMaterial = Resources.Load<Material>("Materials/zc_dm_01");
                }

                return carMaterial;
            }
        }

        /// <summary>
        /// 0是白绿，1是迷彩
        /// </summary>
        private Texture[] textures;

        public Texture[] Textures
        {
            get
            {
                if (textures == null || textures.Length < 1)
                {
                    textures = Resources.LoadAll<Texture>("Materials/Texture");
                }

                return textures;
            }
        }

        private int _showIndex;

        #endregion

        public bool IsLoginNormal;



        #region Owner: Wx Time: 2025-05-09 Desc: 记录当前选中的标签和类型，用于记录模型相机的属性

        private int m_CurrentLabelEntry;
        private string m_CurrentType;

        public int GetCurrentLabelEntry()
        {
            return m_CurrentLabelEntry;
        }

        public string GetCurrentType()
        {
            return m_CurrentType;
        }
        
        #endregion
        

        public void OnCreate(object createParam)
        {
            _machine = new WMachine(this);
            AddEvent();

            _machine.AddNode<ConfigMachineState>();
            _machine.AddNode<LoginMachineState>();
            _machine.AddNode<GameMachineState>();


            //GameEntry.Sever.StartSever("127.0.0.1", 8888);    //注释不需要的服务器
            _machine.Run<ConfigMachineState>();
        }

        public void OnUpdate()
        {
            _machine.Update();
        }

        public void OnFixedUpdate()
        {
            _machine.FixedUpdate();
        }

        public void OnLateUpdate()
        {
            _machine.LateUpdate();
        }

        public void OnDestroy()
        {
            _machine.Destroy();
            _eventGroup.RemoveAllListener();
            _cancellationTokenSource.Cancel();
        }

        private void AddEvent()
        {
            _eventGroup.AddListener<ProcessEventDefine.ChangeLoginMachineCall>(OnChangeLoginMachineHandle);
            _eventGroup.AddListener<ProcessEventDefine.ChangeGameMachineCall>(OnChangeGameMachineHandle);
            _eventGroup.AddListener<UIEventDefine.UIPopTipCall>(OnPopBackHandle);
        }

        private void OnChangeGameMachineHandle(IEventMessage msg)
        {
            GameEntry.UI.CloseAllLoadUIForms();
            _machine.ChangeState<GameMachineState>();
        }

        private void OnChangeLoginMachineHandle(IEventMessage msg)
        {
            GameEntry.UI.CloseAllLoadUIForms();
            _machine.ChangeState<LoginMachineState>();
        }

        private void OnPopBackHandle(IEventMessage msg)
        {
            var message = (UIEventDefine.UIPopTipCall)msg;
            GameEntry.UI.OpenUIFormAsync<UIPopTip>(userData: message).Forget();
        }

        public void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        public void HideAllWithoutTarget(List<Transform> targets, bool isCache = true)
        {
            var list = new List<Renderer>();

            if (targets != null)
            {
                foreach (Transform t in targets)
                {
                    list.AddRange(t.GetComponentsInChildren<Renderer>(true));
                }
            }

            if (isCache)
            {
                LastCachedRenders = list;
            }

            HideAllWithoutTarget(list);
        }

        public void HideAllWithoutTarget(List<Renderer> targets)
        {
            foreach (var item in AllTragetRenderers)
            {
                if (!targets.Contains(item.Key))
                {
                    item.Key.gameObject.SetActive(false);
                }
                else
                {
                    if (!item.Key.isVisible)
                    {
                        //这里得重置材质到默认状态
                        item.Key.materials = item.Value;
                        item.Key.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void ChangeAllMaterialWithTarget(List<Transform> targets, bool isCache = true)
        {
            var list = new List<Renderer>();

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                {
                    continue;
                }

                list.AddRange(targets[i].GetComponentsInChildren<Renderer>());
            }

            if (isCache)
            {
                LastCachedRenders = list;
            }

            ChangeAllMaterialWithTarget(list);
        }

        public void ChangeAllMaterialWithTarget(List<Renderer> targets)
        {
            foreach (var item in AllTragetRenderers)
            {
                if (!targets.Contains(item.Key))
                {
                    item.Key.enabled = true;
                    var materials = new Material[item.Value.Length];
                    for (int i = 0; i < item.Value.Length; i++)
                    {
                        materials[i] = TargetMaterial;
                    }

                    item.Key.materials = materials;
                }
                else
                {
                    item.Key.materials = item.Value;
                }
            }
        }

        /// <summary>
        /// 显示所有模型
        /// </summary>
        public void ShowAllParts()
        {
            foreach (var (key, _) in AllTragetRenderers)
            {
                key.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 恢复色彩渲染所有模型
        /// </summary>
        public void ShowAllRenderer()
        {
            foreach (var item in AllTragetRenderers)
            {
                item.Key.materials = item.Value;
            }
        }

        /// <summary>
        /// 重置虚拟相机
        /// </summary>
        public void RestCinemachineCamera()
        {
            Debug.Log("重置虚拟相");
            ViewMainCamera.ForceCameraPosition(AppConst.Constant.CameraDefaultPos, AppConst.Constant.CameraDefaultRot);
        }

        #region 查看原理&结构模型时的相机位置控制 Owner:王柏雁 2025-4-10

        /// <summary>
        /// 原理模型相机位置控制
        /// </summary>
        /// <param name="mType"></param>
        public void RestCinemachineCameraPrincip(string mType)
        {
            RestCinemachineCamera(1, mType);
        }

        /// <summary>
        /// 结构模型相机位置控制
        /// </summary>
        /// <param name="mType"></param>
        public void RestCinemachineCameraStructral(string mType)
        {
            RestCinemachineCamera(0, mType);
        }

        /// <summary>
        /// 根据模型不同，改变相机位置
        /// </summary>
        /// <param name="mLabelEntry">0:结构，1:原理</param>
        /// <param name="mType">文本内容</param>
        private void RestCinemachineCamera(int mLabelEntry, string mType)
        {
            if (Instance.CinemachineCameraController == null)
            {
                Debug.LogError("没有找到相机控制器");
                return;
            }

            #region Owner: Wx Time: 2025-05-09 Desc: 记录当前选中的标签和类型，用于记录模型相机的属性

            m_CurrentLabelEntry = mLabelEntry;
            m_CurrentType = mType;

            #endregion

            var cameraPosConfig = ModelCameraPositionTable.Instance.GetCameraPositionByLabelAndType(mLabelEntry, mType);
            if (cameraPosConfig == null)
            {
                Debug.Log("未找到相机位置配置");
                return;
            }
            var posX = cameraPosConfig.CPosition[0];
            var posY = cameraPosConfig.CPosition[1];
            var posZ = cameraPosConfig.CPosition[2];
            var cameraPos = new Vector3(posX, posY, posZ);

            var rotX = cameraPosConfig.CRotation[0];
            var rotY = cameraPosConfig.CRotation[1];
            var rotZ = cameraPosConfig.CRotation[2];
            // var cameraRotEuler = Quaternion.Euler(rotX, rotY, rotZ);
            // var cameraRotAngles = Quaternion.Euler(rotX, rotY, rotZ).eulerAngles;
            var cameraRot = new Vector3(rotX, rotY, rotZ);

            var minDis = cameraPosConfig.SWDistance[0];
            var maxDis = cameraPosConfig.SWDistance[1];
            var limitRollDis = new Vector2(minDis, maxDis);

            var defDistance = cameraPosConfig.DefDistance;

            Vector3 panOffset = new Vector3(cameraPosConfig.PanOffset[0], cameraPosConfig.PanOffset[1],
                cameraPosConfig.PanOffset[2]);
#if UNITY_EDITOR

            Debug.Log(
                "↓↓↓ 相机重置位置 ↓↓↓\n" +
                $"选择显示部件：{cameraPosConfig.MType}\n" +
                $"相机重置位置：{cameraPos}\n" +
                $"相机重置旋转：{cameraRot}\n" +
                $"相机重置距离：{defDistance}\n" +
                $"相机距离限制：{limitRollDis.x}~{limitRollDis.y}");

#endif
            // Tips: 这里其实可以用CinemachineCamera中的ForceCameraPosition方法
            Instance.CinemachineCameraController.ResetCameraPosition(cameraPos, cameraRot, defDistance, limitRollDis,
                panOffset);
        }

        #endregion

        public void InitCarMaterial()
        {
            _showIndex = 0;
            int index = _showIndex == 0 ? 1 : 0;
            camouflageMaterial = MachineCoating[_showIndex];

            ChangeMaterial("cheTi_02");
        }

        /// <summary>
        /// 涂装切换
        /// </summary>
        public void ChangeMaterial(string index)
        {
            if (!_isComplete) return;

            _showIndex = index.Contains("01") ? 0 : 1;

            Material element = null;
            for (int i = 0; i < MachineCoating.Length; i++)
            {
                if (MachineCoating[i].name.Contains(index))
                {
                    element = MachineCoating[i];
                    element.renderQueue = _rendererQueueDefult;
                    continue;
                }
            }

            List<Material> materials = new List<Material>();
            materials.Add(element);
            for (int i = 0; i < MachineCoating.Length; i++)
            {
                if (MachineCoating[i] != element)
                {
                    MachineCoating[i].renderQueue = 1999;
                    materials.Add(MachineCoating[i]);
                }
            }

            MachineCoating = materials.ToArray();
            Renderer.materials = MachineCoating;

            _isComplete = false;
            MachineCoating[0].DOFloat(25f, "_NoiseInfluence", 2f).OnComplete(DoOnComplete);
        }

        private void DoOnComplete()
        {
            camouflageMaterial?.DOFloat(-24f, "_NoiseInfluence", 0f).OnComplete(() =>
            {
                _isComplete = true;
                camouflageMaterial = MachineCoating[0];
            });

            if (camouflageMaterial == null)
            {
                _isComplete = true;
                camouflageMaterial = MachineCoating[0];
            }

            CarMaterial.mainTexture = Textures[_showIndex];
            ClearAllRenderer();
        }

        /// <summary>
        /// 替换贴图的时候清空
        /// </summary>
        void ClearAllRenderer()
        {
            allTragetRenderers.Clear();
            ShowAllParts();
            ShowAllRenderer();
        }

        /// <summary>
        /// 音量控制
        /// </summary>
        /// <param name="value"></param>
        public void ChangeAudioValue(float value)
        {
        }

        /// <summary>
        /// 画质选择
        /// </summary>
        /// <param name="index"></param>
        public void ChangeQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }

        /// <summary>
        /// 初始化模型参数
        /// </summary>
        public void InitModelParam(FaultCheckConfig4th targetModels)
        {
            if (Instance.MonitorMode == EnumMonitorMode.Train) return;
            Transform tmpTran = MainTarget.FindTheChildNode(targetModels.TargetModel[0]);
            ModelParamControl modelParam = tmpTran?.gameObject.AddComponent<ModelParamControl>();
            modelParam.InitModel(targetModels);
        }

        public GameObject currentAnimModel;
        public Animator currentAnim;
        public PlayableDirector currentPlayable;

        /// <summary>
        /// 用于控制原理学习3D动画控制
        /// </summary>
        /// <param name="modelPath"></param>
        public void ModelAnimController(string modelPath)
        {
            //显示动画主模型
            AnimModel.gameObject.SetActive(true);
            MainTarget.gameObject.SetActive(false);

            currentAnimModel?.SetActive(false);
            currentAnimModel = null;

            Transform animModel = AnimModel.Find(modelPath);
            animModel.gameObject.SetActive(true);
            Animator anim = animModel.GetComponentInChildren<Animator>();
            PlayableDirector playable = animModel.GetComponentInChildren<PlayableDirector>();

            anim?.Play(animModel.name);
            playable?.Play();

            currentAnim = anim;
            currentPlayable = playable;
            currentAnimModel = animModel.gameObject;
            Instance.ViewMainCamera.Target.TrackingTarget = animModel.GetChild(0);
            // Tips:2025-4-11注释掉，因为多了新功能相机平移
            // Instance.ViewMainCamera.ForceCameraPosition(new Vector3(5f, 2.6f, -4f),
            //     Quaternion.Euler(new Vector3(16f, -43f, 0)));
        }

        /// <summary>
        /// 初始化模型动画
        /// </summary>
        public void InitModelAnim()
        {
            currentPlayable?.Stop();
            currentAnimModel?.SetActive(false);
            MainTarget.gameObject.SetActive(true);
            Instance.ViewMainCamera.Target.TrackingTarget = null;
            currentAnimModel = null;
            // RestCinemachineCamera();
        }

        /// <summary>
        /// 补充添加
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="materials"></param>
        public void AppendTargetRenderers(Renderer renderer, Material[] materials)
        {
            if (allTragetRenderers != null)
            {
                if (allTragetRenderers.ContainsKey(renderer))
                {
                    allTragetRenderers[renderer] = materials;
                }
                else
                {
                    allTragetRenderers.Add(renderer, materials);
                }
            }
        }
    }
}