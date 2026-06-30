using R3;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LUP.PCR
{
    public enum FarmUIBtnType
    {
        Product,
        Upgrade,
    }
    public class FarmTaskUIView : MonoBehaviour
    {
        [Header("탭")]
        [SerializeField] private Button productionTab;
        [SerializeField] private Button upgradeTab;
        [SerializeField] private Button backBtn;

        [Header("패널")]
        [SerializeField] private GameObject productionPanel;
        [SerializeField] private GameObject upgradePanel;

        [Header("실행 버튼")]
        [SerializeField] private Button btnProductionToggle;
        [SerializeField] private Button btnUpgrade;
        [SerializeField] Text productionToggleText;

        [Header("건물정보 텍스트")]
        [SerializeField] Text buildingNameText;

        [Header("업그레이드 패널 UI")]
        [SerializeField] private Text levelChangeText;

        [Header("업그레이드 효과")]
        [SerializeField] private Text effectNameText;
        [SerializeField] private Text effectValueText;

        [Header("필요 자원")]
        [SerializeField] private GameObject costSlot1;
        [SerializeField] private Image costIcon1;
        [SerializeField] private Text costText1;

        [SerializeField] private GameObject costSlot2;
        [SerializeField] private Image costIcon2;
        [SerializeField] private Text costText2;

        private readonly CompositeDisposable cd = new();

        private void OnDestroy()
        {
            cd.Dispose();
        }

        public void Bind(FarmTaskUIViewModel vm)
        {
            btnProductionToggle.onClick.AddListener(() => vm.OnClickWorkRequest?.OnNext(Unit.Default));
            btnUpgrade.onClick.AddListener(() => vm.OnClickUpgrade?.OnNext(Unit.Default));
            backBtn?.onClick.AddListener(() => vm.OnClickBack?.OnNext(Unit.Default));

            productionTab?.onClick.AddListener(() => vm.OnTabChanged?.OnNext(FarmUIBtnType.Product));
            upgradeTab?.onClick.AddListener(() => vm.OnTabChanged?.OnNext(FarmUIBtnType.Upgrade));

            vm.OnTabChanged?.Subscribe(ChangeOptionBtn).AddTo(cd);

            vm.Level.DistinctUntilChanged().Subscribe(value =>
            {
                levelChangeText.text = $"Lv.{value} >> Lv.{value + 1}";
            }).AddTo(cd);
            vm.BuildingName.DistinctUntilChanged().Subscribe(value =>
            {
                buildingNameText.text = value;
            }).AddTo(cd);
            vm.IsWorkRequested.DistinctUntilChanged().Subscribe(value =>
            {
                if (value)
                {
                    productionToggleText.text = "요청 취소";
                    btnProductionToggle.image.color = Color.gray;
                }
                else
                {
                    productionToggleText.text = "작업 요청";
                    btnProductionToggle.image.color = Color.white;
                }
            }).AddTo(cd);
            vm.IsConstructing.DistinctUntilChanged().Subscribe(value =>
            {
                if (value)
                {
                    upgradeTab.interactable = false;
                    if (upgradePanel.activeSelf)
                    {
                        ChangeOptionBtn(FarmUIBtnType.Product);
                    }
                }
                else
                {
                    upgradeTab.interactable = true;
                }
            }).AddTo(cd);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ChangeOptionBtn(FarmUIBtnType.Product);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void ChangeOptionBtn(FarmUIBtnType type)
        {
            productionTab.image.color = new Color(1f, 1f, 1f, 0f);
            upgradeTab.image.color = new Color(1f, 1f, 1f, 0f);

            switch (type)
            {
                case FarmUIBtnType.Product:
                    productionTab.image.color = new Color(1f, 1f, 1f, 1f);
                    upgradePanel.SetActive(false);
                    productionPanel.SetActive(true);
                    break;
                case FarmUIBtnType.Upgrade:
                    upgradeTab.image.color = new Color(1f, 1f, 1f, 1f);
                    productionPanel.SetActive(false);
                    upgradePanel.SetActive(true);
                    break;
            }
        }


        public void UpdateUIStats(FarmUIData data)
        {
            buildingNameText.text = data.buildingName;

           if (data.isWorkRequested)
           {
               productionToggleText.text = "요청 취소";
               btnProductionToggle.image.color = Color.gray;
           }
           else
           {
               productionToggleText.text = "작업 요청";
               btnProductionToggle.image.color = Color.white;
           }

            if (data.isConstructing)
            {
                upgradeTab.interactable = false;

                if (upgradePanel.activeSelf) ChangeOptionBtn(FarmUIBtnType.Product);
            }
            else
            {
                upgradeTab.interactable = true;
            }

            UpdateUpgradePanel(data);
        }
        private void UpdateUpgradePanel(FarmUIData data)
        {

            if (data.isMaxLevel)
            {
                levelChangeText.text = "Max Level";
                effectNameText.text = "";
                effectValueText.text = "";
                costSlot1.SetActive(false);
                costSlot2.SetActive(false);
                btnUpgrade.interactable = false;
                return;
            }


            levelChangeText.text = $"Lv.{data.level} >> Lv.{data.level + 1}";


            effectNameText.text = data.effectName;
            effectValueText.text = $"{data.currentStatValue} <color=green>+{data.nextStatAddedValue}</color>";


            if (data.costAmount1 > 0)
            {
                costSlot1.SetActive(true);
                costText1.text = data.costAmount1.ToString();

            }
            else costSlot1.SetActive(false);


            if (data.costAmount2 > 0)
            {
                costSlot2.SetActive(true);
                costText2.text = data.costAmount2.ToString();

            }
            else costSlot2.SetActive(false);


            btnUpgrade.interactable = !data.isConstructing;
        }
        public void ChangeTab(FarmUIBtnType type)
        {
            ChangeOptionBtn(type);
        }
    }


}
