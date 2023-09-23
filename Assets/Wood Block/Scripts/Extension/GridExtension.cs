using System;
using UnityEngine;

namespace WoodBlock
{
    public static class GridExtension
    {
        public static Vector2Int GetCellPosition(this Cell[,] matrix, Cell origin)
        {
            for (int y = matrix.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if (matrix[x, y] == origin) return new Vector2Int(x, y);
                }
            }

            throw new ArgumentNullException($"{origin} is not finding in {nameof(matrix)}");
        }
    }
}