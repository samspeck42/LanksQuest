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

namespace Adventure
{
    public class GameWorld
    {
        private const float TRANSITION_SCROLL_SPEED = 14.0f;
        private const int MAP_TRANSITION_TIME = 100;

        private Area currentArea;
        public Area CurrentArea { get { return currentArea; } }
        private Map currentMap;
        public Map CurrentMap { get { return currentMap; } }
        private Player player;
        public Player Player { get { return player; } }
        private Effect changeColorsEffect;
        //public Effect InvertColorsEffect { get { return invertColorsEffect; } }
        private ContentManager content;
        public ContentManager Content { get { return content; } }
        private GraphicsDevice graphicsDevice;
        public GraphicsDevice GraphicsDevice { get { return graphicsDevice; } }
        public SpriteFont Font;
        public Texture2D SquareTexture;
        public GameWorldState State { get { return state; } }

        private Camera camera;

        private Area transitionArea = null;
        private Camera transitionCamera = null;
        private Vector2 transitionVelocity = Vector2.Zero;
        private Directions transitionDirection = Directions.Down;

        private RenderTarget2D renderTarget;
        private float screenFade = 1.0f;
        private int mapTransitionTimer = 0;
        private bool enteredNewMap;
        private GameWorldState state = GameWorldState.Playing;

        private MapTransition currentMapTransition = null;

        public static Random Random = new Random();

        public void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            camera = new Camera();
            currentMap = new Dungeon(this, 0);
            //CurrentMap = new Overworld(this);
            player = new Player(this);
            player.Center = new Vector2(300, 300);
            enteredNewMap = true;
        }

        public void LoadContent()
        {
            renderTarget = new RenderTarget2D(GraphicsDevice, Adventure.SCREEN_WIDTH, Adventure.SCREEN_HEIGHT);

            Player.LoadContent();

            currentMap.Load();
            currentArea = currentMap.GetAreaByIndex(0);
            changeColorsEffect = Content.Load<Effect>("Effects/ChangeColorsEffect");
            Font = Content.Load<SpriteFont>("Fonts/font");
            SquareTexture = Content.Load<Texture2D>("square");
        }

