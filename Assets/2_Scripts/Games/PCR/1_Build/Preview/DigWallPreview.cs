using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    public class DigWallPreview : MonoBehaviour
    {
        private HashSet<Tile> CanDigTile;
        private HashSet<Tile> CanNotDigTile;

        private void Awake()
        {
            CanDigTile = new HashSet<Tile>();
            CanNotDigTile = new HashSet<Tile>();
        }

        public void UpdateAllDigWallPreview(TileMap tileMap)
        {
            foreach (Tile tile in tileMap.tiles)
            {

                if (tile.tileInfo.tileType == TileType.WALL)
                {
                    AddCanDigTile(tile);
                }
                else
                {
                    AddCanNotDigTile(tile);
                }
            }

            Debug.Log("DigWallPreview Init");

        }

        public void Show()
        {

            foreach (Tile tile in CanDigTile)
            {
                tile.ShowCanDigWallMark();
            }

            foreach (Tile tile in CanNotDigTile)
            {
                tile.ShowCanNotDigWallMark();
            }
        }

        public void Hide()
        {
            foreach (Tile tile in CanDigTile)
            {
                tile.HideCanDigWallMark();
            }

            foreach (Tile tile in CanNotDigTile)
            {
                tile.HideCanNotDigWallMark();
            }
        }

        public void AddCanDigTile(Tile tile)
        {
            if (!CanDigTile.Contains(tile))
            {
                CanDigTile.Add(tile);
            }
        }

        public void RemoveCanDigTile(Tile tile)
        {
            if (CanDigTile.Contains(tile))
            {
                CanDigTile.Remove(tile);
            }
        }
        public void AddCanNotDigTile(Tile tile)
        {
            if (!CanNotDigTile.Contains(tile))
            {
                CanNotDigTile.Add(tile);
            }
        }

        public void RemoveCanNotDigTile(Tile tile)
        {
            if (CanNotDigTile.Contains(tile))
            {
                CanNotDigTile.Remove(tile);
            }
        }
    }

}
