using UnityEngine;

[CreateAssetMenu(fileName = "PCRProductionStaticData", menuName = "Scriptable Objects/PCRProductionStaticData")]
public class PCRProductionStaticDataLoader : BaseStaticDataLoader<PCRProductionStaticData>
{
    protected override string CSV_URL =>
        "https://docs.google.com/spreadsheets/d/1_8bIM_IAx1r9BhxDEX2Pn3jaf5O42iigmWJmDKKXs6k/export?format=csv&gid=1913495394#gid=1913495394";
}
