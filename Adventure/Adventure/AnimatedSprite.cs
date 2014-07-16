using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    public class AnimatedSprite
    {
        private Texture2D sprite;
        public Texture2D Sprite
        {
            get { return sprite; }
            set
            {
                sprite = value;
                frameWidth = sprite.Width / numFrames;
                frameHeight = sprite.Height;
            }
        }

        private Rectangle bounds;
        public Rectangle Bounds { get { return bounds; } }

        private int frameWidth;
        public int FrameWidth { get { return frameWidth; } }
        private int frameHeight;
        public int FrameHeight { get { return frameHeight; } }
        private int numFrames;
        public int NumFrames { get { return numFrames; } }

        public int Delay;
        public float Rotation;
        public bool Flip;
        public float LayerDepth = 0.5f;


        private int currentFrame;
        private int currentDelay;

        private bool isDoneAnimating = false;
        public bool IsDoneAnimating { get { return isDoneAnimating; } }
        private bool loopsContinuously = true;
        private int numLoops = 0;
        private int loopCounter = 0;

        public AnimatedSprite(Texture2D sprite, Rectangle bounds)
        {
            this.sprite = sprite;
            this.bounds = bounds;
            frameWidth = sprite.Width;
            frameHeight = sprite.Height;
            numFrames = 1;
            Delay = 0;
            Rotation = 0f;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

        }

        public AnimatedSprite(Texture2D sprite, Rectangle bounds, int numFrames, int delay)
        {
            this.sprite = sprite;
            this.bounds = bounds;
            frameWidth = sprite.Width / numFrames;
            frameHeight = sprite.Height;
            this.numFrames = numFrames;
            Delay = delay;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

        }

        public AnimatedSprite(Rectangle bounds)
        {
            this.sprite = null;
            this.bounds = bounds;
            numFrames = 1;
            Delay = 0;
            Rotation = 0f;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

        }

        public AnimatedSprite(Rectangle bounds, int numFrames, int delay)
        {
            this.sprite = null;
            this.bounds = bounds;
            this.numFrames = numFrames;
            Delay = delay;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

        }

        public AnimatedSprite(Rectangle bounds, int numFrames, int delay, int numLoops)
        {
            this.sprite = null;
            this.bounds = bounds;
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
            if (sprite != null)
            {
                SpriteEffects effect = Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(sprite, new Vector2((int)Math.Round(position.X) + (Bounds.Width / 2), (int)Math.Round(position.Y) + (Bounds.Height / 2)),
                    new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight), Color.White, Rotation,
                    new Vector2(Bounds.Center.X, Bounds.Center.Y), 1, effect, LayerDepth);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            if (sprite != null)
            {
                SpriteEffects effect = Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(sprite, new Vector2((int)Math.Round(position.X) + (Bounds.Width / 2), (int)Math.Round(position.Y) + (Bounds.Height / 2)),
                    new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight), color, Rotation,
                    new Vector2(Bounds.Center.X, Bounds.Center.Y), 1, effect, LayerDepth);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            if (sprite != null)
            {
                SpriteEffects effect = Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(sprite, rectangle,
                    new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight), Color.White, Rotation,
                    Vector2.Zero, effect, LayerDepth);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            if (sprite != null)
            {
                SpriteEffects effect = Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(sprite, rectangle,
                    new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight), color, Rotation,
                    Vector2.Zero, effect, LayerDepth);
            }
        }
    }
}
