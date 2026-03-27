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
        private BuildingType pendingBuildingType = BuildingType.NONE;

        private Button buildingDeleteButton;
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

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            float distance = -ray.origin.z / ray.direction.z;
            Vector3 worldPos = ray.origin + ray.direction * distance;
            int gridX = Mathf.FloorToInt(worldPos.x / tileSize);
            int gridY = Mathf.FloorToInt(-worldPos.y / tileSize);
            Vector2Int targetPos = new Vector2Int(gridX, gridY);

            BuildingInfo existingBuilding = GetBuildingAtPos(targetPos);

            if (pendingBuildingType != BuildingType.NONE)
            {
                // 건물 배치 모드: 좌클릭으로 건물 배치
                if (e.type == EventType.MouseDown && e.button == 0 && existingBuilding == null)
                {
                    mapData.BuildingInfoList.Add(new BuildingInfo(mapData.GenerateId(), 1, targetPos, (int)pendingBuildingType, false));
                    e.Use();
                }
            }
            else
            {
                // 벽 편집 모드
                if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                {
                    WallInfo existingWall = mapData.WallInfoList.FirstOrDefault(w => w.gridPos == targetPos);

                    if (e.button == 0 && existingBuilding == null)
                    {
                        if (!e.shift && existingWall == null)
                        {
                            mapData.WallInfoList.Add(new WallInfo(1, targetPos));
                            e.Use(); // 이벤트를 소모하여 이후 다른 UI에 전달하지 않게 함
                        }
                        else if (e.shift && existingWall != null)
                        {
                            mapData.WallInfoList.Remove(existingWall);
                            e.Use();
                        }
                    }
                }

                // 건물 선택/이동 모드
                if (e.button == 0)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        if (selectedBuilding != null)
                        {
                            // 내려놓기
                            selectedBuilding = null;
                            buildingDeleteButton.style.display = DisplayStyle.None;
                        }
                        else if (existingBuilding != null)
                        {
                            // 선택
                            selectedBuilding = existingBuilding;
                            buildingDeleteButton.style.display = DisplayStyle.Flex;
                        }
                        else
                        {
                            buildingDeleteButton.style.display = DisplayStyle.None;
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
                }
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

            Toggle editToggle = new UnityEngine.UIElements.Toggle("Scene Edit Mode");
            editToggle.value = isEditingMode;
            editToggle.RegisterValueChangedCallback
                (evt =>
                {
                    isEditingMode = evt.newValue;
                    SceneView.RepaintAll(); // 씬을 껐다 켰을 때 강제 리프레시
                });
            root.Add(editToggle);

            Button loadButton = new UnityEngine.UIElements.Button(() =>
            {
                LoadDataFromJson();
            });
            loadButton.text = "Load from JSON";
            loadButton.style.height = 30;
            loadButton.style.marginTop = 5;
            root.Add(loadButton);

            Button saveButton = new UnityEngine.UIElements.Button(() =>
            {
                SaveMapDataToJson();
            });
            saveButton.text = "Save to JSON";
            saveButton.style.height = 30;
            saveButton.style.marginTop = 10;
            root.Add(saveButton);

            // 건물 배치
            Label placeLabel = new Label("건물 배치");
            placeLabel.style.marginTop = 15;
            placeLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(placeLabel);

            EnumField buildingTypeField = new EnumField("배치할 건물", BuildingType.NONE);
            buildingTypeField.RegisterValueChangedCallback(evt =>
            {
                pendingBuildingType = (BuildingType)evt.newValue;
                // 배치 모드 전환 시 선택 중인 건물 해제
                selectedBuilding = null;
                buildingDeleteButton.style.display = DisplayStyle.None;
            });
            root.Add(buildingTypeField);

            buildingDeleteButton = new Button(() =>
            {
                mapData.BuildingInfoList.Remove(selectedBuilding);
                SaveMapDataToJson();
                selectedBuilding = null;
                buildingDeleteButton.style.display = DisplayStyle.None;
            });
            buildingDeleteButton.text = "건물 삭제";
            buildingDeleteButton.style.height = 30;
            buildingDeleteButton.style.marginTop = 5;
            buildingDeleteButton.style.display = DisplayStyle.None;
            root.Add(buildingDeleteButton);
        }

        private void LoadDataFromJson()
        {
            if (File.Exists(dataPath))
            {
                string jsonText = File.ReadAllText(dataPath);
                mapData = JsonUtility.FromJson<ProductionRuntimeData>(jsonText);
                SceneView.RepaintAll();

                Debug.Log("<color=green>[Map Editor]</color> 맵 데이터를 성공직으로 불러왔습니다!");
            }
            else
            {
                Debug.LogError("저장된 JSON 파일이 없습니다: " + dataPath);
            }
        }
        private void SaveMapDataToJson()
        {
            string jsonText = JsonUtility.ToJson(mapData, true);
            File.WriteAllText(dataPath, jsonText);

            Debug.Log("<color=cyan>[Map Editor]</color> 맵 데이터가 성공적으로 저장되었습니다!");
        }

        private BuildingInfo GetBuildingAtPos(Vector2Int pos)
        {
            foreach (BuildingInfo b in mapData.BuildingInfoList)
            {
                Vector2Int size = BuildingSizeTable.Get((BuildingType)b.buildingType);
                if (pos.x >= b.gridPos.x && pos.x < b.gridPos.x + size.x &&
                    pos.y >= b.gridPos.y && pos.y < b.gridPos.y + size.y)
                {
                    return b;
                }
            }
            return null;
        }

        private void DrawGridInScene()
        {
            // 벽: 파란색
            Handles.color = new Color(0.2f, 0.6f, 1f, 0.5f);
            foreach (WallInfo wall in mapData.WallInfoList)
            {
                int wX = wall.gridPos.x;
                float drawY = -wall.gridPos.y * tileSize;

                Vector3[] verts = new Vector3[]
                {
                   new Vector3(wX * tileSize,           drawY,           0),
                   new Vector3((wX + 1) * tileSize,     drawY,           0),
                   new Vector3((wX + 1) * tileSize,     drawY - tileSize, 0),
                   new Vector3(wX * tileSize,           drawY - tileSize, 0)
                };
                Handles.DrawAAConvexPolygon(verts);
            }

            // 건물: 초록색 / 선택된 건물: 노란색
            foreach (BuildingInfo building in mapData.BuildingInfoList)
            {
                bool isSelected = building == selectedBuilding;
                Handles.color = isSelected
                    ? new Color(1f, 0.9f, 0f, 0.6f)
                    : new Color(0.2f, 0.9f, 0.3f, 0.5f);

                Vector2Int size = BuildingSizeTable.Get((BuildingType)building.buildingType);
                int bX = building.gridPos.x;
                float drawY = -building.gridPos.y * tileSize;

                Vector3[] verts = new Vector3[]
                {
                    new Vector3(bX * tileSize,                drawY,                    0),
                    new Vector3((bX + size.x) * tileSize,     drawY,                    0),
                    new Vector3((bX + size.x) * tileSize,     drawY - size.y * tileSize, 0),
                    new Vector3(bX * tileSize,                drawY - size.y * tileSize, 0)
                };
                Handles.DrawAAConvexPolygon(verts);
            }
        }
    }
}
