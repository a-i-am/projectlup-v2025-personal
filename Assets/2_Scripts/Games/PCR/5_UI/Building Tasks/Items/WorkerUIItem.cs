using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

namespace LUP.PCR
{
    public class WorkerUIItem : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Image statusIconImage;
        [SerializeField] private Button btn;

        public void Setup(WorkerAI worker, Sprite icon, Action onClick)
        {
            if (nameText != null)
            {
                nameText.text = worker.name;
            }

            if (statusIconImage != null)
            {
                if (icon != null)
                {
                    statusIconImage.sprite = icon;
                    statusIconImage.enabled = true;
                }
                else
                {
                    statusIconImage.enabled = false;
                }
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => onClick?.Invoke());
        }

    }

}

