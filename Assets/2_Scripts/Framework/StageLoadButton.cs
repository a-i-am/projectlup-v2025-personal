using LUP;
using UnityEngine;

public class StageLoadButton : MonoBehaviour
{
    public void LoadStage(int stage)
    {
        Debug.Log(stage);
        LUP.Define.StageKind stageKind = (LUP.Define.StageKind)stage;
        LUP.StageManager.Instance.GetCurrentStage().LoadStage(stageKind);
    }
}
