using UnityEngine;

[CreateAssetMenu(fileName = "PCRConstructionStaticData", menuName = "Scriptable Objects/PCRConstructionStaticData")]
public class PCRConstructionStaticDataLoader : BaseStaticDataLoader<PCRConstructionStaticData>
{
    protected override string CSV_URL =>
        "https://docs.google.com/spreadsheets/d/1_8bIM_IAx1r9BhxDEX2Pn3jaf5O42iigmWJmDKKXs6k/export?format=csv&gid=333375154#gid=333375154";
}
