using UnityEngine;

[CreateAssetMenu(fileName = "InitialWallStaticData", menuName = "Scriptable Objects/InitialWallStaticData")]
public class InitialWallStaticDataLoader : BaseStaticDataLoader<InitialWallStaticData>
{
    protected override string CSV_URL =>
        "https://docs.google.com/spreadsheets/d/1_8bIM_IAx1r9BhxDEX2Pn3jaf5O42iigmWJmDKKXs6k/export?format=csv&gid=1589778283";

}
