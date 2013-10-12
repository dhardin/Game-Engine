using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace Game
{
    class DrawContext
    {
        public SpriteBatch Batch { get; set; }
        public GraphicsDevice Device { get; set; }
        public Texture2D Blank { get; set; }
        public GameTime Time { get; set; }
        public Vector2 Offset { get; set; }

        public void DrawShadowedString(SpriteFont font, string str, Vector2 pos, Color color)
        {
            Batch.DrawString(font, str, new Vector2(pos.X + 2, pos.Y + 2), new Color(0.0f, 0.0f, 0.0f, 0.85f));
            Batch.DrawString(font, str, pos, color);
        }

         public void DrawShadowedString(SpriteFont font, string str, Vector2 pos, Vector2 orgin, Color color)
        {
            Batch.DrawString(font, str, new Vector2(pos.X + 2, pos.Y + 2), new Color(0.0f, 0.0f, 0.0f, 0.85f), 0, orgin, 1, SpriteEffects.None, 1);
            Batch.DrawString(font, str, pos, color, 0, orgin, 1, SpriteEffects.None, 1);
        }

        public void DrawShadowedTexture(Texture2D texture, Vector2 pos, Color color)
        {
            Batch.Draw(texture, new Vector2(pos.X + 2, pos.Y + 2), new Color(0.0f, 0.0f, 0.0f, 0.85f));
            Batch.Draw(texture, pos, color);
        }

    }
}
