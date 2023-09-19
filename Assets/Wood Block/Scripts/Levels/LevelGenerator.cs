using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WoodBlock
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private bool _indexUp = true;
        [SerializeField] private int _level = 1;


        [Button(nameof(Generate))]
        private void Generate()
        {
            _tilemap.RefreshAllTiles();
            _tilemap.ResizeBounds();
            var worldPositions = new List<Vector2Int>();

            BoundsInt bounds = _tilemap.cellBounds;
            TileBase[] allTiles = _tilemap.GetTilesBlock(bounds);

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null)
                    {
                        worldPositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            LevelMap map = ScriptableObject.CreateInstance<LevelMap>();
            int minX = worldPositions.Min(v => v.x);
            int minY = worldPositions.Min(v => v.y);
            for (int i = 0; i < worldPositions.Count; i++)
            {
                worldPositions[i] -= new Vector2Int(minX, minY);
            }

            map.AddCells(worldPositions);
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(map, $"Assets/Wood Block/Configs/Maps/Level {_level}.asset");
#endif
            if(_indexUp) _level++;
        }
    }
}