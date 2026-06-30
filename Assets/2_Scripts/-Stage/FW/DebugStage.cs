using UnityEngine;

namespace LUP
{
    public class DebugStage : BaseStage
    {
        public Define.StageKind TargetStage = Define.StageKind.Main;


        protected override void Awake()
        {
            base.Awake();
            StageKind = Define.StageKind.Debug;

        }

        void Start()
        {

        }

        void Update()
        {

        }


        protected override void LoadResources()
        {

        }

        protected override void GetDatas()
        {

        }

        protected override void SaveDatas()
        {

        }


    }
}

