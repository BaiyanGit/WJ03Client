using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
/// <summary>
/// 项目启动
/// </summary>
public class StartProject : MonoBehaviour
{
    /// <summary>
    /// 加密
    /// </summary>
    public bool isNeedKey;

    private void Start()
    {
        //加密判断
        if (isNeedKey)
        {
            var exePath = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.LinuxPlayer:
                    exePath = Application.streamingAssetsPath + "/Release/GetMachineCode_Linux";
                    break;
                case RuntimePlatform.LinuxEditor:
                    exePath = Application.streamingAssetsPath + "/Release/GetMachineCode_Linux";
                    break;
                case RuntimePlatform.WindowsPlayer:
                    exePath = Application.streamingAssetsPath + "/Release/GetMachineCode.exe";
                    break;
                case RuntimePlatform.WindowsEditor:
                    exePath = Application.streamingAssetsPath + "/Release/GetMachineCode.exe";
                    break;
                default:
                    break;

            }
            try
            {
                Process.Start(exePath);
            }
            catch
            {
                ExitExe();
            }

            StartCoroutine(ReadMachineCode());

        }
        //禁用鼠标
        //Cursor.visible = false;

        //Application.targetFrameRate = 50;


        //Screen.SetResolution(1920, 1080, true);

        Screen.fullScreen = true;
    }

    private void ProStart()
    {


#if UNITY_EDITOR

#else
		 for (int i = 0 ; i < Display.displays.Length ; i++)
        {
            Display.displays[ i ].Activate();
        }
#endif

    }

    IEnumerator ReadMachineCode()
    {
        yield return new WaitForSeconds(2);
        string path = Application.streamingAssetsPath + "/Release/code.txt";
        string peizhi = "";
        if (File.Exists(@path))
        {
            peizhi = File.ReadAllText(@path);
            peizhi = peizhi.Replace("\n", "");
        }
        else
        {
            ExitExe();
        }
        if (peizhi != "")
        {
            string machinecode = peizhi;
            //将机器码转换为注册码
            string md = MD5(machinecode + "ZhiQi2686741852");
            string registercode = md + md + md;
            //读取register中的txt
            path = Application.streamingAssetsPath + "/Release/register.txt";
            if (File.Exists(@path))
            {
                peizhi = File.ReadAllText(@path);
                peizhi = peizhi.Replace("\n", "");
            }
            //对比注册码和register
            if (peizhi != registercode)
            {
                ExitExe();

            }
        }
        else
        {
            ExitExe();
        }
        ProStart();
    }

    public static string MD5(string str)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        byte[] bytes = Encoding.UTF8.GetBytes(str);

        string md5Str = BitConverter.ToString(md5.ComputeHash(bytes));

        return md5Str;
    }

    /// <summary>
    /// 退出程序
    /// </summary>
    public void ExitExe()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}
