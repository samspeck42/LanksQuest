using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Adventure.Entities.Items;

namespace Adventure.PlayerStateHandlers
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
        public virtual string WalkingSpritesId { get { return PlayerSpriteHandler.WALKING_SPRITES_ID; } }

        public virtual float WalkSpeed { get { return Player.NORMAL_WALK_SPEED; } }

        public PlayerStateHandler(Player player)
        {
            this.player = player;
        }

        public virtual void Start() { }
        public abstract void Update(GameTime gameTime);
        public virtual void End(PlayerState newState) { }

        public virtual void OnInteractButtonPressed() { }
        public virtual void OnAttackButtonPressed() { }
        public virtual void OnEquippedItemButtonPressed(int index) { }
        public virtual void OnDirectionButtonPressed(Directions direction) { }
        public void OnButtonPressed(PlayerButtons button)
        {
            switch (button)
            {
                case PlayerButtons.Interact:
                    OnInteractButtonPressed();
                    break;
                case PlayerButtons.Attack:
                    OnAttackButtonPressed();
                    break;
                case PlayerButtons.EquippedItem1:
                    OnEquippedItemButtonPressed(0);
                    break;
                case PlayerButtons.EquippedItem2:
                    OnEquippedItemButtonPressed(1);
                    break;
                case PlayerButtons.Up:
                    OnDirectionButtonPressed(Directions.Up);
                    break;
                case PlayerButtons.Down:
                    OnDirectionButtonPressed(Directions.Down);
                    break;
                case PlayerButtons.Left:
                    OnDirectionButtonPressed(Directions.Left);
                    break;
                case PlayerButtons.Right:
                    OnDirectionButtonPressed(Directions.Right);
                    break;
                default:
                    break;
            }
        }

        public virtual void OnInteractButtonReleased() { }
        public virtual void OnAttackButtonReleased() { }
        public virtual void OnEquippedItemButtonReleased(int index) { }
        public virtual void OnDirectionButtonReleased(Directions direction) { }
        public void OnButtonReleased(PlayerButtons button)
        {
            switch (button)
            {
                case PlayerButtons.Interact:
                    OnInteractButtonReleased();
                    break;
                case PlayerButtons.Attack:
                    OnAttackButtonReleased();
                    break;
                case PlayerButtons.EquippedItem1:
                    OnEquippedItemButtonReleased(0);
                    break;
                case PlayerButtons.EquippedItem2:
                    OnEquippedItemButtonReleased(1);
                    break;
                case PlayerButtons.Up:
                    OnDirectionButtonReleased(Directions.Up);
                    break;
                case PlayerButtons.Down:
                    OnDirectionButtonReleased(Directions.Down);
                    break;
                case PlayerButtons.Left:
                    OnDirectionButtonReleased(Directions.Left);
                    break;
                case PlayerButtons.Right:
                    OnDirectionButtonReleased(Directions.Right);
                    break;
                default:
                    break;
            }
        }
    }

    public enum PlayerState
    {
        Normal,
        Hurt,
        Attacking,
        Grabbing,
        Pushing,
        Pulling,
        Carrying,
        UsingBow,
        OpeningChest,
        LeavingMap
    }
}
