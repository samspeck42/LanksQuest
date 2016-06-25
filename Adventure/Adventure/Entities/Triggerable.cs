using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adventure.Entities
{
    public interface Triggerable
    {
        void TriggerOn();
        void TriggerOff();
    }
}
