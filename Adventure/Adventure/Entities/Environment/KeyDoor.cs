using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Adventure.Maps;

namespace Adventure.Entities.Environment
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

        public KeyDoor(GameWorld game, Map map, Area area)
            : base(game, map, area)
        { }

        public void StartInteraction()
        {
            if (CanStartInteraction && gameWorld.Player.Inventory.NumKeys > 0)
            {
                gameWorld.Player.Inventory.UseKey();
                startOpening();
            }
        }
    }
}
