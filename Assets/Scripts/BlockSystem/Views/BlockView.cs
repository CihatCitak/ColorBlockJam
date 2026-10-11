using DG.Tweening;
using Game.GridSystem;
using Game.InputSystem;
using UnityEngine;

namespace Game.BlockSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    internal class BlockView : MonoBehaviour, IDraggable
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidthId = Shader.PropertyToID("_OutlineWidth");

        private static MaterialPropertyBlock SharedProperties;

        [SerializeField] private Rigidbody2D _body;
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Material[] _plainMaterials;
        [SerializeField] private Material[] _outlinedMaterials;

        private IGrid _grid;
        private BlockConfig _config;
        private Vector2 _target;
        private Tween _snap;

        public Vector2 Position => _body.position;

        public void Build(Color color, Vector2Int originCell, IGrid grid, BlockConfig config)
        {
            _grid = grid;
            _config = config;
            enabled = false;

            SharedProperties ??= new MaterialPropertyBlock();
            SharedProperties.SetColor(BaseColorId, color);
            SharedProperties.SetColor(OutlineColorId, config.OutlineColor);
            SharedProperties.SetFloat(OutlineWidthId, config.OutlineWidth);

            foreach (var partRenderer in _renderers)
                partRenderer.SetPropertyBlock(SharedProperties);

            MoveTo(grid.CellToWorld(originCell));
        }

        public void BeginDrag()
        {
            _snap?.Kill();
            SetOutlineVisible(true);
            _target = _body.position;
            _body.bodyType = RigidbodyType2D.Dynamic;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;
            enabled = true;
        }

        public void DragTo(Vector2 point)
        {
            _target = point;
        }

        public void EndDrag()
        {
            enabled = false;
            SetOutlineVisible(false);
            _body.velocity = Vector2.zero;
            _body.interpolation = RigidbodyInterpolation2D.None;
            _body.bodyType = RigidbodyType2D.Kinematic;

            var target = _grid.CellToWorld(_grid.WorldToCell(_body.position));
            _snap = DOTween.To(() => _body.position, MoveTo, target, _config.SnapDuration)
                .SetEase(_config.SnapEase)
                .SetLink(gameObject);
        }

        private void FixedUpdate()
        {
            var velocity = (_target - _body.position) / Time.fixedDeltaTime;
            _body.velocity = Vector2.ClampMagnitude(velocity, _config.MoveSpeed);
        }

        private void SetOutlineVisible(bool visible)
        {
            var materials = visible ? _outlinedMaterials : _plainMaterials;
            foreach (var partRenderer in _renderers)
                partRenderer.sharedMaterials = materials;
        }

        private void MoveTo(Vector2 position)
        {
            _body.position = position;
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
    }
}
