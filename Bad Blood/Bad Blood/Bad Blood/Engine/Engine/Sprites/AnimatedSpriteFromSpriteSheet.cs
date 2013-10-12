using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace Engine.Sprites
{
    public class AnimatedSpriteFromSpriteSheet
    {
        #region Fields
        TimeSpan _runningTime;
        #endregion

        #region Properties
        public SpriteSheet SpriteSheet { get; set; }
        public string FirstFrame { get; set; }
        public int FramesPerSecond { get; set; }
        public int FrameCount { get; set; }
        public int CurrentFrame { get; private set; }
        public bool IsLooping { get; set; }
        public bool IsDone 
        {
            get 
            { 
                return !IsLooping && 
                    CurrentFrame >= SpriteSheet.GetIndex(FirstFrame) + FrameCount; 
            }
        }
        #endregion

        public AnimatedSpriteFromSpriteSheet(SpriteSheet sheet, string firstFrame, int fps, int frameCount)
        {
            IsLooping = true;
            SpriteSheet = sheet;
            FirstFrame = firstFrame;
            FramesPerSecond = fps;
            FrameCount = frameCount;
            CurrentFrame = SpriteSheet.GetIndex(FirstFrame);
        }

        public void Update(GameTime gameTime)
        {
            if (!IsDone)
            {
                _runningTime += gameTime.ElapsedGameTime;
                int firstFrame = SpriteSheet.GetIndex(FirstFrame);
                
                int framesToAdvance = (int)(_runningTime.TotalSeconds * FramesPerSecond);
                if(IsLooping)
                {
                    framesToAdvance %= FrameCount;
                }

                CurrentFrame = firstFrame + framesToAdvance;
            }
        }

        public void Draw(SpriteBatch batch, Vector2 position, Color color, 
            AnchorType anchor = AnchorType.TopLeft, SpriteEffects flip = SpriteEffects.None, float rotation = 0.0f, float scale = 1.0f)
        {
            if (!IsDone)
            {
                Rectangle sourceRect = SpriteSheet.SourceRectangle(CurrentFrame);
                Vector2 spriteAnchor = sourceRect.GetAnchor(anchor);
                batch.Draw(SpriteSheet.Texture,
                    position,
                    sourceRect,
                    color,
                    rotation,
                    spriteAnchor,
                    scale,
                    flip,
                    0);
            }
        }

        public void Reset()
        {
            CurrentFrame = SpriteSheet.GetIndex(FirstFrame);
            _runningTime = TimeSpan.Zero;
        }
    }
}
