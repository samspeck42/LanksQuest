using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    public class Sprite
    {
        private Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                frameWidth = texture.Width / numFrames;
                frameHeight = texture.Height;
            }
        }
        private Vector2 origin;

        private int frameWidth;
        public int FrameWidth { get { return frameWidth; } }
        private int frameHeight;
        public int FrameHeight { get { return frameHeight; } }
        private int numFrames;
        public int NumFrames { get { return numFrames; } }

        public int Delay;
        public float Rotation;
        public bool Flip;


        private int currentFrame;
        private int currentDelay;

        private bool isDoneAnimating = false;
        public bool IsDoneAnimating { get { return isDoneAnimating; } }
        private bool loopsContinuously = true;
        private int numLoops = 0;
        private int loopCounter = 0;

        public Sprite(Texture2D sprite, Vector2 origin)
        {
            this.texture = sprite;
            this.origin = origin;
            frameWidth = sprite.Width;
            frameHeight = sprite.Height;
            numFrames = 1;
            Delay = 0;
            Rotation = 0f;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

        }

        public Sprite(Texture2D sprite, Vector2 origin, int numFrames, int delay)
        {
            this.texture = sprite;
            this.origin = origin;
            frameWidth = sprite.Width / numFrames;
            frameHeight = sprite.Height;
            this.numFrames = numFrames;
            Delay = delay;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

        }

        public Sprite(Vector2 origin)
        {
            this.texture = null;
            this.origin = origin;
            numFrames = 1;
            Delay = 0;
            Rotation = 0f;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

        }

        public Sprite(Vector2 origin, int numFrames, int delay)
        {
            this.texture = null;
            this.origin = origin;
            this.numFrames = numFrames;
            Delay = delay;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

        }

        public Sprite(Vector2 origin, int numFrames, int delay, int numLoops)
        {
            this.texture = null;
            this.origin = origin;
            this.numFrames = numFrames;
            Delay = delay;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

            this.numLoops = numLoops;
            loopsContinuously = false;
        }

        public void UpdateAnimation()
        {
            if (!isDoneAnimating)
            {
                if (currentDelay >= Delay)
                {
                    currentFrame++;
                    currentDelay = 0;
                    if (currentFrame >= numFrames)
                    {
                        currentFrame = 0;

                        if (!loopsContinuously)
                        {
                            loopCounter++;
                            if (loopCounter >= numLoops)
                                isDoneAnimating = true;
                            currentFrame = numFrames - 1;
                        }
                    }
                }
                else
                    currentDelay++;
            }
        }

        public void ResetAnimation()
        {
            currentFrame = 0;
            currentDelay = 0;

            if (!loopsContinuously)
            {
                isDoneAnimating = false;
                loopCounter = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (texture != null)
            {
                SpriteEffects effect = Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(texture, position,
                    new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight), Color.White, Rotation,
                    this.origin, 1, effect, 0);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            if (texture != null)
            {
                SpriteEffects effect = Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(texture, position,
                    new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight), color, Rotation,
                    this.origin, 1, effect, 0);
            }
        }
    }
}
