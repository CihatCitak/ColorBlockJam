using UnityEngine;

namespace Game.ExitSystem
{
    internal class DoorView : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private static MaterialPropertyBlock SharedProperties;

        [SerializeField] private Renderer[] _renderers;

        private ExitView _exit;

        public void Setup(ExitView exit, Color color)
        {
            _exit = exit;

            SharedProperties ??= new MaterialPropertyBlock();
            SharedProperties.SetColor(BaseColorId, color);
            SharedProperties.SetColor(EmissionColorId, color);

            foreach (var doorRenderer in _renderers)
                doorRenderer.SetPropertyBlock(SharedProperties);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            _exit.TryExit(collision);
        }
    }
}
