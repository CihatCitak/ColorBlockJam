using UnityEngine;

namespace Game.BlockSystem
{
    [CreateAssetMenu(menuName = "Game/Block Authoring Config", fileName = "BlockAuthoringConfig")]
    internal class BlockAuthoringConfig : ScriptableObject
    {
        [SerializeField] private Block _blockPrefab;
        [SerializeField] private GameObject _cellColliderPrefab;
        [SerializeField] private GameObject _centerPartPrefab;
        [SerializeField] private GameObject _edgePartPrefab;
        [SerializeField] private GameObject _outerCornerPartPrefab;
        [SerializeField] private GameObject _innerCornerPartPrefab;
        [SerializeField] private Material _outlineMaskMaterial;
        [SerializeField] private Material _outlineFillMaterial;

        public Block BlockPrefab => _blockPrefab;
        public GameObject CellColliderPrefab => _cellColliderPrefab;
        public Material OutlineMaskMaterial => _outlineMaskMaterial;
        public Material OutlineFillMaterial => _outlineFillMaterial;

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
