using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class PlayerMovementHandler : MovementHandler
    {
        private Player player;

        public PlayerMovementHandler(Player player)
            : base(player)
        {
            this.player = player;
        }

        public override void UpdateMovement(GameTime gameTime)
        {
            movement = Vector2.Zero;

            if (player.CanWalk)
            {
                Vector2 inputMovementVector = DirectionsHelper.GetDirectionVector(player.Input.InputDirection);
                Vector2 velocity = new Vector2(
                    inputMovementVector.X * player.WalkSpeed,
                    inputMovementVector.Y * player.WalkSpeed);

                movement.X = velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                movement.Y = velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
