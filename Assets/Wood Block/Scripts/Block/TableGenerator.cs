using System.Collections.Generic;
using System.Linq;
using KimicuUtility;
using UnityEngine;
using UnityEngine.Events;

namespace WoodBlock
{
    [RequireComponent(typeof(TableLayout))]
    public class TableGenerator : MonoBehaviour
    {
        [SerializeField] private bool _fillOnAwake = true;
        [SerializeField] private List<ObjectChance<Block>> _blockTemplates = new();

        private TableLayout _tableLayout;

        public UnityEvent OnLooseCalledFromShitEvent;

        private void Awake()
        {
            _tableLayout ??= GetComponent<TableLayout>();

            _tableLayout.AmountPoints = 3;
            foreach (TablePoint point in _tableLayout.Points)
                point.DroppedBlock.AddListener(OnDroppedBlock);

            if (_fillOnAwake) TryFillPoints();
        }

        private void OnDroppedBlock()
        {
            TryFillPoints();
            if (_tableLayout.Points.Any()) CheckForLose();
        }

        private void CheckForLose()
        {
            var remainingDictionary = _tableLayout.Points
                .Where(point => point is { IsAvailable: false })
                .ToDictionary(point => point.CellsInBlock, point => point.CellsInBlock.First().Value);

            if (GridMap.Instance.TryLoss(remainingDictionary))
            {
                Debug.Log("You Lose!", this);
                OnLooseCalledFromShitEvent?.Invoke();
            }
        }

        public void TryFillPoints()
        {
            if(_tableLayout.Points == null) return;
            if (_tableLayout.Points.Where(p => p.IsAvailable).ToArray().Length != _tableLayout.AmountPoints)
                return;
            foreach (TablePoint tablePoint in _tableLayout.Points)
            {
                tablePoint.CreateBlock(_blockTemplates.RandomWithChance().Peek());
            }
        }
    }
}