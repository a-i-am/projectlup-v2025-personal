using UnityEngine;
namespace LUP.PCR
{
    public class StorageContentRules : MonoBehaviour
    {
        [SerializeField] GameObject emptySlot;

        [Header("생산 스탯")]
        int outputTime;
        int currMainResourceStorageLimits;
        int currSlotSumCount;

        void Start()
        {

        }
    }




}
