using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure.Entities.MovementHandlers
{
    public class StraightMovementHandler : MovementHandler
    {
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        private Vector2 velocity;
        private bool travellingSetDistance;
        private float distanceToTravel;
        private float distanceTravelled = 0;

        public StraightMovementHandler(Entity entity, Vector2 velocity)
            : base(entity)
        {
            this.velocity = velocity;
            this.travellingSetDistance = false;
            this.distanceToTravel = 0;
        }

        public StraightMovementHandler(Entity entity, float speed, float angle)
            : base(entity)
        {
            this.velocity = new Vector2(
                speed * (float)Math.Cos(angle),
                speed * (float)Math.Sin(angle));
            this.travellingSetDistance = false;
            this.distanceToTravel = 0;
        }

        public StraightMovementHandler(Entity entity, Vector2 velocity, float distanceToTravel)
            : base(entity)
        {
            this.velocity = velocity;
            this.travellingSetDistance = true;
            this.distanceToTravel = distanceToTravel;
        }

        public StraightMovementHandler(Entity entity, float speed, float angle, float distanceToTravel)
            : base(entity)
        {
            this.velocity = new Vector2(
                speed * (float)Math.Cos(angle),
                speed * (float)Math.Sin(angle));
            this.travellingSetDistance = true;
            this.distanceToTravel = distanceToTravel;
        }

        public override void Start()
        {
            base.Start();

            distanceTravelled = 0f;
        }

        public override void UpdateMovement(GameTime gameTime)
        {
            movement.X = velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            movement.Y = velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!travellingSetDistance)
                return;

            float distance = movement.Length();

            if (distanceTravelled + distance >= distanceToTravel)
            {
                distance = distanceToTravel - distanceTravelled;
                float angle = (float)Math.Atan2(movement.Y, movement.X);

                movement.X = distance * (float)Math.Cos(angle);
                movement.Y = distance * (float)Math.Sin(angle);

                isFinished = true;
            }
            else
            {
                distanceTravelled += distance;
            }
        }
    }
}
