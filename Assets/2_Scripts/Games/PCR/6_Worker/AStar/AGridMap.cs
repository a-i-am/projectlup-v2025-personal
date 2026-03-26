using UnityEngine;
using System.Collections.Generic;

namespace LUP.PCR
{
    public class AGridMap : MonoBehaviour
    {
        [Header("Map Settings")]
        [SerializeField] float tileSize = 5;

        public ANode[,] grid;
        public Tile[,] sourceTiles;

        Vector3 gridStartPoint;
        [HideInInspector] public List<ANode> pathToDraw;
        [HideInInspector] public ANode debugStartNode;
        [HideInInspector] public ANode debugTargetNode;

        public bool IsIdxValid(int x, int y)
        {
            return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
        }

        public void InitMap(Tile[,] tileData)
        {
            gridStartPoint = transform.position;
            sourceTiles = tileData;
            CreateGridFromData();
        }

        void CreateGridFromData()
        {
            if (sourceTiles == null) { return; }

            int width = sourceTiles.GetLength(0);
            int height = sourceTiles.GetLength(1);

            grid = new ANode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPosition = GridToWorldPosition(new Vector2Int(x, y));

                    TileType currentType = sourceTiles[x, y].tileInfo.tileType;

                    bool walkable = currentType != TileType.WALL;
                    bool ladder = currentType == TileType.LADDER;

                    grid[x, y] = new ANode(walkable, ladder, worldPosition, x, y);
                }
            }

        }

        // Vector2Int(데이터좌표) -> Vector3(월드 좌표) 변환 
        public Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            float xPos = gridPos.x * tileSize + tileSize / 2f;
            float yPos = -(gridPos.y * tileSize + tileSize / 2f);

            return gridStartPoint + new Vector3(xPos, yPos, -2.5f);
        }

        public ANode GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            if (grid == null) 
            {
                return null; 
            }

            int x = Mathf.FloorToInt((worldPosition.x - gridStartPoint.x) / tileSize);
            int y = Mathf.FloorToInt(-(worldPosition.y - gridStartPoint.y) / tileSize);

            x = Mathf.Clamp(x, 0, grid.GetLength(0) - 1);
            y = Mathf.Clamp(y, 0, grid.GetLength(1) - 1);

            return grid[x, y];
        }

        public ANode GetNodeFromGridPos(Vector2Int pos)
        {
            if (grid == null) 
            {
                return null; 
            }

            if (pos.x >= 0 && pos.y >= 0 && pos.x < grid.GetLength(0) && pos.y < grid.GetLength(1))
            {

                return grid[pos.x, pos.y];
            }

            return null;
        }

        public Vector3 GetNodeFootPosition(ANode node)
        {
            Vector3 centerPos = node.worldPos;

            return new Vector3(centerPos.x, centerPos.y - (tileSize / 2f), centerPos.z);
        }
        private void OnDrawGizmos()
        {
            if (grid == null)
            {
                return;
            }

            foreach (ANode node in grid)
            {
                if (!node.isWalkable)
                {
                    // 벽
                    Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
                }
                else if (node.isLadder)
                {
                    // 사다리
                    Gizmos.color = new Color(0f, 0f, 1f, 0.4f);
                }
                else
                {
                    // 일반 바닥
                    Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
                }

                Gizmos.DrawCube(
                    node.worldPos,
                    Vector3.one * (tileSize * 0.9f)
                );
            }

            // A* 경로
            if (pathToDraw != null && pathToDraw.Count > 0)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < pathToDraw.Count - 1; i++)
                {
                    Gizmos.DrawLine(
                        pathToDraw[i].worldPos,
                        pathToDraw[i + 1].worldPos
                    );
                }
            }

            // 시작 노드
            if (debugStartNode != null)
            {
                Gizmos.color = new Color(0f, 0.5f, 1f, 0.6f);
                Gizmos.DrawSphere(debugStartNode.worldPos, tileSize * 0.4f);
            }

            // 목표 노드
            if (debugTargetNode != null)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
                Gizmos.DrawSphere(debugTargetNode.worldPos, tileSize * 0.4f);
                Gizmos.DrawLine(
                    debugTargetNode.worldPos,
                    debugTargetNode.worldPos + Vector3.up * tileSize
                );
            }
        }

        [ContextMenu("Print Walkable Nodes")]
        public void PrintWalkableNodes()
        {
            if (grid == null)
            {
                Debug.LogWarning("그리드가 아직 생성되지 않았습니다. 게임 실행(Play) 후에 눌러보세요.");
                return;
            }

            string result = "--- 갈 수 있는 좌표 목록 (Walkable Nodes) ---\n";
            int count = 0;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y].isWalkable)
                    {
                        result += $"[{x}, {y}] ";
                        count++;

                        // 보기 좋게 10개씩 줄바꿈
                        if (count % 10 == 0) result += "\n";
                    }
                }
            }
            Debug.Log($"{result}\n------------------------------------------\n총 {count}개의 이동 가능 타일 발견");
        }


    }

}
