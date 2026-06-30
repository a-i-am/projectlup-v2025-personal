using UnityEngine;

[CreateAssetMenu(fileName = "TutorialStaticData", menuName = "Scriptable Objects/TutorialStaticData")]
public class TutorialStaticDataLoader : BaseStaticDataLoader<TutorialStaticData>
{
    protected override string CSV_URL =>
        "https://docs.google.com/spreadsheets/d/11yM9l6g4opxVTflwsOVV0nZoIPUQ9VnA0rhkasLEi7I/export?format=csv&gid=1504098664";
}
