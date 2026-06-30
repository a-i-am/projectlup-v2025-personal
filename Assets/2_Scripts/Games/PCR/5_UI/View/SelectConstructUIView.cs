using R3;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace LUP.PCR
{
    public class SelectConstructUIView : MonoBehaviour
    {
        [SerializeField] private Button wheatFarmBtn;
        [SerializeField] private Button moleFarmBtn;
        [SerializeField] private Button powerStationBtn;
        [SerializeField] private Button stoneMineBtn;
        [SerializeField] private Button workStationBtn;
        [SerializeField] private Button restaurantBtn;
        [SerializeField] private Button ladderBtn;

        [SerializeField] private Button backBtn;

        private readonly CompositeDisposable cd = new();

        [SerializeField] private RectTransform constructionPanel;

        private Vector2 onScreenConstructionPanelPos;
        private Vector2 offScreenConstructionPanelPos;

        private void Start()
        {
            onScreenConstructionPanelPos = Vector2.zero;
            offScreenConstructionPanelPos = new Vector2(0f, -200f);
        }

        public void Bind(SelectConstructViewModel vm)
        {
            cd.Clear();




            wheatFarmBtn?.onClick.RemoveAllListeners();
            wheatFarmBtn?.onClick.AddListener(() => vm.OnBuildingSelected.OnNext(BuildingType.WHEATFARM));

            moleFarmBtn?.onClick.RemoveAllListeners();
            moleFarmBtn?.onClick.AddListener(() => vm.OnBuildingSelected.OnNext(BuildingType.MOLEFARM));

            powerStationBtn?.onClick.RemoveAllListeners();
            powerStationBtn?.onClick.AddListener(() => vm.OnBuildingSelected.OnNext(BuildingType.POWERSTATION));

            stoneMineBtn?.onClick.RemoveAllListeners();
            stoneMineBtn?.onClick.AddListener(() => vm.OnBuildingSelected.OnNext(BuildingType.STONEMINE));

            workStationBtn?.onClick.RemoveAllListeners();
            workStationBtn?.onClick.AddListener(() => vm.OnBuildingSelected.OnNext(BuildingType.WORKSTATION));

            restaurantBtn?.onClick.RemoveAllListeners();
            restaurantBtn?.onClick.AddListener(() => vm.OnBuildingSelected.OnNext(BuildingType.RESTAURANT));

            ladderBtn?.onClick.RemoveAllListeners();
            ladderBtn?.onClick.AddListener(() => vm.OnBuildingSelected.OnNext(BuildingType.LADDER));

        }

        private void OnDestroy()
        {
            cd.Dispose();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            constructionPanel.DOAnchorPos(onScreenConstructionPanelPos, 0.2f).SetEase(Ease.OutCubic);
        }

        public void Hide()
        {
            constructionPanel.DOAnchorPos(offScreenConstructionPanelPos, 0.2f)
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
    }

}
