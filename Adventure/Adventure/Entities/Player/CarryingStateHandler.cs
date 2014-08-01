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

        CarriableEntity carriableEntity;

        public CarryingStateHandler(Player player)
            : base(player) { }

        public override void Start(Entity associatedEntity)
        {
            carriableEntity = (CarriableEntity)associatedEntity;
            carriableEntity.StartBeingLifted();
            carriableEntity.Area.Entities.Remove(carriableEntity);
        }

        public override void Update(GameTime gameTime)
        {
            carriableEntity.Update(gameTime);
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
            carriableEntity.StartBeingThrown(player.FaceDirection);
            carriableEntity.Area.Entities.Add(carriableEntity);
            carriableEntity = null;
        }
    }
}
