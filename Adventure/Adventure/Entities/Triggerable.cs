using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adventure
{
    public interface Triggerable
    {
        void TriggerOn();
        void TriggerOff();
    }
}
