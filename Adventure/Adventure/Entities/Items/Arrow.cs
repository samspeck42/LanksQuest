using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace Adventure
{
    class Arrow : Entity
    {
        private const float FIRE_SPEED = 6.0f;
        private const int LIFETIME = 180;
        private const int BLINK_START_TIME = 120;
        private const int BLINK_DELAY = 2;
        private const int DAMAGE = 2;
        private const int WALL_INTERSECT_DISTANCE = 10;

        private Sprite verticalSprite;
        private Sprite horizontalSprite;
        private bool isFired;
        public bool IsFired { get { return isFired; } }
        private bool hasHit;
        private int lifeTimer = 0;
        private Entity entityHit = null;
        private Vector2 entityHitPosition = Vector2.Zero;

        public Vector2 TipPosition
        {
            get
            {
                if (FaceDirection == Directions4.Up)
                    return new Vector2(BoundingBox.ActualX + (Width / 2), BoundingBox.ActualY);
                else if (FaceDirection == Directions4.Down)
                    return new Vector2(BoundingBox.ActualX + (Width / 2), BoundingBox.ActualY + Height);
                else if (FaceDirection == Directions4.Left)
                    return new Vector2(BoundingBox.ActualX, BoundingBox.ActualY + (Height / 2));
                else if (FaceDirection == Directions4.Right)
                    return new Vector2(BoundingBox.ActualX + Width, BoundingBox.ActualY + (Height / 2));
                return Vector2.Zero;
            }
        }

        public Arrow(GameWorld game, Area area, Directions4 direction)
            : base(game, area)
        {
            Vector2 origin = new Vector2(4, 10);
            //verticalSprite = new Sprite(this, origin);
            origin = new Vector2(10, 4);
            //horizontalSprite = new Sprite(this, origin);

            FaceDirection = direction;

            if (FaceDirection == Directions4.Up)
            {
                //CurrentSprite = verticalSprite;
                BoundingBox.RelativeX = -4;
                BoundingBox.RelativeY = -10;
                BoundingBox.Width = 8;
                BoundingBox.Height = 20;
                CurrentSprite.Rotation = 0;
            }
            else if (FaceDirection == Directions4.Down)
            {
                //CurrentSprite = verticalSprite;
                BoundingBox.RelativeX = -4;
                BoundingBox.RelativeY = -10;
                BoundingBox.Width = 8;
                BoundingBox.Height = 20;
                CurrentSprite.Rotation = MathHelper.Pi;
            }
            else if (FaceDirection == Directions4.Left)
            {
                //CurrentSprite = horizontalSprite;
                BoundingBox.RelativeX = -10;
                BoundingBox.RelativeY = -4;
                BoundingBox.Width = 20;
                BoundingBox.Height = 8;
                CurrentSprite.Rotation = 0;
            }
            else if (FaceDirection == Directions4.Right)
            {
                //CurrentSprite = horizontalSprite;
                BoundingBox.RelativeX = -10;
                BoundingBox.RelativeY = -4;
                BoundingBox.Width = 20;
                BoundingBox.Height = 8;
                CurrentSprite.Rotation = MathHelper.Pi;
            }

            isFired = false;
            hasHit = false;
            IsInAir = true;
            Damage = DAMAGE;
            diesOutsideArea = true;
        }

        public override void LoadContent()
        {
            //verticalSprite.Texture = game.Content.Load<Texture2D>("Sprites/Items/arrow_vertical");
            //horizontalSprite.Texture = game.Content.Load<Texture2D>("Sprites/Items/arrow_horizontal");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //bool isInsideWall = false;
            //Point tipCell = Area.ConvertPositionToCell(TipPosition);
            //Rectangle collisionRect = new Rectangle();

            //if (area.GetCollisionAtCell(tipCell) == TileCollision.Obstacle)
            //{
            //    isInsideWall = true;
            //    collisionRect = Area.CreateRectangleForCell(tipCell);
            //}
            //else
            //{
            //    foreach (Entity entity in area.GetActiveEntitiesAtCell(tipCell))
            //    {
            //        if (entity.Contains(TipPosition) && canBeStoppedBy(entity))
            //        {
            //            isInsideWall = true;
            //            collisionRect = entity.HitBox2;
            //        }
            //    }
            //}

            //if (isInsideWall)
            //{
            //    float intersectDistance = 0f;
            //    if (FaceDirection == Directions4.Up)
            //        intersectDistance = collisionRect.Bottom - TipPosition.Y;
            //    else if (FaceDirection == Directions4.Down)
            //        intersectDistance = TipPosition.Y - collisionRect.Top;
            //    else if (FaceDirection == Directions4.Left)
            //        intersectDistance = collisionRect.Right - TipPosition.X;
            //    else if (FaceDirection == Directions4.Right)
            //        intersectDistance = TipPosition.X - collisionRect.Left;

            //    if (intersectDistance >= 10)
            //    {
            //        Velocity = Vector2.Zero;

            //        if (FaceDirection == Directions4.Up)
            //            HitBoxPositionY = collisionRect.Bottom - WALL_INTERSECT_DISTANCE;
            //        else if (FaceDirection == Directions4.Down)
            //            HitBoxPositionY = collisionRect.Top + WALL_INTERSECT_DISTANCE - Height;
            //        else if (FaceDirection == Directions4.Left)
            //            HitBoxPositionX = collisionRect.Right - WALL_INTERSECT_DISTANCE;
            //        else if (FaceDirection == Directions4.Right)
            //            HitBoxPositionX = collisionRect.Left + WALL_INTERSECT_DISTANCE - Width;

            //        isFired = false;
            //        hasHit = true;
            //    }

            //}

            if (hasHit)
            {
                lifeTimer++;
                if (lifeTimer >= LIFETIME)
                {
                    isAlive = false;
                }

                if (entityHit != null)
                {
                    BoundingBox.ActualX = entityHit.BoundingBox.ActualX + entityHitPosition.X;
                    BoundingBox.ActualY = entityHit.BoundingBox.ActualY + entityHitPosition.Y;

                    if (!entityHit.IsAlive || !entityHit.IsActive)
                        this.isAlive = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            bool shouldDraw = true;
            if (lifeTimer >= BLINK_START_TIME)
            {
                int n = lifeTimer % (BLINK_DELAY * 2);
                if (n < BLINK_DELAY)
                    shouldDraw = false;
            }
            if (shouldDraw)
            {
                base.Draw(spriteBatch, changeColorsEffect);
            }
        }

        public override void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox)
        {
            
        }

        public void Fire()
        {
            isFired = true;

            if (FaceDirection == Directions4.Up)
                Velocity.Y = -FIRE_SPEED;
            else if (FaceDirection == Directions4.Down)
                Velocity.Y = FIRE_SPEED;
            else if (FaceDirection == Directions4.Left)
                Velocity.X = -FIRE_SPEED;
            else if (FaceDirection == Directions4.Right)
                Velocity.X = FIRE_SPEED;

            area.Entities.Add(this);
        }

        public void HitEntity(Entity entity)
        {
            isFired = false;
            hasHit = true;
            Velocity = Vector2.Zero;

            entityHit = entity;
            entityHitPosition = new Vector2(BoundingBox.ActualX - entity.BoundingBox.ActualX, 
                BoundingBox.ActualY - entity.BoundingBox.ActualY);
        }

        private bool canBeStoppedBy(Entity entity)
        {
            return !entity.IsObstacle; // && !(entity is Spikes);
        }
    }
}
