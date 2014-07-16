using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Adventure
{
    public class Bow : Entity, EquippableItem
    {
        private AnimatedSprite sprite;
        private SoundEffect arrowFireSound;

        public Texture2D InventoryScreenIcon { get { return sprite.Sprite; } }
        public Point InventoryScreenPoint { get { return new Point(0, 0); } }

        private bool isDoneBeingUsed = false;
        public bool IsDoneBeingUsed { get { return isDoneBeingUsed; } }

        private bool isInUse = false;
        private Arrow arrow;

        public Bow(GameWorld game, Area area)
            : base(game, null)
        {
            Rectangle bounds = new Rectangle(0, 0, 32, 16);
            sprite = new AnimatedSprite(bounds);

            CurrentSprite = sprite;

            IsAffectedByWallCollisions = false;
        }

        public override void LoadContent()
        {
            sprite.Sprite = game.Content.Load<Texture2D>("Sprites/Items/bow");
            arrowFireSound = game.Content.Load<SoundEffect>("Audio/arrow_fire");
        }

        public override void Update()
        {
            if (isInUse)
            {
                if (FaceDirection == Directions.Left || FaceDirection == Directions.Right)
                {
                    Center = new Vector2(Center.X, game.Player.Center.Y - 5);
                    if (FaceDirection == Directions.Left)
                    {
                        Center = new Vector2(game.Player.Position.X - (Height / 2), Center.Y);
                        arrow.Center = new Vector2(Center.X - 4, Center.Y);
                    }
                    else
                    {
                        Center = new Vector2(game.Player.Position.X + game.Player.Width + (Height / 2), Center.Y);
                        arrow.Center = new Vector2(Center.X + 4, Center.Y);
                    }
                }
                else if (FaceDirection == Directions.Up || FaceDirection == Directions.Down)
                {
                    Center = new Vector2(game.Player.Center.X, Center.Y);
                    if (FaceDirection == Directions.Up)
                    {
                        Position.Y = game.Player.Center.Y - 24;
                        arrow.Center = new Vector2(Center.X, Center.Y - 4);
                    }
                    else
                    {
                        Position.Y = game.Player.Center.Y;
                        arrow.Center = new Vector2(Center.X, Center.Y + 4);
                    }
                }
            }

            base.Update();
        }

        public override void OnEntityCollision(Entity other)
        {
            
        }

        public void StartBeingUsed()
        {
            arrow = new Arrow(game, game.CurrentArea, game.Player.FaceDirection);
            arrow.LoadContent();

            if (game.Player.FaceDirection == Directions.Left)
            {
                FaceDirection = Directions.Left;
                CurrentSprite.Rotation = 3f * MathHelper.PiOver2;
            }
            else if (game.Player.FaceDirection == Directions.Right)
            {
                FaceDirection = Directions.Right;
                CurrentSprite.Rotation = MathHelper.PiOver2;
            }
            else if (game.Player.FaceDirection == Directions.Down)
            {
                FaceDirection = Directions.Down;
                CurrentSprite.Rotation = MathHelper.Pi;
            }
            else if (game.Player.FaceDirection == Directions.Up)
            {
                FaceDirection = Directions.Up;
                CurrentSprite.Rotation = 0f;
            }

            isDoneBeingUsed = false;
            isInUse = true;
        }

        public void FireArrow()
        {
            isDoneBeingUsed = true;
            isInUse = false;
            arrow.Fire();
            arrowFireSound.Play(1f, 0, 0);
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            base.Draw(spriteBatch, changeColorsEffect);

            arrow.CurrentSprite.LayerDepth = CurrentSprite.LayerDepth - 0.1f;
            arrow.Draw(spriteBatch, changeColorsEffect);
        }
    }
}
