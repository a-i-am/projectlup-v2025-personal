using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    public class WorkerSystem : MonoBehaviour
    {
        public static WorkerSystem Instance { get; private set; }

        [Header("Worker Settings")]
        [SerializeField] private GameObject workerLogicPrefab;
        [SerializeField] private List<GameObject> workerModelPrefabs = new List<GameObject>();
        [SerializeField] private Transform workerContainer;

        private BuildingBase defaultRestaurant;
        private BuildingBase defaultStation;
        private int maxWorkerCount = 50;
        private bool isInitialized = false;

        private TileMap tileMap;
        private AGridMap aGrid;


        private ProductionRuntimeData pcrRuntimeData;
        private List<int> curReservedBuildingIdList;
        private List<int> curAssignedBuildingIdList;

        private Dictionary<int, BuildingBase> curBuildings;
        private List<WorkerInfo> curWorkerInfoList;


        private List<BuildingBase> taskBuildingList = new List<BuildingBase>();
        private List<WorkerAI> activeWorkers;
        private Queue<StructureBase> taskQueue = new Queue<StructureBase>(GridSize.x * GridSize.y);

        public void InitWorkerSystem(BuildingSystem buildingSystem, TileMap tileMap)
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            aGrid = GetComponentInChildren<AGridMap>();
            this.tileMap = tileMap;
            aGrid.InitMap(tileMap.tiles);

            curBuildings = buildingSystem.GetCurrentBuildingDictionary();

            ProductionStage stage = StageManager.Instance.GetCurrentStage() as ProductionStage;
            pcrRuntimeData = stage.productionRuntimeData;

            curReservedBuildingIdList = pcrRuntimeData.ReservedBuildingIdList;
            curAssignedBuildingIdList = pcrRuntimeData.AssignedBuildingIdList;

            curWorkerInfoList = pcrRuntimeData.WorkerInfoList;


            curWorkerInfoList.Clear();
            activeWorkers = new List<WorkerAI>(maxWorkerCount);

            InitDefaults();

            AddWorkPlaces();

            TestDebuging();
            isInitialized = true;
        }
        private void InitDefaults()
        {

            if (curBuildings.TryGetValue(1, out BuildingBase b1) && b1 is BuildingRestaurant)
            {
                defaultRestaurant = b1;
            }

            if (curBuildings.TryGetValue(2, out BuildingBase b2) && b2 is BuildingWorkStation)
            {
                defaultStation = b2;
            }

            if (defaultRestaurant == null)
            {
                foreach (var building in curBuildings.Values)
                {
                    if (building is BuildingWorkStation)
                    {
                        defaultStation = building;
                        break;
                    }
                }
            }
            if (defaultStation == null)
            {
                foreach (var building in curBuildings.Values)
                {
                    if (building is BuildingWorkStation)
                    {
                        defaultStation = building;
                        break;
                    }
                }
            }






            int defaultWorkerCount = 5;
            if (curWorkerInfoList.Count == 0)
            {
                for (int i = 0; i < defaultWorkerCount; i++)
                {
                    WorkerInfo testInfo = new WorkerInfo();
                    testInfo.id = i;
                    testInfo.name = $"DefaultWorker_{i}";

                    curWorkerInfoList.Add(testInfo);
                    CreateWorkerObject(testInfo);
                }
            }
        }
        private void AddWorkPlaces()
        {
            foreach ((int id, BuildingBase building) in curBuildings)
            {
                if (building == defaultRestaurant || building == defaultStation) continue;

                taskBuildingList.Add(building);
            }


        }

        private void IgnoreCollisionWithOtherWorkers(CharacterController newWorkerCC)
        {
            foreach (WorkerAI existingWorker in activeWorkers)
            {
                CharacterController existingCC = existingWorker.GetComponent<CharacterController>();

                if (existingCC != null && newWorkerCC != existingCC)
                {
                    Physics.IgnoreCollision(newWorkerCC, existingCC, true);
                }
            }
        }

        private void CreateWorkerObject(WorkerInfo info, int prefabIndex = -1)
        {
            if (defaultStation == null)
            {
                return;
            }

            ANode spawnNode = aGrid.GetNodeFromGridPos(defaultStation.entrancePos);

            if (spawnNode != null)
            {
                Vector3 floorPos = aGrid.GetNodeFootPosition(spawnNode);
                Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                Vector3 spawnPos = floorPos + randomOffset;

                GameObject logicObj = Instantiate(workerLogicPrefab, spawnPos, Quaternion.identity, workerContainer);
                WorkerAI ai = logicObj.GetComponent<WorkerAI>();

                if (ai == null)
                {
                    ai = ai.GetComponentInChildren<WorkerAI>();
                }

                int targetIndex = prefabIndex;
                if (targetIndex < 0 || targetIndex >= workerModelPrefabs.Count)
                {
                    targetIndex = Random.Range(0, workerModelPrefabs.Count);
                }

                GameObject modelObj = Instantiate(workerModelPrefabs[targetIndex], logicObj.transform);
                modelObj.transform.localPosition = Vector3.zero;
                modelObj.transform.localRotation = Quaternion.identity;

                CharacterController newCC = logicObj.GetComponent<CharacterController>();
                if (newCC != null)
                {
                    IgnoreCollisionWithOtherWorkers(newCC);
                }

                if (!activeWorkers.Contains(ai))
                {
                    activeWorkers.Add(ai);
                }

                ai.Initialize(info, defaultRestaurant, defaultStation);
            }
        }

        private void TestDebuging()
        {


















        }

        private void Update()
        {
            if (!isInitialized || activeWorkers == null) return;

            int count = activeWorkers.Count;

            foreach(WorkerAI worker in activeWorkers)
            {
                if (worker != null)
                {
                    worker.UpdateBT();
                }
            }

            AssignPendingTasks();
        }

        private void AssignPendingTasks()
        {
            if (taskQueue.Count == 0) return;

            List<WorkerAI> idleWorkers = GetIdleWorkers();

            if (idleWorkers.Count == 0)
            {
                return;
            }

            int loopCount = taskQueue.Count;
            for (int i = 0; i < loopCount; i++)
            {
                if (idleWorkers.Count == 0) break;


                StructureBase target = taskQueue.Peek();


                if (target.HasWorker())
                {
                    taskQueue.Dequeue();
                    continue;
                }

                if (target is ProductableBuilding pb && !pb.IsWorkRequested)
                {
                    taskQueue.Dequeue();
                    continue;
                }


                WorkerAI bestWorker = GetBestInIdleWorkers(idleWorkers, target);

                if (bestWorker != null)
                {
                    taskQueue.Dequeue();

                    target.SetWorker(bestWorker);
                    bestWorker.AssignTask(target);
                    idleWorkers.Remove(bestWorker);
                }
            }
        }
        public void RegisterTask(StructureBase structure)
        {
            if (!taskQueue.Contains(structure))
            {
                taskQueue.Enqueue(structure);
            }
        }

        public List<WorkerAI> GetIdleWorkers()
        {
            List<WorkerAI> idleList = new List<WorkerAI>();

            for (int i = 0; i < activeWorkers.Count; i++)
            {
                WorkerAI w = activeWorkers[i];


                if (w != null && !w.HasTask)
                {
                    idleList.Add(w);
                }
            }

            return idleList;
        }
        private WorkerAI GetBestInIdleWorkers(List<WorkerAI> candidates, StructureBase structure)
        {
            if (candidates == null || candidates.Count == 0) return null;

            ANode targetNode = aGrid.GetNodeFromWorldPosition(structure.transform.position);

            if (targetNode == null || !targetNode.isWalkable) return null;

            WorkerAI bestWorker = null;
            float minScore = float.MaxValue;
            float tolerance = 0.1f;

            foreach (var w in candidates)
            {
                if (w == null) continue;

                ANode workerNode = aGrid.GetNodeFromWorldPosition(w.transform.position);
                if (workerNode == null) continue;


                int dx = Mathf.Abs(workerNode.indexX - targetNode.indexX);
                int dy = Mathf.Abs(workerNode.indexY - targetNode.indexY);
                float distScore = dx + dy;

                if (distScore < minScore - tolerance)
                {
                    minScore = distScore;
                    bestWorker = w;
                }
                else if (Mathf.Abs(distScore - minScore) <= tolerance)
                {

                    if (bestWorker != null && w.GetInstanceID() < bestWorker.GetInstanceID())
                    {
                        bestWorker = w;
                    }
                    else if (bestWorker == null)
                    {
                        bestWorker = w;
                    }
                }
            }

            return bestWorker;
        }
    }
}

