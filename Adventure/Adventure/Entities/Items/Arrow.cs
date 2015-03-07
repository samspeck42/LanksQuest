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
        private const string NORMAL_SPRITES_ID = "normal_sprites";
        private const string TIP_HIT_BOX_ID = "tip_hit_box";

        private const float FIRE_SPEED = 360;
        private const int LIFETIME = 3000;
        private const int BLINK_START_TIME = 2000;

        public override TileCollision ObstacleTileCollisions
        {
            get
            {
                return TileCollision.Wall | TileCollision.Doorway;
            }
        }
        public override int Damage { get { return 2; } }
        public override DrawLayer DrawLayer
        {
            get
            {
                return DrawLayer.High;
            }
        }

        private MovementHandler movementHandler = null;
        //private bool isFired = false;
        private bool hasHit = false;
        private int lifeTimer = 0;
        private Entity entityHit = null;
        private Vector2 entityHitPosition = Vector2.Zero;


        public Arrow(GameWorld game, Area area)
            : base(game, area)
        {
            BoundingBox.RelativeX = -4;
            BoundingBox.RelativeY = -4;
            BoundingBox.Width = 8;
            BoundingBox.Height = 8;

            HitBox tipHitBox = new HitBox(this, TIP_HIT_BOX_ID);
            tipHitBox.Width = 6;
            tipHitBox.Height = 6;
            tipHitBox.IsActive = false;
            HitBoxes.Add(tipHitBox);

            SpriteSet spriteSet = new SpriteSet();
            Vector2 origin = new Vector2(4, 4);
            Sprite sprite = new Sprite("Sprites/Items/arrow", this, origin);
            spriteSet.SetSprite(Directions4.Right, sprite);

            sprite = new Sprite("Sprites/Items/arrow", this, origin);
            sprite.Rotation = MathHelper.PiOver2;
            spriteSet.SetSprite(Directions4.Down, sprite);

            sprite = new Sprite("Sprites/Items/arrow", this, origin);
            sprite.Rotation = MathHelper.Pi;
            spriteSet.SetSprite(Directions4.Left, sprite);

            sprite = new Sprite("Sprites/Items/arrow", this, origin);
            sprite.Rotation = 3 * MathHelper.PiOver2;
            spriteSet.SetSprite(Directions4.Up, sprite);

            spriteHandler.AddSpriteSet(NORMAL_SPRITES_ID, spriteSet);
            spriteHandler.SetSprite(NORMAL_SPRITES_ID);

            diesOutsideArea = true;
        }

        public override void Update(GameTime gameTime)
        {
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

            spriteHandler.Update(gameTime);

            if (movementHandler != null)
            {
                movementHandler.Update(gameTime);
            }

            if (hasHit)
            {
                lifeTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (lifeTimer >= BLINK_START_TIME && !spriteHandler.IsBlinking)
                {
                    spriteHandler.StartBlinking();
                }
                if (lifeTimer >= LIFETIME)
                {
                    Die();
                }

                if (entityHit != null)
                {
                    BoundingBox.ActualX = entityHit.BoundingBox.ActualX + entityHitPosition.X;
                    BoundingBox.ActualY = entityHit.BoundingBox.ActualY + entityHitPosition.Y;

                    if (!entityHit.IsAlive || !entityHit.IsVisible)
                        Die();
                }
            }
        }

        public override void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox)
        {
            if (thisHitBox.IsId(TIP_HIT_BOX_ID))
            {
                if (other is Enemy)
                {
                    Enemy enemy = (Enemy)other;
                    if (enemy.TakesDamageFromArrow(otherHitBox))
                    {
                        enemy.TakeDamage(this, KnockBackType.FaceDirection);
                        this.hitEntity(enemy);
                    }
                }
                if (other is Switch)
                {
                    Switch sw = (Switch)other;
                    if (sw.IsActivatedByArrow(otherHitBox))
                    {
                        sw.Activate();
                        this.hitEntity(sw);
                    }
                }
            }
        }

        public void Fire(Directions4 direction)
        {
            //isFired = true;
            FaceDirection = direction;
            Vector2 directionVector = DirectionsHelper.GetDirectionVector(FaceDirection);

            HitBox tipHitBox = GetHitBoxById(TIP_HIT_BOX_ID);
            tipHitBox.RelativeX = (13 * directionVector.X) - (tipHitBox.Width / 2);
            tipHitBox.RelativeY = (13 * directionVector.Y) - (tipHitBox.Height / 2);
            tipHitBox.IsActive = true;

            Vector2 velicity = directionVector * FIRE_SPEED;
            movementHandler = new StraightMovementHandler(this, velicity);
            movementHandler.Start();

            area.Entities.Add(this);
        }

        public override void OnMovementEvent(MovementEvent movementEvent)
        {
            if (movementEvent == MovementEvent.CollisionWithObstacle)
            {
                ((StraightMovementHandler)movementHandler).Velocity = Vector2.Zero;
                //isFired = false;
                hasHit = true;
                GetHitBoxById(TIP_HIT_BOX_ID).IsActive = false;
            }
        }

        private void hitEntity(Entity entity)
        {
            //isFired = false;
            if (movementHandler != null)
                ((StraightMovementHandler)movementHandler).Velocity = Vector2.Zero;
            hasHit = true;
            GetHitBoxById(TIP_HIT_BOX_ID).IsActive = false;

            entityHit = entity;
            entityHitPosition = new Vector2(BoundingBox.ActualX - entity.BoundingBox.ActualX, 
                BoundingBox.ActualY - entity.BoundingBox.ActualY);
        }
    }
}
