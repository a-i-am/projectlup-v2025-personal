using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    public abstract class StructureBase : MonoBehaviour
    {
        public string placeName;
        public Vector2Int entrancePos;

        [Header("Animation Settings")]
        public WorkerActionState requiredAction;

        [Header("Path Settings")]
        public Transform entranceAnchor;
        public Transform workSpotAnchor;

        public List<Vector3> localWaypoints = new List<Vector3>();
        public Vector3 WorkSpotWorldPos => workSpotAnchor != null ? workSpotAnchor.position : transform.position;

        protected bool hasWork;
        public bool IsWorkRequested { get; protected set; } = false;
        public WorkerAI AssignedWorker { get; protected set; }


        public void SetupEntranceByAnchor(AGridMap gridMap)
        {
            if (entranceAnchor != null && gridMap != null)
            {
                ANode node = gridMap.GetNodeFromWorldPosition(entranceAnchor.position);
                if (node != null)
                {
                    entrancePos = new Vector2Int(node.indexX, node.indexY);
                    return;
                }
            }
        }
        public void ToggleWorkRequest()
        {
            IsWorkRequested = !IsWorkRequested;

            if (IsWorkRequested)
            {
                if (WorkerSystem.Instance != null)
                {
                    WorkerSystem.Instance.RegisterTask(this);
                }
            }
            else
            {

            }
        }
        public virtual void SetWorker(WorkerAI worker)
        {
            this.AssignedWorker = worker;
        }


        public virtual void ReleaseWorker()
        {
            if (AssignedWorker != null)
            {
                AssignedWorker.HasTask = false;
            }

            ExitWorker();
            AssignedWorker = null;

            if (IsWorkRequested && WorkerSystem.Instance != null)
            {
                WorkerSystem.Instance.RegisterTask(this);
            }
            else
            {

            }
        }
        public void EnterWorker()
        {
            hasWork = true;
        }

        public void ExitWorker()
        {
            if (AssignedWorker != null && entranceAnchor != null)
            {
                UnitMover mover = AssignedWorker.GetComponent<UnitMover>();
                CharacterController cc = AssignedWorker.GetComponent<CharacterController>();

                if (mover != null)
                {
                    mover.Stop();
                }

                if (cc != null)
                {
                    cc.enabled = false;
                }

                if (AssignedWorker != null && entranceAnchor != null)
                {
                    AssignedWorker.transform.position = entranceAnchor.position;
                    AssignedWorker.transform.rotation = entranceAnchor.rotation;
                }

                if (cc != null)
                {
                    cc.enabled = true;
                }
            }

            hasWork = false;
        }
        public bool HasWorker() => AssignedWorker != null;

        private void OnDrawGizmosSelected()
        {
            if (entranceAnchor == null || workSpotAnchor == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;


            Vector3 prevPos = entranceAnchor.position;

            foreach (Vector3 localPoint in localWaypoints)
            {

                Vector3 worldPoint = transform.TransformPoint(localPoint);

                Gizmos.DrawSphere(worldPoint, 0.3f);
                Gizmos.DrawLine(prevPos, worldPoint);
                prevPos = worldPoint;
            }


            Gizmos.DrawLine(prevPos, workSpotAnchor.position);
        }
    }
}
