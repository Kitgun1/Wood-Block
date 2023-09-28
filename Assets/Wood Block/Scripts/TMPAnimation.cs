using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using KiUtility;

namespace WoodBlock
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMPAnimation : MonoBehaviour
    {
        [SerializeField] private float _stepDuration;
        [SerializeField, ResizableTextArea] private List<string> _steps = new();

        private readonly KiCoroutine _routine = new();

        private TMP_Text _tmpText;
        private int _currentStep = 0;

        private void Awake()
        {
            _tmpText ??= GetComponent<TMP_Text>();
            _routine.StartRoutine(KiCoroutine.Delay(_stepDuration, StepEnded));
        }

        private void StepEnded()
        {
            if (_currentStep >= _steps.Count) _currentStep = 0;
            _tmpText.text = _steps[_currentStep];
            _currentStep++;
            _routine.StartRoutine(KiCoroutine.Delay(_stepDuration, StepEnded), true);
        }
    }
}