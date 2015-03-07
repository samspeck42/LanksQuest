using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    public abstract class Door : Entity
    {
        protected const string CLOSED_SPRITES_ID = "closed_sprites";
        protected const string OPENING_SPRITES_ID = "opening_sprites";

        public override bool IsObstacle
        {
            get
            {
                return state != DoorState.Open;
            }
        }

        protected SoundEffect openSound;
        protected SoundEffect closeSound;

        protected DoorState state = DoorState.Closed;

        protected abstract string closedSpriteName { get; }
        protected abstract string openingSpriteName { get; }

        public Door(GameWorld game, Area area)
            : base(game, area)
        {
            BoundingBox.RelativeX = -16;
            BoundingBox.RelativeY = -16;
            BoundingBox.Width = 32;
            BoundingBox.Height = 32;

            SpriteSet spriteSet = new SpriteSet();
            Vector2 origin = new Vector2(16, 16);
            Sprite sprite = new Sprite(closedSpriteName, this, origin);
            sprite.Rotation = MathHelper.Pi;
            spriteSet.SetSprite(Directions4.Up, sprite);

            sprite = new Sprite(closedSpriteName, this, origin);
            spriteSet.SetSprite(Directions4.Down, sprite);

            sprite = new Sprite(closedSpriteName, this, origin);
            sprite.Rotation = MathHelper.PiOver2;
            spriteSet.SetSprite(Directions4.Left, sprite);

            sprite = new Sprite(closedSpriteName, this, origin);
            sprite.Rotation = 3 * MathHelper.PiOver2;
            spriteSet.SetSprite(Directions4.Right, sprite);

            spriteHandler.AddSpriteSet(CLOSED_SPRITES_ID, spriteSet);

            spriteSet = new SpriteSet();
            origin = new Vector2(16, 16);
            sprite = new Sprite(openingSpriteName, this, origin, 8, 32, 1);
            sprite.Rotation = MathHelper.Pi;
            spriteSet.SetSprite(Directions4.Up, sprite);

            sprite = new Sprite(openingSpriteName, this, origin, 8, 32, 1);
            spriteSet.SetSprite(Directions4.Down, sprite);

            sprite = new Sprite(openingSpriteName, this, origin, 8, 32, 1);
            sprite.Rotation = MathHelper.PiOver2;
            spriteSet.SetSprite(Directions4.Left, sprite);

            sprite = new Sprite(openingSpriteName, this, origin, 8, 32, 1);
            sprite.Rotation = 3 * MathHelper.PiOver2;
            spriteSet.SetSprite(Directions4.Right, sprite);

            spriteHandler.AddSpriteSet(OPENING_SPRITES_ID, spriteSet);

            spriteHandler.SetSprite(CLOSED_SPRITES_ID);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            openSound = game.Content.Load<SoundEffect>("Audio/door_open");
            closeSound = game.Content.Load<SoundEffect>("Audio/door_close");
        }

        protected override void processAttributeData(Dictionary<string, string> dataDict)
        {
            base.processAttributeData(dataDict);

            state = (DoorState)Enum.Parse(typeof(DoorState), dataDict["state"]);
        }

        public override string ToString()
        {
            return "(" + base.ToString() + ")(" + ((int)FaceDirection).ToString() + ")(" + state.ToString() + ")";
        }

        public override void Update(GameTime gameTime)
        {
            if (state == DoorState.Opening && spriteHandler.IsCurrentSpriteDoneAnimating)
            {
                state = DoorState.Open;
            }

            spriteHandler.Update(gameTime);
        }

        protected void startOpening()
        {
            if (game.CurrentArea.Entities.Contains(this))
            {
                state = DoorState.Opening;
                spriteHandler.SetSprite(OPENING_SPRITES_ID);
                openSound.Play(1f, 0, 0);
            }
            else
            {
                state = DoorState.Open;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            if (state != DoorState.Open)
                base.Draw(spriteBatch, changeColorsEffect);
        }
    }

    public enum DoorState
    {
        Open,
        Closed,
        Opening,
        Closing
    }
}
