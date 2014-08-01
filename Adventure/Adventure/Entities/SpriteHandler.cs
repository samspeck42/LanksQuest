using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Adventure
{
    public class SpriteHandler
    {
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

        public void Load(ContentManager content)
        {
            foreach (SpriteSet spriteSet in spriteSetDict.Values)
                spriteSet.Load(content);
            foreach (Sprite sprite in spriteDict.Values)
                sprite.Load(content);
        }

        public void AddSprite(string spriteId, Sprite sprite)
        {
            spriteDict.Add(spriteId, sprite);
        }

        public void AddSpriteSet(string spriteSetId, SpriteSet spriteSet)
        {
            spriteSetDict.Add(spriteSetId, spriteSet);
        }

        public void SetSprite(string spriteSetId)
        {
            currentSpriteId = spriteSetId;
            currentSpriteDirection = entity.FaceDirection;
            GetCurrentSprite().ResetAnimation();
        }

        public bool IsCurrentSprite(string spriteSetId)
        {
            return currentSpriteId.Equals(spriteSetId);
        }

        public bool IsCurrentSpriteDoneAnimating()
        {
            return GetCurrentSprite().IsDoneAnimating;
        }

        public Sprite GetCurrentSprite()
        {
            if (spriteSetDict.ContainsKey(currentSpriteId))
                return spriteSetDict[currentSpriteId].GetSprite(currentSpriteDirection);
            else if (spriteDict.ContainsKey(currentSpriteId))
                return spriteDict[currentSpriteId];
            else
                return null;
        }

        /// <summary>
        /// Updates sprite's direction based on current entity face direction without changing the actual sprite
        /// or sprite set being used.
        /// </summary>
        public void UpdateSpriteDirection()
        {
            currentSpriteDirection = entity.FaceDirection;
            GetCurrentSprite().ResetAnimation();
        }

        public virtual void Update(GameTime gameTime)
        {
            // update current sprite's animation
            GetCurrentSprite().Update(gameTime);

            // entity's face direction has changed, so update sprite's direction
            if (entity.FaceDirection != currentSpriteDirection)
                UpdateSpriteDirection();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            GetCurrentSprite().Draw(spriteBatch, new Vector2(50, 50));//entity.Position);
        }
    }
}
