using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Editor.UI
{
    public static class UIAutoCreatePathSetting
    {
        /// <summary>
        /// 工具路径
        /// </summary>
        public const string ToolPath = "Assets/Scripts/MFramework/Editor/GenerateUI/";

        /// <summary>
        /// 预制体模板路径
        /// </summary>
        public const string PrefabTemplatePath = ToolPath + "Resources/UITemplate";

        /// <summary>
        /// 预制体生成路径
        /// </summary>
        public const string PrefabCreatePath = "Assets/Resources/Prefab/UI/";

        /// <summary>
        /// View代码生成配置文件路径
        /// </summary>
        public const string UIViewAutoCreateConfigPath = ToolPath + "Template/UIViewAutoCreateConfig.asset";

        /// <summary>
        /// 代码模板路径
        /// </summary>
        public const string TemplateFilePath = ToolPath + "Template/";
        public const string ModelTemplateName = "UIModelTemplate.txt";
        public const string ViewTemplateName = "UIViewTemplate.txt";
        public const string ControlTemplateName = "UIControlTemplate.txt";
        public const string EnumTemplateName = "EnumUIFormTemplate.txt";

        /// <summary>
        /// 代码生成路径
        /// </summary>
        public static readonly string GenerateCsFilePath = $"{Application.dataPath}/Scripts/Hotfix/UI/";
        public static readonly string GenerateEnumFilePath = $"{Application.dataPath}/Scripts/Hotfix/Enum/EnumUIForm.cs";
        
    }
}