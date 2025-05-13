using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using YooAsset;
using System.IO;

namespace Wx.Runtime.Excel
{
    /// <summary>
    /// json文件读取器，如果后读取的文件已存在，则会覆盖前文件！！！
    /// </summary>
    public class WExcel : WModule
    {
        private List<Type> _allConfigTypes;
        private Dictionary<Type, object> _allConfigDict;
        private const string FileSeparator = "Table";
        private const string FileFolder = "ExcelData";
        private const string FileSuffix = ".json";

        public override int Priority => 2;

        protected override void Awake()
        {
            base.Awake();
            _allConfigTypes = GetAllAttributeTypes();
            _allConfigDict = new Dictionary<Type, object>();
            WLog.Log($"{nameof(WExcel)} initialize !");
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
        }

        public void UpdateAllAttributeTypes()
        {
            _allConfigTypes = GetAllAttributeTypes();
        }
        private List<Type> GetAllAttributeTypes()
        {
            //标签查找
            //var assembly = Assembly.GetAssembly(typeof(ExcelConfigAttribute));
            //WLog.Log($"<color=yellow>{assembly}</color>");
            var types = Utility.Assembly.GetTypes();

            bool IsMyAttribute(IEnumerable<Attribute> o)
            {
                return o.OfType<ExcelConfigAttribute>().Any();
            }

            var typeIes = types.Where(o => IsMyAttribute(Attribute.GetCustomAttributes(o, true)));
            //去除abstract父类
            return typeIes.Where(o => o.IsAbstract == false).ToList();
        }

        private string ResourcesPath(MemberInfo type)
        {
            var jsonName = type.Name.Replace(FileSeparator, "");
            var jsonPath = $"{FileFolder}/{jsonName}";
            return jsonPath;
        }

        private string StreamingPath(MemberInfo type)
        {
            var jsonName = type.Name.Replace(FileSeparator, "");
            var jsonPath = $"{Application.streamingAssetsPath}/{FileFolder}/{jsonName}{FileSuffix}";
            return jsonPath;
        }

        private string YooAssetsPath(MemberInfo type)
        {
            var jsonName = type.Name.Replace(FileSeparator, "");
            return jsonName;
        }

        public async UniTask LoadResourcesAsync()
        {
            foreach (var configType in _allConfigTypes)
            {
                _allConfigDict.TryAdd(configType, null);

                object asset = null;
                var jsonPath = ResourcesPath(configType);
                var loadAsync = await Resources.LoadAsync(jsonPath);
                var json = loadAsync as TextAsset;
                if (json != null)
                {
                    asset = JsonMapper.ToObject(json.text, configType);
                    if (asset is ExcelObject excelObject)
                    {
                        excelObject.EndInit();
                    }
                }

                _allConfigDict[configType] = asset;
            }
        }

        /// <summary>
        /// 加载所有 SteamingAssets 文件夹下的配置文件
        /// </summary>
        public async UniTask LoadStreamingAsync()
        {
            foreach (var configType in _allConfigTypes)
            {
                _allConfigDict.TryAdd(configType, null);

                string json = string.Empty;
                object asset = null;
                var jsonPath = StreamingPath(configType);


#if UNITY_STANDALONE_LINUX

                json = File.ReadAllText(jsonPath);
#else
                var webRequest = UnityWebRequest.Get(jsonPath);
                await webRequest.SendWebRequest();
                if (string.IsNullOrEmpty(webRequest.error))
                {
                    json = webRequest.downloadHandler.text;
                }
#endif

                //TODO:判定json中是否存在特殊字符
                if (json.Contains("\ufeff"))
                {
                    json = json.Replace("\ufeff", "");
                }

                // Debug.Log(configType);
                asset = JsonMapper.ToObject(json, configType);
                if (asset is ExcelObject excelObject)
                {
                    excelObject.EndInit();
                }

                _allConfigDict[configType] = asset;
            }
        }

        /// <summary>
        /// Resources同步读取
        /// </summary>
        public void LoadResources()
        {
            foreach (var configType in _allConfigTypes)
            {
                _allConfigDict.TryAdd(configType, null);

                object asset = null;
                var jsonPath = ResourcesPath(configType);
                var json = Resources.Load<TextAsset>(jsonPath);
                if (json != null)
                {
                    asset = JsonMapper.ToObject(json.text, configType);
                    if (asset is ExcelObject excelObject)
                    {
                        excelObject.EndInit();
                    }
                }

                _allConfigDict[configType] = asset;
            }
        }

        public async UniTask LoadYooAssets()
        {
            foreach (var configType in _allConfigTypes)
            {
                _allConfigDict.TryAdd(configType, null);

                object asset = null;
                var jsonPath = YooAssetsPath(configType);
                WLog.Log($"<color=red>{jsonPath}</color>");
                var handle = YooAssets.LoadAssetAsync<TextAsset>(jsonPath);
                await handle;
                var json = handle.AssetObject as TextAsset;
                if (json != null)
                {
                    asset = JsonMapper.ToObject(json.text, configType);
                    if (asset is ExcelObject excelObject)
                    {
                        excelObject.EndInit();
                    }
                }

                _allConfigDict[configType] = asset;
            }
        }
    }
}