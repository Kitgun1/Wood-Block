using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WoodBlock
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TileBase _tileBase;
        [SerializeField] private LevelMap _loadMap;

        [Button(nameof(Load) + " and " + nameof(ClearMap))]
        private void Load()
        {
            ClearMap();
            var positions = _loadMap.GetPositions();

            int minX = positions.Max(v => v.x);
            int minY = positions.Max(v => v.y);


            foreach (Vector2Int position in _loadMap.GetPositions())
            {
                Vector2Int newPos =
                    position - new Vector2Int(Mathf.FloorToInt(minX / 2 + 1), Mathf.FloorToInt(minY / 2 + 1));
                _tilemap.SetTile((Vector3Int)newPos, _tileBase);
            }
        }

        [Button(nameof(ClearMap))]
        private void ClearMap()
        {
            _tilemap.ClearAllTiles();
        }
    }
}