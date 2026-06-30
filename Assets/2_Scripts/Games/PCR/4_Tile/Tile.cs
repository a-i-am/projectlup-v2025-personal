using UnityEngine;

namespace LUP.PCR
{
    public class Tile : MonoBehaviour
    {
        public TileInfo tileInfo;

        [SerializeField]
        private GameObject[] tileVisualObjects;
        [SerializeField]
        private GameObject floorVisual;
        [SerializeField]
        private GameObject canActMark;
        [SerializeField]
        private GameObject canNotActMark;
        [SerializeField]
        private GameObject darkVisionMark;

        private void Start()
        {
            HideCanDigWallMark();
            HideCanNotDigWallMark();
        }

        public void SetTileInfo(TileInfo tileInfo)
        {
            this.tileInfo = tileInfo;
        }

        public void UpdateVisualState(bool isUpperFloor = false)
        {
            if (tileInfo.tileType == TileType.PATH || tileInfo.tileType == TileType.NONE)
            {
                foreach (GameObject obj in tileVisualObjects)
                {
                    if (obj)
                    {
                        obj.SetActive(true);
                    }
                }
            }
            else if (tileInfo.tileType == TileType.WALL)
            {
                foreach (GameObject obj in tileVisualObjects)
                {
                    if (obj)
                    {
                        obj.SetActive(false);
                    }
                }
            }
            else if (tileInfo.tileType == TileType.BUILDING || tileInfo.tileType == TileType.LADDER)
            {
                foreach (GameObject obj in tileVisualObjects)
                {
                    if (obj)
                    {
                        obj.SetActive(false);
                    }
                }



                if (isUpperFloor == false && floorVisual != null)
                {
                    floorVisual.SetActive(true);
                }
            }
        }

        public void ShowCanDigWallMark()
        {
            if (canActMark)
            {
                canActMark.SetActive(true);
            }
        }
        public void HideCanDigWallMark()
        {
            if (canActMark)
            {
                canActMark.SetActive(false);
            }
        }
        public void ShowCanNotDigWallMark()
        {
            if (canNotActMark)
            {
                canNotActMark.SetActive(true);
            }
        }
        public void HideCanNotDigWallMark()
        {
            if (canNotActMark)
            {
                canNotActMark.SetActive(false);
            }
        }

        public void ShowDarkVisionMark()
        {
            if (darkVisionMark)
            {
                darkVisionMark.SetActive(true);
            }
        }
        public void HideDarkVisionMark()
        {
            if (darkVisionMark)
            {
                darkVisionMark.SetActive(false);
            }
        }
    }


}
