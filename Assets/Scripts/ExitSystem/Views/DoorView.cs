using UnityEngine;

namespace Game.ExitSystem
{
    internal class DoorView : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private static MaterialPropertyBlock SharedProperties;

        [SerializeField] private Renderer[] _renderers;

        public void SetColor(Color color)
        {
            SharedProperties ??= new MaterialPropertyBlock();
            SharedProperties.SetColor(BaseColorId, color);
            SharedProperties.SetColor(EmissionColorId, color);

            foreach (var doorRenderer in _renderers)
                doorRenderer.SetPropertyBlock(SharedProperties);
        }
    }
}
