using UnityEngine;

namespace LUP.PCR
{

    public class RoamAroundBuilding : WorkerBlackboardNode
    {
        private BuildingBase waitingRoom;
        private float waitTimer = 0f;
        private float waitDuration = 2f;
        private bool isWaiting = false;

        public RoamAroundBuilding(WorkerBlackboard bb) : base(bb) { }

        protected override NodeState OnUpdate()
        {
            if (!HasData(BBKeys.WorkerStation)) return NodeState.FAILURE;
            waitingRoom = GetData<BuildingBase>(BBKeys.WorkerStation);

            if (waitingRoom == null) return NodeState.FAILURE;

            if (isWaiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitDuration)
                {
                    isWaiting = false;
                    waitTimer = 0f;

                    SetNewRandomDestination();
                }
                return NodeState.RUNNING;
            }

            if (Mover.IsMoving)
            {
                Mover.MoveAlongPath();
                return NodeState.RUNNING;
            }
            else
            {

                isWaiting = true;

                waitDuration = Random.Range(1.0f, 3.0f);
                return NodeState.RUNNING;
            }
        }

        private void SetNewRandomDestination()
        {
            Vector2Int center = waitingRoom.entrancePos;
            int radius = 3;

            for (int i = 0; i < 10; i++)
            {
                int randomX = Random.Range(-radius, radius + 1);
                int randomY = 0;



                Vector2Int randomPos = new Vector2Int(center.x + randomX, center.y + randomY);

                if(Mover.SetDestination(randomPos))
                {
                    return;
                }
            }

            Mover.Stop();
        }
    }
}
