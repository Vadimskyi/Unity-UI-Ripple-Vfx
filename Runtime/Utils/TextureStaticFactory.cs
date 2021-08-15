/* Copyright (C) 2020 Vadimskyi - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the GPL-3.0 License license.
 */

using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace VadimskyiLab.Utils
{
    /// <summary>
    /// Creates and reuses(pooling) textures for ButtonRippleEffect
    /// </summary>
    public static class TextureStaticFactory
    {
        private static volatile int _textureIndex = 0;
        private static ConcurrentQueue<Texture2D> _gridPool;
        private static ConcurrentQueue<Texture2D> _circlePool;
        private static Color _transparent = new Color(255, 255, 255, 0);

        static TextureStaticFactory()
        {
            _gridPool = new ConcurrentQueue<Texture2D>();
            _circlePool = new ConcurrentQueue<Texture2D>();
        }

        public static void ReturnTextureRGB24(Texture2D tex)
        {
            if (tex == null) return;

            if (_circlePool.Contains(tex))
            {
                UnityEngine.Debug.LogError($"ReturnTextureRGB24 was already returned!!!");
                return;
            }

            _gridPool.Enqueue(tex);
        }

        public static void ReturnTextureRGBA32(Texture2D tex)
        {
            if (tex == null) return;

            if (_circlePool.Contains(tex))
            {
                UnityEngine.Debug.LogError($"ReturnTextureRGBA32 was already returned!!!");
                return;
            }

            _circlePool.Enqueue(tex);
        }
        
        /// <summary>
        /// Creates a texture and fills it with colored tiles.
        /// </summary>
        public static Texture2D FillTile(int texWidth, int texHeight, Vector2Int gridSize, Vector2Int padding, Color colorBack, Color colorFront)
        {
            Texture2D tex = GetTextureRGBA32(texWidth, texHeight);

            Color[] colors = new Color[texWidth * texHeight];

            // TODO: Simplify it.
            float tileWidth = (texWidth - (gridSize.x + 1) * padding.x) / (float)gridSize.x;
            float tileHeight = (texHeight - ((gridSize.y + 1) * (float)padding.y)) / gridSize.y;

            float paddingRatioX = (padding.x) / (tileWidth + padding.x);
            float paddingRatioY = (padding.y) / (tileHeight + padding.y);

            float modX = 0;
            float modY = 0;
            float paddingX = padding.x;
            float paddingY = padding.y;

            for (int y = 0; y < texHeight; y++)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    modX = (x % (tileWidth + paddingX)) / (tileWidth + paddingX);
                    modY = (y % (tileHeight + paddingY)) / (tileHeight + paddingY);

                    colors[x + y * texWidth] = modX >= paddingRatioX && modY >= paddingRatioY ? colorBack : colorFront;

                    //hack for dealing with rounding error
                    if (x == 0 || x + 1 == texWidth
                               || y == 0 || y + 1 == texHeight) colors[x + y * texWidth] = colorFront;
                }
            }

            tex.SetPixels(colors);
            tex.Apply();

            return tex;
        }

        /// <summary>
        /// Creates a filled circle texture
        /// </summary>
        public static Texture2D CreateCircleTexture(Color color, int width, int height, int x, int y, int radius)
        {
            Texture2D tex = GetTextureRGBA32(width, height);

            float rSquared = radius * radius;
            var colors = tex.GetPixels32();

            int index = 0;
            for (int u = x - radius; u < x + radius + 1; u++)
            {
                for (int v = y - radius; v < y + radius + 1; v++)
                {
                    index = u * width + v;
                    if (index >= colors.Length) continue;
                    if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                        colors[index] = color;
                    else
                        colors[index] = _transparent;
                }
            }
            tex.SetPixels32(colors);
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Warning! Don't forget to return this texture using ReturnTextureRGB24 method!
        /// </summary>
        public static Texture2D GetTextureRGB24(int width, int height)
        {
            Texture2D tex = null;
            if (!_gridPool.IsEmpty && _gridPool.TryDequeue(out tex))
            {
                if (tex.width != width || tex.height != height)
                    tex.Resize(width, height);
            }
            else
            {
                tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                tex.name = "tex_rgb24.jpg";
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.filterMode = FilterMode.Bilinear;
            }
            return tex;
        }

        /// <summary>
        /// Warning! Don't forget to return this texture using ReturnTextureRGBA32 method!
        /// </summary>
        public static Texture2D GetTextureRGBA32(int width, int height)
        {
            Texture2D tex = null;

            if (!_circlePool.IsEmpty && _circlePool.TryDequeue(out tex))
            {
                if (tex.width != width || tex.height != height)
                    tex.Resize(width, height);
            }
            else
            {
                tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                var index = Interlocked.Increment(ref _textureIndex);
                tex.name = $"tex_rgba32_#{index}.jpg";
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.filterMode = FilterMode.Bilinear;
            }

            return tex;
        }
    }
}
