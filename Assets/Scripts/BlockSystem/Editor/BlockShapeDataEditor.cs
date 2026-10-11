using UnityEditor;
using UnityEngine;

namespace Game.BlockSystem
{
    [CustomEditor(typeof(BlockShapeData))]
    internal class BlockShapeDataEditor : Editor
    {
        private BlockShapePreview _preview;

        private BlockShapeData Shape => (BlockShapeData)target;

        private void OnEnable()
        {
            _preview = new BlockShapePreview();
        }

        private void OnDisable()
        {
            _preview.Dispose();
        }

        public override bool HasPreviewGUI()
        {
            return Shape.Prefab != null;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            _preview.Draw(Shape.Prefab.gameObject, rect, background);
        }
    }
}
