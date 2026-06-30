using UnityEngine;

[CreateAssetMenu(fileName = "InitialBuildingStaticData", menuName = "Scriptable Objects/InitialBuildingStaticData")]
public class InitialBuildingStaticDataLoader : BaseStaticDataLoader<InitialBuildingStaticData>
{
    protected override string CSV_URL =>
        "https://docs.google.com/spreadsheets/d/1_8bIM_IAx1r9BhxDEX2Pn3jaf5O42iigmWJmDKKXs6k/export?format=csv&gid=541251900#gid=541251900";


}
