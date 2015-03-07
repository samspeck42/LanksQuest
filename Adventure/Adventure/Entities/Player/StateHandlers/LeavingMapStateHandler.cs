using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class LeavingMapStateHandler : PlayerStateHandler
    {
        public override PlayerState State
        {
            get { return PlayerState.LeavingMap; }
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


        private MapTransition mapTransition;
        private bool isMapTransitionStarted = false;

        public LeavingMapStateHandler(Player player, MapTransition mapTransition)
            : base(player)
        {
            this.mapTransition = mapTransition;
        }

        public override bool CanStartUsingEquippableItem(EquippableItem item)
        {
            return false;
        }

        public override void Start()
        {
            Vector2 velocity = DirectionsHelper.GetDirectionVector(mapTransition.Direction) * 80;
            player.StartMovement(new StraightMovementHandler(player, velocity, Area.TILE_WIDTH));
            player.FaceDirection = mapTransition.Direction;
        }

        public override void Update(GameTime gameTime)
        {
            if (player.MovementHandler.IsFinished && !isMapTransitionStarted)
            {
                player.Game.StartMapTransition(mapTransition);
                isMapTransitionStarted = true;
            }
        }

        public override void End(PlayerState newState)
        {
            player.StartMovement(new PlayerMovementHandler(player));
        }
    }
}
