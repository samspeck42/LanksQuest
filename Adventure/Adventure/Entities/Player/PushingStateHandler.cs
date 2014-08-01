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
            player.Velocity = DirectionsHelper.GetDirectionVector(player.FaceDirection) * Player.PUSH_SPEED;
            player.IsReadyToPush = false;
            movableEntity.StartBeingPushed(player.FaceDirection);
        }

        public override void Update(GameTime gameTime)
        {
            if (movableEntity.ReachedPushDestination())
            {
                movableEntity.EndPush();

                if (player.FaceDirection == Directions4.Left)
                    player.BoundingBox.ActualX = movableEntity.BoundingBox.ActualX + movableEntity.Width;
                else if (player.FaceDirection == Directions4.Right)
                    player.BoundingBox.ActualX = movableEntity.BoundingBox.ActualX - player.Width;
                else if (player.FaceDirection == Directions4.Up)
                    player.BoundingBox.ActualY = movableEntity.BoundingBox.ActualY + movableEntity.Height;
                else if (player.FaceDirection == Directions4.Down)
                    player.BoundingBox.ActualY = movableEntity.BoundingBox.ActualY - player.Height;

                player.Velocity = Vector2.Zero;
                player.StartToPushTimer = 0;
                movableEntity = null;
                player.IsReadyToPush = false;
                player.EnterState(new NormalStateHandler(player));
            }
        }
    }
}
