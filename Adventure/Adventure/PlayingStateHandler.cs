using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class PlayingStateHandler : GameWorldStateHandler
    {
        public override GameWorldState State
        {
            get { return GameWorldState.Playing; }
        }

        public PlayingStateHandler(GameWorld gameWorld)
            : base(gameWorld) { }

        public override void Update(GameTime gameTime)
        {
            gameWorld.CurrentMap.Update();
            Point previousMapCell = gameWorld.CurrentMap.GetCurrentCell();
            gameWorld.Player.Update(gameTime);
            Point currentMapCell = gameWorld.CurrentMap.GetCurrentCell();

            //List<Pickup> pickupsToAdd = new List<Pickup>();
            List<Entity> entitiesToRemove = new List<Entity>();

            foreach (Entity entity in gameWorld.CurrentArea.GetActiveEntities())
            {
                if (entity is Player)
                    continue;

                entity.Update(gameTime);

                if (!entity.IsAlive)
                {
                    entitiesToRemove.Add(entity);
                    continue;
                }
            }

            if (entitiesToRemove.Count > 0)
                gameWorld.CurrentArea.Entities = gameWorld.CurrentArea.Entities.Except(entitiesToRemove).ToList();

            // check collisions between active hit boxes
            List<Entity> entitiesToCheck = new List<Entity>();
            entitiesToCheck.Add(gameWorld.Player);
            entitiesToCheck.AddRange(gameWorld.CurrentArea.GetActiveEntities());
            while (entitiesToCheck.Count > 1)
            {
                Entity entity = entitiesToCheck[0];
                entitiesToCheck.RemoveAt(0);
                foreach (Entity other in entitiesToCheck)
                {
                    foreach (HitBox hitBox in entity.GetActiveHitBoxes())
                    {
                        foreach (HitBox otherHitBox in other.GetActiveHitBoxes())
                        {
                            if (hitBox.CollidesWith(otherHitBox))
                            {
                                entity.OnEntityCollision(other, hitBox, otherHitBox);
                                other.OnEntityCollision(entity, otherHitBox, hitBox);
                            }
                        }
                    }
                }
            }

            // update camera
            gameWorld.UpdateCamera();

            // check if player has left area
            if (gameWorld.CurrentArea != gameWorld.CurrentMap.GetPlayerArea() && gameWorld.CurrentMap.GetPlayerArea() != null)
            {
                //startAreaTransition(previousMapCell, currentMapCell);
            }

            // check if player is leaving map
            foreach (MapTransition mapTransition in gameWorld.CurrentArea.MapTransitions)
            {
                if (gameWorld.Player.BoundingBox.CollidesWith(Area.CreateRectangleForCell(mapTransition.LocationCell)) &&
                    gameWorld.Player.CanLeaveArea)
                {
                    gameWorld.Player.StartLeavingMap(mapTransition);
                }
            }
        }
    }
}
