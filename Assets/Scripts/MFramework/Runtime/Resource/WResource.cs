using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.Resource
{
    public class WResource : WModule
    {
        public override int Priority => 1;


        private BuildInResource _buildInResource;
        private WebResource _webResource;
        private YooResource _yooResource;
        private IOFileResource _IOFileResource;

        public BuildInResource BuildInResource
        {
            get { return _buildInResource ??= new BuildInResource(); }
            private set { _buildInResource = value; }
        }
        
        public WebResource WebResource
        {
            get { return _webResource ??= new WebResource(); }
            private set { _webResource = value; }
        }
        
        public YooResource YooResource
        {
            get { return _yooResource ??= new YooResource(); }
            private set { _yooResource = value; }
        }

        public IOFileResource IOFileResource
        {
            get { return _IOFileResource ??= new IOFileResource(); }
            private set { _IOFileResource = value; }
        }

        protected override void Awake()
        {
            base.Awake();

            _buildInResource = new BuildInResource();
            _webResource = new WebResource();
            _yooResource = new YooResource();

            WLog.Log($"{nameof(WResource)} initialize !");
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}