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
            Rectangle bounds = new Rectangle(0, 0, 32, 32);
            unpressedSprite = new Sprite(bounds);
            pressedSprite = new Sprite(bounds);

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
