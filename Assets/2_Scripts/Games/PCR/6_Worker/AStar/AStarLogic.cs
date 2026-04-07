using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

namespace LUP.PCR
{
    struct PQNode
    {
        int x;
        int y;
        int index;
    }

    public class AStarLogic
    {
        int gridSize;
        bool[] isVisited;
        int[] bestScore;
        int[] cameFromIndex;

        struct CandidateNode
        {
            int tileIndex;
            int totalScore; 
        }

        public AStarLogic(int gridSize)
        {
            this.gridSize = gridSize;
            isVisited = new bool[gridSize * gridSize];
            bestScore = new int[gridSize * gridSize];
            cameFromIndex = new int[gridSize * gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                bestScore[i] = int.MaxValue;
            }
        }
        public int[] FindPath(int startY, int startX, int destY, int destX)
        {
            int[] path = new int[gridSize * gridSize];
            int pathLength = 0;
            int startIndex = startY * gridSize + startX;
            int destIndex = destY * gridSize + destX;

            int startG = 0;
            int startH = Math.Abs(destY - startY) + Math.Abs(destX - startX);
            int startF = startG + startH;

            return path;
        }

    }
}
