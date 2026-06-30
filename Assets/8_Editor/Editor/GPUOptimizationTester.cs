using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace LUP.PCR
{
    public class GPUOptimizationTester : EditorWindow
    {
        private class InstancingData
        {
            public Material material;
            public int sameMeshesCount;
            public string meshName;
        }
        private List<InstancingData> recommendedDatas = new List<InstancingData>();
        private Transform targetRootObject;

        private VisualElement instancingView;
        private VisualElement occlusionView;
        private ScrollView scrollView;
        private Button analyzeButton;

        [MenuItem("Tools/GPU Optimization")]
        public static void ShowWindow()
        {
            GetWindow<GPUOptimizationTester>("GPU Optimization Tester");
        }
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            Label titleLabel = new Label("최적화 대상 폴더(부모) 지정");
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginTop = 10;
            titleLabel.style.marginBottom = 5;
            root.Add(titleLabel);

            ObjectField objectField = new ObjectField("타겟 루트 (ex. TileMap)");
            objectField.objectType = typeof(Transform);
            objectField.RegisterValueChangedCallback(evt =>
            {
                targetRootObject = evt.newValue as Transform;
                if (analyzeButton != null) analyzeButton.SetEnabled(targetRootObject != null);
            });
            root.Add(objectField);

            VisualElement tabContainer = new VisualElement();
            tabContainer.style.flexDirection = FlexDirection.Row;
            tabContainer.style.marginTop = 15;
            tabContainer.style.marginBottom = 15;

            Button tab1Btn = new Button(() => SwitchTab(0)) { text = "GPU 인스턴싱 (배칭)" };
            tab1Btn.style.flexGrow = 1;
            tab1Btn.style.height = 30;

            Button tab2Btn = new Button(() => SwitchTab(1)) { text = "오클루전 컬링" };
            tab2Btn.style.flexGrow = 1;
            tab2Btn.style.height = 30;

            tabContainer.Add(tab1Btn);
            tabContainer.Add(tab2Btn);
            root.Add(tabContainer);

            instancingView = new VisualElement();

            analyzeButton = new Button(AnalyzeSceneForInstancing);
            analyzeButton.text = "지정된 폴더 배칭 분석하기";
            analyzeButton.style.height = 30;
            analyzeButton.style.marginBottom = 10;
            analyzeButton.SetEnabled(false);
            instancingView.Add(analyzeButton);

            VisualElement btnContainer = new VisualElement();
            btnContainer.style.flexDirection = FlexDirection.Row;
            Button enableAllBtn = new Button(() => SetAllInstancing(true)) { text = "전체 켜기" };
            enableAllBtn.style.flexGrow = 1;
            Button disableAllBtn = new Button(() => SetAllInstancing(false)) { text = "전체 끄기" };
            disableAllBtn.style.flexGrow = 1;
            btnContainer.Add(enableAllBtn);
            btnContainer.Add(disableAllBtn);
            instancingView.Add(btnContainer);

            scrollView = new ScrollView();
            scrollView.style.marginTop = 15;
            instancingView.Add(scrollView);

            occlusionView = new VisualElement();
            occlusionView.style.display = DisplayStyle.None;

            Label occDesc = new Label("오클루전 컬링: 카메라에 보이지 않는 타일을 렌더링에서 제외");
            occDesc.style.whiteSpace = WhiteSpace.Normal;
            occDesc.style.marginBottom = 15;
            occDesc.style.color = Color.yellow;
            occlusionView.Add(occDesc);

            Button applyStaticBtn = new Button(ApplyStaticFlags);
            applyStaticBtn.text = "자식 오브젝트 Static 적용";
            applyStaticBtn.style.height = 30;
            applyStaticBtn.style.marginBottom = 5;
            occlusionView.Add(applyStaticBtn);

            Button bakeBtn = new Button(BakeOcclusionCulling);
            bakeBtn.text = "오클루전 컬링 Bake";
            bakeBtn.style.height = 40;
            bakeBtn.style.backgroundColor = new Color(0.2f, 0.6f, 0.2f);
            occlusionView.Add(bakeBtn);

            Button clearBtn = new Button(ClearOcclusionCulling);
            clearBtn.text = "데이터 지우기";
            clearBtn.style.marginTop = 15;
            occlusionView.Add(clearBtn);

            root.Add(instancingView);
            root.Add(occlusionView);
        }
        private void SwitchTab(int index)
        {
            instancingView.style.display = (index == 0) ? DisplayStyle.Flex : DisplayStyle.None;
            occlusionView.style.display = (index == 1) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ApplyStaticFlags()
        {
            if (targetRootObject == null)
            {
                Debug.LogWarning("타겟 루트 지정 필요");
                return;
            }

            MeshRenderer[] renderers = targetRootObject.GetComponentsInChildren<MeshRenderer>(true);
            int count = 0;

            foreach (var renderer in renderers)
            {

                StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(renderer.gameObject);
                flags |= StaticEditorFlags.OccluderStatic;
                flags |= StaticEditorFlags.OccludeeStatic;

                GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, flags);
                count++;
            }

            Debug.Log($"[Occlusion] {count}개의 오브젝트에 Static 속성 적용 완료");
        }
        private void BakeOcclusionCulling()
        {
            Debug.Log("오클루전 데이터 베이킹 시작");
            StaticOcclusionCulling.Compute();
            Debug.Log("베이킹 완료!");
        }
        private void ClearOcclusionCulling()
        {
            StaticOcclusionCulling.Clear();
            Debug.Log("오클루전 데이터 삭제 완료");
        }
        private void AnalyzeSceneForInstancing()
        {
            if (targetRootObject == null) return;
            recommendedDatas.Clear();

            MeshRenderer[] renderers = targetRootObject.GetComponentsInChildren<MeshRenderer>(true);
            Dictionary<Material, Dictionary<Mesh, int>> analysisDict = new Dictionary<Material, Dictionary<Mesh, int>>();

            foreach (MeshRenderer renderer in renderers)
            {
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                if (filter == null || filter.sharedMesh == null) continue;

                Mesh mesh = filter.sharedMesh;
                Material[] materials = renderer.sharedMaterials;

                foreach (Material mat in materials)
                {
                    if (mat == null) continue;

                    if (!analysisDict.ContainsKey(mat)) analysisDict[mat] = new Dictionary<Mesh, int>();
                    if (!analysisDict[mat].ContainsKey(mesh)) analysisDict[mat][mesh] = 0;

                    analysisDict[mat][mesh]++;
                }
            }

            foreach (var matPair in analysisDict)
            {
                var mostUsedMeshPair = matPair.Value.OrderByDescending(m => m.Value).First();
                if (mostUsedMeshPair.Value >= 2)
                {
                    recommendedDatas.Add(new InstancingData
                    {
                        material = matPair.Key,
                        sameMeshesCount = mostUsedMeshPair.Value,
                        meshName = mostUsedMeshPair.Key.name
                    });
                }
            }

            recommendedDatas = recommendedDatas.OrderByDescending(d => d.sameMeshesCount).ToList();
            RefreshScrollView();
        }

        private void RefreshScrollView()
        {
            scrollView.Clear();

            if (recommendedDatas.Count == 0)
            {
                scrollView.Add(new Label("조건에 맞는 머티리얼이 없습니다."));
                return;
            }

            foreach (var data in recommendedDatas)
            {
                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.alignItems = Align.Center;
                row.style.marginBottom = 5;

                Toggle toggle = new Toggle(data.material.name);
                toggle.value = data.material.enableInstancing;
                toggle.style.width = 250;

                Label infoLabel = new Label($"[배칭 대기: {data.sameMeshesCount}개] (메시: {data.meshName})");
                infoLabel.style.color = toggle.value ? new Color(0.2f, 0.8f, 0.2f) : Color.gray;

                toggle.RegisterValueChangedCallback(evt =>
                {
                    SetInstancing(data.material, evt.newValue);
                    infoLabel.style.color = evt.newValue ? new Color(0.2f, 0.8f, 0.2f) : Color.gray;
                });

                row.Add(toggle);
                row.Add(infoLabel);
                scrollView.Add(row);
            }
        }
        private void SetAllInstancing(bool state)
        {
            foreach (var data in recommendedDatas) SetInstancing(data.material, state);
            RefreshScrollView();
        }
        private void SetInstancing(Material mat, bool enable)
        {
            if (mat.enableInstancing == enable) return;
            mat.enableInstancing = enable;
            EditorUtility.SetDirty(mat);
        }
    }
}
