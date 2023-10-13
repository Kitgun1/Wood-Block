using System;
using TMPro;
using UnityEngine;

namespace WoodBlock
{
    public abstract class ValueDisplay : MonoBehaviour
    {
        [SerializeField] protected TMP_Text ValueTMP;

        protected abstract void Start();

        protected abstract void UpdateText(int value);
    }
}