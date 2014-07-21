using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public abstract class Switch : ActivatingEntity
    {
        protected bool isOn = false;
        public bool IsOn { get { return isOn; } }

        public Switch(GameWorld game, Area area)
            : base(game, area)
        {
        }
    }
}
