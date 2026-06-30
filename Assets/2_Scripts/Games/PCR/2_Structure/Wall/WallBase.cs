using UnityEngine;

namespace LUP.PCR
{
    public abstract class WallBase : StructureBase
    {
        protected WallInfo wallInfo;

        public WallInfo GetWallInfo()
        {
            return wallInfo;
        }

        public void SetWallInfo(WallInfo wallInfo)
        {
            this.wallInfo = wallInfo;
        }

        public bool CanDig(int digPower)
        {
            if (wallInfo != null)
            {
                return wallInfo.wallType <= digPower;
            }

            return true;
        }
    }

}
