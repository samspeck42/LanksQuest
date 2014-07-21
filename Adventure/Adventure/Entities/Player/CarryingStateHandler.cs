using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

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

        public override void Update(GamePadState gamepadState, GamePadState previousGamepadState)
        {
            carriableEntity.Update();
            if (gamepadState.IsButtonDown(Buttons.A) && previousGamepadState.IsButtonUp(Buttons.A))
            {
                startThrowing();
                player.EnterState(new NormalStateHandler(player));
            }
        }

        public override void End(PlayerState newState)
        {
            if (carriableEntity != null)
                startThrowing();
        }

        private void startThrowing()
        {
            carriableEntity.StartBeingThrown(player.FaceDirection);
            carriableEntity.Area.Entities.Add(carriableEntity);
            carriableEntity = null;
        }
    }
}
