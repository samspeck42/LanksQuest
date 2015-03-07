using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class BounceMovementHandler : MovementHandler
    {
        private const float GRAVITATIONAL_ACCELERATION = 30f;

        public float GroundVelocityX
        {
            get
            {
                return groundVelocity.X;
            }
            set
            {
                groundVelocity.X = value;
            }
        }
        public float GroundVelocityY
        {
            get
            {
                return groundVelocity.Y;
            }
            set
            {
                groundVelocity.Y = value;
            }
        }

        private Vector2 groundVelocity;
        private float upwardSpeed;
        private float bounceSpeedReduction;
        private float height;
        private int numBounces;
        private float initialPositionRelativeY;
        private float bounceSpeed;
        private int bounceCounter = 0;

        public BounceMovementHandler(Entity entity, Vector2 groundVelocity, float initialUpwardSpeed,
            float initialBounceSpeed, float bounceSpeedReduction, float initialHeight, int numBounces)
            : base(entity)
        {
            this.groundVelocity = groundVelocity;
            this.upwardSpeed = initialUpwardSpeed;
            this.bounceSpeed = Math.Max(0f, initialBounceSpeed);
            this.bounceSpeedReduction = bounceSpeedReduction;
            this.height = Math.Max(0f, initialHeight);
            this.numBounces = numBounces;
            this.initialPositionRelativeY = entity.BoundingBox.EntityPositionRelativeY;
        }

        public BounceMovementHandler(Entity entity, float groundSpeed, float angle, float initialUpwardSpeed,
            float initialBounceSpeed, float bounceSpeedReduction, float initialHeight, int numBounces)
            : base(entity)
        {
            this.groundVelocity = new Vector2(
                groundSpeed * (float)Math.Cos(angle),
                groundSpeed * (float)Math.Sin(angle));
            this.upwardSpeed = initialUpwardSpeed;
            this.bounceSpeed = Math.Max(0f, initialBounceSpeed);
            this.bounceSpeedReduction = bounceSpeedReduction;
            this.height = Math.Max(0f, initialHeight);
            this.numBounces = numBounces;
            this.initialPositionRelativeY = entity.ObstacleCollisionBox.EntityPositionRelativeY;
        }

        public override void Start()
        {
            base.Start();

            entity.ObstacleCollisionBox.EntityPositionRelativeY = initialPositionRelativeY - height;
        }

        public override void UpdateMovement(GameTime gameTime)
        {
            height += upwardSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (height <= 0)
            {
                height = 0;
                upwardSpeed = bounceSpeed;
                bounceSpeed *= bounceSpeedReduction;
                bounceCounter++;
                entity.OnMovementEvent(MovementEvent.CollisionWithGround);
            }
            else
            {
                upwardSpeed -= GRAVITATIONAL_ACCELERATION;
            }

            entity.ObstacleCollisionBox.EntityPositionRelativeY = initialPositionRelativeY - height;

            movement.X = groundVelocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            movement.Y = groundVelocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (bounceCounter > numBounces)
                isFinished = true;
        }
    }
}
