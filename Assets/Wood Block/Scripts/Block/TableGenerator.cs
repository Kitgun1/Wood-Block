using System.Collections.Generic;
using System.Linq;
using KimicuUtility;
using UnityEngine;
using UnityEngine.Events;

namespace WoodBlock
{
    public class TableGeneratorHistory
    {
        public TablePoint point;
        public Block prefab;
    }

    public sealed class TableGeneratorGenerationHistory
    {
        public Block[] prefabs;
    }

    [RequireComponent(typeof(TableLayout))]
    public class TableGenerator : MonoBehaviour
    {
        [SerializeField] private bool _fillOnAwake = true;
        [SerializeField] private List<ObjectChance<Block>> _blockTemplates = new();

        private readonly Stack<TableGeneratorHistory> _history = new();
        private readonly Stack<TableGeneratorGenerationHistory> _generationHistory = new();

        private TableLayout _tableLayout;
        public UnityEvent OnLooseCalledFromShitEvent;

        public static TableGenerator Instance { get; private set; }

        private void Awake()
        {
            _tableLayout = _tableLayout != null ? _tableLayout : GetComponent<TableLayout>();

            _tableLayout.AmountPoints = 3;
            foreach (TablePoint point in _tableLayout.Points)
            {
                point.DroppedBlock.AddListener(OnDroppedBlock);
                point.OnBlockDropped += OnBlockDropped;
            }

            if (_fillOnAwake) 
                TryFillPoints();

            Instance = this;
        }

        private void OnBlockDropped(TablePoint point)
        {
            _history.Push(new() { point = point, prefab = point.prefab });
        }
        
        private void OnDroppedBlock()
        {
            TryFillPoints();

            if (_tableLayout.Points.Any()) 
                CheckForLose();
        }

        private void CheckForLose()
        {
            var remainingDictionary = _tableLayout.Points
                .Where(point => point is { IsEmpty: false })
                .ToDictionary(point => point.CellsInBlock, point => point.CellsInBlock.First().Value);

            if (GridMap.Instance.TryLoss(remainingDictionary))
            {
                Debug.Log("You Lose!", this);
                OnLooseCalledFromShitEvent?.Invoke();
            }
        }

        public void TryFillPoints()
        {
            if(_tableLayout.Points == null) 
                return;

            foreach (var point in _tableLayout.Points)
                if (!point.IsEmpty)
                    return;

            if (_generationHistory.TryPop(out var history))
            {
                int index = 0;
                foreach (TablePoint tablePoint in _tableLayout.Points)
                {
                    tablePoint.CreateBlock(history.prefabs[index++]);
                }
            }
            else
            {
                foreach (TablePoint tablePoint in _tableLayout.Points)
                {
                    tablePoint.CreateBlock(_blockTemplates.RandomWithChance().Peek());
                }
            }
        }


        public void Undo()
        {
            var history = _history.Pop();

            if (!history.point.IsEmpty)
            {
                var genHistory = new List<Block>();
                foreach (TablePoint tablePoint in _tableLayout.Points)
                {
                    genHistory.Add(tablePoint.prefab);
                    tablePoint.Clear();
                }
                _generationHistory.Push(new() { prefabs = genHistory.ToArray() });
            }

            history.point.CreateBlock(history.prefab);
        }
    }
}