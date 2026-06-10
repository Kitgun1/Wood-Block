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
        Transform child = null;
        if (isQuest && _rootOfQuestContainer.transform.childCount > 0)
        {
            child = _rootOfQuestContainer.transform.GetChild(0);
        }
        else
        {
            child = _rootOfQuestContainer.transform;
        }

        if (child != null)
        {
            var obj = Instantiate(child, Vector3.zero, Quaternion.identity, _losePanelRoot);
            if (obj.GetComponent<UISizeController>() != null)
            {
                Destroy(obj.GetComponent<UISizeController>());
            }
            obj.GetComponent<RectTransform>().localPosition = new Vector3(0, -60, 0);

            if (isQuest && _rootOfQuestContainer.transform.childCount > 0)
                obj.GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;
        }
    }
    public void OnWin()
    {
        if (_rootOfQuestContainer.transform.childCount > 0)
        {
            var child = _rootOfQuestContainer.transform.GetChild(0);
            var obj = Instantiate(child, Vector3.zero, Quaternion.identity, _winPanelRoot);
            if (obj.GetComponent<UISizeController>() != null)
            {
                Destroy(obj.GetComponent<UISizeController>());
            }
            obj.GetComponent<RectTransform>().localPosition = new Vector3(0, -60, 0);
            obj.GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;
        }
        else
        {
            Debug.LogWarning("No quests in quest container to display on Win Panel.");
        }
    }
}
