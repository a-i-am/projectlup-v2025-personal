using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CurrentQuestListData : BaseRuntimeData
{
    [SerializeField] private string _name = "CurrentQuestListData";
    [SerializeField] private List<string> _questNames;
    [SerializeField] private List<int> _questamount;

    public List<string> questNames
    {
        get => _questNames;
        set => SetValue(ref _questNames, value);
    }

    public List<int> questamount
    {
        get => _questamount;
        set => SetValue(ref _questamount, value);
    }

    public string name
    {
        get => _name;
        set => SetValue(ref _name, value);
    }

    public override void ResetData()
    {
        _questNames = new List<string>();
        _name = "CurrentQuestListData";
    }
}
