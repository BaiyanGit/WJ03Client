using UnityEngine;

namespace Hotfix.UI
{
    public class LabelTipItem : MonoBehaviour
    {
        Vector3 followPos;
        RectTransform rectTrans;
        RectTransform rectMine;

        private void Start()
        {
            rectMine = GetComponent<RectTransform>();
        }

        public void InitFollowPos(Vector3 vector,Transform rect)
        {
            followPos = vector;
            rectTrans = rect as RectTransform;
        }

        private void Update()
        {
            rectMine.anchoredPosition = ChangeUIPosition(followPos);
        }

        private Vector2 ChangeUIPosition(Vector3 pos)
        {
            //Debug.Log(pos);
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
            Vector2 UGUIPos = new Vector2();
            bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, screenPos, null/*Camera.main*/, out UGUIPos);

            if (!isRect)
            {
                UGUIPos = Vector2.zero;
            }

            return UGUIPos;
        }


        private Vector3 ChangeUIToVector3(Vector2 pos)
        {
            Vector3 modelPos = Camera.main.ScreenToWorldPoint(pos);
            Vector3 vector3 = new Vector3();
            bool isPos = RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, pos, Camera.main, out vector3);
            if (!isPos)
            {
                vector3 = Vector3.zero;
            }

            return vector3;
        }
    }
}

