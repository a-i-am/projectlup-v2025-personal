using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

namespace LUP.PCR
{

    using UnityEngine;

    public class PCRCameraController : MonoBehaviour
    {
        private Camera cam;

        [Range(1, 10)]
        [Header("줌 인/줌 아웃")]
        [SerializeField] private float minZoomDistance = 10f;
        [SerializeField] private float maxZoomDistance;
        [SerializeField] private float zoomSpeed = 5f;
        private float currentZoomDist;

        [Header("드래그")]
        public float dragSpeed = 1.0f;
        private Vector3 dragOrigin;
        private bool isDragging = false;

        private float mapWidth;
        private float mapHeight;
        private float mapZPos;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void Start()
        {
            mapWidth = GridSize.x * GridSize.tileSize;
            mapHeight = GridSize.y * GridSize.tileSize;
            mapZPos = GridSize.mapZPos;

            CalculateMaxZoomDistance();


            currentZoomDist = Mathf.Abs(transform.position.z - mapZPos);

            ZoomOutMax();
        }

        private void ZoomOutMax()
        {
            currentZoomDist = maxZoomDistance;

            float centerX = mapWidth * 0.5f;
            float centerY = -mapHeight * 0.5f;
            float centerZ = mapZPos - currentZoomDist;

            transform.position = new Vector3(centerX, centerY, centerZ);
        }
        private void Update()
        {
            HandleInput();
        }

        private void LateUpdate()
        {

            ClampCameraPosition();
        }


        private void CalculateMaxZoomDistance()
        {

            float distHeight = (mapHeight * 0.5f) / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);


            float distWidth = (mapWidth * 0.5f) / (Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * cam.aspect);


            maxZoomDistance = Mathf.Min(distHeight, distWidth);
        }

        private void HandleInput()
        {
            float scrollDelta = 0f;


            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;



                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;


                scrollDelta = deltaMagnitudeDiff * 0.01f * zoomSpeed;
            }

            else
            {

                scrollDelta = -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * 5f;
            }


            if (Mathf.Abs(scrollDelta) > 0.001f)
            {
                currentZoomDist += scrollDelta;

                currentZoomDist = Mathf.Clamp(currentZoomDist, minZoomDistance, maxZoomDistance);


                Vector3 pos = transform.position;
                pos.z = mapZPos - currentZoomDist;
                transform.position = pos;
            }



            if (Input.touchCount >= 2)
            {
                isDragging = false;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                dragOrigin = GetWorldPositionOnScreen(Input.mousePosition);
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 currentPos = GetWorldPositionOnScreen(Input.mousePosition);
                Vector3 difference = dragOrigin - currentPos;


                transform.position += new Vector3(difference.x, difference.y, 0);
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }


        private Vector3 GetWorldPositionOnScreen(Vector3 screenPos)
        {

            screenPos.z = currentZoomDist;
            return cam.ScreenToWorldPoint(screenPos);
        }


        private void ClampCameraPosition()
        {

            float halfFrustumHeight = currentZoomDist * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float halfFrustumWidth = halfFrustumHeight * cam.aspect;




            float mapLeft = 0f;
            float mapRight = mapWidth;
            float mapBottom = -mapHeight;
            float mapTop = 0f;


            float minX = mapLeft + halfFrustumWidth;
            float maxX = mapRight - halfFrustumWidth;


            float minY = mapBottom + halfFrustumHeight;
            float maxY = mapTop - halfFrustumHeight;

            Vector3 newPos = transform.position;


            if (maxX < minX) newPos.x = mapWidth * 0.5f;
            else newPos.x = Mathf.Clamp(newPos.x, minX, maxX);


            if (maxY < minY) newPos.y = -mapHeight * 0.5f;
            else newPos.y = Mathf.Clamp(newPos.y, minY, maxY);





            transform.position = newPos;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            float w = (mapWidth > 0) ? mapWidth : GridSize.x * GridSize.tileSize;
            float h = (mapHeight > 0) ? mapHeight : GridSize.y * GridSize.tileSize;
            float z = (mapZPos != 0) ? mapZPos : GridSize.mapZPos;

            Vector3 center = new Vector3(w * 0.5f, -h * 0.5f, z);
            Vector3 size = new Vector3(w, h, 1f);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}
