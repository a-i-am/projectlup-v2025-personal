using UnityEngine;

namespace LUP.PCR
{
    public class SafeAreaApplier : MonoBehaviour
    {
        Vector2 minAnchor;
        Vector2 maxAnchor;

        private void Start()
        {
            RectTransform rect = this.GetComponent<RectTransform>();

            minAnchor = Screen.safeArea.min;
            maxAnchor = Screen.safeArea.max;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;

            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            rect.anchorMin = minAnchor;
            rect.anchorMax = maxAnchor;
        }
    }
}


