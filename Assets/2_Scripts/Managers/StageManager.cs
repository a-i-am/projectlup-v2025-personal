using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LUP
{
    [Serializable]
    public class StageTransition
    {
        public Define.StageKind start;
        public Define.StageKind end;
    }

    public class StageManager : Singleton<StageManager>
    {
        [Header("스테이지들 설정하는 부분")]
        public SceneList FW_StageList;
        public SceneList RL_StageList;
        public SceneList ST_StageList;
        public SceneList ES_StageList;
        public SceneList PCR_StageList;
        public SceneList DSG_StageList;

        [Header("Fade Settings")]
        private CanvasGroup fadeCanvas;
        [SerializeField] private float fadeDuration = 1f;

        [Header("초기에 오픈될 스테이지 지정")]
        [SerializeField] private Define.StageKind startStageKind;

        [Header("건들지 마세요!!!")]
        [ReadOnly, SerializeField] private Define.StageKind currentStageKind = Define.StageKind.Unknown;

        [SerializeField,ReadOnly]
        private BaseStage currentStageInstance= null;
        private bool isTransitioning = false;


        private List<List<Define.StageKind>> transitionTable = new List<List<Define.StageKind>>();


        private Dictionary<Define.StageKind, SceneList> sceneNameMap = new Dictionary<Define.StageKind, SceneList>();

        public override void Awake()
        {
            base.Awake();

            InitializeTransitionTable();
            InitializeFadeCanvas();
            InitializeSceneMap();
            if (currentStageInstance == null)
            {
                LoadStage(startStageKind);
            }
        }

        private void InitializeFadeCanvas()
        {
            if (!fadeCanvas)
            {
                GameObject fadeObj = GameObject.Find("FadeCanvas");
                if (!fadeObj)
                {
                    fadeObj = new GameObject("FadeCanvas");

                    Canvas canvas = fadeObj.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.sortingOrder = 999;


                    UnityEngine.UI.CanvasScaler scaler = fadeObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                    scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);


                    fadeObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                    fadeCanvas = fadeObj.AddComponent<CanvasGroup>();

                    GameObject fadeImage = new GameObject("FadeImage");
                    fadeImage.layer = LayerMask.NameToLayer("UI");
                    fadeImage.transform.SetParent(fadeObj.transform, false);

                    UnityEngine.UI.Image image = fadeImage.AddComponent<UnityEngine.UI.Image>();
                    image.color = Color.black;
                    image.raycastTarget = false;


                    RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;


                    DontDestroyOnLoad(fadeObj);

                    Debug.Log("FadeCanvas created and set to DontDestroyOnLoad");
                }
                else
                {
                    fadeCanvas = fadeObj.GetComponent<CanvasGroup>();
                    Debug.Log("FadeCanvas found in scene");
                }
            }


            if (fadeCanvas)
            {
                fadeCanvas.alpha = 0f;
                fadeCanvas.blocksRaycasts = false;
            }
        }


        private void InitializeTransitionTable()
        {
            List<Define.StageKind> Transition = new List<Define.StageKind>();


            SetTransition(Transition, Define.StageKind.Unknown);
            SetTransition(Transition, Define.StageKind.Debug);
            SetTransition(Transition, Define.StageKind.Intro);
            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.RL);
            SetTransition(Transition, Define.StageKind.ST);
            SetTransition(Transition, Define.StageKind.DSG);
            SetTransition(Transition, Define.StageKind.ES);
            SetTransition(Transition, Define.StageKind.PCR);

            AddTransitionToList(Transition);
            Transition.Clear();



            SetTransition(Transition, Define.StageKind.Debug);
            SetTransition(Transition, Define.StageKind.Intro);
            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.RL);
            SetTransition(Transition, Define.StageKind.ST);
            SetTransition(Transition, Define.StageKind.DSG);
            SetTransition(Transition, Define.StageKind.ES);
            SetTransition(Transition, Define.StageKind.PCR);

            AddTransitionToList(Transition);
            Transition.Clear();


            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.RL);
            SetTransition(Transition, Define.StageKind.ST);
            SetTransition(Transition, Define.StageKind.DSG);
            SetTransition(Transition, Define.StageKind.ES);
            SetTransition(Transition, Define.StageKind.PCR);

            AddTransitionToList(Transition);
            Transition.Clear();


            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.Intro);

            AddTransitionToList(Transition);
            Transition.Clear();


            SetTransition(Transition, Define.StageKind.RL);
            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.Intro);
            SetTransition(Transition, Define.StageKind.PCR);

            AddTransitionToList(Transition);
            Transition.Clear();


            SetTransition(Transition, Define.StageKind.ST);
            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.Intro);
            SetTransition(Transition, Define.StageKind.PCR);

            AddTransitionToList(Transition);
            Transition.Clear();


            SetTransition(Transition, Define.StageKind.ES);
            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.Intro);
            SetTransition(Transition, Define.StageKind.PCR);

            AddTransitionToList(Transition);
            Transition.Clear();


            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.Intro);
            SetTransition(Transition, Define.StageKind.PCR);
            SetTransition(Transition, Define.StageKind.RL);
            SetTransition(Transition, Define.StageKind.ST);
            SetTransition(Transition, Define.StageKind.DSG);
            SetTransition(Transition, Define.StageKind.ES);
            SetTransition(Transition, Define.StageKind.PCR);

            AddTransitionToList(Transition);
            Transition.Clear();


            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.Intro);
            SetTransition(Transition, Define.StageKind.PCR);
            SetTransition(Transition, Define.StageKind.DSG);

            AddTransitionToList(Transition);
            Transition.Clear();


            SetTransition(Transition, Define.StageKind.Debug);
            SetTransition(Transition, Define.StageKind.Intro);
            SetTransition(Transition, Define.StageKind.Main);
            SetTransition(Transition, Define.StageKind.RL);
            SetTransition(Transition, Define.StageKind.ST);
            SetTransition(Transition, Define.StageKind.DSG);
            SetTransition(Transition, Define.StageKind.ES);
            SetTransition(Transition, Define.StageKind.PCR);

            AddTransitionToList(Transition);
            Transition.Clear();
        }

        private void SetTransition(List<Define.StageKind> from, Define.StageKind to)
        {
            from.Add(to);
        }

        private void AddTransitionToList(List<Define.StageKind> from)
        {
            List<Define.StageKind> list = new List<Define.StageKind>(from);
            transitionTable.Add(list);
        }


        public void LoadStage(Define.StageKind targetStageKind, int sceneindex = -1)
        {
            if (isTransitioning)
            {
                Debug.LogWarning("Already transitioning!");
                return;
            }


            if (!IsValidTransition(currentStageKind, targetStageKind))
            {
                Debug.LogError($"Invalid transition: {currentStageKind} → {targetStageKind}");
                return;
            }


            StartCoroutine(TransitionCoroutine(targetStageKind, sceneindex));
        }


        private bool IsValidTransition(Define.StageKind from, Define.StageKind to)
        {
            return transitionTable[(int)from].Contains(to);
        }


        private IEnumerator TransitionCoroutine(Define.StageKind targetStageKind, int sceneindex= -1)
        {
            isTransitioning = true;


            yield return StartCoroutine(OnStageExit());

            string sceneName;
            if (sceneindex == -1)
            {
                sceneName = sceneNameMap.ContainsKey(targetStageKind)
                ? sceneNameMap[targetStageKind].sceneNames[0]
                : targetStageKind.ToString();
            }
            else
            {
                sceneName = sceneNameMap[targetStageKind].sceneNames[sceneindex];
            }
            Debug.Log("SceneName:" + sceneName);




            if (SceneManager.GetSceneByName(sceneName).IsValid() == false &&
                SceneUtility.GetBuildIndexByScenePath(sceneName) == -1)
            {
                Debug.LogError($"Scene '{sceneName}' not found in Build Settings! Add it to File → Build Settings → Scenes In Build");
                isTransitioning = false;
                yield break;
            }

            UnityEngine.AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            if (asyncLoad == null)
            {
                Debug.LogError($"Failed to load scene '{sceneName}'");
                isTransitioning = false;
                yield break;
            }

            while (!asyncLoad.isDone)
            {

                float progress = asyncLoad.progress;
                yield return new WaitForSeconds(0.1f);
            }

            currentStageInstance = FindFirstObjectByType<BaseStage>();
            while (currentStageInstance == null)
            {
                currentStageInstance = FindFirstObjectByType<BaseStage>();
                Debug.Log("Find BaseStage");
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log(currentStageInstance);
            yield return StartCoroutine(OnStageEnter());

            currentStageKind = targetStageKind;
            isTransitioning = false;

            Debug.Log("TransitionCoroutine : " + currentStageKind);
        }

        private IEnumerator FadeOut()
        {
            if (!fadeCanvas)
            {
                Debug.LogError("FadeCanvas is null! This should not happen.");
                yield break;
            }

            Debug.Log($"FadeOut Start - FadeCanvas: {fadeCanvas.name}, Alpha: {fadeCanvas.alpha}, Active: {fadeCanvas.gameObject.activeSelf}");
            fadeCanvas.blocksRaycasts = true;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            fadeCanvas.alpha = 1f;
            Debug.Log($"FadeOut End - Alpha: {fadeCanvas.alpha}");
        }

        private IEnumerator FadeIn()
        {
            if (!fadeCanvas)
            {
                Debug.LogError("FadeCanvas is null! This should not happen.");
                yield break;
            }

            Debug.Log($"FadeIn Start - Alpha: {fadeCanvas.alpha}");
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = false;
            Debug.Log($"FadeIn End - Alpha: {fadeCanvas.alpha}, Active: {fadeCanvas.gameObject.activeSelf}");
        }

        private IEnumerator OnStageEnter()
        {
            if (currentStageInstance)
            {
                Debug.Log("OnStageEnter");
                yield return StartCoroutine(currentStageInstance.OnStageEnter());
            }
            yield return StartCoroutine(FadeIn());
        }

        private IEnumerator OnStageExit()
        {
            if (currentStageInstance)
            {
                Debug.Log("OnStageExit");
                yield return StartCoroutine(currentStageInstance.OnStageExit());
            }
            yield return StartCoroutine(FadeOut());
        }

        public BaseStage GetCurrentStage()
        {
            return currentStageInstance;
        }

        private void InitializeSceneMap()
        {
            sceneNameMap.Add(Define.StageKind.Debug, FW_StageList);
            sceneNameMap.Add(Define.StageKind.Intro, FW_StageList);
            sceneNameMap.Add(Define.StageKind.Main, FW_StageList);
            sceneNameMap.Add(Define.StageKind.RL, RL_StageList);
            sceneNameMap.Add(Define.StageKind.ST, ST_StageList);
            sceneNameMap.Add(Define.StageKind.DSG, DSG_StageList);
            sceneNameMap.Add(Define.StageKind.ES, ES_StageList);
            sceneNameMap.Add(Define.StageKind.PCR, PCR_StageList);
            sceneNameMap.Add(Define.StageKind.Unknown, FW_StageList);
        }
    }
}
