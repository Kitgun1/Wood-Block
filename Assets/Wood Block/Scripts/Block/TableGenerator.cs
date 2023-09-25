using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WoodBlock
{
    [RequireComponent(typeof(TableLayout))]
    public class TableGenerator : MonoBehaviour
    {
        [SerializeField] private bool _fillOnAwake = true;
        [SerializeField] private List<Block> _blockTemplates = new();

        private TableLayout _tableLayout;

        private void Awake()
        {
            _tableLayout ??= GetComponent<TableLayout>();

            _tableLayout.AmountPoints = 3;
            foreach (TablePoint point in _tableLayout.Points)
                point.DroppedBlock.AddListener(OnDroppedBlock);

            if (_fillOnAwake) FillPoints();
        }

        private void OnDroppedBlock()
        {
            FillPoints();
        }

        public void FillPoints()
        {
            if (_tableLayout.Points.Where(p => p.IsAvailable).ToArray().Length == _tableLayout.AmountPoints)
            {
                foreach (TablePoint tablePoint in _tableLayout.Points)
                {
                    tablePoint.CreateBlock(_blockTemplates[Random.Range(0, _blockTemplates.Count)]);
                    
                }
            }
        }
    }
}