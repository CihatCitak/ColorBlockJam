using Game.BlockSystem;
using UnityEngine;

namespace Game.ExitSystem
{
    internal class ExitView : MonoBehaviour
    {
        private ExitPlacement _placement;
        private Vector2 _gate;

        public void Setup(ExitPlacement placement, Vector2 gate)
        {
            _placement = placement;
            _gate = gate;
        }

        public void TryExit(Collision2D collision)
        {
            var body = collision.rigidbody;
            if (body == null || !body.TryGetComponent(out IBlock block))
                return;

            if (ExitFit.Fits(block.Color, block.Cells, _placement.Color, _placement.Cell, _placement.Length, _placement.Outward))
                block.Exit(_placement.Outward, _gate);
        }
    }
}
