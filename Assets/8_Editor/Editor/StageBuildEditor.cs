using LUP;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class StageBuildEditor
{
    private const string INIT_SCENE_NAME = "Init";

    [MenuItem("Tools/Register Scenes")]
    public static void RebuildBuildSettings()
    {

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();


        string[] sceneGUIDs = AssetDatabase.FindAssets(INIT_SCENE_NAME + " t:Scene");
        if (sceneGUIDs.Length == 0)
        {
            Debug.LogError($"❌ '{INIT_SCENE_NAME}' 씬을 찾을 수 없음.");
            return;
        }

        string initScenePath = AssetDatabase.GUIDToAssetPath(sceneGUIDs[0]);


        EditorSceneManager.OpenScene(initScenePath);


        StageManager manager = Object.FindAnyObjectByType<StageManager>();
        if (manager == null)
        {
            Debug.LogError("❌ Init 씬에서 StageManager를 찾을 수 없음.");
            return;
        }


        List<EditorBuildSettingsScene> newBuildScenes = new List<EditorBuildSettingsScene>();
        List<SceneList> stageLists = new List<SceneList>();
        stageLists.Add(manager.FW_StageList);
        stageLists.Add(manager.RL_StageList);
        stageLists.Add(manager.ST_StageList);
        stageLists.Add(manager.ES_StageList);
        stageLists.Add(manager.PCR_StageList);
        stageLists.Add(manager.DSG_StageList);
        HashSet<string> addedPaths = new HashSet<string>();

        foreach (var listSO in stageLists)
        {
            if (listSO == null) continue;
            if (listSO.sceneNames == null) continue;

            foreach (var sceneName in listSO.sceneNames)
            {
                if (string.IsNullOrEmpty(sceneName))
                    continue;


                string[] guids = AssetDatabase.FindAssets(sceneName + " t:Scene");
                if (guids.Length == 0)
                {
                    Debug.LogWarning($"⚠️ 씬 이름 '{sceneName}' 에 해당하는 씬 에셋을 찾을 수 없음.");
                    continue;
                }

                string path = AssetDatabase.GUIDToAssetPath(guids[0]);

                if (addedPaths.Contains(path))
                    continue;

                addedPaths.Add(path);
                newBuildScenes.Add(new EditorBuildSettingsScene(path, true));
            }
        }


        EditorBuildSettings.scenes = newBuildScenes.ToArray();

        Debug.Log($"✅ Build Settings 등록 완료! 총 {newBuildScenes.Count}개 씬 적용됨");
    }
}
