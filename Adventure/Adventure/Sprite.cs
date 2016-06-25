using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Adventure.Entities;

namespace Adventure
{
    public class Sprite
    {
        public Texture2D Texture
        {
            get { return texture; }
        }

        public float Delay;
        public float Rotation;
        public bool Flip;
        public bool IsDoneAnimating { get { return isDoneAnimating; } }
        public int FrameWidth { get { return frameWidth; } }
        public int FrameHeight { get { return frameHeight; } }
        public int NumFrames { get { return numFrames; } }


        private int currentFrame;
        private float currentDelay;

        private Texture2D texture;
        private string textureName;
        private Vector2 origin;
        private bool isDoneAnimating = false;
        private int frameWidth;
        private int frameHeight;
        private int numFrames;
        private bool loopsContinuously = true;
        private int numLoops = 0;
        private int loopCounter = 0;
        private Entity owner;
        private Dictionary<string, Rectangle>[] hitBoxTextureData;
        private Dictionary<string, string> hitBoxTextureNameDict;

        //public Sprite(Texture2D sprite, Vector2 origin)
        //{
        //    this.texture = sprite;
        //    this.origin = origin;
        //    frameWidth = sprite.Width;
        //    frameHeight = sprite.Height;
        //    numFrames = 1;
        //    Delay = 0;
        //    Rotation = 0f;
        //    Flip = false;

        //    currentFrame = 0;
        //    currentDelay = 0;

        //}

        //public Sprite(Texture2D sprite, Vector2 origin, int numFrames, int delay)
        //{
        //    this.texture = sprite;
        //    this.origin = origin;
        //    frameWidth = sprite.Width / numFrames;
        //    frameHeight = sprite.Height;
        //    this.numFrames = numFrames;
        //    Delay = delay;
        //    Flip = false;

        //    currentFrame = 0;
        //    currentDelay = 0;

        //}

        public Sprite(string textureName, Entity owner, Vector2 origin)
        {
            this.textureName = textureName;
            this.owner = owner;
            this.texture = null;
            this.origin = origin;
            numFrames = 1;
            Delay = 0;
            Rotation = 0f;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

            hitBoxTextureData = new Dictionary<string, Rectangle>[numFrames];
            hitBoxTextureNameDict = new Dictionary<string, string>();
        }

        public Sprite(string textureName, Entity owner, Vector2 origin, int numFrames, float delay)
        {
            this.textureName = textureName;
            this.owner = owner;
            this.texture = null;
            this.origin = origin;
            this.numFrames = numFrames;
            Delay = delay;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

            hitBoxTextureData = new Dictionary<string, Rectangle>[numFrames];
            hitBoxTextureNameDict = new Dictionary<string, string>();
        }

        public Sprite(string textureName, Entity owner, Vector2 origin, int numFrames, float delay, int numLoops)
        {
            this.textureName = textureName;
            this.owner = owner;
            this.texture = null;
            this.origin = origin;
            this.numFrames = numFrames;
            Delay = delay;
            Flip = false;

            currentFrame = 0;
            currentDelay = 0;

            this.numLoops = numLoops;
            loopsContinuously = false;

            hitBoxTextureData = new Dictionary<string, Rectangle>[numFrames];
            hitBoxTextureNameDict = new Dictionary<string, string>();
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(textureName);
            frameWidth = texture.Width / numFrames;
            frameHeight = texture.Height;
            loadHitBoxTextures(content);
        }

        /// <summary>
        /// Add a texture that defines a hit box's relative position and size for each frame of animation
        /// of this sprite.
        /// </summary>
        /// <param name="hitBoxTextureName">The name of the texture that defines the hit box for each frame. The texture is
        /// expected to be the same size as the sprite texture, and each frame should have a single non-white
        /// rectangle that defines the hit box's size and location.</param>
        /// <param name="hitBoxId">The string id of the hit box this texture modifies.</param>
        public void AddHitBoxTexture(string hitBoxTextureName, string hitBoxId)
        {
            hitBoxTextureNameDict.Add(hitBoxTextureName, hitBoxId);
        }

