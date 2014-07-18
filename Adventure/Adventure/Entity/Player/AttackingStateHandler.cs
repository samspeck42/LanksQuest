using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adventure
{
    public class AttackingStateHandler : PlayerStateHandler
    {
        public override PlayerState State
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanMove
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanGetHurt
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanLeaveArea
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanStartAttacking
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanStartUsingEquippableItem(EquippableItem item)
        {
            throw new NotImplementedException();
        }

        public AttackingStateHandler(Player player)
            : base(player) { }

        public override void Start(Entity associatedEntity)
        {
            throw new NotImplementedException();
        }

        public override void Update(Microsoft.Xna.Framework.Input.GamePadState gamepadState, Microsoft.Xna.Framework.Input.GamePadState previousGamepadState)
        {
            throw new NotImplementedException();
        }

        public override void End(PlayerState newState)
        {
            throw new NotImplementedException();
        }
    }
}
