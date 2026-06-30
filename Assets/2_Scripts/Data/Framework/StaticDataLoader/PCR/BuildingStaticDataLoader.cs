using UnityEngine;

[CreateAssetMenu(fileName = "BuildingStaticData", menuName = "Scriptable Objects/BuildingStaticData")]
public class BuildingStaticDataLoader : BaseStaticDataLoader<BuildingStaticData>
{
    protected override string CSV_URL =>
        "https://docs.google.com/spreadsheets/d/1_8bIM_IAx1r9BhxDEX2Pn3jaf5O42iigmWJmDKKXs6k/export?format=csv&gid=55090956#gid=55090956";


}
