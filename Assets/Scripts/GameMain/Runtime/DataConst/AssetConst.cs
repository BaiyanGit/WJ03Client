
using YooAsset;

namespace GameMain.Runtime
{
    public static partial class AppConst
    {
        public static class AssetConst
        {
            public static string assetsSever = "http://127.0.0.1:8088";
            public static YooAssetSettings yooAssetSettings = null;
            public static EnumPlayMode playMode = EnumPlayMode.BuildIn;
            public static EPlayMode ePlayMode = EPlayMode.EditorSimulateMode;
            public static string packageName = "DefaultPackage";
            public static EDefaultBuildPipeline buildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline;

            public static string severVersion = "V1.0";
        }
    }
}
