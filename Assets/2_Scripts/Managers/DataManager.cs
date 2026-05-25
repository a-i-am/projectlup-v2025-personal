using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUP
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField]
        List<BaseStaticDataLoader> dataList;

        public List<BaseStaticDataLoader> GetStaticData(Define.StageKind stagekind, int stagetype)
        {
            List<BaseStaticDataLoader> datas = ResourceManager.Instance.LoadStaticData(stagekind, stagetype);

            if (datas == null || datas.Count == 0)
            {
                Debug.LogError($"Failed to load static data");
                return null;
            }

            return datas;
        }

        public List<BaseRuntimeData> GetRuntimeData(Define.StageKind stagekind, int stagetype)
        {
            List<BaseRuntimeData> runtimeDataList = new List<BaseRuntimeData>();
            BaseRuntimeData data = null;

            string filename = "";

            switch (stagekind)
            {
                case LUP.Define.StageKind.RL:
                    filename = Define.RuntimeDataTypes.ToFilename(Define.RuntimeDataType.RoguelikeRuntime);
                    //data = JsonDataHelper.LoadData<RoguelikeRuntimeData>(filename);
                    break;
                case LUP.Define.StageKind.DSG:
                    switch (stagetype)
                    {
                        case 1:
                            filename = Define.RuntimeDataTypes.ToFilename(Define.RuntimeDataType.DeckStrategyRuntime);
                            //data = JsonDataHelper.LoadData<DeckStrategyRuntimeData>(filename);
                            break;
                        case 2:
                            filename = Define.RuntimeDataTypes.ToFilename(Define.RuntimeDataType.DSGEnemyRuntime);
                            //data = JsonDataHelper.LoadData<DSGEnemyStageRuntimeData>(filename);
                            break;
                    }
                    break;
                case LUP.Define.StageKind.Main:
                    switch (stagetype)
                    {
                        case 1:
                            filename = Define.RuntimeDataTypes.ToFilename(Define.RuntimeDataType.Versions);
                            data = JsonDataHelper.LoadData<VersionsData>(filename);
                            break;
                        case 2:
                            filename = Define.RuntimeDataTypes.ToFilename(Define.RuntimeDataType.QuestList);
                            data = JsonDataHelper.LoadData<CurrentQuestListData>(filename);
                            break;
                    }
                    break;
                case LUP.Define.StageKind.PCR:
                    filename = Define.RuntimeDataTypes.ToFilename(Define.RuntimeDataType.ProductionRuntime);
                    data = JsonDataHelper.LoadData<ProductionRuntimeData>(filename);
                    break;
                case LUP.Define.StageKind.ES:
                    filename = Define.RuntimeDataTypes.ToFilename(Define.RuntimeDataType.ExtractionShooterRuntime);
                    //data = JsonDataHelper.LoadData<ExtractionRuntimeData>(filename);
                    break;
                case LUP.Define.StageKind.ST:
                    filename = Define.RuntimeDataTypes.ToFilename(Define.RuntimeDataType.ShootingRuntime);
                    //data = JsonDataHelper.LoadData<ShootingRuntimeData>(filename);
                    break;
                default:
                    Debug.LogError($"No runtime data defined for StageKind: { stagekind}");
                    return null;
            }

            if (data == null)
            {
                Debug.LogError($"Failed to load runtime data");
                return null;
            }

            data.filename = filename;

            if (!JsonDataHelper.FileExists(filename))
            {
                data.ResetData();
                JsonDataHelper.SaveData(data, filename);
                Debug.Log($"[DataManager] 새 런타임 데이터 생성 및 저장: {filename}");
            }

            runtimeDataList.Add(data);

            return runtimeDataList;
        }

        public override void Awake()
        {
            base.Awake();

            // RuntimeData가 코루틴을 사용할 수 있도록 설정
            BaseRuntimeData.SetCoroutineRunner(this);
        }

        public void SaveRuntimeData(BaseRuntimeData runtimeData)
        {
            JsonDataHelper.SaveData(runtimeData, runtimeData.filename);
        }

        public void SaveRuntimeDataList(List<BaseRuntimeData> runtimeDataList)
        {
            if (runtimeDataList == null || runtimeDataList.Count == 0)
            {
                Debug.LogWarning("SaveRuntimeDataList: No runtime data to save");
                return;
            }

            foreach (var data in runtimeDataList)
            {
                if (data != null)
                    JsonDataHelper.SaveData(data, data.filename);
            }
        }
    }
}
