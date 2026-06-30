using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace LUP
{
    public class IntroStage : BaseStage
    {

        public VideoPlayer videoplayer;
        public VideoClip clip;
        public Slider videoVolume;
        public AudioSource audioSource;
        void SetVideoClip(VideoClip clip)
        {
            videoplayer.clip = clip;
            videoplayer.audioOutputMode = VideoAudioOutputMode.AudioSource;


            videoplayer.EnableAudioTrack(0, true);
            videoplayer.SetDirectAudioMute(0, false);


            videoplayer.SetTargetAudioSource(0, audioSource);


            audioSource.volume = videoVolume.value;
        }
        void Awake()
        {
            StageKind = Define.StageKind.Intro;
        }
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            videoVolume.onValueChanged.AddListener(SetVideoVolume);
        }

        private void Update()
        {

        }

        public override IEnumerator OnStageEnter()
        {
            yield return base.OnStageEnter();


            SetVideoClip(clip);
            videoplayer.Play();
            yield return null;
        }


        void SetVideoVolume(float value)
        {
            Debug.LogFormat("VideoVolume : {0}", value);
            videoplayer.SetDirectAudioVolume(0,value);
            audioSource.volume = videoVolume.value;
        }

        protected override void LoadResources()
        {
            clip = ResourceManager.Instance.LoadVideoClip<VideoClip>("SampleVideo2");

        }

        protected override void GetDatas()
        {

        }

        protected override void SaveDatas()
        {

        }
    }
}

