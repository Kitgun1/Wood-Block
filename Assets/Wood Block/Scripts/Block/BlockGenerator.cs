using System;
using System.Linq;
using KimicuUtility;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WoodBlock
{
    #region Custom Types

    [Serializable]
    public class DictionaryVector2CellInBlock : SerializableDictionary<Vector2, CellInBlock>
    {
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DictionaryVector2CellInBlock))]
    public class MyDictionaryExampleDrawer : DictionaryDrawer<Vector2, CellInBlock>
    {
    }
#endif

    #endregion

    public class BlockGenerator : MonoBehaviour
    {
        [SerializeField, ReadOnly] private DictionaryVector2CellInBlock _cells = new();

        public DictionaryVector2CellInBlock GetCells() => _cells;

        [Button(nameof(GenerateCells))]
        private void GenerateCells()
        {
            _cells.Clear();

            var cells = transform.GetComponentsInChildren<CellInBlock>();
            var localPositions = cells.Select(cellInBlock => cellInBlock.transform.localPosition).ToList();

            Vector3 minPos = Vector3.one * 100;
            minPos = localPositions.Aggregate((v1, v2) => Mathf.Min(Vector3.Distance(v1, v2),
                Vector3.Distance(v1, minPos)) == Vector3.Distance(v1, minPos)
                ? v1
                : v2);
            Vector3 offset = Vector3.zero - new Vector3(minPos.x, -minPos.y);

            for (int i = 0; i < localPositions.Count; i++)
            {
                localPositions[i] += offset;
                Vector2Int pos = new((int)localPositions[i].x, (int)localPositions[i].y);
                _cells.Add(pos, cells[i]);
            }
        }
    }
}