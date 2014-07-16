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

        private AnimatedSprite verticalSprite;
        private AnimatedSprite horizontalSprite;
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
                if (FaceDirection == Directions.Up)
                    return new Vector2(Position.X + (Width / 2), Position.Y);
                else if (FaceDirection == Directions.Down)
                    return new Vector2(Position.X + (Width / 2), Position.Y + Height);
                else if (FaceDirection == Directions.Left)
                    return new Vector2(Position.X, Position.Y + (Height / 2));
                else if (FaceDirection == Directions.Right)
                    return new Vector2(Position.X + Width, Position.Y + (Height / 2));
                return Vector2.Zero;
            }
        }

        public Arrow(GameWorld game, Area area, Directions direction)
            : base(game, area)
        {
            IsAffectedByWallCollisions = false;
            Rectangle bounds = new Rectangle(0, 0, 8, 20);
            verticalSprite = new AnimatedSprite(bounds);
            bounds = new Rectangle(0, 0, 20, 8);
            horizontalSprite = new AnimatedSprite(bounds);

            FaceDirection = direction;

            if (FaceDirection == Directions.Up)
            {
                CurrentSprite = verticalSprite;
                CurrentSprite.Rotation = 0;
            }
            else if (FaceDirection == Directions.Down)
            {
                CurrentSprite = verticalSprite;
                CurrentSprite.Rotation = MathHelper.Pi;
            }
            else if (FaceDirection == Directions.Left)
            {
                CurrentSprite = horizontalSprite;
                CurrentSprite.Rotation = 0;
            }
            else if (FaceDirection == Directions.Right)
            {
                CurrentSprite = horizontalSprite;
                CurrentSprite.Rotation = MathHelper.Pi;
            }

            isFired = false;
            hasHit = false;
            IsInAir = true;
            Damage = DAMAGE;
        }

        public override void LoadContent()
        {
            verticalSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Items/arrow_vertical");
            horizontalSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Items/arrow_horizontal");
        }

        public override void Update()
        {
            base.Update();

            bool isInsideWall = false;
            Point tipCell = Area.ConvertPositionToCell(TipPosition);
            Rectangle collisionRect = new Rectangle();

            if (area.GetCollisionAtCell(tipCell) == TileCollision.Impassable)
            {
                isInsideWall = true;
                collisionRect = Area.CreateRectangleForCell(tipCell);
            }
            else
            {
                foreach (Entity entity in area.GetActiveEntitiesAtCell(tipCell))
                {
                    if (entity.Contains(TipPosition) && canBeStoppedBy(entity))
                    {
                        isInsideWall = true;
                        collisionRect = entity.HitBox;
                    }
                }
            }

            if (isInsideWall)
            {
                float intersectDistance = 0f;
                if (FaceDirection == Directions.Up)
                    intersectDistance = collisionRect.Bottom - TipPosition.Y;
                else if (FaceDirection == Directions.Down)
                    intersectDistance = TipPosition.Y - collisionRect.Top;
                else if (FaceDirection == Directions.Left)
                    intersectDistance = collisionRect.Right - TipPosition.X;
                else if (FaceDirection == Directions.Right)
                    intersectDistance = TipPosition.X - collisionRect.Left;

                if (intersectDistance >= 10)
                {
                    Velocity = Vector2.Zero;

                    if (FaceDirection == Directions.Up)
                        Position.Y = collisionRect.Bottom - WALL_INTERSECT_DISTANCE;
                    else if (FaceDirection == Directions.Down)
                        Position.Y = collisionRect.Top + WALL_INTERSECT_DISTANCE - Height;
                    else if (FaceDirection == Directions.Left)
                        Position.X = collisionRect.Right - WALL_INTERSECT_DISTANCE;
                    else if (FaceDirection == Directions.Right)
                        Position.X = collisionRect.Left + WALL_INTERSECT_DISTANCE - Width;

                    isFired = false;
                    hasHit = true;
                }

            }

            if (hasHit)
            {
                lifeTimer++;
                if (lifeTimer >= LIFETIME)
                {
                    isAlive = false;
                }

                if (entityHit != null)
                {
                    Position.X = entityHit.Position.X + entityHitPosition.X;
                    Position.Y = entityHit.Position.Y + entityHitPosition.Y;

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

        public override void OnEntityCollision(Entity other)
        {
        }

        public void Fire()
        {
            isFired = true;

            if (FaceDirection == Directions.Up)
                Velocity.Y = -FIRE_SPEED;
            else if (FaceDirection == Directions.Down)
                Velocity.Y = FIRE_SPEED;
            else if (FaceDirection == Directions.Left)
                Velocity.X = -FIRE_SPEED;
            else if (FaceDirection == Directions.Right)
                Velocity.X = FIRE_SPEED;

            area.Entities.Add(this);
        }

        public void HitEntity(Entity entity)
        {
            isFired = false;
            hasHit = true;
            Velocity = Vector2.Zero;

            entityHit = entity;
            entityHitPosition = new Vector2(Position.X - entity.Position.X, Position.Y - entity.Position.Y);
        }

        private bool canBeStoppedBy(Entity entity)
        {
            return !entity.IsPassable && !(entity is Spikes);
        }
    }
}
