using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewSetting : IUIView
    {
        public Image imgBg;
		public Button btnCamouflage;
		public Button btnGreenishWhite;
		public Toggle togPreformant;
		public Toggle togBalanced;
		public Toggle togHighFidelity;
		public Button btnDown;
		public Button btnUp;

		public Slider audioSlider;

		public ToggleGroup qualityGroup;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			btnCamouflage = handle.transform.Find("Img_Bg/List/MaterialSetting/ButtonList/Btn_Camouflage").GetComponent<Button>();
			btnGreenishWhite = handle.transform.Find("Img_Bg/List/MaterialSetting/ButtonList/Btn_GreenishWhite").GetComponent<Button>();
			togPreformant = handle.transform.Find("Img_Bg/List/QualitySetting/ToggleList/Tog_Preformant").GetComponent<Toggle>();
			togBalanced = handle.transform.Find("Img_Bg/List/QualitySetting/ToggleList/Tog_Balanced").GetComponent<Toggle>();
			togHighFidelity = handle.transform.Find("Img_Bg/List/QualitySetting/ToggleList/Tog_HighFidelity").GetComponent<Toggle>();
			btnDown = handle.transform.Find("Img_Bg/List/AudioSetting/SetAudio/Btn_Down").GetComponent<Button>();
			btnUp = handle.transform.Find("Img_Bg/List/AudioSetting/SetAudio/Btn_Up").GetComponent<Button>();

            audioSlider = btnDown.transform.parent.Find("AudioSlider").GetComponent<Slider>();
			qualityGroup = (qualityGroup != null) ? qualityGroup : togBalanced.transform.parent.GetComponent<ToggleGroup>();
        }
    }
}