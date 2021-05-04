using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kam.TooltipUI
{
    public class TooltipPopup : MonoBehaviour
    {
        public  RectTransform   popupObject;
        public  TextMeshProUGUI infoText;
        public  Vector3         offset;
        public  float           padding;

        private Canvas          popupCanvas;

        private void Awake()
        {
            popupCanvas = GetComponentInParent<Canvas>();
            HideInfo();
        }

        private void Update()
        {
            FollowCursor();
        }

        private void FollowCursor()
        {
            if (!popupObject.gameObject.activeSelf) { return; }

            Vector3 newPos = Input.mousePosition + offset;
            newPos.z = 0;

            float rightEdgeToScreen = Screen.width - (newPos.x + popupObject.rect.width * popupCanvas.scaleFactor) - padding;
            float leftEdgeToScreen = 0 - newPos.x  + padding;

            float upEdgeToScreen = Screen.height - (newPos.y + popupObject.rect.height * popupCanvas.scaleFactor) - padding;

            if (rightEdgeToScreen < 0)
            {
                newPos.x += rightEdgeToScreen;
            }

            if(leftEdgeToScreen > 0)
            {
                newPos.x += leftEdgeToScreen;
            }

            if(upEdgeToScreen < 0)
            {
                newPos.y += upEdgeToScreen;
            }

            popupObject.position = newPos;
        }

        public void DisplayInfo(StringBuilder text)
        {
            infoText.text = text.ToString();
            popupObject.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
        }

        public void HideInfo()
        {
            popupObject.gameObject.SetActive(false);
        }
    }
}
