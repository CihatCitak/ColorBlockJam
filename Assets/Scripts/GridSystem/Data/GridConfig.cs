using UnityEngine;

namespace Game.GridSystem
{
    [CreateAssetMenu(menuName = "Game/Grid Config", fileName = "GridConfig")]
    internal class GridConfig : ScriptableObject
    {
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private float _wallThickness = 0.5f;
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private GameObject _wallCornerPrefab;
        [SerializeField] private GameObject _wallLCornerPrefab;

        public float CellSize => _cellSize;
        public float WallThickness => _wallThickness;
        public GameObject CellPrefab => _cellPrefab;

        public GameObject GetWallPrefab(WallPieceType type)
        {
            return type switch
            {
                WallPieceType.Corner => _wallCornerPrefab,
                WallPieceType.LCorner => _wallLCornerPrefab,
                _ => _wallPrefab
            };
        }
    }
}
