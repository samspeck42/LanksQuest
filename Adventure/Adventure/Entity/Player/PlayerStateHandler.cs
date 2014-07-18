using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public abstract class PlayerStateHandler
    {
        protected Player player;
        public abstract PlayerState State { get; }
        public abstract bool CanMove { get; }
        public abstract bool CanGetHurt { get; }
        public abstract bool CanLeaveArea { get; }
        public abstract bool CanStartAttacking { get; }
        public abstract bool CanStartUsingEquippableItem(EquippableItem item);

        public PlayerStateHandler(Player player)
        {
            this.player = player;
        }

        public abstract void Start(Entity associatedEntity);
        public abstract void Update(GamePadState gamepadState, GamePadState previousGamepadState);
        public abstract void End(PlayerState newState);
    }
}
