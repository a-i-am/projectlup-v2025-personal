using NUnit.Framework.Constraints;
using R3;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

namespace LUP.PCR
{
    public abstract class ProductableBuilding : BuildingBase
    {

        public PCRProductionStaticData currentProductionData;

        protected ProductionInfo productionInfo;

        public ReactiveProperty<int> level = new ReactiveProperty<int>(0);
        public ReactiveProperty<float> productionPerHour = new ReactiveProperty<float>(0f);
        public ReactiveProperty<int> currentStorage = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> maxStorage = new ReactiveProperty<int>(0);
        public ReactiveProperty<bool> isWorkRequested = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> isConstructing = new ReactiveProperty<bool>(false);

        protected IBuildState constructState;
        protected IBuildState productableState;


        public abstract void SetupProductionData();

        public abstract void StartProduction();

        public abstract void StopProduction();

        public abstract void CompleteProduction();

        public ProductionInfo GetProductionInfo()
        {
            return productionInfo;
        }

        public void SetProductionInfo(ProductionInfo info)
        {
            productionInfo = info;
        }
    }

}
