using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

namespace LUP.PCR
{
    public class BuildingUIItem : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Button btn;

        public void Setup(BuildingBase building, Action onClick)
        {
            if (nameText != null)
            {
                nameText.text = building.buildingName.Value;
            }

            btn.onClick.RemoveAllListeners();

            btn.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}

