using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewStudentInfoRecord : IUIView
    {
        public Image imgBg;
		public TMP_InputField tmpinputName;
		public TMP_InputField tmpinputID;
		public TMP_InputField tmpinputScore;
		public TMP_InputField tmpinputEvaluation;
		public Button btnSure;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			tmpinputName = handle.transform.Find("Img_Bg/StudentName/TmpInput_Name").GetComponent<TMP_InputField>();
			tmpinputID = handle.transform.Find("Img_Bg/StudentID/TmpInput_ID").GetComponent<TMP_InputField>();
			tmpinputScore = handle.transform.Find("Img_Bg/TrainScore/TmpInput_Score").GetComponent<TMP_InputField>();
			tmpinputEvaluation = handle.transform.Find("Img_Bg/TrainEvaluation/TmpInput_Evaluation").GetComponent<TMP_InputField>();
			btnSure = handle.transform.Find("Img_Bg/Btn_Sure").GetComponent<Button>();
        }
    }
}