using UnityEngine;

namespace LUP.PCR
{
    public class MainUIModel
    {
        private PCRResourceCenter resourceCenter;

        public void InitModel(PCRResourceCenter resourceCenter)
        {
            this.resourceCenter = resourceCenter;
        }

        public PCRResourceCenter GetResourceCenter()
        {
            return resourceCenter;
        }
    }
}
