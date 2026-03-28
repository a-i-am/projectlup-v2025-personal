using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LUP.PCR
{
    [InitializeOnLoad]
    public static class MapEditorOverlay
    {
        // ── 데이터 ──────────────────────────────────────────────────────────────
        private static ProductionRuntimeData mapData = new ProductionRuntimeData();
        private static readonly float tileSize = GridSize.tileSize;

        private static bool isEditingMode  = false;
        private static bool isPanelVisible = true;

        private static bool IsActive
        {
            get => SessionState.GetBool("MapEditor.IsActive", false);
            set => SessionState.SetBool("MapEditor.IsActive", value);
        }

        private static BuildingInfo selectedBuilding    = null;
        private static BuildingType  pendingBuildingType = BuildingType.NONE;
        private static Vector2Int    hoveredGridPos;

        private static string DataPath =>
            Application.dataPath + "/Resources/Data/SavedData/production_runtime.json";

        // ── 프리팹 경로 테이블 ────────────────────────────────────────────────────
        private static readonly Dictionary<BuildingType, string> prefabPaths =
            new Dictionary<BuildingType, string>
        {
            { BuildingType.WHEATFARM,    "Assets/3_Prefabs/PCR/Juha/Structure/WheatFarm_SCI-FI.prefab" },
            { BuildingType.MUSHROOMFARM, "Assets/3_Prefabs/PCR/Juha/Structure/CornFarm_SCI-FI.prefab" },
            { BuildingType.MOLEFARM,     "Assets/3_Prefabs/PCR/Juha/Structure/TomatoFarm_SCI-FI.prefab" },
            { BuildingType.RESTAURANT,   "Assets/3_Prefabs/PCR/Juha/Structure/Restaurant_SCI-FI.prefab" },
            { BuildingType.POWERSTATION, "Assets/3_Prefabs/PCR/Juha/Structure/PowerStation_SCI-FI.prefab" },
            { BuildingType.STONEMINE,    "Assets/3_Prefabs/PCR/Juha/Structure/CrystalMine_SCI-FI.prefab" },
            { BuildingType.IRONMINE,     "Assets/3_Prefabs/PCR/Juha/Structure/Dust_SCI-FI_Iron.prefab" },
            { BuildingType.COALMINE,     "Assets/3_Prefabs/PCR/Juha/Structure/Dust_SCI-FI_Stone.prefab" },
            { BuildingType.LADDER,       "Assets/3_Prefabs/PCR/Juha/Structure/Ladder.prefab" },
            { BuildingType.WORKSTATION,  "Assets/3_Prefabs/PCR/Juha/Structure/Worker Idle Room_SCI-FI.prefab" },
        };

        private const string WallPrefabPath = "Assets/3_Prefabs/PCR/Juha/InnerWall_SCI-FI.prefab";

        private static readonly Dictionary<BuildingType, GameObject> prefabCache  = new Dictionary<BuildingType, GameObject>();
        private static readonly Dictionary<BuildingType, Texture2D>  previewCache = new Dictionary<BuildingType, Texture2D>();
        private static GameObject wallPrefabCache  = null;
        private static Texture2D  wallPreviewCache = null;
        private static bool       wallPrefabLoaded = false;

        // ── 씬 프리뷰 오브젝트 ───────────────────────────────────────────────────
        // 실제 프리팹을 HideAndDontSave로 인스턴스화 → 게임 뷰와 동일하게 렌더링
        private const string PreviewPrefix = "[MapEditorPreview]";

        private static readonly Dictionary<int, GameObject>       buildingPreviewMap = new Dictionary<int, GameObject>();
        private static readonly Dictionary<Vector2Int, GameObject> wallPreviewMap    = new Dictionary<Vector2Int, GameObject>();

        private static bool previewDirty = true;

        // ── 팔레트 ──────────────────────────────────────────────────────────────
        private static readonly BuildingType[] paletteItems =
        {
            BuildingType.NONE,
            BuildingType.WHEATFARM,
            BuildingType.MUSHROOMFARM,
            BuildingType.MOLEFARM,
            BuildingType.RESTAURANT,
            BuildingType.POWERSTATION,
            BuildingType.STONEMINE,
            BuildingType.IRONMINE,
            BuildingType.COALMINE,
            BuildingType.LADDER,
            BuildingType.WORKSTATION,
        };

        private static Vector2 paletteScrollPos = Vector2.zero;

        private const float PaletteX     = 10f;
        private const float PaletteStartY = 265f;  // 패널·힌트 아래 고정 시작 위치
        private const float PaletteW     = 200f;
        private const float PaletteItemH = 34f;
        private const float PaletteItemPad = 3f;
        private const float PaletteThumbS  = 26f;  // 벽 썸네일 크기

        // ── 초기화 ───────────────────────────────────────────────────────────────
        static MapEditorOverlay()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            AssemblyReloadEvents.beforeAssemblyReload += ClearAllPreviews;
            EditorApplication.quitting                += ClearAllPreviews;

            // 이전 세션 고아 오브젝트 정리
            CleanOrphanedPreviews();

            if (IsActive)
            {
                LoadDataFromJson();
                previewDirty = true;
            }
        }

        [MenuItem("Tools/PCR Map Editor")]
        public static void Activate()
        {
            IsActive       = true;
            isPanelVisible = true;
            previewDirty   = true;

            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = (SceneView)SceneView.sceneViews[0];
            if (sceneView != null)
            {
                sceneView.maximized = true;
                sceneView.Focus();
            }

            SceneView.RepaintAll();
        }

        // ── 씬 GUI ───────────────────────────────────────────────────────────────
        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!IsActive) return;

            if (isEditingMode)
                UpdateHoveredGridPos();

            HandleKeyboardShortcuts();

            if (isEditingMode)
            {
                if (previewDirty)
                    RebuildPreviewObjects();

                HandleSceneInput();

                // HandleSceneInput에서 gridPos가 갱신된 후 프리뷰 동기화
                if (!previewDirty && selectedBuilding != null)
                    SyncSelectedBuildingPreview();
            }
            else
            {
                ClearAllPreviews();
            }

            DrawGridInScene();

            Handles.BeginGUI();
            DrawOverlayPanel();
            DrawShortcutHints();
            if (isEditingMode)
                DrawBuildingPalette();
            Handles.EndGUI();

            sceneView.Repaint();
        }

        // ── 단축키 ───────────────────────────────────────────────────────────────
        private static void HandleKeyboardShortcuts()
        {
            Event e = Event.current;
            if (e.type != EventType.KeyDown) return;

            switch (e.keyCode)
            {
                case KeyCode.Tab:
                    isPanelVisible = !isPanelVisible;
                    e.Use();
                    break;
                case KeyCode.E:
                    isEditingMode = !isEditingMode;
                    if (!isEditingMode) selectedBuilding = null;
                    e.Use();
                    break;
                case KeyCode.L:
                    LoadDataFromJson();
                    e.Use();
                    break;
                case KeyCode.S:
                    SaveMapDataToJson();
                    e.Use();
                    break;
                case KeyCode.Delete:
                    if (selectedBuilding != null)
                    {
                        RemoveBuildingPreview(selectedBuilding.buildingId);
                        mapData.BuildingInfoList.Remove(selectedBuilding);
                        SaveMapDataToJson();
                        selectedBuilding = null;
                    }
                    else
                    {
                        WallInfo hoveredWall = mapData.WallInfoList
                            .FirstOrDefault(w => w.gridPos == hoveredGridPos);
                        if (hoveredWall != null)
                        {
                            RemoveWallPreview(hoveredWall.gridPos);
                            mapData.WallInfoList.Remove(hoveredWall);
                            SaveMapDataToJson();
                        }
                    }
                    e.Use();
                    break;
            }
        }

        // UI 패널 위에 마우스가 있는지 확인 (GUI 좌표계)
        private static bool IsMouseOverUI()
        {
            Vector2 mp = Event.current.mousePosition;

            // 메인 패널
            if (isPanelVisible && new Rect(10, 10, 260, 162).Contains(mp))
                return true;

            // 단축키 힌트 패널
            const float lineH = 16f;
            float hintH = 2 * lineH + 8f;
            if (new Rect(10, 210, 200, hintH).Contains(mp))
                return true;

            // 건물 팔레트
            float totalContentH = paletteItems.Length * (PaletteItemH + PaletteItemPad) + PaletteItemPad;
            float availH        = Screen.height - PaletteStartY - 20f;
            float paletteH      = Mathf.Min(totalContentH, availH);
            if (new Rect(PaletteX, PaletteStartY, PaletteW, paletteH).Contains(mp))
                return true;

            return false;
        }

        // ── 씬 뷰 입력 처리 ──────────────────────────────────────────────────────
        private static void HandleSceneInput()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (IsMouseOverUI()) return;

            Event e = Event.current;
            BuildingInfo existingBuilding = GetBuildingAtPos(hoveredGridPos);

            // 건물 이동
            if (e.button == 0 && e.type == EventType.MouseMove)
            {
                if (selectedBuilding != null)
                    selectedBuilding.gridPos = hoveredGridPos;
                e.Use();
            }

            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                if (selectedBuilding != null)
                {
                    Vector2Int size = BuildingSizeTable.Get((BuildingType)selectedBuilding.buildingType);
                    if (IsAreaFreeForBuilding(selectedBuilding.gridPos, size, selectedBuilding))
                    {
                        // 확정: 프리뷰 위치를 이미 SyncSelectedBuildingPreview가 맞춰줬으므로 그대로
                        selectedBuilding = null;
                    }
                }
                else if (existingBuilding != null)
                {
                    selectedBuilding = existingBuilding;
                }
                else if (pendingBuildingType != BuildingType.NONE)
                {
                    Vector2Int size = BuildingSizeTable.Get(pendingBuildingType);
                    if (IsAreaFreeForBuilding(hoveredGridPos, size))
                    {
                        BuildingInfo newBuilding = new BuildingInfo(
                            mapData.GenerateId(), 1, hoveredGridPos, (int)pendingBuildingType, false);
                        mapData.BuildingInfoList.Add(newBuilding);
                        SpawnBuildingPreview(newBuilding);
                        e.Use();
                    }
                }
            }

            // 벽 추가
            if (pendingBuildingType == BuildingType.NONE && selectedBuilding == null)
            {
                if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
                {
                    WallInfo existingWall = mapData.WallInfoList
                        .FirstOrDefault(w => w.gridPos == hoveredGridPos);
                    if (existingBuilding == null && existingWall == null)
                    {
                        WallInfo newWall = new WallInfo(1, hoveredGridPos);
                        mapData.WallInfoList.Add(newWall);
                        SpawnWallPreview(newWall);
                        e.Use();
                    }
                }
            }
        }

        private static void UpdateHoveredGridPos()
        {
            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (ray.direction.z == 0f) return;
            float dist = -ray.origin.z / ray.direction.z;
            Vector3 wp = ray.origin + ray.direction * dist;
            hoveredGridPos = new Vector2Int(
                Mathf.FloorToInt(wp.x / tileSize),
                Mathf.FloorToInt(-wp.y / tileSize)
            );
        }

        // ── 프리뷰 오브젝트 관리 ─────────────────────────────────────────────────
        private static void RebuildPreviewObjects()
        {
            ClearAllPreviews();
            previewDirty = false;

            foreach (WallInfo wall in mapData.WallInfoList)
                SpawnWallPreview(wall);

            foreach (BuildingInfo building in mapData.BuildingInfoList)
                SpawnBuildingPreview(building);
        }

        private static void SpawnWallPreview(WallInfo wall)
        {
            GameObject prefab = GetCachedWallPrefab();
            if (prefab == null) return;

            GameObject go = CreatePreviewInstance(prefab,
                new Vector3(wall.gridPos.x * tileSize, -wall.gridPos.y * tileSize, 0f));
            wallPreviewMap[wall.gridPos] = go;
        }

        private static void SpawnBuildingPreview(BuildingInfo building)
        {
            GameObject prefab = GetCachedPrefab((BuildingType)building.buildingType);
            if (prefab == null) return;

            GameObject go = CreatePreviewInstance(prefab,
                new Vector3(building.gridPos.x * tileSize, -building.gridPos.y * tileSize, 0f));
            buildingPreviewMap[building.buildingId] = go;
        }

        private static GameObject CreatePreviewInstance(GameObject prefab, Vector3 worldPos)
        {
            GameObject go = Object.Instantiate(prefab, worldPos, Quaternion.identity);
            go.name      = PreviewPrefix + prefab.name;
            go.hideFlags = HideFlags.HideAndDontSave;

            // 게임 로직 스크립트 비활성화 (렌더링만 유지)
            foreach (MonoBehaviour mono in go.GetComponentsInChildren<MonoBehaviour>(true))
                mono.enabled = false;

            return go;
        }

        // 드래그 중 선택된 건물 프리뷰 위치 실시간 갱신
        private static void SyncSelectedBuildingPreview()
        {
            if (selectedBuilding == null) return;
            if (buildingPreviewMap.TryGetValue(selectedBuilding.buildingId, out GameObject go) && go != null)
            {
                go.transform.position = new Vector3(
                    selectedBuilding.gridPos.x * tileSize,
                    -selectedBuilding.gridPos.y * tileSize,
                    0f
                );
            }
        }

        private static void RemoveBuildingPreview(int buildingId)
        {
            if (buildingPreviewMap.TryGetValue(buildingId, out GameObject go))
            {
                if (go != null) Object.DestroyImmediate(go);
                buildingPreviewMap.Remove(buildingId);
            }
        }

        private static void RemoveWallPreview(Vector2Int gridPos)
        {
            if (wallPreviewMap.TryGetValue(gridPos, out GameObject go))
            {
                if (go != null) Object.DestroyImmediate(go);
                wallPreviewMap.Remove(gridPos);
            }
        }

        private static void ClearAllPreviews()
        {
            foreach (var go in buildingPreviewMap.Values)
                if (go != null) Object.DestroyImmediate(go);
            buildingPreviewMap.Clear();

            foreach (var go in wallPreviewMap.Values)
                if (go != null) Object.DestroyImmediate(go);
            wallPreviewMap.Clear();
        }

        // 도메인 리로드 후 남은 고아 오브젝트 정리
        private static void CleanOrphanedPreviews()
        {
            foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (go != null && go.name.StartsWith(PreviewPrefix))
                    Object.DestroyImmediate(go);
            }
        }

        // ── 씬 뷰 오버레이 패널 ──────────────────────────────────────────────────
        private static void DrawOverlayPanel()
        {
            if (!isPanelVisible) return;

            const float panelX = 10f;
            const float panelY = 10f;
            const float panelW = 260f;
            const float pad    = 8f;
            const float panelH = 162f;

            GUI.Box(new Rect(panelX, panelY, panelW, panelH), GUIContent.none, EditorStyles.helpBox);
            GUILayout.BeginArea(new Rect(panelX + pad, panelY + pad, panelW - pad * 2, panelH - pad * 2));

            GUILayout.Label("PCR Map Editor", new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 });

            EditorGUI.BeginChangeCheck();
            bool newEditMode = EditorGUILayout.ToggleLeft("Scene Edit Mode  [E]", isEditingMode);
            if (EditorGUI.EndChangeCheck())
            {
                isEditingMode = newEditMode;
                if (!isEditingMode) selectedBuilding = null;
            }

            GUILayout.Space(4);

            if (GUILayout.Button("Load from JSON  [L]", GUILayout.Height(26)))
                LoadDataFromJson();

            GUILayout.Space(4);

            if (GUILayout.Button("Save to JSON  [S]", GUILayout.Height(26)))
                SaveMapDataToJson();

            GUILayout.FlexibleSpace();
            GUIStyle hintStyle = new GUIStyle(EditorStyles.miniLabel);
            hintStyle.normal.textColor = new Color(0.55f, 0.55f, 0.55f);
            GUILayout.Label("[Tab] 패널  [E] 편집모드  [L] 로드  [S] 저장", hintStyle);

            GUILayout.EndArea();
        }

        // ── 단축키 전용 플로팅 힌트 ──────────────────────────────────────────────
        private static void DrawShortcutHints()
        {
            if (!isEditingMode) return;

            string[] lines =
            {
                "── 단축키 전용 ──────────────",
                "[Delete]  커서 위치 오브젝트 삭제",
            };

            const float x     = 10f;
            const float y     = 210f;
            const float w     = 200f;
            const float lineH = 16f;
            float totalH = lines.Length * lineH + 8f;

            EditorGUI.DrawRect(new Rect(x, y, w, totalH), new Color(0f, 0f, 0f, 0.45f));

            GUIStyle style = new GUIStyle(EditorStyles.miniLabel);
            style.normal.textColor = new Color(1f, 0.85f, 0.4f);

            GUILayout.BeginArea(new Rect(x + 4, y + 4, w - 8, totalH - 8));
            foreach (string line in lines)
                GUILayout.Label(line, style);
            GUILayout.EndArea();
        }

        // ── 씬 뷰 좌측 세로 건물 팔레트 ────────────────────────────────────────
        private static void DrawBuildingPalette()
        {
            float viewH         = Screen.height;
            float availH        = viewH - PaletteStartY - 20f;
            float totalContentH = paletteItems.Length * (PaletteItemH + PaletteItemPad) + PaletteItemPad;

            Rect scrollViewRect = new Rect(PaletteX, PaletteStartY, PaletteW, Mathf.Min(totalContentH, availH));
            Rect contentRect    = new Rect(0, 0, PaletteW, totalContentH);

            EditorGUI.DrawRect(scrollViewRect, new Color(0f, 0f, 0f, 0.45f));

            paletteScrollPos = GUI.BeginScrollView(scrollViewRect, paletteScrollPos, contentRect, false, false);

            float curY = PaletteItemPad;
            foreach (BuildingType type in paletteItems)
            {
                bool isSelected = type == pendingBuildingType;
                Rect itemRect   = new Rect(0, curY, PaletteW, PaletteItemH);

                EditorGUI.DrawRect(itemRect, isSelected
                    ? new Color(1f, 0.85f, 0.2f, 0.35f)
                    : new Color(1f, 1f,    1f,   0.04f));

                if (type == BuildingType.NONE)
                {
                    // 벽: 썸네일 + 이름
                    Texture2D thumb = GetPreviewTexture(BuildingType.NONE);
                    float     labelX = PaletteItemPad * 2;
                    if (thumb != null)
                    {
                        float tY = curY + (PaletteItemH - PaletteThumbS) * 0.5f;
                        GUI.DrawTexture(
                            new Rect(PaletteItemPad, tY, PaletteThumbS, PaletteThumbS),
                            thumb, ScaleMode.ScaleToFit);
                        labelX = PaletteItemPad + PaletteThumbS + PaletteItemPad;
                    }

                    GUIStyle ws = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleLeft };
                    ws.normal.textColor = isSelected ? new Color(1f, 0.95f, 0.4f) : new Color(0.9f, 0.9f, 0.9f);
                    GUI.Label(new Rect(labelX, curY, PaletteW - labelX - PaletteItemPad, PaletteItemH), "— 벽 편집", ws);
                }
                else
                {
                    // 건물: 이름만 표시
                    GUIStyle ns = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleLeft };
                    ns.normal.textColor = isSelected ? new Color(1f, 0.95f, 0.4f) : new Color(0.9f, 0.9f, 0.9f);
                    GUI.Label(new Rect(PaletteItemPad * 2, curY, PaletteW - PaletteItemPad * 3, PaletteItemH),
                        type.ToString(), ns);
                }

                if (GUI.Button(itemRect, GUIContent.none, GUIStyle.none))
                {
                    pendingBuildingType = type;
                    selectedBuilding    = null;
                }

                curY += PaletteItemH + PaletteItemPad;
            }

            GUI.EndScrollView();
        }

        // ── 팔레트용 AssetPreview 캐시 ───────────────────────────────────────────
        private static Texture2D GetPreviewTexture(BuildingType type)
        {
            if (type == BuildingType.NONE)
            {
                if (wallPreviewCache == null)
                {
                    GameObject wp = GetCachedWallPrefab();
                    if (wp != null) wallPreviewCache = AssetPreview.GetAssetPreview(wp);
                }
                return wallPreviewCache;
            }

            if (previewCache.TryGetValue(type, out Texture2D cached) && cached != null)
                return cached;

            GameObject prefab = GetCachedPrefab(type);
            if (prefab == null) return null;

            Texture2D preview = AssetPreview.GetAssetPreview(prefab);
            if (preview != null) previewCache[type] = preview;
            return preview;
        }

        private static GameObject GetCachedPrefab(BuildingType type)
        {
            if (prefabCache.TryGetValue(type, out GameObject cached)) return cached;

            if (!prefabPaths.TryGetValue(type, out string path) || string.IsNullOrEmpty(path))
            {
                prefabCache[type] = null;
                return null;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            prefabCache[type] = prefab;
            return prefab;
        }

        private static GameObject GetCachedWallPrefab()
        {
            if (!wallPrefabLoaded)
            {
                wallPrefabCache = AssetDatabase.LoadAssetAtPath<GameObject>(WallPrefabPath);
                wallPrefabLoaded = true;
            }
            return wallPrefabCache;
        }

        // ── 데이터 I/O ──────────────────────────────────────────────────────────
        private static void LoadDataFromJson()
        {
            if (File.Exists(DataPath))
            {
                string jsonText = File.ReadAllText(DataPath);
                mapData      = JsonUtility.FromJson<ProductionRuntimeData>(jsonText);
                previewDirty = true;
                SceneView.RepaintAll();
                Debug.Log("<color=green>[Map Editor]</color> 맵 데이터를 성공적으로 불러왔습니다!");
            }
            else
            {
                Debug.LogError("저장된 JSON 파일이 없습니다: " + DataPath);
            }
        }

        private static void SaveMapDataToJson()
        {
            string jsonText = JsonUtility.ToJson(mapData, true);
            File.WriteAllText(DataPath, jsonText);
            Debug.Log("<color=cyan>[Map Editor]</color> 맵 데이터가 성공적으로 저장되었습니다!");
        }

        // ── 그리드 헬퍼 ─────────────────────────────────────────────────────────
        private static BuildingInfo GetBuildingAtPos(Vector2Int pos)
        {
            foreach (BuildingInfo b in mapData.BuildingInfoList)
            {
                Vector2Int size = BuildingSizeTable.Get((BuildingType)b.buildingType);
                if (pos.x >= b.gridPos.x && pos.x < b.gridPos.x + size.x &&
                    pos.y >= b.gridPos.y && pos.y < b.gridPos.y + size.y)
                    return b;
            }
            return null;
        }

        private static bool IsAreaFreeForBuilding(Vector2Int pos, Vector2Int size, BuildingInfo ignore = null)
        {
            for (int dx = 0; dx < size.x; dx++)
            {
                for (int dy = 0; dy < size.y; dy++)
                {
                    Vector2Int cell = new Vector2Int(pos.x + dx, pos.y + dy);

                    if (mapData.WallInfoList.Any(w => w.gridPos == cell))
                        return false;

                    foreach (BuildingInfo b in mapData.BuildingInfoList)
                    {
                        if (b == ignore) continue;
                        Vector2Int bs = BuildingSizeTable.Get((BuildingType)b.buildingType);
                        if (cell.x >= b.gridPos.x && cell.x < b.gridPos.x + bs.x &&
                            cell.y >= b.gridPos.y && cell.y < b.gridPos.y + bs.y)
                            return false;
                    }
                }
            }
            return true;
        }

        // ── 씬 색상 큐브 (편집 기준선) ────────────────────────────────────────────
        private static void DrawGridInScene()
        {
            BuildingInfo hoveredBuilding = GetBuildingAtPos(hoveredGridPos);

            // 벽: 파란색 / 호버: 밝은 파란색
            foreach (WallInfo wall in mapData.WallInfoList)
            {
                bool isHovered = isEditingMode && wall.gridPos == hoveredGridPos;
                Handles.color = isHovered
                    ? new Color(0.5f, 0.85f, 1f, 0.55f)
                    : new Color(0.2f, 0.6f,  1f, 0.3f);

                int   wX    = wall.gridPos.x;
                float drawY = -wall.gridPos.y * tileSize;
                Handles.DrawAAConvexPolygon(new Vector3[]
                {
                    new Vector3(wX * tileSize,           drawY,            0),
                    new Vector3((wX + 1) * tileSize,     drawY,            0),
                    new Vector3((wX + 1) * tileSize,     drawY - tileSize, 0),
                    new Vector3(wX * tileSize,           drawY - tileSize, 0),
                });
            }

            // 건물: 초록색 / 호버: 청록색 / 선택: 노란색 or 셀별 빨간색
            foreach (BuildingInfo building in mapData.BuildingInfoList)
            {
                bool       isSelected = building == selectedBuilding;
                bool       isHovered  = isEditingMode && !isSelected && building == hoveredBuilding;
                Vector2Int size       = BuildingSizeTable.Get((BuildingType)building.buildingType);

                if (!isSelected)
                {
                    Handles.color = isHovered
                        ? new Color(0.3f, 1f,   0.6f, 0.5f)
                        : new Color(0.2f, 0.9f, 0.3f, 0.25f);

                    int   bX    = building.gridPos.x;
                    float drawY = -building.gridPos.y * tileSize;
                    Handles.DrawAAConvexPolygon(new Vector3[]
                    {
                        new Vector3(bX * tileSize,                drawY,                     0),
                        new Vector3((bX + size.x) * tileSize,     drawY,                     0),
                        new Vector3((bX + size.x) * tileSize,     drawY - size.y * tileSize, 0),
                        new Vector3(bX * tileSize,                drawY - size.y * tileSize, 0),
                    });
                }
                else
                {
                    for (int dx = 0; dx < size.x; dx++)
                    {
                        for (int dy = 0; dy < size.y; dy++)
                        {
                            Vector2Int cell = new Vector2Int(building.gridPos.x + dx, building.gridPos.y + dy);

                            bool wallOverlap = mapData.WallInfoList.Any(w => w.gridPos == cell);
                            bool bldgOverlap = mapData.BuildingInfoList.Any(b =>
                            {
                                if (b == building) return false;
                                Vector2Int bs = BuildingSizeTable.Get((BuildingType)b.buildingType);
                                return cell.x >= b.gridPos.x && cell.x < b.gridPos.x + bs.x &&
                                       cell.y >= b.gridPos.y && cell.y < b.gridPos.y + bs.y;
                            });

                            Handles.color = (wallOverlap || bldgOverlap)
                                ? new Color(1f, 0.2f, 0.2f, 0.6f)
                                : new Color(1f, 0.9f, 0f,   0.6f);

                            float cX = (building.gridPos.x + dx) * tileSize;
                            float cY = -(building.gridPos.y + dy) * tileSize;
                            Handles.DrawAAConvexPolygon(new Vector3[]
                            {
                                new Vector3(cX,            cY,            0),
                                new Vector3(cX + tileSize, cY,            0),
                                new Vector3(cX + tileSize, cY - tileSize, 0),
                                new Vector3(cX,            cY - tileSize, 0),
                            });
                        }
                    }
                }
            }
        }
    }
}
