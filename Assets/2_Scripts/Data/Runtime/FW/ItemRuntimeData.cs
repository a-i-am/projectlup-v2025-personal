using System;
using UnityEngine;

[Serializable]
public class TestInvenRuntimeData : BaseRuntimeData
{
    [SerializeField] private string _slotKey;
    [SerializeField] private int _itemID;
    [SerializeField] private int _quantity = 0;

    public string slotKey
    {
        get => _slotKey;
        set => SetValue(ref _slotKey, value);
    }

    public int itemID
    {
        get => _itemID;
        set => SetValue(ref _itemID, value);
    }

    public int quantity
    {
        get => _quantity;
        set => SetValue(ref _quantity, value);
    }

    public override void ResetData()
    {
        _slotKey = "";
        _itemID = 0;
        _quantity = 0;
    }
}
