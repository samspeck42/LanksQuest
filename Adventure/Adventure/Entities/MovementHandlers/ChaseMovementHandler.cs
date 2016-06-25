using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure.Entities.MovementHandlers
{
    public class ChaseMovementHandler : MovementHandler
    {
        private float speed;
        private Entity entityBeingChased;

        public ChaseMovementHandler(Entity entity, float speed, Entity entityBeingChased)
            : base(entity)
        {
            this.speed = speed;
            this.entityBeingChased = entityBeingChased;
        }

        public override void UpdateMovement(GameTime gameTime)
        {
            Vector2 direction = entityBeingChased.Center - entity.Center;
            if (direction != Vector2.Zero)
                direction.Normalize();

            movement.X = (speed * (float)gameTime.ElapsedGameTime.TotalSeconds) * direction.X;
            movement.Y = (speed * (float)gameTime.ElapsedGameTime.TotalSeconds) * direction.Y;
        }
    }
}
