using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime.Event;
using YooAsset;

namespace GameMain.Runtime
{
    public partial class AppEntry : MonoBehaviour
    {
        [SerializeField] 
        private EnumPlayMode playMode = EnumPlayMode.BuildIn;
        
        [SerializeField]
        private EPlayMode ePlayMode = EPlayMode.EditorSimulateMode;
        [SerializeField]
        private string packageName = "DefaultPackage";
        [SerializeField]
        private EDefaultBuildPipeline buildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline;
        [SerializeField] private string assetsSever = "https://127.0.0.1";
        
        
        public static readonly CancellationTokenSource CancellationTokenSource = new();
        private readonly EventGroup _event = new();
        
        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            InitBuildInModules();
            InitCustomModules();
            

            Singleton.CreateSingleton<ConfigManager>();
            Singleton.CreateSingleton<GameManager>();

            InitEventListener();
            InitConfig();
            await InitData();
        }

        private void Update()
        {
            Wx.Runtime.AppEntry.Update();
        }

        private void FixedUpdate()
        {
            Wx.Runtime.AppEntry.FixedUpdate();
        }

        private void LateUpdate()
        {
            Wx.Runtime.AppEntry.LateUpdate();
        }

        private void OnDestroy()
        {
            CancellationTokenSource.Cancel();
            Wx.Runtime.AppEntry.Destroy();
        }

        private async UniTask InitData()
        {
            switch (playMode)
            {
                case EnumPlayMode.BuildIn:
                    //开启项目流程
                    GameManager.Instance.BuildInStartGame();
                    Singleton.DestroySingleton<ConfigManager>();
                    Singleton.DestroySingleton<GameManager>();
                    break;
                case EnumPlayMode.YooAsset:
                    InitAssetSever();
                    //开启补丁流程
                    // 加载更新页面
                    var canDo = await InitYooAssets();
                    if (!canDo) return;
                    Singleton.CreateSingleton<PatchMachine>();
                    break;
                default:
                    break;
            }
        }
        
        private void InitConfig()
        {
            AppConst.AssetConst.playMode = playMode;
            AppConst.AssetConst.ePlayMode = ePlayMode;
            AppConst.AssetConst.packageName = packageName;
            AppConst.AssetConst.buildPipeline = buildPipeline;
            ConfigManager.Instance.InitClientConfig();
        }

        private void InitEventListener()
        {
            _event.AddListener<UserEventDefine.UserTryGetVersion>(OnUserTryGetVersionHandle);
        }

        private async void OnUserTryGetVersionHandle(IEventMessage msg)
        {
            await InitData();
        }

        private void InitAssetSever()
        {
            AppConst.AssetConst.assetsSever = assetsSever;
        }

        private async UniTask<bool> InitYooAssets()
        {
            YooAssets.Initialize();
            var go = Resources.Load<GameObject>(nameof(PatchWindow));
            GameObject.Instantiate(go);
            if (ePlayMode != EPlayMode.HostPlayMode && ePlayMode != EPlayMode.WebPlayMode) return true;
            var handle = await ConfigManager.Instance.InitYooAssetConfig();
            switch (handle)
            {
                case -1:
                    PatchEventDefine.ReRequestVersion.SendEventMessage();
                    return false;
                case 1:
                    PatchEventDefine.ReDownLoadApplication.SendEventMessage();
                    return false;
                default: return true;
            }

            return true;
        }
        
    }

}