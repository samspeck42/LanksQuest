using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public abstract class CarriableEntity : Entity
    {
        protected const float THROW_SPEED = 14f;
        protected const float FALL_SPEED = 2.5f;
        protected const int THROW_TIME = 12;

        protected bool isBeingCarried;
        protected bool isThrown;
        public bool IsThrown { get { return isThrown; } }

        protected Directions throwDirection;
        protected int throwTimer;

        public CarriableEntity(GameWorld game, Area area)
            : base(game, area)
        {
            isBeingCarried = false;
            isThrown = false;
            throwDirection = Directions.Up;
            throwTimer = 0;
            IsAffectedByWallCollisions = false;
            CanLeaveArea = false;
        }

        public override void Update()
        {
            base.Update();

            if (isBeingCarried)
                doCarry();

            if (isThrown)
                doThrow();
        }

        public void StartBeingLifted()
        {
            Position.X = game.Player.Center.X - (Width / 2);
            Position.Y = game.Player.Position.Y - Height;
            isBeingCarried = true;
            IsPassable = true;
        }

        private void doCarry()
        {
            Position.X = game.Player.Center.X - (Width / 2);
            Position.Y = game.Player.Position.Y - Height;
        }

        public void StartBeingThrown(Directions direction)
        {
            if (direction == Directions.Up)
            {
                Velocity.Y = -THROW_SPEED;
            }
            else if (direction == Directions.Down)
            {
                Velocity.Y = THROW_SPEED;
            }
            else if (direction == Directions.Left)
            {
                Velocity.X = -THROW_SPEED;
                Velocity.Y = FALL_SPEED;
            }
            else if (direction == Directions.Right)
            {
                Velocity.X = THROW_SPEED;
                Velocity.Y = FALL_SPEED;
            }

            isThrown = true;
            isBeingCarried = false;
            throwDirection = direction;
            throwTimer = 0;
        }

        private void doThrow()
        {
            throwTimer++;

            if (throwTimer >= THROW_TIME)
                endThrow();

            Vector2 collisionPos = new Vector2();
            if (throwDirection == Directions.Up)
                collisionPos = new Vector2(Position.X + (Width / 2), Position.Y);
            else if (throwDirection == Directions.Down)
                collisionPos = new Vector2(Position.X + (Width / 2), Position.Y + Height);
            else if (throwDirection == Directions.Left)
                collisionPos = new Vector2(Position.X, Position.Y + Height);
            else if (throwDirection == Directions.Right)
                collisionPos = new Vector2(Position.X + Width, Position.Y + Height);

            List<TileCollision> impassableTileCollisions = 
                new List<TileCollision> { TileCollision.Impassable, TileCollision.Doorway };
            if (impassableTileCollisions.Contains(area.GetCollisionAtCell(Area.ConvertPositionToCell(collisionPos))))
            {
                Rectangle cellRectangle = Area.CreateRectangleForCell(Area.ConvertPositionToCell(collisionPos));

                float intersectDistance = 0f;
                if (throwDirection == Directions.Up)
                    intersectDistance = cellRectangle.Bottom - collisionPos.Y;
                else if (throwDirection == Directions.Down)
                    intersectDistance = collisionPos.Y - cellRectangle.Top;
                else if (throwDirection == Directions.Left)
                    intersectDistance = cellRectangle.Right - collisionPos.X;
                else if (throwDirection == Directions.Right)
                    intersectDistance = collisionPos.X - cellRectangle.Left;

                if (intersectDistance >= 3)
                {
                    if (throwDirection == Directions.Up)
                        Position.Y = cellRectangle.Bottom - 3;
                    else if (throwDirection == Directions.Down)
                        Position.Y = cellRectangle.Top + 3 - Height;
                    else if (throwDirection == Directions.Left)
                        Position.X = cellRectangle.Right - 3;
                    else if (throwDirection == Directions.Right)
                        Position.X = cellRectangle.Left + 3 - Width;

                    endThrow();
                }
            }
        }

        protected void endThrow()
        {
            Velocity = Vector2.Zero;
            throwTimer = 0;

            land();
            isThrown = false;
        }

        protected abstract void land();
    }
}
