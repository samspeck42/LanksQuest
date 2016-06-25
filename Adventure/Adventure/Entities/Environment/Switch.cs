using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Adventure.Maps;

namespace Adventure.Entities.Environment
{
    public abstract class Switch : ActivatingEntity
    {
        public Switch(GameWorld game, Map map, Area area)
            : base(game, map, area)
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
