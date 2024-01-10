#if DEVELOPMENT_BUILD || UNITY_EDITOR
using UnityEngine;

namespace Logger
{
    public struct LogEntity
    {
        public readonly string Text;
        public readonly Color Color;

        public LogEntity(string text, Color color)
        {
            Text = text;
            Color = color;
        }
    }
}

#endif