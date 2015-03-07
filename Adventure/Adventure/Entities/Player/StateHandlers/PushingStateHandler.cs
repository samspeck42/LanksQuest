using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public class PushingStateHandler : PlayerStateHandler
    {
        public override PlayerState State
        {
            get { return PlayerState.Pushing; }
        }

        public override bool CanWalk
        {
            get { return false; }
        }

        public override bool CanGetHurt
        {
            get { return false; }
        }

        public override bool CanLeaveArea
        {
            get { return false; }
        }

        public override bool CanStartAttacking
        {
            get { return false; }
        }

        public override bool CanInteract
        {
            get { return false; }
        }

        public override bool CanChangeFaceDirection
        {
            get { return false; }
        }

        public override bool CanStartUsingEquippableItem(EquippableItem item)
        {
            return false;
        }


        private Directions4 direction;

        public PushingStateHandler(Player player, Directions4 direction)
            : base(player) 
        {
            this.direction = direction;
        }

        public override void Start()
        {
            Vector2 velocity = DirectionsHelper.GetDirectionVector(direction) * 80;
            player.StartMovement(new StraightMovementHandler(player, velocity, Area.TILE_WIDTH));
        }

        public override void Update(GameTime gameTime)
        {
            if (player.MovementHandler.IsFinished)
            {
                player.StartMovement(new PlayerMovementHandler(player));
                player.EnterState(new NormalStateHandler(player));
            }
        }

        public override void End(PlayerState newState)
        {
            player.StartMovement(new PlayerMovementHandler(player));
        }
    }
}
