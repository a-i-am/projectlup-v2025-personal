using UnityEngine;
using UnityEngine.UI;
using System;

namespace LUP.PCR
{
    public class BuildingActionMenu : MonoBehaviour
    {
        [SerializeField] private Canvas contentRoot;
        [Header("건물 들어가기 버튼")]
        [SerializeField] private Button floatingBtnProduct;
        [SerializeField] private Button floatingBtnUpgrade;

        public event Action<FarmUIBtnType> OnSelectMenu;

        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;

            if (contentRoot != null)
            {
                contentRoot.renderMode = RenderMode.WorldSpace;
                contentRoot.worldCamera = mainCam;
            }

            if (floatingBtnProduct != null)
            {
                floatingBtnProduct.onClick.AddListener(() => OnSelectMenu?.Invoke(FarmUIBtnType.Product));
            }

            if (floatingBtnUpgrade != null)
            {
                floatingBtnUpgrade.onClick.AddListener(() => OnSelectMenu?.Invoke(FarmUIBtnType.Upgrade));
            }

            Hide();
        }

        private void LateUpdate()
        {
            if (contentRoot.gameObject.activeSelf && mainCam != null)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
            }
        }

        public void Show() => contentRoot.gameObject.SetActive(true);
        public void Hide() => contentRoot.gameObject.SetActive(false);

        public void Toggle()
        {
            if (contentRoot.gameObject.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}
