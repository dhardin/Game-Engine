using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Extensions
{
    public static class PointExtensions
    {
        public static Vector2 ToVector2(this Point pt)
        {
            return new Vector2(pt.X, pt.Y);
        }
    }
}
