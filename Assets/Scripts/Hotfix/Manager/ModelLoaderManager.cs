using System.Collections.Generic;
using Wx.Runtime.Singleton;
using TriLibCore;
using UnityEngine;
using System.IO;
using System.Linq;
using Hotfix;
using Hotfix.ExcelData;
using Wx.Runtime;

public class ModelLoaderManager : SingletonInstance<ModelLoaderManager>, ISingleton
{
    private Dictionary<string, ExtendModelConfig> m_PathConfigDic = new();
    private List<string> m_PathList = new();
    private List<GameObject> m_LoadedModelCache = new();

    /// <summary>
    /// 已加载的模型数量
    /// </summary>
    private int m_LoadModelCount;

    public void OnCreate(object createParam)
    {
    }

    public void OnDestroy()
    {
    }

    public void OnFixedUpdate()
    {
    }

    public void OnLateUpdate()
    {
    }

    public void OnUpdate()
    {
    }

    public void LoadAllModels()
    {
        m_PathConfigDic = new();

        var temp = Application.streamingAssetsPath + "/Models";
        for (int i = 0; i < ExtendModelConfigTable.Instance.dataList.Count; i++)
        {
            string path = string.Format("{0}/{1}", temp, ExtendModelConfigTable.Instance.dataList[i].Name);
            m_PathConfigDic.Add(path, ExtendModelConfigTable.Instance.dataList[i]);
        }

        if (m_PathConfigDic.Count != 0)
        {
            m_PathList = m_PathConfigDic.Keys.ToList();
            LoadAModel(m_PathList[0], m_PathConfigDic[m_PathList[0]]);
        }
    }

    private void LoadAModel(string path, ExtendModelConfig config)
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        assetLoaderOptions.Timeout = 0;
        
        //assetLoaderOptions.AnimationType = TriLibCore.General.AnimationType.Generic;

        AssetLoader.LoadModelFromFile(path, null, OnModelImportFinished, OnModelImportting, OnModelImportError, null, assetLoaderOptions, config);
    }

    /// <summary>
    /// 当模型导入中
    /// </summary>
    /// <param name="alc"></param>
    /// <param name="progress"></param>
    private void OnModelImportting(AssetLoaderContext alc, float progress)
    {
        //回调，更新界面进度
        Debug.LogFormat("{0}:{1}", alc.Filename, progress);
    }

    /// <summary>
    /// 当模型导入完成
    /// </summary>
    /// <param name="alc"></param>
    private void OnModelImportFinished(AssetLoaderContext alc)
    {
        GameObject mTargetGo = alc.RootGameObject;

        //将模型放到合适的路径下
        var config = alc.CustomData as ExtendModelConfig;
        var target = GameManager.Instance.MainTarget.FindTheChildNode(config.RelativePath);
        mTargetGo.transform.SetParent(target);

        //将模型信息更新到结构认知要缓存的整体渲染缓存里
        var renderers = mTargetGo.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            GameManager.Instance.AppendTargetRenderers(renderer, renderer.materials);
        }

        m_LoadedModelCache.Add(mTargetGo);

        alc.Stream.Dispose();

        DoProgressLogic();
    }

    /// <summary>
    /// 当模型导入报错
    /// </summary>
    /// <param name="obj"></param>
    private void OnModelImportError(IContextualizedError obj)
    {
        DoProgressLogic();
        Debug.LogWarning($"An error occurred while loading your Model: {obj.GetInnerException()}");
    }

    private void DoProgressLogic()
    {
        m_LoadModelCount++;

        float progress = m_LoadModelCount / (float)m_PathConfigDic.Count;
        // Debug.LogFormat("TotalProgress:{0}", progress);

        if (m_LoadModelCount >= m_PathConfigDic.Count)
        {
            //m_OnFinsihCallback?.Invoke();
            //等待一段时间，给模型更多的初始化时间
            //Invoke("DoModelInitFinish", (float)Math.PI);

            // Debug.Log("LoaderFinished!");
            return;
        }

        m_PathList.RemoveAt(0);
        LoadAModel(m_PathList[0], m_PathConfigDic[m_PathList[0]]);
    }
}