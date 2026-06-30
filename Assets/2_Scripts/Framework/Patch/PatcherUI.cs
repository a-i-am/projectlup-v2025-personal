using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LUP
{
    public class PatcherUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Patcher patcher;

        [Header("UI Elements")]
        [SerializeField] private GameObject downloadPanel;
        [SerializeField] private Image progressBar;

        [Header("Settings")]
        [SerializeField] private bool autoHideWhenComplete = true;

        private bool isDownloading = false;
        private float lastProgress = 0f;

        void Start()
        {
            if (patcher == null)
                patcher = FindFirstObjectByType<Patcher>();

            if (patcher == null)
            {
                Debug.LogError("Patcher를 찾을 수 없습니다.");
                return;
            }


            if (downloadPanel != null)
            {
                downloadPanel.SetActive(false);
            }


            StartCoroutine(MonitorPatchProgress());
        }

        void Update()
        {
            if (patcher == null) return;

            float currentProgress = patcher.TotalProgress;


            if (Mathf.Abs(currentProgress - lastProgress) > 0.001f)
            {
                UpdateProgressUI(currentProgress);
                lastProgress = currentProgress;


                if (currentProgress > 0f && currentProgress < 1f && !isDownloading)
                {
                    isDownloading = true;
                    ShowDownloadPanel();
                }


                if (currentProgress >= 1f && isDownloading)
                {
                    isDownloading = false;
                    OnDownloadComplete();
                }
            }
        }

        private void UpdateProgressUI(float progress)
        {
            if (progressBar != null)
            {
                progressBar.fillAmount = progress;
            }
        }

        private void ShowDownloadPanel()
        {
            if (downloadPanel != null)
            {
                downloadPanel.SetActive(true);
            }
        }

        private void OnDownloadComplete()
        {
            Debug.Log("[PatcherUI] 다운로드 완료");

            if (autoHideWhenComplete)
            {
                StartCoroutine(HideDownloadPanelAfterDelay(0.1f));
            }
        }

        private IEnumerator HideDownloadPanelAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (downloadPanel != null)
            {
                downloadPanel.SetActive(false);
            }
        }

        private IEnumerator MonitorPatchProgress()
        {
            while (true)
            {
                if (patcher != null)
                {
                    float progress = patcher.TotalProgress;


                    if (progress > 0f && !isDownloading)
                    {
                        isDownloading = true;
                        ShowDownloadPanel();
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
