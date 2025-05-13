using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DataScreenInfoItem : MonoBehaviour
{
    private TextMeshProUGUI m_tmpState;
    private TextMeshProUGUI m_tmpEvent;
    private TextMeshProUGUI m_tmpTime;

    private Sprite rightBg;
    private Sprite errorBg;

    Image imageBg;

    private void Awake()
    {
        rightBg ??= Resources.Load<Sprite>("Image/Right");
        errorBg ??= Resources.Load<Sprite>("Image/Error");

        m_tmpState ??= transform.Find("TmpState").GetComponent<TextMeshProUGUI>();
        m_tmpEvent ??= transform.Find("TmpEvent").GetComponent<TextMeshProUGUI>();
        m_tmpTime ??= transform.Find("TmpTime").GetComponent<TextMeshProUGUI>();
        imageBg ??= transform.GetComponent<Image>();
        imageBg.sprite ??= rightBg;
    }

    public void InitData(string state, string eventMsg)
    {
        Awake();

        m_tmpState.text = state;
        m_tmpEvent.text = eventMsg;
        m_tmpTime.text = System.DateTime.Now.ToString();

        if (state == "Õý³£")
        {
            imageBg.sprite = rightBg;
            return;
        }

        imageBg.sprite = errorBg;
    }
}