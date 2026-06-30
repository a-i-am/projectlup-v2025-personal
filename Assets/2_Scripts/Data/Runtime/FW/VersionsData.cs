using System;
using UnityEngine;

[Serializable]
public class VersionsData : BaseRuntimeData
{
    [SerializeField] private string _name = "Versions";
    [SerializeField] private string _Videohash = "";
    [SerializeField] private string _Audiohash = "";
    [SerializeField] private string _Imagehash = "";
    [SerializeField] private string _VFXhash = "";
    [SerializeField] private string _GUIhash = "";
    [SerializeField] private string _Modelhash = "";
    [SerializeField] private string _Shaderhash = "";
    [SerializeField] private string _Datahash = "";
    public string Videohash
    {
        get => _Videohash;
        set => SetValue(ref _Videohash, value);
    }
    public string Audiohash
    {
        get => _Audiohash;
        set => SetValue(ref _Audiohash, value);
    }
    public string Imagehash
    {
        get => _Imagehash;
        set => SetValue(ref _Imagehash, value);
    }
    public string VFXhash
    {
        get => _VFXhash;
        set => SetValue(ref _VFXhash, value);
    }
    public string GUIhash
    {
        get => _GUIhash;
        set => SetValue(ref _GUIhash, value);
    }
    public string Modelhash
    {
        get => _Modelhash;
        set => SetValue(ref _Modelhash, value);
    }
    public string Shaderhash
    {
        get => _Shaderhash;
        set => SetValue(ref _Shaderhash, value);
    }
    public string Datahash
    {
        get => _Datahash;
        set => SetValue(ref _Datahash, value);
    }

    public string name
    {
        get => _name;
        set => SetValue(ref _name, value);
    }

    public override void ResetData()
    {
        _name = "Versions";
        _Videohash = "";
        _Audiohash = "";
        _Imagehash = "";
        _VFXhash = "";
        _GUIhash = "";
        _Modelhash = "";
        _Shaderhash = "";
        _Datahash = "";
    }
}
