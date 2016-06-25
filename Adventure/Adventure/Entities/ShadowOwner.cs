using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public interface ShadowOwner
    {
        ShadowSizes ShadowSize { get; }

        Vector2 ShadowCenter { get; }
    }

    public enum ShadowSizes
    {
        None = -1,
        Small = 0,
        Medium = 1
    }
}
