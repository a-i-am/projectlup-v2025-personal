using UnityEngine;
using UnityEngine.UI;

namespace LUP.PCR
{
    public class WorkerOverlayUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Image statusIcon;
        [SerializeField] private Canvas canvas;

        [Header("Icons")]
        [SerializeField] private Sprite iconIdle;
        [SerializeField] private Sprite iconMoving;
        [SerializeField] private Sprite iconWorking;
        [SerializeField] private Sprite iconHungry;

        private WorkerAI targetWorker;
        private Camera mainCam;

        private void Start()
        {
            mainCam = Camera.main;


            if (canvas != null)
            {
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = mainCam;
            }
        }

        public void Setup(WorkerAI worker)
        {
            targetWorker = worker;
            UpdateIcon();
        }

        private void LateUpdate()
        {
            if (mainCam != null)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
            }


            if (targetWorker != null)
            {
                UpdateIcon();
            }
        }

        private void UpdateIcon()
        {
            if (targetWorker.IsHunger)
            {
                statusIcon.sprite = iconHungry;
                statusIcon.enabled = true;
            }
            else if (targetWorker.HasTask)
            {



                statusIcon.sprite = iconWorking;
                statusIcon.enabled = true;
            }
            else
            {

                statusIcon.sprite = iconIdle;

                statusIcon.enabled = true;
            }
        }
    }
}
