using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;
using UnityEngine.Video;

namespace Hotfix.UI
{
    /// <summary>
    /// 原理学习界面
    /// </summary>
    public class UIViewPrincipleLearning : IUIView
    {
        public RawImage rawVideo;
        public Button videoStateBtn;
        public Image videoStateImage;
        public VideoPlayer videoPlayer;
        public Slider videoSlider;
        public DragHelper videoDragHelper;
        public Button videoFastBackBtn;
        public Button videoPlayBtn;
        public Button videoPauseBtn;
        public Button videoFastForwardBtn;

        public RectTransform videoVolume;
        public Slider videoVolumeSlider;
        public TMP_Text ratioText;

        public ToggleGroup MainStructuralToggleGroup;
        public GameObject MainStructuralItemPrefab;
        public Transform StructuralPanel;
        public ToggleGroup StructuralToggleGroup;

        public GameObject StructuralItemPrefab;
        public TMP_Text DescriptionText;

        public Transform _tsRight;
        public ToggleGroup BottomToggleGroup;

        public void Init(GameObject handle)
        {
            videoPlayer = handle.transform.Find("Img_Bg/VideoShowContent/VideoPlayer").GetComponent<VideoPlayer>();
            rawVideo = handle.transform.Find("Img_Bg/VideoShowContent/Raw_Video").GetComponent<RawImage>();
            videoStateBtn = rawVideo.GetComponent<Button>();
            videoStateImage = videoStateBtn.transform.Find("Image/Image").GetComponent<Image>();
            videoSlider = videoPlayer.transform.Find("Slider").GetComponent<Slider>();
            videoDragHelper = videoSlider.GetComponent<DragHelper>();
            videoFastBackBtn = videoPlayer.transform.Find("Button_FastBack").GetComponent<Button>();
            videoPlayBtn = videoPlayer.transform.Find("Button_Play").GetComponent<Button>();
            videoPauseBtn = videoPlayer.transform.Find("Button_Pause").GetComponent<Button>();
            videoFastForwardBtn = videoPlayer.transform.Find("Button_FastForward").GetComponent<Button>();
            videoVolume = videoPlayer.transform.Find("GameObject").GetComponent<RectTransform>();
            videoVolumeSlider = videoVolume.Find("Slider").GetComponent<Slider>();
            ratioText = videoPlayer.transform.Find("Text (TMP)").GetComponent<TMP_Text>();

            MainStructuralToggleGroup = handle.transform.Find("Img_Bg/LeftToggleList").GetComponent<ToggleGroup>();
            MainStructuralItemPrefab =
                GameEntry.Resource.BuildInResource.Load<GameObject>(AppConst.AssetPathConst.MainStructureItem);
            StructuralPanel = handle.transform.Find("Img_Bg/ToggleList");
            StructuralToggleGroup = StructuralPanel.Find("Viewport/Ts_EngineContent").GetComponent<ToggleGroup>();
            StructuralItemPrefab =
                GameEntry.Resource.BuildInResource.Load<GameObject>(AppConst.AssetPathConst.PrincipleItem);
            DescriptionText = handle.transform.Find("Img_Bg/Left/Scroll View/Viewport/Text (TMP)").GetComponent<TMP_Text>();

            _tsRight = handle.transform.Find("Img_Bg/DetailList/Ts_Right");
            BottomToggleGroup = _tsRight.GetComponent<ToggleGroup>();

            // MainStructuralToggleGroup.allowSwitchOff = true;
            StructuralToggleGroup.allowSwitchOff = true;
        }
    }
}