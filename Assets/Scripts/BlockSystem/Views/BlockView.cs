using DG.Tweening;
using Game.Core.Events;
using Game.Events;
using Game.GridSystem;
using Game.InputSystem;
using UnityEngine;

namespace Game.BlockSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    internal class BlockView : MonoBehaviour, IDraggable, IBlock
    {
        private const int ExitClearanceCells = 1;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidthId = Shader.PropertyToID("_OutlineWidth");

        private static MaterialPropertyBlock SharedProperties;

        [SerializeField] private Rigidbody2D _body;
        [SerializeField] private Collider2D[] _cellColliders;
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Material[] _plainMaterials;
        [SerializeField] private Material[] _outlinedMaterials;

        private IGrid _grid;
        private BlockConfig _config;
        private BlockColorData _color;
        private ParticleSystem _crushEffect;
        private Vector2 _target;
        private Tween _snap;
        private bool _isExiting;
        private Vector2Int _exitDirection;
        private Vector2 _exitTarget;
        private float _gateDistance;

        public Vector2 Position => _body.position;
        public BlockColorData Color => _color;

        public RectInt Cells
        {
            get
            {
                var min = new Vector2Int(int.MaxValue, int.MaxValue);
                var max = new Vector2Int(int.MinValue, int.MinValue);
                foreach (var cellCollider in _cellColliders)
                {
                    var cell = _grid.WorldToCell(cellCollider.transform.position);
                    min = Vector2Int.Min(min, cell);
                    max = Vector2Int.Max(max, cell);
                }

                return new RectInt(min, max - min + Vector2Int.one);
            }
        }

        public void Build(BlockColorData color, Vector2Int originCell, IGrid grid, BlockConfig config, ParticleSystem crushEffect)
        {
            _grid = grid;
            _config = config;
            _color = color;
            _crushEffect = crushEffect;
            enabled = false;

            SharedProperties ??= new MaterialPropertyBlock();
            SharedProperties.SetColor(BaseColorId, color.Color);
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
            if (!_isExiting)
                _target = point;
        }

        public void EndDrag()
        {
            if (_isExiting)
                return;

            StopMoving();

            var target = _grid.CellToWorld(_grid.WorldToCell(_body.position));
            _snap = DOTween.To(() => _body.position, MoveTo, target, _config.SnapDuration)
                .SetEase(_config.SnapEase)
                .SetLink(gameObject);
        }

        public void Exit(Vector2Int direction, Vector2 gate)
        {
            if (_isExiting)
                return;

            _isExiting = true;
            _snap?.Kill();
            StopMoving();
            foreach (var cellCollider in _cellColliders)
                cellCollider.enabled = false;

            _exitDirection = direction;
            _gateDistance = Vector2.Dot(gate, direction);

            var cells = Cells;
            var length = direction.x != 0 ? cells.width : cells.height;
            var cell = _grid.WorldToCell(_body.position);
            _exitTarget = _grid.CellToWorld(cell + direction * (length + ExitClearanceCells));

            DOTween.To(() => _body.position, MoveTo, _grid.CellToWorld(cell), _config.SnapDuration)
                .SetEase(_config.SnapEase)
                .SetLink(gameObject)
                .OnComplete(Grind);
        }

        private void Grind()
        {
            DOTween.To(() => _body.position, MoveTo, _exitTarget, _config.ExitSpeed)
                .SetSpeedBased()
                .SetEase(_config.ExitEase)
                .SetLink(gameObject)
                .OnUpdate(CrushPartsPastGate)
                .OnComplete(OnExited);
        }

        private void FixedUpdate()
        {
            var velocity = (_target - _body.position) / Time.fixedDeltaTime;
            _body.velocity = Vector2.ClampMagnitude(velocity, _config.MoveSpeed);
        }

        private void StopMoving()
        {
            enabled = false;
            SetOutlineVisible(false);
            _body.velocity = Vector2.zero;
            _body.interpolation = RigidbodyInterpolation2D.None;
            _body.bodyType = RigidbodyType2D.Kinematic;
        }

        private void CrushPartsPastGate()
        {
            foreach (var partRenderer in _renderers)
            {
                if (partRenderer.enabled && Vector2.Dot(partRenderer.bounds.center, _exitDirection) >= _gateDistance)
                    Crush(partRenderer);
            }
        }

        private void Crush(Renderer partRenderer)
        {
            partRenderer.enabled = false;
            var emitParams = new ParticleSystem.EmitParams
            {
                position = partRenderer.bounds.center,
                startColor = _color.Color,
                applyShapeToPosition = true
            };
            _crushEffect.Emit(emitParams, _config.CrushParticlesPerPart);
        }

        private void OnExited()
        {
            foreach (var partRenderer in _renderers)
            {
                if (partRenderer.enabled)
                    Crush(partRenderer);
            }

            EventBus.Fire(new BlockExitedEvent());
            Destroy(gameObject);
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
