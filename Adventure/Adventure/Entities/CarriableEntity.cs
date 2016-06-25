using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine;
using Microsoft.Xna.Framework;
using Adventure.Entities.MovementHandlers;
using Adventure.Maps;

namespace Adventure.Entities
{
    public abstract class CarriableEntity : Entity, Interactable, ShadowOwner
    {
        protected const float THROW_SPEED = 700f;
        private const string OBSTACLE_COLLISION_BOX_ID = "obstacle_collision_box";
        
        public bool IsThrown { get { return isThrown; } }
        public virtual bool CanStartInteraction { get { return !isThrown; } }
        public bool MustBeAllignedWithToInteract { get { return false; } }

        public override HitBox ObstacleCollisionBox { get { return GetHitBoxById(OBSTACLE_COLLISION_BOX_ID); } }
        public override TileCollision ObstacleTileCollisions
        {
            get
            {
                return TileCollision.Wall | TileCollision.Doorway;
            }
        }
        public override DrawLayer DrawLayer
        {
            get
            {
                return isThrown ? DrawLayer.High : DrawLayer.Middle;
            }
        }

        public virtual ShadowSizes ShadowSize { get { return ShadowSizes.Medium; } }
        public virtual Vector2 ShadowCenter
        {
            get
            {
                return new Vector2(
                    this.BoundingBox.Left + (this.BoundingBox.Width / 2),
                    this.BoundingBox.Bottom - 5);
            }
        }

        protected bool isThrown = false;
        protected int numBouncesBeforeLand;
        protected MovementHandler movementHandler = null;

        public CarriableEntity(GameWorld game, Map map, Area area, int numBouncesBeforeLand)
            : base(game, map, area)
        {
            this.numBouncesBeforeLand = numBouncesBeforeLand;

            HitBox obstacleCollisionBox = new HitBox(this, OBSTACLE_COLLISION_BOX_ID);
            obstacleCollisionBox.IsActive = true;
            HitBoxes.Add(obstacleCollisionBox);
        }

        public override void Update(GameTime gameTime)
        {
            if (movementHandler != null)
            {
                movementHandler.Update(gameTime);

                if (movementHandler.IsFinished)
                    land();
            }

            spriteHandler.Update(gameTime);
        }

        public void StartInteraction()
        {
            if (CanStartInteraction)
                gameWorld.Player.StartLifting(this);
        }

        public void StartThrow(Directions4 direction)
        {
            Vector2 directionVector = DirectionsHelper.GetDirectionVector(direction);

            movementHandler = new BounceMovementHandler(this,
                new Vector2(
                    directionVector.X * THROW_SPEED,
                    directionVector.Y * THROW_SPEED),
                0f, 180f, 0.8f, gameWorld.Player.Height, numBouncesBeforeLand);
            movementHandler.Start();

            isThrown = true;
            FaceDirection = direction;
        }

        protected virtual void land()
        {
            isThrown = false;
        }
    }
}
