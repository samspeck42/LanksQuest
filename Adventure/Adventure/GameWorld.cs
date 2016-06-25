using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TileEngine;
using Adventure.Maps;
using Adventure.Entities;
using Adventure.PlayerStateHandlers;

namespace Adventure
{
    public class GameWorld
    {
        private const float TRANSITION_SCROLL_SPEED = 14.0f;
        private const int MAP_TRANSITION_TIME = 100;

        public Map CurrentMap { get { return currentMap; } }
        public Area CurrentArea { get { return currentMap.CurrentArea; } }
        public Player Player { get { return player; } }
        //public Effect InvertColorsEffect { get { return invertColorsEffect; } }
        public ContentManager Content { get { return content; } }
        public GraphicsDevice GraphicsDevice { get { return graphicsDevice; } }
        public SpriteFont Font;
        public Texture2D SquareTexture;
        public GameWorldState State { get { return state; } }
        public static Random Random = new Random();

        private Map currentMap;
        private Player player;
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private Effect changeColorsEffect;
        private Camera camera;
        private int transitionAreaIndex = -1;
        private Camera transitionCamera = null;
        private Vector2 transitionVelocity = Vector2.Zero;
        private Directions transitionDirection = Directions.Down; 
        private RenderTarget2D renderTarget;
        private float screenFade = 1.0f;
        private int mapTransitionTimer = 0;
        private bool enteredNewMap = false;
        private GameWorldState state = GameWorldState.Playing;
        private MapTransition currentMapTransition = null;


        public GameWorld(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            camera = new Camera();
            player = new Player(this);
            renderTarget = new RenderTarget2D(GraphicsDevice, AdventureGame.SCREEN_WIDTH, AdventureGame.SCREEN_HEIGHT);
        }

        public void LoadContent()
        {
            Player.LoadContent();
            changeColorsEffect = Content.Load<Effect>("Effects/ChangeColorsEffect");
            Font = Content.Load<SpriteFont>("Fonts/font");
            SquareTexture = Content.Load<Texture2D>("square");
        }

        public void Initialize()
        {
            player.Center = new Vector2(300, 300);
            currentMap = Map.FromFile("dungeon0", this);
            currentMap.SetCurrentAreaIndex(0);
            currentMap.Enter();
        }

        public void Update(GameTime gameTime)
        {
            if (enteredNewMap)
            {
                currentMap.Enter();
                enteredNewMap = false;
            }
            if (state == GameWorldState.Playing)
            {
                currentMap.Update();
                Point previousMapCell = CurrentMap.GetPlayerMapCell();
                Player.Update(gameTime);
                Point currentMapCell = CurrentMap.GetPlayerMapCell();

                //List<Pickup> pickupsToAdd = new List<Pickup>();
                List<Entity> entitiesToRemove = new List<Entity>();

                foreach (Entity entity in CurrentArea.GetActiveEntities())
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
                    CurrentArea.Entities = CurrentArea.Entities.Except(entitiesToRemove).ToList();
                //if (pickupsToAdd.Count > 0)
                //    currentArea.Entities.AddRange(pickupsToAdd);

                // check collisions between active hit boxes
                List<Entity> entitiesToCheck = new List<Entity>();
                entitiesToCheck.Add(Player);
                entitiesToCheck.AddRange(CurrentArea.GetActiveEntities());
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
                UpdateCamera();

                // check if player has left area
                if (CurrentArea != CurrentMap.GetAreaByIndex(CurrentMap.GetPlayerAreaIndex()) && CurrentMap.GetAreaByIndex(CurrentMap.GetPlayerAreaIndex()) != null)
                {
                    startAreaTransition(previousMapCell, currentMapCell);
                }

                // check if player is leaving map
                foreach (MapTransition mapTransition in CurrentArea.MapTransitions)
                {
                    if (Player.CollidesWith(Area.CreateRectangleForCell(mapTransition.LocationCell)) && 
                        Player.CanLeaveArea)
                    {
                        //StartMapTransition(mapTransition);
                        Player.StartLeavingMap(mapTransition);
                    }
                }
            }
            else if (state == GameWorldState.AreaTransition)
            {
                doAreaTransition();
            }
            else if (state == GameWorldState.MapTransition)
            {
                doMapTransition(gameTime);
            }
        }

        public void UpdateCamera()
        {
            camera.LockToTarget(Player.BoundingBox.ToRectangle(), AdventureGame.SCREEN_WIDTH, AdventureGame.SCREEN_HEIGHT);
            camera.ClampToArea(CurrentArea.WidthInPixels - AdventureGame.SCREEN_WIDTH, CurrentArea.HeightInPixels - AdventureGame.SCREEN_HEIGHT);
        }

