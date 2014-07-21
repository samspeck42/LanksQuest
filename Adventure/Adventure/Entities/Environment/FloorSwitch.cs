using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    public class FloorSwitch : ActivatingEntity
    {
        private Sprite unpressedSprite;
        private Sprite pressedSprite;

        public FloorSwitch(GameWorld game, Area area)
            : base(game, area)
        {
            hitBoxOffset = Vector2.Zero;
            hitBoxWidth = 32;
            hitBoxHeight = 32;

            Vector2 origin = new Vector2(0, 0);
            unpressedSprite = new Sprite(origin);
            pressedSprite = new Sprite(origin);

            CurrentSprite = unpressedSprite;
            IsOnGround = true;
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Player)
            {
                Player player = (Player)other;
                if (!hasTriggeredActivations && this.Contains(player.FootPosition))
                {
                    CurrentSprite = pressedSprite;
                    tryToTriggerActivations();
                }
            }
        }

        public override void LoadContent()
        {
            unpressedSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/floor_switch_unpressed");
            pressedSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/floor_switch_pressed");
        }
    }
}
