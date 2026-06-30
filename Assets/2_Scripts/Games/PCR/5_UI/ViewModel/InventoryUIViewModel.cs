using R3;
using System;
using UnityEngine;

namespace LUP.PCR
{
    public sealed class InventoryUIViewModel
    {
        public ReadOnlyReactiveProperty<int> Stone { get; }
        public ReadOnlyReactiveProperty<int> Coal { get; }
        public ReadOnlyReactiveProperty<int> Iron { get; }
        public ReadOnlyReactiveProperty<int> Wheat { get; }
        public ReadOnlyReactiveProperty<int> Mushroom { get; }
        public ReadOnlyReactiveProperty<int> Meat { get; }
        public ReadOnlyReactiveProperty<int> Food { get; }
        public ReadOnlyReactiveProperty<int> Power { get; }
        public ReadOnlyReactiveProperty<int> Diamond { get; }

        public Subject<Unit> ClickBack { get; } = new();

        public InventoryUIViewModel(PCRResourceCenter rc)
        {
            Stone = rc.Observe(ResourceType.Stone);
            Coal = rc.Observe(ResourceType.Coal);
            Iron = rc.Observe(ResourceType.Iron);
            Wheat = rc.Observe(ResourceType.Wheat);
            Mushroom = rc.Observe(ResourceType.Mushroom);
            Meat = rc.Observe(ResourceType.Meat);
            Food = rc.Observe(ResourceType.Food);
            Power = rc.Observe(ResourceType.Power);
            Diamond = rc.Observe(ResourceType.Diamond);
        }

    }
}
