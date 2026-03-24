using UnityEngine;
using UnityEngine.UI;
using System;

namespace LUP.PCR
{
    public class ConstructionOverlayView : MonoBehaviour
    {
        [Header("업그레이드 UI")]
        [SerializeField] private Canvas contentRoot; // UI 전체 부모 (UI 활성/비활성)
        [SerializeField] private Slider progressBar;
        [SerializeField] private Text timerText; // 00:00:00
        [SerializeField] private Button btnAccelerate; // 가속 버튼

        public event Action OnClickAccelerate;

        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;

            if (contentRoot != null)
            {
                contentRoot.renderMode = RenderMode.WorldSpace;
                contentRoot.worldCamera = mainCam;
            }

            if (btnAccelerate != null)
            {
                btnAccelerate.onClick.AddListener(() => OnClickAccelerate?.Invoke());
            }
            Hide(); // 시작할 땐 숨김
        }

        private void LateUpdate()
        {
            if (mainCam != null)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
            }
        }

        // State에서 매 프레임 호출할 갱신 함수
        public void UpdateView(float progressRatio, float remainingTime)
        {
            if (progressBar != null)
            {
                progressBar.value = progressRatio;
            }

            // 시:분:초
            if (timerText != null)
            {
                TimeSpan span = TimeSpan.FromSeconds(remainingTime);
                timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                    span.Hours, span.Minutes, span.Seconds);
            }
        }

        public void Show() => contentRoot.gameObject.SetActive(true);
        public void Hide() => contentRoot.gameObject.SetActive(false);
    }
}