        public void Update(GameTime gameTime)
        {
            if (enteredNewMap)
            {
                CurrentMap.Enter();
                enteredNewMap = false;
            }
            if (state == GameWorldState.Playing)
            {
                CurrentMap.Update();
                Point previousMapCell = CurrentMap.GetCurrentCell();
                Player.Update(gameTime);
                Point currentMapCell = CurrentMap.GetCurrentCell();

                //List<Pickup> pickupsToAdd = new List<Pickup>();
                List<Entity> entitiesToRemove = new List<Entity>();

                foreach (Entity entity in currentArea.GetActiveEntities())
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
                    currentArea.Entities = currentArea.Entities.Except(entitiesToRemove).ToList();
                //if (pickupsToAdd.Count > 0)
                //    currentArea.Entities.AddRange(pickupsToAdd);

                // check collisions between active hit boxes
                List<Entity> entitiesToCheck = new List<Entity>();
                entitiesToCheck.Add(Player);
                entitiesToCheck.AddRange(currentArea.GetActiveEntities());
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
                if (currentArea != CurrentMap.GetPlayerArea() && CurrentMap.GetPlayerArea() != null)
                {
                    startAreaTransition(previousMapCell, currentMapCell);
                }

                // check if player is leaving map
                foreach (MapTransition mapTransition in currentArea.MapTransitions)
                {
                    if (Player.BoundingBox.CollidesWith(Area.CreateRectangleForCell(mapTransition.LocationCell)) && 
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
            camera.LockToTarget(Player.BoundingBox.ToRectangle(), Adventure.SCREEN_WIDTH, Adventure.SCREEN_HEIGHT);
            camera.ClampToArea(currentArea.WidthInPixels - Adventure.SCREEN_WIDTH, currentArea.HeightInPixels - Adventure.SCREEN_HEIGHT);
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
                if (currentMapTransition.IsDungeonTransition)
                    currentMap = new Dungeon(this, currentMapTransition.DestinationDungeonNumber);
                else
                    currentMap = new Overworld(this);
                currentMap.Load();
                currentArea = currentMap.GetAreaByIndex(currentMapTransition.DestinationAreaIndex);
                Point destinationCenterPoint = Area.CreateRectangleForCell(currentMapTransition.DestinationCell).Center;
                Player.Center = new Vector2(destinationCenterPoint.X, destinationCenterPoint.Y) +
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

            if (transitionVelocity.X < 0 && transitionCamera.Position.X <= (transitionArea.WidthInPixels - Adventure.SCREEN_WIDTH))
            {
                transitionCamera.Position.X = transitionArea.WidthInPixels - Adventure.SCREEN_WIDTH;
                Player.Position.X += transitionArea.WidthInPixels;
                Player.Position.Y += (currentArea.CellCoordinates.Y - transitionArea.CellCoordinates.Y) * Map.MAP_CELL_HEIGHT;
                finishAreaTransition();
            }
            else if (transitionVelocity.X > 0 && transitionCamera.Position.X >= 0)
            {
                transitionCamera.Position.X = 0;
                Player.Position.X -= currentArea.WidthInPixels;
                Player.Position.Y += (currentArea.CellCoordinates.Y - transitionArea.CellCoordinates.Y) * Map.MAP_CELL_HEIGHT;
                finishAreaTransition();
            }
            else if (transitionVelocity.Y < 0 && transitionCamera.Position.Y <= (transitionArea.HeightInPixels - Adventure.SCREEN_HEIGHT))
            {
                transitionCamera.Position.Y = transitionArea.HeightInPixels - Adventure.SCREEN_HEIGHT;
                Player.Position.X += (currentArea.CellCoordinates.X - transitionArea.CellCoordinates.X) * Map.MAP_CELL_WIDTH;
                Player.Position.Y += transitionArea.HeightInPixels;
                finishAreaTransition();
            }
            else if (transitionVelocity.Y > 0 && transitionCamera.Position.Y >= 0)
            {
                transitionCamera.Position.Y = 0;
                Player.Position.X += (currentArea.CellCoordinates.X - transitionArea.CellCoordinates.X) * Map.MAP_CELL_WIDTH;
                Player.Position.Y -= currentArea.HeightInPixels;
                finishAreaTransition();
            }
        }

        private void startAreaTransition(Point previousMapCell, Point currentMapCell)
        {
            state = GameWorldState.AreaTransition;
            transitionArea = CurrentMap.GetPlayerArea();
            transitionCamera = new Camera();
            if (currentMapCell.X < previousMapCell.X)
            {
                // going west
                transitionCamera.Position = new Vector2(transitionArea.WidthInPixels,
                    camera.Position.Y + ((currentArea.CellCoordinates.Y - transitionArea.CellCoordinates.Y) * Map.MAP_CELL_HEIGHT));
                transitionVelocity = new Vector2(-TRANSITION_SCROLL_SPEED, 0);
                transitionDirection = Directions.Left;
            }
            else if (currentMapCell.X > previousMapCell.X)
            {
                // going east
                transitionCamera.Position = new Vector2(-Adventure.SCREEN_WIDTH,
                    camera.Position.Y + ((currentArea.CellCoordinates.Y - transitionArea.CellCoordinates.Y) * Map.MAP_CELL_HEIGHT));
                transitionVelocity = new Vector2(TRANSITION_SCROLL_SPEED, 0);
                transitionDirection = Directions.Right;
            }
            else if (currentMapCell.Y < previousMapCell.Y)
            {
                // going north
                transitionCamera.Position = new Vector2(camera.Position.X +
                    ((currentArea.CellCoordinates.X - transitionArea.CellCoordinates.X) * Map.MAP_CELL_WIDTH), transitionArea.HeightInPixels);
                transitionVelocity = new Vector2(0, -TRANSITION_SCROLL_SPEED);
                transitionDirection = Directions.Up;
            }
            else if (currentMapCell.Y > previousMapCell.Y)
            {
                // going south
                transitionCamera.Position = new Vector2(camera.Position.X +
                    ((currentArea.CellCoordinates.X - transitionArea.CellCoordinates.X) * Map.MAP_CELL_WIDTH), -Adventure.SCREEN_HEIGHT);
                transitionVelocity = new Vector2(0, TRANSITION_SCROLL_SPEED);
                transitionDirection = Directions.Down;
            }
        }

        private void finishAreaTransition()
        {
            state = GameWorldState.Playing;
            currentArea = transitionArea;
            camera = transitionCamera;
            transitionArea = null;
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
                transitionArea.DrawBackground(spriteBatch);
                transitionArea.DrawEntities(spriteBatch, changeColorsEffect);
                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, changeColorsEffect, camera.TransformMatrix);
            currentArea.DrawBackground(spriteBatch);
            currentArea.DrawEntities(spriteBatch, changeColorsEffect);
            Player.Draw(spriteBatch, changeColorsEffect);
            currentArea.DrawForeground(spriteBatch);
            spriteBatch.End();

            if (state == GameWorldState.AreaTransition)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, changeColorsEffect, transitionCamera.TransformMatrix);
                transitionArea.DrawForeground(spriteBatch);
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
