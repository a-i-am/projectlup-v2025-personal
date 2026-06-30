using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LUP.PCR
{
    public class DigWallState : ITaskState
    {
        private TaskController taskController;

        public DigWallState(TaskController controller)
        {
            taskController = controller;
        }

        public void InputHandle()
        {
            if (!taskController)
            {
                Debug.Log("taskController is Null");

                return;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
            {

                if (EventSystem.current.IsPointerOverGameObject()) return;

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

                if (Physics.Raycast(ray, out wallHit, 1000f, LayerMask.GetMask("Wall")))
                {
                    WallBase wall = wallHit.collider.GetComponent<WallBase>();
                    if (wall)
                    {
                        taskController.buildingSystem.RemoveWall(wall);
                        taskController.ReturnToIdleState();
                    }
                    else
                    {
                        taskController.ReturnToIdleState();
                    }
                }
                else
                {
                    taskController.ReturnToIdleState();
                }
            }
        }

        public void Open()
        {

            Debug.Log("DigWall State Open");
            taskController.digWallPreview.Show();
        }

        public void Close()
        {

            Debug.Log("DigWall State Close");
            taskController.digWallPreview.Hide();
        }





















    }

}
