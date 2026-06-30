using UnityEngine;
using System;
using UnityEngine.UI;
using R3;
using DG.Tweening;

namespace LUP.PCR
{
    public class MainUIView : MonoBehaviour
    {
        [Header("작업 버튼")]
        [SerializeField] private Button digBtn;
        [SerializeField] private Button constructBtn;
        [SerializeField] private Button inventoryBtn;

        [Header("자원 표시")]
        [SerializeField] private Text foodText;
        [SerializeField] private Text powerText;
        [SerializeField] private Text stoneText;
        [SerializeField] private Text ironText;

        [Header("패널")]
        [SerializeField] private RectTransform upperPanel;
        [SerializeField] private RectTransform taskPanel;

        private readonly CompositeDisposable cd = new();
        private MainUIViewModel mainVM;

        private Vector2 onScreenUpperPanelPos;
        private Vector2 offScreenUpperPanelPos;

        private Vector2 onScreenTaskPanelPos;
        private Vector2 offScreenTaskPanelPos;

        private void Start()
        {
            onScreenUpperPanelPos = Vector2.zero;
            offScreenUpperPanelPos = new Vector2(0, 80f);

            onScreenTaskPanelPos = new Vector2(-15f, 15f);
            offScreenTaskPanelPos = new Vector2(115f, 0);


            taskPanel.anchoredPosition = onScreenTaskPanelPos;
            upperPanel.anchoredPosition = onScreenUpperPanelPos;
        }

        public void Bind(MainUIViewModel vm)
        {
            cd.Clear();
            mainVM = vm;


            digBtn.onClick.AddListener(() => mainVM.ClickDig.OnNext(Unit.Default));
            constructBtn.onClick.AddListener(() => mainVM.ClickConstruct.OnNext(Unit.Default));
            inventoryBtn.onClick.AddListener(() => mainVM.ClickInventory.OnNext(Unit.Default));


            vm.Food.DistinctUntilChanged().Subscribe(value => foodText.text = value.ToString()).AddTo(cd);
            vm.Power.DistinctUntilChanged().Subscribe(value => powerText.text = value.ToString()).AddTo(cd);
            vm.Stone.DistinctUntilChanged().Subscribe(value => stoneText.text = value.ToString()).AddTo(cd);
            vm.Iron.DistinctUntilChanged().Subscribe(value => ironText.text = value.ToString()).AddTo(cd);
        }

        private void OnDestroy()
        {
            cd.Dispose();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            taskPanel.DOAnchorPos(onScreenTaskPanelPos, 0.2f).SetEase(Ease.OutCubic);
            upperPanel.DOAnchorPos(onScreenUpperPanelPos, 0.2f)
                .SetEase(Ease.OutCubic);
        }

        public void Hide()
        {
            taskPanel.DOAnchorPos(offScreenTaskPanelPos, 0.2f).SetEase(Ease.InCubic);
            upperPanel.DOAnchorPos(offScreenUpperPanelPos, 0.2f)
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
    }
}
