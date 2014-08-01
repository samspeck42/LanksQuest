using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public class HurtStateHandler : PlayerStateHandler
    {

        public override PlayerState State
        {
            get { return PlayerState.Hurt; }
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

        public HurtStateHandler(Player player)
            : base(player) { }

        public override void Start(Entity associatedEntity)
        {
            player.Health -= associatedEntity.Damage;
            player.HitSound.Play(0.75f, 0, 0);
            player.KnockBack(associatedEntity);
            player.StartInvincibility();
        }

        public override void Update(GameTime gameTime)
        {
            if (!player.IsKnockedBack)
                player.EnterState(new NormalStateHandler(player));
        }
    }
}
