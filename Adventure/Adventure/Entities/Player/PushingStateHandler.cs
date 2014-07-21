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

        MovableBlock movableEntity;

        public PushingStateHandler(Player player)
            : base(player) { }

        public override void Start(Entity associatedEntity)
        {
            movableEntity = (MovableBlock)associatedEntity;
            player.Velocity = Entity.GetDirectionVector(player.FaceDirection) * Player.PUSH_SPEED;
            player.IsReadyToPush = false;
            movableEntity.StartBeingPushed(player.FaceDirection);
        }

        public override void Update(GamePadState gamepadState, GamePadState previousGamepadState)
        {
            if (movableEntity.ReachedPushDestination())
            {
                movableEntity.EndPush();

                if (player.FaceDirection == Directions.Left)
                    player.HitBoxPositionX = movableEntity.HitBoxPosition.X + movableEntity.Width;
                else if (player.FaceDirection == Directions.Right)
                    player.HitBoxPositionX = movableEntity.HitBoxPosition.X - player.Width;
                else if (player.FaceDirection == Directions.Up)
                    player.HitBoxPositionY = movableEntity.HitBoxPosition.Y + movableEntity.Height;
                else if (player.FaceDirection == Directions.Down)
                    player.HitBoxPositionY = movableEntity.HitBoxPosition.Y - player.Height;

                player.Velocity = Vector2.Zero;
                player.StartToPushTimer = 0;
                movableEntity = null;
                player.IsReadyToPush = false;
                player.EnterState(new NormalStateHandler(player));
            }
        }
    }
}
