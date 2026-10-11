using UnityEditor;
using UnityEngine;

namespace Game.LevelSystem
{
    internal static class EditorRects
    {
        public static void Outline(Rect rect, Color color, float width)
        {
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, width), color);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMax - width, rect.width, width), color);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, width, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.xMax - width, rect.yMin, width, rect.height), color);
        }

        public static Rect Inset(Rect rect, float amount)
        {
            return new Rect(rect.x + amount, rect.y + amount, rect.width - amount * 2f, rect.height - amount * 2f);
        }
    }
}
