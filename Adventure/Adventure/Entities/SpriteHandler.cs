using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Adventure.Entities
{
    public class SpriteHandler
    {
        private const int BLINK_DELAY = 30;

        public bool IsBlinking { get { return isBlinking; } }
        public Sprite CurrentSprite
        {
            get
            {
                if (spriteSetDict.ContainsKey(currentSpriteId))
                    return spriteSetDict[currentSpriteId].GetSprite(currentSpriteDirection);
                else if (spriteDict.ContainsKey(currentSpriteId))
                    return spriteDict[currentSpriteId];
                else
                    return null;
            }
        }
        public bool IsCurrentSpriteDoneAnimating
        {
            get
            {
                return CurrentSprite.IsDoneAnimating;
            }
        }

        private bool isBlinking = false;
        private int blinkTimer = 0;
        protected Dictionary<string, SpriteSet> spriteSetDict = new Dictionary<string, SpriteSet>();
        protected Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
        protected string currentSpriteId;
        protected Directions4 currentSpriteDirection;
        protected Entity entity;

        public SpriteHandler(Entity entity)
        {
            this.currentSpriteId = "";
            this.currentSpriteDirection = Directions4.None;
            this.entity = entity;
        }

        public void LoadSprites(ContentManager content)
        {
            foreach (SpriteSet spriteSet in spriteSetDict.Values)
                spriteSet.LoadContent(content);
            foreach (Sprite sprite in spriteDict.Values)
                sprite.LoadContent(content);
        }

        public void AddSprite(string spriteId, Sprite sprite)
        {
            spriteDict.Add(spriteId, sprite);
        }

        public void AddSpriteSet(string spriteSetId, SpriteSet spriteSet)
        {
            spriteSetDict.Add(spriteSetId, spriteSet);
        }

        /// <summary>
        /// Sets the owner entity's current sprite or sprite set to the one corresponding to the given id.
        /// The owner entity's current face direction will be used to determine which sprite will be used
        /// if a sprite set is specified.
        /// </summary>
        /// <param name="spriteId">The id of the sprite or sprite set to set as the owner entity's current
        /// sprite.</param>
        public void SetSprite(string spriteId)
        {
            currentSpriteId = spriteId;
            currentSpriteDirection = entity.FaceDirection;
            CurrentSprite.ResetAnimation();
        }

        public bool IsCurrentSprite(string spriteSetId)
        {
            return currentSpriteId.Equals(spriteSetId);
        }

        public void StartBlinking()
        {
            isBlinking = true;
            blinkTimer = 0;
        }

        public void StopBlinking()
        {
            isBlinking = false;
        }

        /// <summary>
        /// Updates sprite's direction based on current entity face direction without changing the actual sprite
        /// or sprite set being used.
        /// </summary>
        public void UpdateSpriteDirection()
        {
            currentSpriteDirection = entity.FaceDirection;
            CurrentSprite.ResetAnimation();
        }

        public virtual void Update(GameTime gameTime)
        {
            // update current sprite's animation
            CurrentSprite.Update(gameTime);

            // entity's face direction has changed, so update sprite's direction
            if (entity.FaceDirection != currentSpriteDirection)
                UpdateSpriteDirection(); 

            if (isBlinking)
                blinkTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentSprite != null)
            {
                bool shouldDraw = isBlinking ? blinkTimer % (BLINK_DELAY * 2) < BLINK_DELAY : true;
                if (shouldDraw)
                    CurrentSprite.Draw(spriteBatch, entity.Position);
            }
        }
    }
}
