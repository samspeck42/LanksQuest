using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    public class PressureSwitch : Switch
    {
        private const string PRESSED_SPRITE_ID = "pressed_sprite";
        private const string UNPRESSED_SPRITE_ID = "unpressed_sprite";

        public override DrawLayer DrawLayer
        {
            get
            {
                return DrawLayer.Low;
            }
        }

        public PressureSwitch(GameWorld game, Area area)
            : base(game, area)
        {
            BoundingBox.RelativeX = -12;
            BoundingBox.RelativeY = -12;
            BoundingBox.Width = 24;
            BoundingBox.Height = 24;

            Vector2 origin = new Vector2(16, 16);
            Sprite sprite = new Sprite("Sprites/Environment/floor_switch_pressed", this, origin);
            spriteHandler.AddSprite(PRESSED_SPRITE_ID, sprite);
            sprite = new Sprite("Sprites/Environment/floor_switch_unpressed", this, origin);
            spriteHandler.AddSprite(UNPRESSED_SPRITE_ID, sprite);

            spriteHandler.SetSprite(UNPRESSED_SPRITE_ID);
        }

        public override void Update(GameTime gameTime)
        {
            spriteHandler.Update(gameTime);
        }

        public override void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox)
        {
            if (other.ActivatesPressureSwitch() && this.Contains(other.Position))
            {
                Activate();
            }
        }

        public override void Activate()
        {
            if (!hasTriggeredActivations)
            {
                spriteHandler.SetSprite(PRESSED_SPRITE_ID);
                tryToTriggerActivations();
            }
        }
    }
}
