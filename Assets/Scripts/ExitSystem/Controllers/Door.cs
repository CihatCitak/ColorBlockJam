using Game.BlockSystem;
using UnityEngine;

namespace Game.ExitSystem
{
    internal class Door : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private static MaterialPropertyBlock SharedProperties;

        [SerializeField] private Renderer[] _renderers;

        private ExitPlacement _placement;
        private Vector2Int _outward;
        private Vector2 _gate;
        private Rigidbody2D _touchingBody;
        private IBlock _touchingBlock;

        public void Setup(ExitPlacement placement, Vector2 gate)
        {
            _placement = placement;
            _outward = ExitGeometry.Outward(placement.Side);
            _gate = gate;

            SharedProperties ??= new MaterialPropertyBlock();
            SharedProperties.SetColor(BaseColorId, placement.Color.Color);

            foreach (var doorRenderer in _renderers)
                doorRenderer.SetPropertyBlock(SharedProperties);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var body = collision.rigidbody;
            if (body == null || !body.TryGetComponent(out IBlock block))
                return;

            _touchingBody = body;
            _touchingBlock = block;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_touchingBlock == null || collision.rigidbody != _touchingBody)
                return;

            if (ExitFit.Fits(_touchingBlock.Color, _touchingBlock.Cells, _placement.Color, _placement.Cell, _placement.Length, _outward))
                _touchingBlock.Exit(_outward, _gate);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.rigidbody != _touchingBody)
                return;

            _touchingBody = null;
            _touchingBlock = null;
        }
    }
}
