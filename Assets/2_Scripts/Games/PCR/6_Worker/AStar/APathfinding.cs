using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    public enum LadderState
    {
        None, Bottom, Middle, Top, Single
    }
    public class APathfinding
    {
        private static readonly int[,] dirs = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
        AGridMap gridMap;
        Heap<ANode> openList;
        HashSet<ANode> closedSet;
        List<ANode> cachedPath;
        List<ANode> cachedNeighbors;

        private int currentFrameCounter = 0;

        public APathfinding(AGridMap map)
        {
            gridMap = map;

            openList = new Heap<ANode>(gridMap.grid.Length);
            closedSet = new HashSet<ANode>();
            cachedPath = new List<ANode>();
            cachedNeighbors = new List<ANode>(4);
        }
        int GetDistance(ANode a, ANode b)
        {
            return 10 * (Mathf.Abs(a.indexX - b.indexX) + Mathf.Abs(a.indexY - b.indexY));
        }

        public List<ANode> FindPath(ANode startNode, ANode targetNode)
        {
            currentFrameCounter++;

            if (currentFrameCounter == int.MaxValue)
            {
                currentFrameCounter = 1;
                foreach (ANode node in gridMap.grid)
                {
                    node.lastVisitedFrame = 0;
                }
            }

            openList.Clear();
            closedSet.Clear();

            startNode.lastVisitedFrame = currentFrameCounter;
            startNode.gCost = 0;
            startNode.parentNode = null;

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                ANode current = openList.RemoveFirst();
                closedSet.Add(current);

                if (current == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                GetNeighbors(current, cachedNeighbors);

                foreach (ANode neighbor in cachedNeighbors)
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    if(neighbor.lastVisitedFrame != currentFrameCounter)
                    {
                        neighbor.gCost = int.MaxValue;
                        neighbor.parentNode = null;
                        neighbor.lastVisitedFrame = currentFrameCounter;
                    }

                    int newCost = current.gCost + GetDistance(current, neighbor);

                    if (newCost < neighbor.gCost || !openList.Contains(neighbor))
                    {
                        neighbor.gCost = newCost;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parentNode = current;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                        else
                        {
                            openList.UpdateItem(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        List<ANode> RetracePath(ANode start, ANode end)
        {
            cachedPath.Clear();
            ANode current = end;

            while (current != start)
            {
                cachedPath.Add(current);
                current = current.parentNode;
            }

            cachedPath.Reverse();
            return cachedPath;
        }

        void GetNeighbors(ANode node, List<ANode> neighbors)
        {
            neighbors.Clear();

            LadderState state = GetLadderState(node);

            for (int i = 0; i < 4; i++)
            {
                int nx = node.indexX + dirs[i, 0];
                int ny = node.indexY + dirs[i, 1];

                if (!gridMap.IsIdxValid(nx, ny)) continue;

                ANode neighbor = gridMap.grid[nx, ny];
                if (!neighbor.isWalkable) continue;

                int dy = dirs[i, 1];
                if (dy == 0 && CanMoveSide(state))
                {
                    neighbors.Add(neighbor);
                }
                else if (dy == 1 && CanMoveUp(node, neighbor, state))
                {
                    neighbors.Add(neighbor);
                }
                else if (dy == -1 && CanMoveDown(node, neighbor, state))
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        bool HasLadderAt(int x, int y)
        {
            if (!gridMap.IsIdxValid(x, y)) return false;
            return gridMap.grid[x, y].isLadder;
        }

        LadderState GetLadderState(ANode node)
        {
            if (!node.isLadder)
            {
                return LadderState.None;
            }

            bool up = HasLadderAt(node.indexX, node.indexY + 1);
            bool down = HasLadderAt(node.indexX, node.indexY - 1);

            if (!up && !down)
            {
                return LadderState.Single;
            }
            if (!up && down)
            {
                return LadderState.Top;
            }
            if (up && !down)
            {
                return LadderState.Bottom;
            }

            return LadderState.Middle;
        }

        bool CanMoveSide(LadderState state)
        {
            return state != LadderState.Middle;
        }

        bool CanMoveUp(ANode current, ANode neighbor, LadderState state)
        {
            if (neighbor.isLadder) return true;
            return state == LadderState.Top || state == LadderState.Single;
        }

        bool CanMoveDown(ANode current, ANode neighbor, LadderState state)
        {
            if (neighbor.isLadder) return true;
            return state == LadderState.Bottom || state == LadderState.Single;
        }
    }
}