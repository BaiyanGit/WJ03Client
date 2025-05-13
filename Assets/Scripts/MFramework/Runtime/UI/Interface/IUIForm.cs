using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.UI
{
    public interface IUIForm
    {
        /// <summary>
        /// 获取界面资源名称。
        /// </summary>
        string UIFormAssetName
        {
            get;
        }

        UIFormLogic Handle
        {
            get;
            set;
        }

        /// <summary>
        /// 获取界面所属的界面组。
        /// </summary>
        UIGroup UIGroup
        {
            get;
        }

        /// <summary>
        /// 获取界面在界面组中的深度。
        /// </summary>
        int DepthInUIGroup
        {
            get;
        }

        /// <summary>
        /// 获取是否暂停被覆盖的界面。
        /// </summary>
        bool PauseCoveredUIForm
        {
            get;
        }

        public bool Paused
        {
            get;
            set;
        }

        public bool Covered
        {
            get;
            set;
        }

        public IUIView UIView
        {
            get;
            set;
        }

        public IUIModel UIModel
        {
            get;
            set;
        }

        UIGroupInfo SetUIGroupInfo();

        /// <summary>
        /// 初始化界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        ///<param name="uiGroup">界面组。</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
        /// <param name="handle">界面实例。</param>
        void OnInit(string uiFormAssetName,UIGroup uiGroup, bool pauseCoveredUIForm,UIFormLogic handle);

        /// <summary>
        /// 界面回收。
        /// </summary>
        void OnRecycle();

        /// <summary>
        /// 界面打开。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        void OnOpen(object userData = null);
        
        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        void Close(object userData = null);

        /// <summary>
        /// 界面暂停。
        /// </summary>
        void OnPause();

        /// <summary>
        /// 界面暂停恢复。
        /// </summary>
        void OnResume();

        /// <summary>
        /// 界面遮挡。
        /// </summary>
        void OnCover();

        /// <summary>
        /// 界面遮挡恢复。
        /// </summary>
        void OnReveal();

        /// <summary>
        /// 界面激活。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        void OnRefocus(object userData = null);

        /// <summary>
        /// 界面轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void OnUpdate(float elapseSeconds, float realElapseSeconds);

        void OnFixedUpdate();

        /// <summary>
        /// 界面深度改变。
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度。</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度。</param>
        void OnDepthChanged(int uiGroupDepth, int depthInUIGroup);
    }
}
