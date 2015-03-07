using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Adventure
{
    public class KeyDoor : Door, Interactable
    {
        public bool CanStartInteraction
        {
            get { return state == DoorState.Closed; }
        }
        public bool MustBeAllignedWithToInteract
        {
            get { return false; }
        }

        protected override string closedSpriteName { get { return "Sprites/Environment/key_door"; } }
        protected override string openingSpriteName { get { return "Sprites/Environment/key_door_opening"; } }

        public KeyDoor(GameWorld game, Area area)
            : base(game, area)
        { }

        public void StartInteraction()
        {
            if (CanStartInteraction && game.Player.Inventory.NumKeys > 0)
            {
                game.Player.Inventory.UseKey();
                startOpening();
            }
        }
    }
}
