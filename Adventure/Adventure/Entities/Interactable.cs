using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adventure
{
    public interface Interactable
    {
        bool CanStartInteraction { get; }

        bool MustBeAllignedWithToInteract { get; }

        void StartInteraction();
    }
}
