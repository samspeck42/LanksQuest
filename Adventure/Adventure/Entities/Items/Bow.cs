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
        private Sprite sprite;
        private SoundEffect arrowFireSound;

        public Texture2D InventoryScreenIcon { get { return null; } }//return sprite.Texture; } }
        public Point InventoryScreenPoint { get { return new Point(0, 0); } }

        private bool isDoneBeingUsed = false;
        public bool IsDoneBeingUsed { get { return isDoneBeingUsed; } }

        private bool isInUse = false;
        private Arrow arrow;

        public Bow(GameWorld game, Area area)
            : base(game, null)
        {
            BoundingBox.RelativeX = -16;
            BoundingBox.RelativeY = -8;
            BoundingBox.Width = 32;
            BoundingBox.Height = 16;

            Vector2 origin = new Vector2(16, 8);
            //sprite = new Sprite(this, origin);

            //CurrentSprite = sprite;
        }

        public override void LoadContent()
        {
            //sprite.Texture = game.Content.Load<Texture2D>("Sprites/Items/bow");
            arrowFireSound = game.Content.Load<SoundEffect>("Audio/arrow_fire");
        }

        public override void Update(GameTime gameTime)
        {
            if (isInUse)
            {
                if (FaceDirection == Directions4.Left || FaceDirection == Directions4.Right)
                {
                    Center = new Vector2(Center.X, game.Player.Center.Y - 5);
                    if (FaceDirection == Directions4.Left)
                    {
                        Center = new Vector2(game.Player.BoundingBox.ActualX - (Height / 2), Center.Y);
                        arrow.Center = new Vector2(Center.X - 4, Center.Y);
                    }
                    else
                    {
                        Center = new Vector2(game.Player.BoundingBox.ActualX + game.Player.Width + (Height / 2), Center.Y);
                        arrow.Center = new Vector2(Center.X + 4, Center.Y);
                    }
                }
                else if (FaceDirection == Directions4.Up || FaceDirection == Directions4.Down)
                {
                    Center = new Vector2(game.Player.Center.X, Center.Y);
                    if (FaceDirection == Directions4.Up)
                    {
                        BoundingBox.ActualY = game.Player.Center.Y - 24;
                        arrow.Center = new Vector2(Center.X, Center.Y - 4);
                    }
                    else
                    {
                        BoundingBox.ActualY = game.Player.Center.Y;
                        arrow.Center = new Vector2(Center.X, Center.Y + 4);
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox)
        {
            
        }

        public void StartBeingUsed()
        {
            arrow = new Arrow(game, game.CurrentArea, game.Player.FaceDirection);
            arrow.LoadContent();

            if (game.Player.FaceDirection == Directions4.Left)
            {
                FaceDirection = Directions4.Left;
                CurrentSprite.Rotation = 3f * MathHelper.PiOver2;
            }
            else if (game.Player.FaceDirection == Directions4.Right)
            {
                FaceDirection = Directions4.Right;
                CurrentSprite.Rotation = MathHelper.PiOver2;
            }
            else if (game.Player.FaceDirection == Directions4.Down)
            {
                FaceDirection = Directions4.Down;
                CurrentSprite.Rotation = MathHelper.Pi;
            }
            else if (game.Player.FaceDirection == Directions4.Up)
            {
                FaceDirection = Directions4.Up;
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

            arrow.Draw(spriteBatch, changeColorsEffect);
        }
    }
}
