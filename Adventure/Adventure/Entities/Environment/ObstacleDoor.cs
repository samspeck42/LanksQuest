using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Adventure.Maps;

namespace Adventure.Entities.Environment
{
    public class ObstacleDoor : Door, Triggerable
    {
        protected const string CLOSING_SPRITES_ID = "closing_sprites";

        protected override string closedSpriteName { get { return "Sprites/Environment/closed_door"; } }
        protected override string openingSpriteName { get { return "Sprites/Environment/closed_door_opening"; } }

        public ObstacleDoor(GameWorld game, Map map, Area area)
            : base(game, map, area)
        {
            SpriteSet spriteSet = new SpriteSet();
            Vector2 origin = new Vector2(16, 16);
            Sprite sprite = new Sprite("Sprites/Environment/closed_door_closing", this, origin, 8, 32, 1);
            sprite.Rotation = MathHelper.Pi;
            spriteSet.SetSprite(Directions4.Up, sprite);

            sprite = new Sprite("Sprites/Environment/closed_door_closing", this, origin, 8, 32, 1);
            spriteSet.SetSprite(Directions4.Down, sprite);

            sprite = new Sprite("Sprites/Environment/closed_door_closing", this, origin, 8, 32, 1);
            sprite.Rotation = MathHelper.PiOver2;
            spriteSet.SetSprite(Directions4.Left, sprite);

            sprite = new Sprite("Sprites/Environment/closed_door_closing", this, origin, 8, 32, 1);
            sprite.Rotation = 3 * MathHelper.PiOver2;
            spriteSet.SetSprite(Directions4.Right, sprite);

            spriteHandler.AddSpriteSet(CLOSING_SPRITES_ID, spriteSet);
        }

        public override void Update(GameTime gameTime)
        {
            if (state == DoorState.Closing && spriteHandler.IsCurrentSpriteDoneAnimating)
            {
                state = DoorState.Closed;
                spriteHandler.SetSprite(CLOSED_SPRITES_ID);
                closeSound.Play(0.75f, 0, 0);
            }

            base.Update(gameTime);
        }

        public void TriggerOn()
        {
            if (state == DoorState.Closed)
                startOpening();
        }

        public void TriggerOff()
        {
            if (state == DoorState.Open)
                startClosing();
        }

        private void startClosing()
        {
            if (gameWorld.CurrentArea.Entities.Contains(this))
            {
                state = DoorState.Closing;
                spriteHandler.SetSprite(CLOSING_SPRITES_ID);
            }
            else
            {
                state = DoorState.Closed;
                spriteHandler.SetSprite(CLOSED_SPRITES_ID);
            }
        }
    }
}
