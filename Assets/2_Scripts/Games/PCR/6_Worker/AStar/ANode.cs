using UnityEngine;

namespace LUP.PCR
{
    public class ANode : IHeapItem<ANode>
    {
        public bool isWalkable;
        public bool isLadder;
        public Vector3 worldPos;
        public int indexX;
        public int indexY;

        public int gCost;
        public int hCost;
        public ANode parentNode;

        public int lastVisitedFrame;
        private int heapIndex;
        
        public int HeapIndex
        {
            get { return heapIndex; }
            set { heapIndex = value; }
        }
        public int CompareTo(ANode nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }

        public ANode(bool nWalkable, bool nLadder, Vector3 nWorldPos, int nIndexX, int nIndexY)
        {
            isWalkable = nWalkable;
            isLadder = nLadder;
            worldPos = nWorldPos;
            indexX = nIndexX;
            indexY = nIndexY;

            lastVisitedFrame = 0;
        }

        public int FCost
        {
            get { return gCost + hCost; }
        }
        
    }
}