        private void loadHitBoxTextures(ContentManager content)
        {
            foreach (string hitBoxTextureName in hitBoxTextureNameDict.Keys)
            {
                Texture2D hitBoxTexture = content.Load<Texture2D>(hitBoxTextureName);
                string hitBoxId = hitBoxTextureNameDict[hitBoxTextureName];

                Color[] colorData = new Color[hitBoxTexture.Width * hitBoxTexture.Height];
                hitBoxTexture.GetData<Color>(colorData);
                int frameWidth = hitBoxTexture.Width / numFrames;
                int frameHeight = hitBoxTexture.Height;

                Color[][,] frameColorData = new Color[numFrames][,];
                for (int row = 0; row < hitBoxTexture.Height; row++)
                {
                    for (int x = 0; x < hitBoxTexture.Width; x++)
                    {
                        int frameNum = x / frameWidth;
                        int col = x % frameWidth;

                        if (frameColorData[frameNum] == null)
                            frameColorData[frameNum] = new Color[frameHeight, frameWidth];

                        frameColorData[frameNum][row, col] = colorData[(row * hitBoxTexture.Width) + x];
                    }
                }

                for (int frameNum = 0; frameNum < numFrames; frameNum++)
                {
                    if (hitBoxTextureData[frameNum] == null)
                        hitBoxTextureData[frameNum] = new Dictionary<string, Rectangle>();

                    Rectangle hitBoxRect = new Rectangle();
                    Color[,] colorMap = frameColorData[frameNum];

                    // find the hit box's position in the frame
                    bool positionFound = false;
                    for (int y = 0; y < colorMap.GetLength(0); y++)
                    {
                        for (int x = 0; x < colorMap.GetLength(1); x++)
                        {
                            if (!colorMap[y, x].Equals(Color.White))
                            {
                                hitBoxRect.X = x;
                                hitBoxRect.Y = y;
                                positionFound = true;
                                break;
                            }
                        }
                        if (positionFound)
                            break;
                    }

                    // find the hit box's width
                    for (int x = hitBoxRect.X; x < colorMap.GetLength(1); x++)
                    {
                        if (colorMap[hitBoxRect.Y, x].Equals(Color.White))
                            break;
                        hitBoxRect.Width++;
                    }

                    // find the hit box's height
                    for (int y = hitBoxRect.Y; y < colorMap.GetLength(0); y++)
                    {
                        if (colorMap[y, hitBoxRect.X].Equals(Color.White))
                            break;
                        hitBoxRect.Height++;
                    }

                    // adjust hit box position so it's relative to the origin
                    hitBoxRect.X -= (int)Math.Round(this.origin.X);
                    hitBoxRect.Y -= (int)Math.Round(this.origin.Y);

                    hitBoxTextureData[frameNum].Add(hitBoxId, hitBoxRect);
                }
            }
        }

        private void updateHitBoxes()
        {
            Dictionary<string, Rectangle> hitBoxDict = hitBoxTextureData[currentFrame];

            if (hitBoxDict != null)
            {
                foreach (string id in hitBoxDict.Keys)
                {
                    HitBox hitBox = owner.GetHitBoxById(id);

                    if (hitBox != null)
                        hitBox.SetFromRectangle(hitBoxDict[id]);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!isDoneAnimating && numFrames > 1)
            {
                if (currentDelay >= Delay)
                {
                    currentFrame += (int)Math.Round(currentDelay / Delay);
                    currentDelay = currentDelay % Delay;
                    if (currentFrame >= numFrames)
                    {
                        if (!loopsContinuously)
                        {
                            loopCounter += currentFrame / numFrames;
                            if (loopCounter >= numLoops)
                            {
                                isDoneAnimating = true;
                                currentFrame = numFrames - 1;
                            }
                        }
                        if (!isDoneAnimating)
                        {
                            currentFrame = currentFrame % numFrames;
                        }
                    }
                }
                else
                {
                    currentDelay += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            updateHitBoxes();
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

            updateHitBoxes();
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
