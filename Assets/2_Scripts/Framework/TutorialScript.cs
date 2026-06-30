using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour, IPointerClickHandler
{
    public int textIndex=0;
    public Text text;
    public TutorialStaticDataLoader tutorialStaticData;

    void Start()
    {
        List<BaseStaticDataLoader> dataList = LUP.ResourceManager.Instance.LoadStaticData(LUP.Define.StageKind.Tutorial, 1);
        if (dataList != null && dataList.Count > 0)
        {
            tutorialStaticData = dataList[0] as TutorialStaticDataLoader;
        }
    }


    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (textIndex >= tutorialStaticData.DataList.Count)
        {
            Destroy(gameObject);
        }
        else
        {
            text.text = tutorialStaticData.DataList[textIndex].description;
        }
        textIndex++;
    }
}
