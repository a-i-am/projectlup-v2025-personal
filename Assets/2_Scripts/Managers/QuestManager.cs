using LUP;
using System.Collections.Generic;
using UnityEngine;
namespace LUP
{
    public class QuestManager : Singleton<QuestManager>
    {
        [SerializeField]
        private List<BaseQuest> allQuest = new List<BaseQuest>();
        [SerializeField]
        private List<BaseQuest> activeQuests = new List<BaseQuest>();
        [SerializeField]
        private CurrentQuestListData currentQuestListData = null;
        private void Awake()
        {
            base.Awake();
            LoadActiveQuest();
        }

        public void AddQuest(BaseQuest quest)
        {

            activeQuests.Add(quest);
            quest.Initialize();
        }

        public void Trigger(int questTargetId, int value)
        {
            foreach (var quest in activeQuests)
            {
                quest.OnTargetTriggered(questTargetId, value);
            }
        }
        public void CompleteQuest(BaseQuest quest)
        {
            Debug.Log($"퀘스트 종료 처리: {quest.name}");
            activeQuests.Remove(quest);
        }

        private void LoadActiveQuest()
        {
            allQuest.Clear();
            activeQuests.Clear();

            BaseQuest[] quests = Resources.LoadAll<BaseQuest>("Data/Quests");
            allQuest.AddRange(quests);


            List<BaseRuntimeData> runtimeDatas = LUP.DataManager.Instance.GetRuntimeData(Define.StageKind.Main, 2);
            if (runtimeDatas != null && runtimeDatas.Count > 0)
            {
                foreach (var runtimeData in runtimeDatas)
                {
                    if (runtimeData is CurrentQuestListData QuestData)
                    {
                        currentQuestListData = QuestData;
                        break;
                    }
                }
            }

            if (currentQuestListData == null || currentQuestListData.questNames == null)
            {
                Debug.Log("[QuestManager] CurrentQuestListData 없음. 활성 퀘스트 없음");
                return;
            }

            for (int i = 0; i < currentQuestListData.questNames.Count; i++)
            {
                string questName = currentQuestListData.questNames[i];
                BaseQuest found = allQuest.Find(q => q.QuestName == questName);

                if (found != null)
                {
                    found.CurrentAmounts = new int[found.Goals.Length];
                    int count = Mathf.Min(found.Goals.Length, currentQuestListData.questamount.Count);
                    for (int j = 0; j < count; j++)
                    {
                        found.CurrentAmounts[j] = currentQuestListData.questamount[j];
                    }
                    activeQuests.Add(found);
                }
                else
                {
                    Debug.LogWarning($"저장된 퀘스트 '{questName}' 를 Resources에서 찾지 못함.");
                }
            }

        }
        public void SaveActiveQuests()
        {
            if (currentQuestListData == null)
            {
                Debug.LogError("[QuestManager] currentQuestListData 없음 – 저장 불가");
                return;
            }

            currentQuestListData.questNames = new List<string>();
            currentQuestListData.questamount = new List<int>();

            foreach (BaseQuest quest in activeQuests)
            {

                currentQuestListData.questNames.Add(quest.QuestName);


                if (quest.CurrentAmounts != null)
                {
                    for (int i = 0; i < quest.CurrentAmounts.Length; i++)
                    {
                        currentQuestListData.questamount.Add(quest.CurrentAmounts[i]);
                    }
                }
                else
                {

                    for (int i = 0; i < quest.Goals.Length; i++)
                    {
                        currentQuestListData.questamount.Add(0);
                    }
                }
            }

            Debug.Log("[QuestManager] 현재 퀘스트 저장 완료");
        }
    }
}

