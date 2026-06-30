using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LUP.PCR
{
    public class IdleState : ITaskState
    {
        private TaskController taskController;
        private bool bIsActiveUI;

        public IdleState(TaskController controller)
        {
            taskController = controller;
            bIsActiveUI = false;
        }

        public void InputHandle()
        {
            if (!taskController)
            {
                return;
            }


            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
            {

                if (EventSystem.current.IsPointerOverGameObject()) return;

                if (bIsActiveUI)
                {
                    taskController.ReturnToIdleState();
                    SetIsActiveUI(false);
                }
                else
                {
                    var pos = Mouse.current.position.ReadValue();
                    var ray = Camera.main.ScreenPointToRay(pos);
                    RaycastHit wallHit;
                    RaycastHit tileHit;


                    if (Physics.Raycast(ray, out tileHit, 1000f, LayerMask.GetMask("Tile")))
                    {
                        Tile tile = tileHit.collider.GetComponent<Tile>();

                        if (tile)
                        {
                            taskController.UpdateLastClickTile(tile);
                        }
                    }

                    if (Physics.Raycast(ray, out wallHit, 1000f, LayerMask.GetMask("Building")))
                    {
                        BuildingBase building = wallHit.collider.GetComponent<BuildingBase>();

                        if (building)
                        {
                            Debug.Log("Find Building");
                            OpenBuildingTaskUI(building);

                        }
                    }
                }
            }
        }

        public void SetIsActiveUI(bool isActive)
        {
            bIsActiveUI = isActive;
        }

        public void Open()
        {
            SetIsActiveUI(false);
            Debug.Log("Idle State Open");
        }

        public void Close()
        {
            Debug.Log("Idle State Close");
        }

        private void OpenBuildingTaskUI(BuildingBase building)
        {
            taskController.NotifyBuildingClicked(building);
        }
    }

}
