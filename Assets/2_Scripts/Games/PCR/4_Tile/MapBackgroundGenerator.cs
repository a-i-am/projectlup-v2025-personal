using UnityEngine;

namespace LUP.PCR
{
    public class MapBackgroundGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject BackgroundPrefab;
        private void Start()
        {
            GenerateBackground();
        }

        public void GenerateBackground()
        {
            GameObject bgObj = Instantiate(BackgroundPrefab, this.transform);

            bgObj.name = "Map Background Plane";

            bgObj.transform.localPosition = BackgroundPrefab.transform.localPosition;
            bgObj.transform.localRotation = BackgroundPrefab.transform.localRotation;
            bgObj.transform.localScale = BackgroundPrefab.transform.localScale;
        }
    }
}
