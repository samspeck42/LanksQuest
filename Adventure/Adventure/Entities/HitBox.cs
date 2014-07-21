using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class HitBox
    {
        public const string OBSTACLE_COLLISION_BOX_ID = "obstacle_collision_box";

        private Entity owner;
        private string id;

        private Vector2 ownerPositionOffset;
        private Rectangle hitBox;
        public float X
        {
            get { return hitBox.X; }
            set
            {
                // only the obstacle collision box can change the position of the entity
                if (id.Equals(OBSTACLE_COLLISION_BOX_ID))
                    owner.Position.X = value - ownerPositionOffset.X;
            }
        }
        public float Y
        {
            get { return hitBox.Y; }
            set
            {
                // only the obstacle collision box can change the position of the entity
                if (id.Equals(OBSTACLE_COLLISION_BOX_ID))
                    owner.Position.Y = value - ownerPositionOffset.Y;
            }
        }

        private bool isActive;
        private bool takesDamageFromPlayerSword;
        private bool takesDamageFromArrow;
        private bool damagesPlayer;
        private int damage;
    }
}
