using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGoalForPanels : MonoBehaviour
{
    [SerializeField] private GameObject _rootOfQuestContainer;
    [SerializeField] private Transform _winPanelRoot;
    [SerializeField] private Transform _losePanelRoot;

    public void OnLose(bool isQuest = true)
    {
        Transform child;
        if (isQuest)
            child = _rootOfQuestContainer.transform.GetChild(0);
        else
            child = _rootOfQuestContainer.transform;

        var obj = Instantiate(child, Vector3.zero,Quaternion.identity, _losePanelRoot);
        Destroy(obj.GetComponent<UISizeController>());
        obj.GetComponent<RectTransform>().localPosition = new Vector3(0, -60, 0);

        if(isQuest)
            obj.GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;
    }
    public void OnWin()
    {
        var child = _rootOfQuestContainer.transform.GetChild(0);
        var obj = Instantiate(child, Vector3.zero, Quaternion.identity, _winPanelRoot);
        Destroy(obj.GetComponent<UISizeController>());
        obj.GetComponent<RectTransform>().localPosition = new Vector3(0, -60, 0);
        obj.GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;
    }
}
