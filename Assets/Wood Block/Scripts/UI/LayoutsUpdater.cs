using System;
using System.Collections;
using System.Linq;
using KimicuUtility;
using UnityEngine;
using UnityEngine.UI;

namespace WoodBlock
{
    public class LayoutsUpdater : MonoBehaviour
    {
        private readonly KiCoroutine _routine = new();
        private static Transform _transform;

        private void Awake() => _transform = transform;
        private void OnEnable() => _routine.StartRoutine(DelayLayouts(), true);
        private void OnDisable() => _routine.StopRoutine();


        private static IEnumerator DelayLayouts()
        {
            while (true)
            {
                UpdateAllLayouts();
                yield return new WaitForSeconds(1f);
            }
        }

        public static void UpdateAllLayouts()
        {
            var layouts = _transform.GetComponentsInChildren<LayoutGroup>();
            var rects = layouts.Select(layout => layout.GetComponent<RectTransform>()).ToList();
            foreach (RectTransform rect in rects) LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}