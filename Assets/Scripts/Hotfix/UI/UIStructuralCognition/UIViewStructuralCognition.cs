using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 结构认知界面
    /// </summary>
    public class UIViewStructuralCognition : IUIView
    {
        public RawImage rawModel;

        public ToggleGroup MainStructuralToggleGroup;
        public GameObject MainStructuralItemPrefab;
        public Transform StructuralPanel;
        public ToggleGroup StructuralToggleGroup;
        public GameObject StructuralItemPrefab;
        public TMP_Text DescriptionText;

        public void Init(GameObject handle)
        {
            rawModel = handle.transform.Find("Img_Bg/ModelShowContent/Raw_Model").GetComponent<RawImage>();

            MainStructuralToggleGroup = handle.transform.Find("Img_Bg/LeftToggleList").GetComponent<ToggleGroup>();
            MainStructuralItemPrefab = GameEntry.Resource.BuildInResource.Load<GameObject>(AppConst.AssetPathConst.MainStructureItem);
            StructuralPanel = handle.transform.Find("Img_Bg/ToggleList");
            StructuralToggleGroup = StructuralPanel.Find("Viewport/Ts_EngineContent").GetComponent<ToggleGroup>();
            StructuralItemPrefab = GameEntry.Resource.BuildInResource.Load<GameObject>(AppConst.AssetPathConst.StructureItem);
            DescriptionText = handle.transform.Find("Img_Bg/Left/Scroll View/Viewport/Text (TMP)").GetComponent<TMP_Text>();

            // MainStructuralToggleGroup.allowSwitchOff = true;
            StructuralToggleGroup.allowSwitchOff = true;
        }
    }
}