using UnityEngine;

namespace ByQuestCreatCameraConfig
{
    using System.Collections.Generic;
    using LitJson;

    public class ObjectSnapManager : MonoBehaviour
    {
        #region 单例

        private static ObjectSnapManager _instance;

        public static ObjectSnapManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var sceneSnap = FindAnyObjectByType<ObjectSnapManager>();
                    if (sceneSnap == null)
                    {
                        var go = new GameObject(nameof(ObjectSnapManager));
                        _instance = go.AddComponent<ObjectSnapManager>();
                    }
                    else
                    {
                        _instance = sceneSnap;
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Public Fields

        [Header("文件名")] public string fileName = "CameraTargetSnap";
        [Header("是否读取文件")] public bool isReadFromFile = false;

        [Space(10), Header("Ctrl+E 切换读取文件")] [Space(10), Header("Ctrl+Alt+S 切换读取文件")]

        #endregion

        #region Private Fields

        private readonly Dictionary<string, BaseData> _objectDataSnaps = new();

        #endregion

        private void Start()
        {
            ReadFromFile();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
            {
                WriteToFile();
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E))
            {
                isReadFromFile = !isReadFromFile;
            }
        }

        public void GetSnapData(string goName, out BaseData data)
        {
            _objectDataSnaps.TryGetValue(goName, out data);
        }

        /// <summary>
        /// 添加对象数据
        /// </summary>
        /// <param name="go"></param>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        public void AddObjectData<T>(T data) where T : BaseData
        {
            if (data is { } snap)
            {
                if (!_objectDataSnaps.TryAdd(data.objectName, snap))
                {
                    _objectDataSnaps[data.objectName] = snap;
                }
            }
            else
            {
                Debug.LogError($"类型不匹配，期望类型：{typeof(T).Name}，实际类型：{data.GetType().Name}");
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        private void WriteToFile()
        {
            var list = new List<BaseData>(_objectDataSnaps.Values);
            var json = JsonMapper.ToJson(list);
            Debug.Log(json);

            var folderPath = $"{Application.streamingAssetsPath}/SceneSnap";
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            var filePath = $"{folderPath}/{fileName}.json";

            Debug.Log($"WriteToFile：{filePath}");

            System.IO.File.WriteAllText(filePath, json);
        }

        private void ReadFromFile()
        {
            var folderPath = $"{Application.streamingAssetsPath}/SceneSnap";
            var filePath = $"{folderPath}/{fileName}.json";

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                var list = JsonMapper.ToObject<List<BaseData>>(json);
                foreach (var snap in list)
                {
                    Debug.Log("<color=yellow>读取对象属性</color>\n" +
                              $"对象名称：{snap.objectName}\n" +
                              $"默认缩放：{snap.distance}\n" +
                              $"最小缩放：{snap.minDistance}\n" +
                              $"最大缩放：{snap.maxDistance}\n" +
                              $"对象位置：({string.Join(",", snap.position)})\n" +
                              $"对象旋转：({string.Join(",", snap.eulerAngles)})");
                    AddObjectData(snap);
                }
            }
            else
            {
                Debug.LogError($"文件不存在：{filePath}");
            }
        }
    }
}