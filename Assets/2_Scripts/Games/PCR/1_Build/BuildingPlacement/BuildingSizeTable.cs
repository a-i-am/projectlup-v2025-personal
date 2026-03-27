using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    public static class BuildingSizeTable
    {
        private static readonly Dictionary<BuildingType, Vector2Int> sizes = new Dictionary<BuildingType, Vector2Int>
        {
            { BuildingType.WHEATFARM,    new Vector2Int(4, 1) },
            { BuildingType.MUSHROOMFARM, new Vector2Int(4, 1) },
            { BuildingType.MOLEFARM,     new Vector2Int(4, 1) },
            { BuildingType.RESTAURANT,   new Vector2Int(4, 1) },
            { BuildingType.POWERSTATION, new Vector2Int(3, 1) },
            { BuildingType.STONEMINE,    new Vector2Int(2, 1) },
            { BuildingType.IRONMINE,     new Vector2Int(2, 1) },
            { BuildingType.COALMINE,     new Vector2Int(2, 1) },
            { BuildingType.LADDER,       new Vector2Int(1, 1) },
            { BuildingType.WORKSTATION,  new Vector2Int(4, 2) },
        };

        public static Vector2Int Get(BuildingType type)
        {
            return sizes.TryGetValue(type, out Vector2Int size) ? size : Vector2Int.one;
        }
    }
}
