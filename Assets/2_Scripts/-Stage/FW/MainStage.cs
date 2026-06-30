using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LUP
{
    public class MainStage : BaseStage
    {

        public AudioSource SFX;
        public AudioSource BGM;
        public float soundVolume= 0;
        public Slider slider;
        [SerializeField]
        private VersionsData versionsdata;
        [SerializeField]
        private AssetBundle AB;
        [SerializeField]
        private AssetBundleManifest AB_Manifest;
        [SerializeField]
        private CurrentQuestListData currentQuestListData;

        [SerializeField] private CreditPanel creditPanel;

        public Inventory inven;

        protected override void Awake()
        {
            base.Awake();
            StageKind = Define.StageKind.Main;

        }


        void Start()
        {
            slider.onValueChanged.AddListener(SetAudioVolume);
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                versionsdata.SaveData();
                currentQuestListData.SaveData();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                QuestManager.Instance.SaveActiveQuests();
                CheckVersions();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                QuestManager.Instance.Trigger(123, 1);
                SoundManager.Instance.PlayBGM("RPG Combat 1 - Duel of the Fates (Loopable)", true);
            }
        }

        public override IEnumerator OnStageEnter()
        {
            yield return base.OnStageEnter();

            yield return null;
        }

        public override IEnumerator OnStageStay()
        {
            yield return base.OnStageStay();

            yield return null;
        }
        public override IEnumerator OnStageExit()
        {
            yield return base.OnStageExit();



            yield return null;
        }
        protected override void LoadResources()
        {







        }

        protected override void GetDatas()
        {
            List<BaseRuntimeData> runtimeDatas = base.GetRuntimeData(this, 1);

            if (runtimeDatas != null && runtimeDatas.Count > 0)
            {
                foreach (var runtimeData in runtimeDatas)
                {
                    if (runtimeData is VersionsData versionData)
                    {
                        versionsdata = versionData;
                    }
                }
            }

            runtimeDatas = base.GetRuntimeData(this, 2);
            if (runtimeDatas != null && runtimeDatas.Count > 0)
            {
                foreach (var runtimeData in runtimeDatas)
                {
                    if (runtimeData is CurrentQuestListData versionData)
                    {
                        currentQuestListData = versionData;
                    }
                }
            }
        }

        protected override void SaveDatas()
        {
            List<BaseRuntimeData> runtimeDataList = new List<BaseRuntimeData>();

            if (versionsdata != null)
            {
                runtimeDataList.Add(versionsdata);
            }

            base.SaveRuntimeDataList(runtimeDataList);
        }


        void SetAudioVolume(float value)
        {
            Debug.LogFormat("SoundVolume : {0}", value);

            LUP.SoundManager.Instance.SetBGMVolume(slider.value);
            LUP.SoundManager.Instance.SetSFXVolume(slider.value);
        }

        public void PlaySFX()
        {
            LUP.SoundManager.Instance.PlaySFX("SampleSFX");
        }
        public void PlayBGM()
        {
            LUP.SoundManager.Instance.PlayBGM("SampleBGM");
        }
        public void StopBGM()
        {
            LUP.SoundManager.Instance.StopBGM();
        }

        public void OnCreditsButtonClicked()
        {
            if (creditPanel != null)
            {
                creditPanel.Show();
            }
            else
            {
                Debug.LogError("MainStage: CreditPanel is not assigned!");
            }
        }

        public void CheckVersions()
        {
            AB = ResourceManager.Instance.GetAssetBundle(Define.AssetBundleKind.Manifest);

            AB_Manifest = AB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            versionsdata.Videohash = AB_Manifest.GetAssetBundleHash("videos").ToString();

            versionsdata.Audiohash = AB_Manifest.GetAssetBundleHash("audios").ToString();

            versionsdata.Imagehash = AB_Manifest.GetAssetBundleHash("image").ToString();

            versionsdata.VFXhash = AB_Manifest.GetAssetBundleHash("vfx").ToString();

            versionsdata.GUIhash = AB_Manifest.GetAssetBundleHash("gui").ToString();

            versionsdata.Modelhash = AB_Manifest.GetAssetBundleHash("models").ToString();

            versionsdata.Shaderhash = AB_Manifest.GetAssetBundleHash("shaders").ToString();

            versionsdata.Datahash = AB_Manifest.GetAssetBundleHash("data").ToString();

            versionsdata.SaveData();
        }
    }
}