        public void EnterState(GameWorldStateHandler newStateHandler)
        {
            //TODO
        }

        private void doMapTransition(GameTime gameTime)
        {
            Player.Update(gameTime);
            mapTransitionTimer++;
            screenFade = MathHelper.Clamp(Math.Abs((float)(mapTransitionTimer - (MAP_TRANSITION_TIME / 2)) / (MAP_TRANSITION_TIME / 2)),
                0f, 1f);

            if ((mapTransitionTimer >= MAP_TRANSITION_TIME / 2) && currentMapTransition != null)
            {
                currentMap.Exit();
                //if (currentMapTransition.IsDungeonTransition)
                //    currentMap = new Dungeon(this, currentMapTransition.DestinationDungeonNumber);
                //else
                //    currentMap = new Overworld(this);
                //currentMap.LoadContent();
                //currentArea = currentMap.GetAreaByIndex(currentMapTransition.DestinationAreaIndex);
                currentMap = Map.FromFile(currentMapTransition.DestinationMapName, this);
                Point destinationPlayerPosition = Area.CreateRectangleForCell(currentMapTransition.DestinationCell).Center;
                Player.Position = new Vector2(destinationPlayerPosition.X, destinationPlayerPosition.Y) +
                    (DirectionsHelper.GetDirectionVector(currentMapTransition.Direction) * 64);
                Player.EnterState(new NormalStateHandler(Player));
                UpdateCamera();
                currentMapTransition = null;
                enteredNewMap = true;
            }
            if (mapTransitionTimer >= MAP_TRANSITION_TIME)
            {
                finishMapTransition();
            }
        }

        private void finishMapTransition()
        {
            state = GameWorldState.Playing;
            mapTransitionTimer = 0;
            screenFade = 1f;

            //Player.Velocity = Vector2.Zero;
        }

        public void StartMapTransition(MapTransition mapTransition)
        {
            state = GameWorldState.MapTransition;
            mapTransitionTimer = 0;
            currentMapTransition = mapTransition;

            //if (Player.Velocity.Y < 0)
            //    Player.Velocity.Y = -Player.MAP_TRANSITION_WALK_SPEED;
            //else if (Player.Velocity.Y > 0)
            //    Player.Velocity.Y = Player.MAP_TRANSITION_WALK_SPEED;
        }

        private void doAreaTransition()
        {
            camera.Position += transitionVelocity;
            transitionCamera.Position += transitionVelocity;

            if (transitionVelocity.X < 0 && transitionCamera.Position.X <= (CurrentMap.GetAreaByIndex(transitionAreaIndex).WidthInPixels - AdventureGame.SCREEN_WIDTH))
            {
                transitionCamera.Position.X = CurrentMap.GetAreaByIndex(transitionAreaIndex).WidthInPixels - AdventureGame.SCREEN_WIDTH;
                Player.Position.X += CurrentMap.GetAreaByIndex(transitionAreaIndex).WidthInPixels;
                Player.Position.Y += (CurrentArea.MapCellCoordinates.Y - CurrentMap.GetAreaByIndex(transitionAreaIndex).MapCellCoordinates.Y) * Map.MAP_CELL_HEIGHT;
                finishAreaTransition();
            }
            else if (transitionVelocity.X > 0 && transitionCamera.Position.X >= 0)
            {
                transitionCamera.Position.X = 0;
                Player.Position.X -= CurrentArea.WidthInPixels;
                Player.Position.Y += (CurrentArea.MapCellCoordinates.Y - CurrentMap.GetAreaByIndex(transitionAreaIndex).MapCellCoordinates.Y) * Map.MAP_CELL_HEIGHT;
                finishAreaTransition();
            }
            else if (transitionVelocity.Y < 0 && transitionCamera.Position.Y <= (CurrentMap.GetAreaByIndex(transitionAreaIndex).HeightInPixels - AdventureGame.SCREEN_HEIGHT))
            {
                transitionCamera.Position.Y = CurrentMap.GetAreaByIndex(transitionAreaIndex).HeightInPixels - AdventureGame.SCREEN_HEIGHT;
                Player.Position.X += (CurrentArea.MapCellCoordinates.X - CurrentMap.GetAreaByIndex(transitionAreaIndex).MapCellCoordinates.X) * Map.MAP_CELL_WIDTH;
                Player.Position.Y += CurrentMap.GetAreaByIndex(transitionAreaIndex).HeightInPixels;
                finishAreaTransition();
            }
            else if (transitionVelocity.Y > 0 && transitionCamera.Position.Y >= 0)
            {
                transitionCamera.Position.Y = 0;
                Player.Position.X += (CurrentArea.MapCellCoordinates.X - CurrentMap.GetAreaByIndex(transitionAreaIndex).MapCellCoordinates.X) * Map.MAP_CELL_WIDTH;
                Player.Position.Y -= CurrentArea.HeightInPixels;
                finishAreaTransition();
            }
        }

