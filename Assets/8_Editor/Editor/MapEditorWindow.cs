using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace LUP.PCR
{
    public class MapEditorWindow : EditorWindow
    {
        [SerializeField] private ProductionRuntimeData mapData = new ProductionRuntimeData();
        private float tileSize = GridSize.tileSize;
        private bool isEditingMode = false;

        private BuildingInfo selectedBuilding = null;
        private VisualElement RightPane;
        private Vector2 scrollPosition;


        private string dataPath => Application.dataPath + "/Resources/Data/SavedData/production_runtime.json";

        [MenuItem("Tools/PCR Map Editor")]
        public static void ShowMapEditor()
        {
            //EditorWindow wnd = GetWindow<MapEditorWindow>();
            //wnd.titleContent = new GUIContent("PCR Map Editor");
            GetWindow<MapEditorWindow>("Map Editor");
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!isEditingMode)
            {
                return;
            }
            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            float distance = -ray.origin.z / ray.direction.z;
            Vector3 worldPos = ray.origin + ray.direction * distance;
            int gridX = Mathf.FloorToInt(worldPos.x / tileSize);
            int gridY = Mathf.FloorToInt(-worldPos.y / tileSize);
            Vector2Int targetPos = new Vector2Int(gridX, gridY);

            if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            {
                WallInfo existingWall = mapData.WallInfoList.FirstOrDefault(w => w.gridPos == targetPos);

                if (e.button == 0)
                {
                    if (!e.shift && existingWall == null)
                    {
                        mapData.WallInfoList.Add(new WallInfo(1, targetPos));
                        e.Use(); // 이벤트를 소모하여 뒤에 있는 다른 UI가 눌리지 않게 함
                    }
                    else if (e.shift && existingWall != null)
                    {
                        mapData.WallInfoList.Remove(existingWall);
                        e.Use();
                    }
                }
            }

            if (e.type == EventType.MouseDown)
            {
                BuildingInfo existingBuilding = mapData.BuildingInfoList.FirstOrDefault(b => b.gridPos == targetPos);

                if (existingBuilding != null)
                {
                    // 선택
                    selectedBuilding = existingBuilding;

                }
                else
                {
                    selectedBuilding = null;
                }

            }

            if (e.type == EventType.MouseMove)
            {
                if (selectedBuilding != null)
                {
                    // 이동
                    selectedBuilding.gridPos = targetPos;
                }

                e.Use();
            }

            DrawGridInScene();
            sceneView.Repaint();
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            Label title = new Label("PCR Scene View Editor");
            title.style.fontSize = 16;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 10;
            root.Add(title);

            var editToggle = new UnityEngine.UIElements.Toggle("Scene Edit Mode");
            editToggle.value = isEditingMode;
            editToggle.RegisterValueChangedCallback(evt => {
                isEditingMode = evt.newValue;
                SceneView.RepaintAll(); // 모드를 켤 때 씬 뷰 즉시 새로고침
            });
            root.Add(editToggle);

            var loadButton = new UnityEngine.UIElements.Button(() =>
            {
                LoadDataFromJson();
            });
            loadButton.text = "Load from JSON";
            loadButton.style.height = 30;
            loadButton.style.marginTop = 5;
            root.Add(loadButton);

            var saveButton = new UnityEngine.UIElements.Button(() => {
                SaveMapDataToJson();
            });
            saveButton.text = "Save to JSON";
            saveButton.style.height = 30;
            saveButton.style.marginTop = 10;
            root.Add(saveButton);
        }
        private void LoadDataFromJson()
        {
            if (File.Exists(dataPath))
            {
                string jsonText = File.ReadAllText(dataPath);
                mapData = JsonUtility.FromJson<ProductionRuntimeData>(jsonText);
                SceneView.RepaintAll();

                Debug.Log("<color=green>[Map Editor]</color> 맵 데이터를 성공적으로 불러왔습니다!");
            }
            else
            {
                Debug.LogError("저장된 JSON 파일이 없습니다: " + dataPath);
            }
        }
        private void SaveMapDataToJson()
        {
            string jsonText = JsonUtility.ToJson(mapData, true);
            System.IO.File.WriteAllText(dataPath, jsonText);
            //File.WriteAllText(dataPath, jsonText);
            
            Debug.Log("<color=cyan>[Map Editor]</color> 맵 데이터가 성공적으로 저장되었습니다!");
        }
        private void DrawGridInScene()
        {
            Handles.color = new Color(0.2f, 0.6f, 1f, 0.5f);

            foreach (WallInfo wall in mapData.WallInfoList)
            {
                int wX = wall.gridPos.x;
                int wY = wall.gridPos.y;
                float drawY = -wY * tileSize;

                Vector3[] verts = new Vector3[]
                {
                   new Vector3(wX * tileSize, drawY, 0),
                   new Vector3((wX + 1) * tileSize, drawY, 0),
                   new Vector3((wX + 1) * tileSize, drawY - tileSize, 0),
                   new Vector3(wX * tileSize, drawY - tileSize, 0)
                };

                Handles.DrawAAConvexPolygon(verts);
            }
        }
    }
}