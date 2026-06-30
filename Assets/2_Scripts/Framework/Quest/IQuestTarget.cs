using UnityEngine;

namespace LUP
{
    public interface IQuestTarget
    {





        int QuestTargetId { get; }
        void Trigger(int value);
    }
}

