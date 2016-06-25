using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TileEngine;
using Adventure.Maps;

namespace Adventure.Entities.MovementHandlers
{
    public abstract class MovementHandler
    {
        /// <summary>
        /// Gets the vector that the entity will attempt to be moved by this update.
        /// </summary>
        public Vector2 Movement { get { return movement; } }

        public bool IsFinished { get { return isFinished; } }

        protected Entity entity;
        protected Vector2 movement = Vector2.Zero;
        protected bool isStarted = false; 
        protected bool isFinished = false;

        public MovementHandler(Entity entity)
        {
            this.entity = entity;
        }

        public virtual void Start() 
        {
            isStarted = true;
        }

        public void Update(GameTime gameTime)
        {
            if (isStarted && !isFinished)
            {
                UpdateMovement(gameTime);
                DoMovement();

                if (isFinished)
                    movement = Vector2.Zero;
            }
        }

        public abstract void UpdateMovement(GameTime gameTime);

        /// <summary>
        /// Attempts to move entity by movement vector, blocking movement if obstacles are encountered.
        /// </summary>
        public void DoMovement()
        {
            int topCellY = (int)Math.Round(entity.ObstacleCollisionBox.Top) / Area.CELL_HEIGHT;
            int bottomCellY = (int)Math.Round(entity.ObstacleCollisionBox.Bottom - 1) / Area.CELL_HEIGHT;
            int leftCellX = (int)Math.Round(entity.ObstacleCollisionBox.Left) / Area.CELL_WIDTH;
            int rightCellX = (int)Math.Round(entity.ObstacleCollisionBox.Right - 1) / Area.CELL_WIDTH;
            int x = 0, y = 0;
            float collisionDist = 0;
            Point curCell = new Point();
            bool collidedX = false;
            List<Entity> entitiesCollidedWithX = new List<Entity>();

            if (movement.X > 0 || movement.X < 0) //moving horizontally
            {
                // check obstacle tile collisions
                for (int c = 0; c < 2; c++)
                {
                    if (movement.X > 0) //moving right
                    {
                        x = rightCellX + 1 + c;
                        collisionDist = (x * Area.CELL_WIDTH) - entity.ObstacleCollisionBox.Right;
                    }
                    else //moving left
                    {
                        x = leftCellX - 1 - c;
                        collisionDist = ((x + 1) * Area.CELL_WIDTH) - entity.ObstacleCollisionBox.Left;
                    }
                    for (y = topCellY; y <= bottomCellY; y++)
                    {
                        curCell = new Point(x, y);
                        if (entity.GameWorld.CurrentArea.GetCollisionAtCell(curCell) != TileCollision.None && 
                            entity.ObstacleTileCollisions.HasFlag(entity.GameWorld.CurrentArea.GetCollisionAtCell(curCell)) &&
                            Math.Abs(collisionDist) < Math.Abs(movement.X))
                        {
                            //a collision with an obstacle tile will occur 
                            collidedX = true;
                            break;
                        }
                    }

                    if (collidedX)
                        break;
                }

                if (entity.IsBlockedByObstacleEntities)
                {
                    // check obstacle entity collisions
                    List<Entity> entitiesInPath = entity.GameWorld.CurrentArea.GetObstacleEntitiesInCollisionPathX(entity, movement.X);
                    foreach (Entity other in entitiesInPath)
                    {
                        float dist = movement.X > 0 ? other.BoundingBox.Left - entity.ObstacleCollisionBox.Right :
                            other.BoundingBox.Right - entity.ObstacleCollisionBox.Left;
                        if (Math.Abs(dist) < Math.Abs(movement.X))
                        {
                            if (collidedX && (dist <= collisionDist))
                            {
                                collisionDist = dist;

                                if (dist < collisionDist)
                                    entitiesCollidedWithX.Clear();

                                entitiesCollidedWithX.Add(other);
                            }
                            else if (!collidedX)
                            {
                                collisionDist = dist;
                                collidedX = true;

                                entitiesCollidedWithX.Add(other);
                            }
                        }
                    }
                }

                if (collidedX)
                {
                    entity.Position.X += collisionDist;
                    movement.X = 0f;
                }
            }

            entity.Position.X += movement.X;


            topCellY = (int)Math.Round(entity.ObstacleCollisionBox.Top) / Area.CELL_HEIGHT;
            bottomCellY = (int)Math.Round(entity.ObstacleCollisionBox.Bottom - 1) / Area.CELL_HEIGHT;
            leftCellX = (int)Math.Round(entity.ObstacleCollisionBox.Left) / Area.CELL_WIDTH;
            rightCellX = (int)Math.Round(entity.ObstacleCollisionBox.Right - 1) / Area.CELL_WIDTH;
            collisionDist = 0;
            bool collidedY = false;
            List<Entity> entitiesCollidedWithY = new List<Entity>();

            if (movement.Y > 0 || movement.Y < 0) //moving vertically
            {
                for (int r = 0; r < 2; r++)
                {
                    if (movement.Y > 0) //moving down
                    {
                        y = bottomCellY + 1 + r;
                        collisionDist = (y * Area.CELL_HEIGHT) - entity.ObstacleCollisionBox.Bottom;
                    }
                    else //moving up
                    {
                        y = topCellY - 1 - r;
                        collisionDist = ((y + 1) * Area.CELL_HEIGHT) - entity.ObstacleCollisionBox.Top;
                    }
                    for (x = leftCellX; x <= rightCellX; x++)
                    {
                        curCell = new Point(x, y);
                        if (entity.GameWorld.CurrentArea.GetCollisionAtCell(curCell) != TileCollision.None && 
                            entity.ObstacleTileCollisions.HasFlag(entity.GameWorld.CurrentArea.GetCollisionAtCell(curCell)) &&
                            Math.Abs(collisionDist) < Math.Abs(movement.Y))
                        {
                            //a collision with an impassable tile will occur
                            collidedY = true;
                            break;
                        }
                    }

                    if (collidedY)
                        break;
                }

                if (entity.IsBlockedByObstacleEntities)
                {
                    // check impassable entity collisions
                    List<Entity> entitiesInPath = entity.GameWorld.CurrentArea.GetObstacleEntitiesInCollisionPathY(entity, movement.Y);
                    foreach (Entity other in entitiesInPath)
                    {
                        float dist = movement.Y > 0 ? other.BoundingBox.Top - entity.ObstacleCollisionBox.Bottom :
                            other.BoundingBox.Bottom - entity.ObstacleCollisionBox.Top;
                        if (Math.Abs(dist) < Math.Abs(movement.Y))
                        {
                            if (collidedY && (dist <= collisionDist))
                            {
                                collisionDist = dist;

                                if (dist < collisionDist)
                                    entitiesCollidedWithY.Clear();
                                entitiesCollidedWithY.Add(other);
                            }
                            else if (!collidedY)
                            {
                                collisionDist = dist;
                                collidedY = true;

                                entitiesCollidedWithY.Add(other);
                            }
                        }
                    }
                }

                if (collidedY)
                {
                    entity.Position.Y += collisionDist;
                    movement.Y = 0f;
                }
            }

            entity.Position.Y += movement.Y;


            if (collidedX || collidedY)
            {
                entity.OnMovementEvent(MovementEvent.CollisionWithObstacle);
            }

            List<Entity> entitiesCollidedWith = new List<Entity>();
            entitiesCollidedWith.AddRange(entitiesCollidedWithX);
            entitiesCollidedWith.AddRange(entitiesCollidedWithY);
            foreach (Entity other in entitiesCollidedWith)
            {
                entity.OnEntityCollision(other, entity.ObstacleCollisionBox, other.BoundingBox);
                other.OnEntityCollision(entity, other.BoundingBox, entity.ObstacleCollisionBox);
            }
        }

        public virtual void End()
        {
            isFinished = true;
        }
    }

    public enum MovementEvent
    {
        CollisionWithObstacle,
        CollisionWithGround
    }
}