        private void startAreaTransition(Point previousMapCell, Point currentMapCell)
        {
            state = GameWorldState.AreaTransition;
            transitionAreaIndex = CurrentMap.GetPlayerAreaIndex();
            transitionCamera = new Camera();
            if (currentMapCell.X < previousMapCell.X)
            {
                // going west
                transitionCamera.Position = new Vector2(CurrentMap.GetAreaByIndex(transitionAreaIndex).WidthInPixels,
                    camera.Position.Y + ((CurrentArea.MapCellCoordinates.Y - CurrentMap.GetAreaByIndex(transitionAreaIndex).MapCellCoordinates.Y) * Map.MAP_CELL_HEIGHT));
                transitionVelocity = new Vector2(-TRANSITION_SCROLL_SPEED, 0);
                transitionDirection = Directions.Left;
            }
            else if (currentMapCell.X > previousMapCell.X)
            {
                // going east
                transitionCamera.Position = new Vector2(-AdventureGame.SCREEN_WIDTH,
                    camera.Position.Y + ((CurrentArea.MapCellCoordinates.Y - CurrentMap.GetAreaByIndex(transitionAreaIndex).MapCellCoordinates.Y) * Map.MAP_CELL_HEIGHT));
                transitionVelocity = new Vector2(TRANSITION_SCROLL_SPEED, 0);
                transitionDirection = Directions.Right;
            }
            else if (currentMapCell.Y < previousMapCell.Y)
            {
                // going north
                transitionCamera.Position = new Vector2(camera.Position.X +
                    ((CurrentArea.MapCellCoordinates.X - CurrentMap.GetAreaByIndex(transitionAreaIndex).MapCellCoordinates.X) * Map.MAP_CELL_WIDTH), CurrentMap.GetAreaByIndex(transitionAreaIndex).HeightInPixels);
                transitionVelocity = new Vector2(0, -TRANSITION_SCROLL_SPEED);
                transitionDirection = Directions.Up;
            }
            else if (currentMapCell.Y > previousMapCell.Y)
            {
                // going south
                transitionCamera.Position = new Vector2(camera.Position.X +
                    ((CurrentArea.MapCellCoordinates.X - CurrentMap.GetAreaByIndex(transitionAreaIndex).MapCellCoordinates.X) * Map.MAP_CELL_WIDTH), -AdventureGame.SCREEN_HEIGHT);
                transitionVelocity = new Vector2(0, TRANSITION_SCROLL_SPEED);
                transitionDirection = Directions.Down;
            }
        }

        private void finishAreaTransition()
        {
            state = GameWorldState.Playing;
            CurrentMap.SetCurrentAreaIndex(transitionAreaIndex);
            camera = transitionCamera;
            transitionAreaIndex = -1;
            transitionCamera = null;
            transitionVelocity = Vector2.Zero;
            Player.StartEnteringArea(transitionDirection);
        }

        public void Draw(SpriteBatch spriteBatch, RenderTarget2D screenRenderTarget)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Black);

            if (state == GameWorldState.AreaTransition)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, changeColorsEffect, transitionCamera.TransformMatrix);
                CurrentMap.GetAreaByIndex(transitionAreaIndex).DrawBackground(spriteBatch);
                CurrentMap.GetAreaByIndex(transitionAreaIndex).DrawEntities(spriteBatch, changeColorsEffect);
                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, changeColorsEffect, camera.TransformMatrix);
            CurrentArea.DrawBackground(spriteBatch);
            CurrentArea.DrawEntities(spriteBatch, changeColorsEffect);
            Player.Draw(spriteBatch, changeColorsEffect);
            CurrentArea.DrawForeground(spriteBatch);
            spriteBatch.End();

            if (state == GameWorldState.AreaTransition)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, changeColorsEffect, transitionCamera.TransformMatrix);
                CurrentMap.GetAreaByIndex(transitionAreaIndex).DrawForeground(spriteBatch);
                spriteBatch.End();
            }


            GraphicsDevice.SetRenderTarget(screenRenderTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw((Texture2D)renderTarget, Vector2.Zero, new Color(new Vector4(screenFade, screenFade, screenFade, 1.0f)));
            spriteBatch.End();
        }
    }
}
