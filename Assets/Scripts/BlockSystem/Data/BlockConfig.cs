using DG.Tweening;
using UnityEngine;

namespace Game.BlockSystem
{
    [CreateAssetMenu(menuName = "Game/Block Config", fileName = "BlockConfig")]
    internal class BlockConfig : ScriptableObject
    {
        [SerializeField, Range(2f, 60f)] private float _moveSpeed = 20f;
        [SerializeField, Range(0f, 1f)] private float _snapDuration = 0.15f;
        [SerializeField] private Ease _snapEase = Ease.OutQuad;
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField, Range(0f, 10f)] private float _outlineWidth = 4f;
        [SerializeField] private Material _outlineMaskMaterial;
        [SerializeField] private Material _outlineFillMaterial;
        [SerializeField] private BlockView _blockPrefab;
        [SerializeField] private GameObject _cellColliderPrefab;
        [SerializeField] private GameObject _centerPartPrefab;
        [SerializeField] private GameObject _edgePartPrefab;
        [SerializeField] private GameObject _outerCornerPartPrefab;
        [SerializeField] private GameObject _innerCornerPartPrefab;

        public float MoveSpeed => _moveSpeed;
        public float SnapDuration => _snapDuration;
        public Ease SnapEase => _snapEase;
        public Color OutlineColor => _outlineColor;
        public float OutlineWidth => _outlineWidth;
        public Material OutlineMaskMaterial => _outlineMaskMaterial;
        public Material OutlineFillMaterial => _outlineFillMaterial;
        public BlockView BlockPrefab => _blockPrefab;
        public GameObject CellColliderPrefab => _cellColliderPrefab;

        public GameObject GetPartPrefab(BlockPartType type)
        {
            return type switch
            {
                BlockPartType.Edge => _edgePartPrefab,
                BlockPartType.OuterCorner => _outerCornerPartPrefab,
                BlockPartType.InnerCorner => _innerCornerPartPrefab,
                _ => _centerPartPrefab
            };
        }
    }
}
