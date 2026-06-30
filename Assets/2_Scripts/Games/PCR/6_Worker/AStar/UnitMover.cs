using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    [RequireComponent(typeof(CharacterController))]
    public class UnitMover : MonoBehaviour
    {
        [SerializeField] float rotateSpeed = 10f;
        [SerializeField] float moveSpeed = 5f;

        private CharacterController characterController;
        private AGridMap gridMap;
        private APathfinding pathfinder;
        private List<ANode> path;

        private int currentIndex;
        public Vector3 CurrentDestination => currentDestination;
        private Vector3 currentDestination;


        private Vector3 currentInternalTarget;
        private Queue<Vector3> internalPathQueue = new Queue<Vector3>();

        public bool IsMoving => (path != null && currentIndex < path.Count) || isMovingInternally;
        private bool isMovingInternally = false;
        public bool IsClimbing => IsMoving && isClimbing;
        private bool isClimbing = false;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            gridMap = transform.root.GetComponentInChildren<AGridMap>();
            pathfinder = new APathfinding(gridMap);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TestPathfindingPerformance();
            }
        }

        public void SetInternalPath(StructureBase building)
        {
            internalPathQueue.Clear();

            if (building.localWaypoints != null && building.localWaypoints.Count > 0)
            {
                foreach (Vector3 localPoint in building.localWaypoints)
                {

                    Vector3 worldPoint = building.transform.TransformPoint(localPoint);
                    internalPathQueue.Enqueue(worldPoint);
                }
            }


            if (building.workSpotAnchor != null)
            {
                internalPathQueue.Enqueue(building.WorkSpotWorldPos);
            }


            if (internalPathQueue.Count > 0)
            {
                currentInternalTarget = internalPathQueue.Dequeue();
                isMovingInternally = true;
            }
        }
        public bool HasInternalPath()
        {

            return isMovingInternally || internalPathQueue.Count > 0;
        }
        public bool MoveInternal()
        {
            if (!isMovingInternally)
            {
                return true;
            }

            Vector3 targetDir = new Vector3(currentInternalTarget.x - transform.position.x, 0, currentInternalTarget.z - transform.position.z);

            if (targetDir.magnitude > 0.01f)
            {
                targetDir.Normalize();
            }
            else
            {
                targetDir = Vector3.zero;
            }


            if (characterController != null)
            {
                characterController.SimpleMove(targetDir * moveSpeed);
            }


            if (targetDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }


            Vector3 myPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetPosXZ = new Vector3(currentInternalTarget.x, 0, currentInternalTarget.z);


            if (Vector3.Distance(myPosXZ, targetPosXZ) < 0.3f)
            {
                if (internalPathQueue.Count > 0)
                {

                    currentInternalTarget = internalPathQueue.Dequeue();
                }
                else
                {

                    isMovingInternally = false;
                    return true;
                }
            }

            return false;
        }

        void FindPath(RaycastHit hit)
        {
            ANode startNode = gridMap.GetNodeFromWorldPosition(transform.position);
            ANode targetNode = gridMap.GetNodeFromWorldPosition(hit.point);
            path = pathfinder.FindPath(startNode, targetNode);

            gridMap.pathToDraw = path;
            currentIndex = 0;
        }

        public void SetDestination(Vector3 worldPos)
        {
            if (gridMap == null || pathfinder == null) return;

            ANode startNode = gridMap.GetNodeFromWorldPosition(transform.position);
            ANode targetNode = gridMap.GetNodeFromWorldPosition(worldPos);

            List<ANode> calculatedPath = pathfinder.FindPath(startNode, targetNode);


            if (calculatedPath == null || calculatedPath.Count == 0)
            {
                currentDestination = worldPos;
                path = null;
            }
            else
            {
                ProcessPath(calculatedPath);
            }
        }


        [ContextMenu("성능 테스트: 길찾기 1000번 실행")]
        public void TestPathfindingPerformance()
        {
            if (gridMap == null || pathfinder == null)
            {
                return;
            }

            List<ANode> walkableNodes = new List<ANode>();

            foreach (ANode node in gridMap.grid)
            {
                if (node.isWalkable)
                {
                    walkableNodes.Add(node);
                }
            }

            if (walkableNodes.Count < 2)
            {
                UnityEngine.Debug.LogWarning("맵에 걸을 수 있는 바닥이 충분하지 않습니다.");
                return;
            }

            ANode startNode = walkableNodes[0];
            ANode targetNode = walkableNodes[walkableNodes.Count - 1];

            UnityEngine.Profiling.Profiler.BeginSample("AStar_Heap_Performance");

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            for (int i = 0; i < 1000; i++)
            {
                pathfinder.FindPath(startNode, targetNode);
            }

            sw.Stop();

            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Debug.Log($"[길찾기 1000회 연산 완료] 걸린 시간: {sw.ElapsedMilliseconds} ms");
        }
        ANode GetStartNodeByPhysics()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2.0f))
            {
                return gridMap.GetNodeFromWorldPosition(hit.point);
            }


            return gridMap.GetNodeFromWorldPosition(transform.position);
        }

        public bool SetDestination(Vector2Int gridPos)
        {
            if (gridMap == null || pathfinder == null)
            {
                return false;
            }

            ANode startNode = GetStartNodeByPhysics();

            if (!startNode.isWalkable)
            {
                startNode = FindNearestWalkableNode(startNode);
            }

            ANode targetNode = gridMap.GetNodeFromGridPos(gridPos);

            if (targetNode == null || !targetNode.isWalkable)
            {
                return false;
            }

            gridMap.debugStartNode = startNode;
            gridMap.debugTargetNode = targetNode;

            if (targetNode == null || !targetNode.isWalkable)
            {


                return false;
            }

            List<ANode> calculatedPath = pathfinder.FindPath(startNode, targetNode);

            if (calculatedPath != null && calculatedPath.Count > 0)
            {

                ProcessPath(calculatedPath);
                return true;
            }
            else
            {
                return false;

            }

        }
        private ANode FindNearestWalkableNode(ANode centerNode)
        {
            if (centerNode.isWalkable)
            {
                return centerNode;
            }

            ANode nearest = null;
            float minDist = float.MaxValue;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    Vector2Int neighborPos = new Vector2Int(centerNode.indexX + x, centerNode.indexY + y);
                    ANode neighbor = gridMap.GetNodeFromGridPos(neighborPos);

                    if (neighbor != null && neighbor.isWalkable)
                    {
                        float dist = Vector3.Distance(transform.position, neighbor.worldPos);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearest = neighbor;
                        }
                    }
                }
            }
            return nearest;
        }
        private void ProcessPath(List<ANode> newPath)
        {
            path = newPath;
            currentIndex = 0;

            gridMap.pathToDraw = path;

            currentDestination = gridMap.GetNodeFootPosition(path[path.Count - 1]);
        }
        public void MoveAlongPath()
        {
            if (!IsMoving)
            {
                isClimbing = false;
                return;
            }

            ANode target = path[currentIndex];
            Vector3 targetPos = gridMap.GetNodeFootPosition(target);
            ANode prev = currentIndex > 0 ? path[currentIndex - 1] : null;
            bool isVerticalMove = (prev != null) && (prev.indexY != target.indexY);

            if (isVerticalMove)
            {

                if (Mathf.Abs(transform.position.x - targetPos.x) > 0.05f)
                {
                    bool wasOnLadder = (prev != null && prev.isLadder);
                    isClimbing = wasOnLadder;

                    Vector3 alignX = new Vector3(targetPos.x, transform.position.y, transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, alignX, moveSpeed * Time.deltaTime);
                }
                else
                {
                    isClimbing = true;

                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                }
            }
            else
            {
                isClimbing = false;
                Vector3 targetDir = new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z);

                if (targetDir.magnitude > 0.01f)
                {
                    targetDir.Normalize();
                }
                else
                {
                    targetDir = Vector3.zero;
                }

                characterController.SimpleMove(targetDir * moveSpeed);

                if (Mathf.Abs(transform.position.z - targetPos.z) > 0.05f)
                {
                    float fixZ = Mathf.MoveTowards(transform.position.z, targetPos.z, moveSpeed * Time.deltaTime);
                    transform.position = new Vector3(transform.position.x, transform.position.y, fixZ);
                }


            }

            if (IsClimbing)
            {

                Quaternion climbRotation = Quaternion.Euler(0, 0, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, climbRotation, rotateSpeed * Time.deltaTime);
            }
            else
            {


                Vector3 direction = (targetPos - transform.position);
                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

                }
            }

            if (isVerticalMove || IsClimbing)
            {
                if (Vector3.Distance(transform.position, targetPos) < 0.15f)
                {
                    currentIndex++;
                }
            }
            else
            {
                Vector3 myPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 targetPosXZ = new Vector3(targetPos.x, 0, targetPos.z);

                if (Vector3.Distance(myPosXZ, targetPosXZ) < 0.15f)
                {
                    currentIndex++;
                }
            }
        }


        public bool IsArrived()
        {
            if (path == null || currentIndex >= path.Count)
            {
                Vector3 myPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 destXZ = new Vector3(currentDestination.x, 0, currentDestination.z);

                if (Vector3.Distance(myPosXZ, destXZ) < 0.3f)
                {
                    return true;
                }
            }
            return false;
        }
        public void Stop()
        {
            path = null;
            currentIndex = 0;
            gridMap.pathToDraw = null;

            internalPathQueue.Clear();
            isMovingInternally = false;
            currentInternalTarget = Vector3.zero;

            isClimbing = false;
        }
    }
}
