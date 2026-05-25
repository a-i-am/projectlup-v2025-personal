using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

namespace LUP.PCR
{
    public class AStarLogic
    {
        int gridSize;
        bool[] isVisited;
        int[] bestScore;
        int[] cameFromIndex;

        struct CandidateNode
        {
            public int tileIndex;
            public int finalCost;
            public int groundCost;
        }

        public AStarLogic(int gridSize)
        {
            this.gridSize = gridSize;
            isVisited = new bool[gridSize * gridSize];
            bestScore = new int[gridSize * gridSize];
            cameFromIndex = new int[gridSize * gridSize];

            for (int i = 0; i < gridSize * gridSize; i++)
            {
                bestScore[i] = int.MaxValue;
            }
        }
        public int[] FindPath(int startY, int startX, int destY, int destX)
        {
            for (int i = 0; i < gridSize * gridSize; i++)
            {
                isVisited[i] = false;
                bestScore[i] = int.MaxValue;
                cameFromIndex[i] = 0;
            }

            int[] path = new int[gridSize * gridSize];
            int pathLength = 0;

            int startIndex = startY * gridSize + startX;
            int destIndex = destY * gridSize + destX;

            int startG = 0;
            int startH = Math.Abs(destY - startY) + Math.Abs(destX - startX);
            int startF = startG + startH;

            CandidateNode[] candidateBinaryHeap = new CandidateNode[gridSize * gridSize];
            int heapSize = 0;

            HeapPush(candidateBinaryHeap, ref heapSize, new CandidateNode
            {
                tileIndex = startIndex,
                finalCost = startF,
                groundCost = startG
            });

            int[] deltaY = new int[] { -1, 0, 1, 0 };
            int[] deltaX = new int[] { 0, -1, 0, 1 };

            while (heapSize > 0)
            {
                CandidateNode current = HeapPop(candidateBinaryHeap, ref heapSize);

                if (isVisited[current.tileIndex])
                {
                    continue;
                }

                isVisited[current.tileIndex] = true;

                if (current.tileIndex == destIndex)
                {
                    break;
                }

                int currentY = current.tileIndex / gridSize;
                int currentX = current.tileIndex % gridSize;

                for (int i = 0; i < 4; i++)
                {
                    int nextY = currentY + deltaY[i];
                    int nextX = currentX + deltaX[i];

                    if (nextX >= gridSize || nextY >= gridSize || nextX < 0 || nextY < 0)
                    {
                        continue;
                    }

                    int nextIndex = nextY * gridSize + nextX;
                    if (isVisited[nextIndex])
                    {
                        continue;
                    }

                    int nextG = current.groundCost + 1;
                    int nextH = Math.Abs(destY - nextY) + Math.Abs(destX - nextX);
                    int nextF = nextG + nextH;

                    if (bestScore[nextIndex] <= nextF)
                    {
                        continue;
                    }

                    bestScore[nextIndex] = nextF;
                    cameFromIndex[nextIndex] = current.tileIndex;
                    HeapPush(candidateBinaryHeap, ref heapSize, new CandidateNode
                    {
                        tileIndex = nextIndex,
                        finalCost = nextF,
                        groundCost = nextG
                    });
                }
            }

            int traceCurrent = destIndex;
            while (traceCurrent != startIndex)
            {
                path[pathLength] = traceCurrent;
                pathLength++;
                traceCurrent = cameFromIndex[traceCurrent];
            }

            path[pathLength] = startIndex;
            pathLength++;

            int left = 0;
            int right = pathLength - 1;
            while (left < right)
            {
                int temp = path[left];
                path[left] = path[right];
                path[right] = temp;
                left++;
                right--;
            }
            return path;
        }

        void HeapPush(CandidateNode[] heap, ref int heapSize, CandidateNode newNode)
        {
            heap[heapSize] = newNode;
            int currentIndex = heapSize;

            while (currentIndex > 0)
            {
                int parentIndex = (currentIndex - 1) / 2;

                if (heap[currentIndex].finalCost < heap[parentIndex].finalCost)
                {
                    CandidateNode temp = heap[currentIndex];
                    heap[currentIndex] = heap[parentIndex];
                    heap[parentIndex] = temp;
                    currentIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }

            heapSize++;
        }

        CandidateNode HeapPop(CandidateNode[] heap, ref int heapSize)
        {
            CandidateNode result = heap[0];
            heap[0] = heap[heapSize - 1];
            heapSize--;

            int currentIndex = 0;
            while (true)
            {
                int leftChild = (currentIndex * 2 + 1);
                int rightChild = (currentIndex * 2 + 2);

                if (leftChild >= heapSize)
                {
                    break;
                }

                int minChild = rightChild >= heapSize ||
                               heap[leftChild].finalCost <= heap[rightChild].finalCost
                               ? leftChild : rightChild;

                if (heap[currentIndex].finalCost > heap[minChild].finalCost)
                {
                    CandidateNode temp = heap[currentIndex];
                    heap[currentIndex] = heap[minChild];
                    heap[minChild] = temp;
                    currentIndex = minChild;
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
}