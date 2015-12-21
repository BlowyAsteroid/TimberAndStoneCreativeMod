using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Utils
{
    public static class RectangleUtils
    {
        private static Rect tempRectangle = new Rect();

        public static Rect get(Rect original)
        {
            return get(original.x, original.y, original.width, original.height);
        }

        public static Rect get(float x, float y, float size)
        {
            return get(x, y, size, size);
        }

        public static Rect get(float x, float y, float width, float height, float offset)
        {
            return get(x - offset, y - offset, width + offset, height + offset);
        }

        public static Rect get(float x, float y, float width, float height)
        {
            tempRectangle.x = x;
            tempRectangle.y = y;
            tempRectangle.width = width;
            tempRectangle.height = height;

            return tempRectangle;
        }
    }
}
