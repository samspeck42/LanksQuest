using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class HitBox
    {
        private Entity owner;
        private string id;
        private Vector2 relativePosition;

        public bool IsActive = false;
        //public bool TakesDamageFromPlayerSword = false;
        //public bool TakesDamageFromArrow = false;
        //public bool TakesDamageFromPot = false;
        public bool CanStopArrow = false;

        public int Width;
        public int Height;

        /// <summary>
        /// Gets and sets the hit box's X position relative to the owner entity's position.
        /// </summary>
        public float RelativeX
        {
            get { return relativePosition.X; }
            set { relativePosition.X = value; }
        }

        /// <summary>
        /// Gets and sets the hit box's Y position relative to the owner entity's position.
        /// </summary>
        public float RelativeY
        {
            get { return relativePosition.Y; }
            set { relativePosition.Y = value; }
        }

        /// <summary>
        /// Gets and sets the owner entity's X position relative to the hit box's position without changing the 
        /// actual position of the hit box.
        /// </summary>
        public float EntityPositionRelativeX
        {
            get { return -relativePosition.X; }
            set
            {
                float actualX = this.ActualX;
                relativePosition.X = -value;
                this.ActualX = actualX;
            }
        }

        /// <summary>
        /// Gets and sets the owner entity's Y position relative to the hit box's position without changing the 
        /// actual position of the hit box.
        /// </summary>
        public float EntityPositionRelativeY
        {
            get { return -relativePosition.Y; }
            set
            {
                float actualY = this.ActualY;
                relativePosition.Y = -value;
                this.ActualY = actualY;
            }
        }

        /// <summary>
        /// Gets and sets the hit box's X position in the area. Changing it moves the entity along with it.
        /// </summary>
        public float ActualX
        {
            get { return owner.Position.X + relativePosition.X; }
            set { owner.Position.X = value - relativePosition.X; }
        }

        /// <summary>
        /// Gets and sets the hit box's Y position in the area. Changing it moves the entity along with it.
        /// </summary>
        public float ActualY
        {
            get { return owner.Position.Y + relativePosition.Y; }
            set { owner.Position.Y = value - relativePosition.Y; }
        }

        public float Top { get { return ActualY; } }

        public float Bottom { get { return ActualY + Height; } }

        public float Left { get { return ActualX; } }

        public float Right { get { return ActualX + Width; } }

        public HitBox(Entity owner, string id)
        {
            this.owner = owner;
            this.id = id;
        }

        public bool IsId(string id)
        {
            return this.id.Equals(id);
        }

        public bool CollidesWith(HitBox other)
        {
            Rectangle thisRectangle = this.ToRectangle();
            Rectangle otherRectangle = other.ToRectangle();

            return thisRectangle.Intersects(otherRectangle) || thisRectangle.Contains(otherRectangle) || otherRectangle.Contains(thisRectangle);
        }

        public bool CollidesWith(Rectangle rectangle)
        {
            Rectangle thisRectangle = ToRectangle();

            return thisRectangle.Intersects(rectangle) || thisRectangle.Contains(rectangle) || rectangle.Contains(thisRectangle);
        }

        public bool Contains(Vector2 point)
        {
            return point.X >= this.ActualX && point.X < (this.ActualX + this.Width) &&
                point.Y >= this.ActualY && point.Y < (this.ActualY + this.Height);
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)Math.Round(this.ActualX), (int)Math.Round(this.ActualY),
                this.Width, this.Height);
        }

        /// <summary>
        /// Sets this hit box's relative position, width, and height using the given rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle whose properties will be used to modify this hit box.</param>
        public void SetFromRectangle(Rectangle rectangle)
        {
            this.RelativeX = rectangle.X;
            this.RelativeY = rectangle.Y;
            this.Width = rectangle.Width;
            this.Height = rectangle.Height;
        }
    }
}
