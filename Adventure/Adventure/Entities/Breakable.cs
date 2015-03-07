using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adventure
{
    interface Breakable
    {
        bool CanBeBroken { get; }

        void StartBreaking();
    }
}
