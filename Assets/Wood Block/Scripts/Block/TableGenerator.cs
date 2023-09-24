using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WoodBlock
{
    [RequireComponent(typeof(TableLayout))]
    public class TableGenerator : MonoBehaviour
    {
        [SerializeField] private List<Block> _blockTemplates = new();

        private TableLayout _tableLayout;

        private void Awake()
        {
            _tableLayout ??= GetComponent<TableLayout>();
            _tableLayout.AmountPoints = 3;
        }

        public void FillPoints()
        {
            foreach (TablePoint tablePoint in _tableLayout.Points)
            {
                if (tablePoint.IsAvailable)
                {
                    tablePoint.CreateBlock(_blockTemplates[Random.Range(0, _blockTemplates.Count)]);
                    tablePoint.DroppedBlock += OnDroppedBlock;
                }
            }
        }

        private void OnDroppedBlock()
        {
            
        }
    }
}