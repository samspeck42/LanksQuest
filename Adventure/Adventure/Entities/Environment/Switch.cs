using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public abstract class Switch : ActivatingEntity
    {
        public Switch(GameWorld game, Area area)
            : base(game, area)
        { }

        public virtual bool IsActivatedByPlayerSword(HitBox thisHitBox)
        {
            return false;
        }

        public virtual bool IsActivatedByArrow(HitBox thisHitBox)
        {
            return false;
        }

        public abstract void Activate();
    }
}
