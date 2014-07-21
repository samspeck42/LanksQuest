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
        public abstract bool CanWalk { get; }
        public abstract bool CanGetHurt { get; }
        public abstract bool CanLeaveArea { get; }
        public abstract bool CanStartAttacking { get; }
        public abstract bool CanInteract { get; }
        public abstract bool CanChangeFaceDirection { get; }
        public abstract bool CanStartUsingEquippableItem(EquippableItem item);

        // states may have different sprite sets for walking and standing still
        public virtual string StillSpritesId { get { return PlayerSpriteHandler.STILL_SPRITES_ID; } }
        public virtual string WalkingSpritesId { get { return PlayerSpriteHandler.ATTACKING_SPRITES_ID; } }

        public PlayerStateHandler(Player player)
        {
            this.player = player;
        }

        public abstract void Start(Entity associatedEntity);
        public abstract void Update(GamePadState gamepadState, GamePadState previousGamepadState);
        public virtual void End(PlayerState newState) { }
    }
}
