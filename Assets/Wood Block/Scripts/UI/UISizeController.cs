using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISizeController : MonoBehaviour
{
    [SerializeField] private RectTransform targetRect;
    [SerializeField] private float referenceWidth = 1920f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private QuestUI _questUI;


    private void Update()
    {
        float currentWidth = Screen.width;

            if (_questUI.Type == QuestType.CollectTimed)
            {
                float scale = referenceWidth / currentWidth;
                scale = Mathf.Clamp(scale, minScale, maxScale);

                targetRect.localScale = new Vector3(scale, scale, 1f);
            }
            else
            {

                float scale = referenceWidth / currentWidth;
                scale = Mathf.Clamp(scale, minScale, maxScale);
                scale *= 1.15f;
                targetRect.localScale = new Vector3(scale, scale, 1f);
            }
    }
}
