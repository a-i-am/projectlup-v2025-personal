using R3;
using UnityEngine;

namespace LUP.PCR
{
    public sealed class FarmTaskUIViewModel
    {
        public ReadOnlyReactiveProperty<int> Level { get; private set; }
        public ReadOnlyReactiveProperty<string> BuildingName { get; private set; }
        public ReadOnlyReactiveProperty<float> ProductionPerHour { get; private set; }
        public ReadOnlyReactiveProperty<int> CurrentStorage { get; private set; }
        public ReadOnlyReactiveProperty<int> MaxStorage { get; private set; }
        public ReadOnlyReactiveProperty<bool> IsWorkRequested { get; private set; }
        public ReadOnlyReactiveProperty<bool> IsConstructing { get; private set; }

        public Subject<Unit> OnClickBack { get; } = new();
        public Subject<Unit> OnClickWorkRequest { get; } = new();
        public Subject<Unit> OnClickUpgrade { get; } = new();
        public Subject<FarmUIBtnType> OnTabChanged { get; } = new();

        public FarmTaskUIViewModel()
        {
            Level = new ReactiveProperty<int>(0);
            BuildingName = new ReactiveProperty<string>("");
            ProductionPerHour = new ReactiveProperty<float>(0f);
            CurrentStorage = new ReactiveProperty<int>(0);
            MaxStorage = new ReactiveProperty<int>(0);
            IsWorkRequested = new ReactiveProperty<bool>(false);
            IsConstructing = new ReactiveProperty<bool>(false);
        }

        public void Observe(ProductableBuilding productableBuilding)
        {
            Level = productableBuilding.level.ToReadOnlyReactiveProperty();
            BuildingName = productableBuilding.buildingName.ToReadOnlyReactiveProperty();
            ProductionPerHour = productableBuilding.productionPerHour.ToReadOnlyReactiveProperty();
            CurrentStorage = productableBuilding.currentStorage.ToReadOnlyReactiveProperty();
            MaxStorage = productableBuilding.maxStorage.ToReadOnlyReactiveProperty();
            IsWorkRequested = productableBuilding.isWorkRequested.ToReadOnlyReactiveProperty();
            IsConstructing = productableBuilding.isConstructing.ToReadOnlyReactiveProperty();
        }
    }
}
