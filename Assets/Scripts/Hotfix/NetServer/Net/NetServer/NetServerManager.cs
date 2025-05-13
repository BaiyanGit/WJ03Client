using UnityEngine;

/// <summary>
/// 网络通信管理类
/// </summary>
public class NetServerManager : MonoBehaviour
{
    private ServNet servNet;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        ServNet.Instance.Close();
    }
}