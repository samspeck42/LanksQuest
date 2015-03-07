using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class CarryingStateHandler : PlayerStateHandler
    {
        public override PlayerState State
        {
            get { return PlayerState.Carrying; }
        }

        public override bool CanWalk
        {
            get { return true; }
        }

        public override bool CanGetHurt
        {
            get { return true; }
        }

        public override bool CanLeaveArea
        {
            get { return false; }
        }

        public override bool CanStartAttacking
        {
            get { return true; }
        }

        public override bool CanInteract
        {
            get { return false; }
        }

        public override bool CanChangeFaceDirection
        {
            get { return true; }
        }

        public override bool CanStartUsingEquippableItem(EquippableItem item)
        {
            return false;
        }

        public CarriableEntity CarriableEntity { get { return carriableEntity; } }

        private CarriableEntity carriableEntity;

        public CarryingStateHandler(Player player, CarriableEntity carriableEntity)
            : base(player) 
        {
            this.carriableEntity = carriableEntity;
        }

        public override void Start()
        {
            carriableEntity.Area.Entities.Remove(carriableEntity);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void End(PlayerState newState)
        {
            if (carriableEntity != null)
                startThrowing();
        }

        public override void OnInteractButtonPressed()
        {
            startThrowing();
            player.EnterState(new NormalStateHandler(player));
        }

        private void startThrowing()
        {
            carriableEntity.BoundingBox.ActualX = player.Center.X - (carriableEntity.Width / 2);
            carriableEntity.BoundingBox.ActualY = player.BoundingBox.Bottom - carriableEntity.Height;

            carriableEntity.StartThrow(player.FaceDirection);
            carriableEntity.Area.Entities.Add(carriableEntity);
            carriableEntity = null;
        }
    }
}
