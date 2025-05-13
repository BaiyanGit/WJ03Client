using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TestFollowCamera : MonoBehaviour
{
    public Camera UI_Camera;//UI相机
    public RectTransform _thisTran;//UI元素
    public Canvas ui_Canvas;
    public Transform _UIParent;
    public Text showName;
    /// <summary>
    /// 背景处理
    /// </summary>
    public RectTransform _bg;
    /// <summary>
    /// 宽度
    /// </summary>
    public float _witch;
    public float _oldWitch;

    GameObject uiSHowText;

    bool _isShowOrHide;



    private void Awake()
    {
        UI_Camera = GameObject.Find("UICamera").GetComponent<Camera>();
        //_thisTran = this.transform as RectTransform;
        _UIParent = GameObject.Find("ShowTip").transform;
        ui_Canvas = GameObject.FindGameObjectWithTag("_TagCanvas").GetComponent<Canvas>();
        uiSHowText = Resources.Load<GameObject>("Prefabs/ShowUITip");

        //Ctrl_MessageCenter.AddMsgListener("ChangeNameShow", ChangeNameShow);

    }

    //public void Init()
    //{
    //    GameObject game = Instantiate(uiSHowText);
    //    _thisTran = game.transform as RectTransform;
    //    _thisTran.parent = _UIParent;
    //    _thisTran.localScale = new Vector3(1, 1, 1);
    //    _thisTran.gameObject.SetActive(false);
    //}

    /// <summary>
    /// 显隐设置
    /// </summary>
    void ChangeNameShow()
    {
        _UIParent.gameObject.SetActive(_isShowOrHide);
        _isShowOrHide = !_isShowOrHide;
    }


    void UpdateUIPosition()
    {
        Vector2 PlayerScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 mouseUGUIPos = new Vector2();
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(ui_Canvas.transform as RectTransform, PlayerScreen, UI_Camera, out mouseUGUIPos);

        //Debug.Log(mouseUGUIPos);
        if (isRect && _thisTran != null)
        {
            _thisTran.anchoredPosition = mouseUGUIPos;
        }
    }

    void Update()
    {
        UpdateUIPosition();
    }

    private void OnDestroy()
    {
        if (_thisTran != null)
        {
            Destroy(_thisTran.gameObject);
        }
        //Ctrl_MessageCenter.RemoveMsgListener("ChangeNameShow", ChangeNameShow);
    }

    public void InitSetText(string content)
    {
        if (showName == null)
        {
            GameObject game = Instantiate(uiSHowText);
            _thisTran = game.transform as RectTransform;
            _thisTran.parent = _UIParent;
            _thisTran.localScale = new Vector3(1, 1, 1);
            _thisTran.gameObject.SetActive(false);
            _bg = _thisTran.Find("BG").transform as RectTransform;
            //showName = transform.GetComponentInChildren<Text>();
            showName = _thisTran.Find("Show").GetComponent<Text>();

        }
        showName.text = content;

        _witch = (content.Length * showName.fontSize) + 20f;


        _bg.localPosition = _bg.localPosition - new Vector3(_witch / 2, 0, 0);
        showName.rectTransform.localPosition = showName.rectTransform.localPosition - new Vector3(_witch / 2, 0, 0);


        _bg.sizeDelta = new Vector2(_witch, _bg.rect.height);
    }

    public void ShowAndHide(bool _isShow)
    {
        _thisTran.gameObject.SetActive(_isShow);
    }

}
