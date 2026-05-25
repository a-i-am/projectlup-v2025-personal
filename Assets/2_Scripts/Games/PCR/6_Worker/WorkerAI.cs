using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

namespace LUP.PCR
{
    [RequireComponent(typeof(Worker))]
    [RequireComponent(typeof(UnitMover))]
    public class WorkerAI : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private float hunger = 0;
        private bool isHunger = false;
        private bool hasTask = false; 
        public bool IsWorking { get; set; }

        [Header("Component")]
        private Worker worker;
        private UnitMover mover;
        private BTNode root;
        private StructureBase currentTaskPlace = null;
        private WorkerInfo myInfo;

        public WorkerBlackboard LocalBlackboard { get; private set; }
        public float LastWorkEndTime { get; private set; } = 0f;

        public float Hunger
        {
            get => hunger;
            set
            {
                hunger = value;
                LocalBlackboard.SetValue(BBKeys.Hunger, hunger);

                CheckHungerState();
            }
        }

        private void CheckHungerState()
        {
            bool shouldBeHungry = hunger >= HungerRules.HungryThreshold;

            if (isHunger != shouldBeHungry)
            {
                IsHunger = shouldBeHungry;
            }
        }

        public bool IsHunger
        {
            get => isHunger;
            set
            {
                isHunger = value;
                LocalBlackboard.SetValue(BBKeys.IsHunger, isHunger);
            }
        }
        public bool HasTask
        {
            get => hasTask;
            set
            {
                hasTask = value;
                LocalBlackboard.SetValue(BBKeys.HasTask, hasTask);

                if (value == false)
                {
                    LastWorkEndTime = Time.time;
                }

            }
        }
        public void Initialize(WorkerInfo info, BuildingBase restaurant, BuildingBase station)
        {
            this.myInfo = info;

            // 이름 설정 (디버깅용)
            gameObject.name = info.name;

            // 컴포넌트 및 블랙보드 초기화
            InitBTReferences();

            // 글로벌 건물 정보 세팅 (기존 SetGlobalBuildings 역할)
            LocalBlackboard.SetValue(BBKeys.Restaurant, restaurant);
            LocalBlackboard.SetValue(BBKeys.WorkerStation, station); // 혹은 workStationList
            
            Debug.Log($"Worker Initialized: {info.name} (ID: {info.id})");

        }
        public void InitBTReferences()
        {
            worker = GetComponent<Worker>();
            mover = GetComponent<UnitMover>();

            if (worker != null)
            {
                worker.InitAnimator();
            }

            LocalBlackboard = new WorkerBlackboard();

            InitBlackboard();
            CheckHungerState();
            SettingBT();
            GetComponentInChildren<WorkerOverlayUI>().Setup(this);
        }
        private void InitBlackboard()
        {
            //정적 데이터(참조) 등록
            LocalBlackboard.SetValue(BBKeys.OwnerAI, this);
            LocalBlackboard.SetValue(BBKeys.Self, worker);
            LocalBlackboard.SetValue(BBKeys.UnitMover, mover);

            // BT 상태 초기화
            LocalBlackboard.SetValue(BBKeys.Hunger, hunger);
            LocalBlackboard.SetValue(BBKeys.IsHunger, IsHunger);
            LocalBlackboard.SetValue(BBKeys.HasTask, hasTask);
        }

        private void SettingBT()
        {
            BTNode hungerSequence = new SequenceNode(new List<BTNode>
            {
                new IsHealthLowChecker(LocalBlackboard),
                new PauseCurrentTask(LocalBlackboard),
                new GoToEatingPlace(LocalBlackboard),
                new EatFood(LocalBlackboard)
            });

            BTNode workingSequence = new SequenceNode(new List<BTNode>
            {
                new IsNewTaskChecker(LocalBlackboard),
                new GoToNewTaskLocation(LocalBlackboard),
                new StartNewTask(LocalBlackboard),
                new PerformTask(LocalBlackboard)
            });

            root = new SelectorNode(new List<BTNode>
            {
                hungerSequence,
                workingSequence,
                new RoamAroundBuilding(LocalBlackboard)
            });
        }
        public void StopWorkAndResetState()
        {
            HasTask = false;
            IsWorking = false;

            LocalBlackboard.Remove(BBKeys.AssignedWorkplace);

            if (worker != null)
            {
                worker.SetActionState(WorkerActionState.Idle);
            }
        }

        public void UpdateBT()
        {
            if (root == null)
            {
                return;
            }

            root?.Evaluate();

            if(!isHunger)
            {
                // 배고프게 만들기
                Hunger = Mathf.Clamp(hunger + Time.deltaTime * 0.1f, 0, 3);
            }
        }

        public void AssignTask(StructureBase workingPlace)
        {
            hasTask = true;
            IsWorking = false;
            
            currentTaskPlace = workingPlace;
            LocalBlackboard.SetValue(BBKeys.AssignedWorkplace, currentTaskPlace);
            
            hasTask = true;
            LocalBlackboard.SetValue(BBKeys.HasTask, hasTask);
        }
    }
}