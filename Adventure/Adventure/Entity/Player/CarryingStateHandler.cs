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

        public override bool CanMove
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

        public override bool CanStartUsingEquippableItem(EquippableItem item)
        {
            return false;
        }

        CarriableEntity carriable;

        public CarryingStateHandler(Player player)
            : base(player) { }

        public override void Start(Entity associatedEntity)
        {
            carriable = (CarriableEntity)associatedEntity;
            carriable.StartBeingLifted();
            carriable.Area.Entities.Remove(carriable);
        }

        public override void Update(GamePadState gamepadState, GamePadState previousGamepadState)
        {
            carriable.Update();
            if (gamepadState.IsButtonDown(Buttons.A) && previousGamepadState.IsButtonUp(Buttons.A))
            {
                startThrowing();
                player.EnterState(PlayerState.Normal);
            }
        }

        public override void End(PlayerState newState)
        {
            startThrowing();
        }

        private void startThrowing()
        {
            carriable.StartBeingThrown(player.FaceDirection);
            carriable.Area.Entities.Add(carriable);
            carriable = null;
        }
    }
}
