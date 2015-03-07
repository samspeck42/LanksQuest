using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TileEngine;
using System.IO;
using System.Reflection;

namespace Adventure
{
    public class Area
    {
        public const int TILE_WIDTH = 32;
        public const int TILE_HEIGHT = 32;

        public static string[] TileTextureImageExtensions = new string[] 
        {
            ".png", ".jpg",
        };

        public int WidthInCells { get { return backgroundTileLayout.GetLength(1); } }
        public int HeightInCells { get { return backgroundTileLayout.GetLength(0); } }

        public int WidthInPixels { get { return WidthInCells * TILE_WIDTH; } }
        public int HeightInPixels { get { return HeightInCells * TILE_HEIGHT; } }

        public Point CellCoordinates = new Point();

        public List<Entity> Entities = new List<Entity>();
        public List<MapTransition> MapTransitions = new List<MapTransition>();

        public bool IsLoaded { get { return isLoaded; } }
        public bool IsBossArea { get { return isBossArea; } }
        public bool IsRewardArea { get { return isRewardArea; } }

        public Map Map { get { return map; } }


        private int[,] backgroundTileLayout;
        private int[,] foregroundTileLayout;
        private int[,] collisionLayout;

        private List<Texture2D> tileTextures = new List<Texture2D>();
        private List<Texture2D> shadowTextures = new List<Texture2D>();

        private bool isLoaded = false;
        private bool isBossArea = false;
        private bool isRewardArea = false;

        private GameWorld game;
        private Map map;

        public Area(GameWorld game, Map map, int width, int height)
        {
            this.game = game;
            this.map = map;

            backgroundTileLayout = new int[height, width];
            foregroundTileLayout = new int[height, width];
            collisionLayout = new int[height, width];

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    backgroundTileLayout[r, c] = -1;
                    foregroundTileLayout[r, c] = -1;
                    collisionLayout[r, c] = 0;
                }
            }
        }

        public void LoadContent(ContentManager content, List<string> textureNames)
        {
            foreach (string textureName in textureNames)
            {
                Texture2D tileTexture = content.Load<Texture2D>("Tiles/" + textureName);
                this.tileTextures.Add(tileTexture);
            }

            this.shadowTextures.Add(content.Load<Texture2D>("Sprites/Environment/shadow_small"));
            this.shadowTextures.Add(content.Load<Texture2D>("Sprites/Environment/shadow_medium"));
        }

        public static Area FromFile(GameWorld game, Map map, string areaName)
        {
            List<string> textureNames = new List<string>();
            Area area = processFile(game, map, "Content/Areas/" + areaName + ".area", textureNames);

            area.LoadContent(game.Content, textureNames);

            area.isLoaded = true;
            return area;
        }

        public static Area FromFile(GraphicsDevice graphicsDevice, string contentPath, string fileName, Map map, out string[] tileTextureNames)
        {
            List<string> textureNames = new List<string>();
            Area area = processFile(null, map, fileName, textureNames);

            tileTextureNames = textureNames.ToArray();

            area.isLoaded = true;
            return area;
        }

        private static Area processFile(GameWorld game, Map map, string fileName, List<string> textureNames)
        {
            Area area = null;

            List<List<int>> tempBackgroundTileLayout = new List<List<int>>();
            List<List<int>> tempForegroundTileLayout = new List<List<int>>();
            List<List<int>> tempCollisionLayout = new List<List<int>>();
            List<string> entityData = new List<string>();
            List<MapTransition> tempMapTransitions = new List<MapTransition>();
            Dictionary<string, string> propertiesDict = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                bool readingTileTextures = false;
                bool readingBackgroundTileLayout = false;
                bool readingForegroundTileLayout = false;
                bool readingCollisionLayout = false;
                bool readingEntities = false;
                bool readingMapTransitions = false;
                bool readingProperties = false;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                        continue;
                    else if (line.Contains("[BackgroundTileTextures]"))
                    {
                        readingTileTextures = true;
                        readingBackgroundTileLayout = false;
                        readingForegroundTileLayout = false;
                        readingCollisionLayout = false;
                        readingEntities = false;
                        readingMapTransitions = false;
                        readingProperties = false;
                    }
                    else if (line.Contains("[BackgroundTileLayout]"))
                    {
                        readingTileTextures = false;
                        readingBackgroundTileLayout = true;
                        readingForegroundTileLayout = false;
                        readingCollisionLayout = false;
                        readingEntities = false;
                        readingMapTransitions = false;
                        readingProperties = false;
                    }
                    else if (line.Contains("[ForegroundTileLayout]"))
                    {
                        readingTileTextures = false;
                        readingBackgroundTileLayout = false;
                        readingForegroundTileLayout = true;
                        readingCollisionLayout = false;
                        readingEntities = false;
                        readingMapTransitions = false;
                        readingProperties = false;
                    }
                    else if (line.Contains("[CollisionLayout]"))
                    {
                        readingTileTextures = false;
                        readingBackgroundTileLayout = false;
                        readingForegroundTileLayout = false;
                        readingCollisionLayout = true;
                        readingEntities = false;
                        readingMapTransitions = false;
                        readingProperties = false;
                    }
                    else if (line.Contains("[Entities]"))
                    {
                        readingTileTextures = false;
                        readingBackgroundTileLayout = false;
                        readingForegroundTileLayout = false;
                        readingCollisionLayout = false;
                        readingEntities = true;
                        readingMapTransitions = false;
                        readingProperties = false;
                    }
                    else if (line.Equals("[MapTransitions]"))
                    {
                        readingTileTextures = false;
                        readingBackgroundTileLayout = false;
                        readingForegroundTileLayout = false;
                        readingCollisionLayout = false;
                        readingEntities = false;
                        readingMapTransitions = true;
                        readingProperties = false;
                    }
                    else if (line.Equals("[Properties]"))
                    {
                        readingTileTextures = false;
                        readingBackgroundTileLayout = false;
                        readingForegroundTileLayout = false;
                        readingCollisionLayout = false;
                        readingEntities = false;
                        readingMapTransitions = false;
                        readingProperties = true;
                    }
                    else if (readingTileTextures)
                    {
                        textureNames.Add(line);
                    }
                    else if (readingBackgroundTileLayout || readingForegroundTileLayout || readingCollisionLayout)
                    {
                        List<int> row = new List<int>();
                        string[] cells = line.Split(' ');
                        foreach (string c in cells)
                        {
                            if (!string.IsNullOrEmpty(c))
                                row.Add(int.Parse(c));
                        }

                        if (readingBackgroundTileLayout)
                            tempBackgroundTileLayout.Add(row);
                        if (readingForegroundTileLayout)
                            tempForegroundTileLayout.Add(row);
                        else if (readingCollisionLayout)
                            tempCollisionLayout.Add(row);
                    }
                    else if (readingEntities)
                    {
                        entityData.Add(line);
                    }
                    else if (readingMapTransitions)
                    {
                        MapTransition mapTransition = MapTransition.CreateFromString(line);
                        tempMapTransitions.Add(mapTransition);
                    }
                    else if (readingProperties)
                    {
                        string[] property = line.Split(':');
                        propertiesDict.Add(property[0], property[1]);
                    }
                }
            }

            int width = tempBackgroundTileLayout[0].Count;
            int height = tempBackgroundTileLayout.Count;

            area = new Area(game, map, width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    area.SetBackgroundTileIndexAtCell(new Point(x, y), tempBackgroundTileLayout[y][x]);
                    area.SetForegroundTileIndexAtCell(new Point(x, y), tempForegroundTileLayout[y][x]);
                    area.SetCollisionAtCell(new Point(x, y), tempCollisionLayout[y][x]);
                }
            }
            foreach (string line in entityData)
            {
                area.Entities.Add(Entity.CreateFromString(line, game, area));
            }
            area.MapTransitions = tempMapTransitions;
            if (propertiesDict.ContainsKey("isBossArea"))
                area.isBossArea = bool.Parse(propertiesDict["isBossArea"]);
            if (propertiesDict.ContainsKey("isRewardArea"))
                area.isRewardArea = bool.Parse(propertiesDict["isRewardArea"]);

            return area;
        }

        public void Save(string fileName, string[] tileTextureNames)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("[BackgroundTileTextures]");
                foreach (string tileTextureName in tileTextureNames)
                {
                    writer.WriteLine(tileTextureName);
                }
                writer.WriteLine();

                writer.WriteLine("[BackgroundTileLayout]");
                for (int y = 0; y < HeightInCells; y++)
                {
                    string line = "";

                    for (int x = 0; x < WidthInCells; x++)
                    {
                        line += backgroundTileLayout[y, x].ToString() + " ";
                    }

                    writer.WriteLine(line);
                }
                writer.WriteLine();

                writer.WriteLine("[ForegroundTileLayout]");
                for (int y = 0; y < HeightInCells; y++)
                {
                    string line = "";

                    for (int x = 0; x < WidthInCells; x++)
                    {
                        line += foregroundTileLayout[y, x].ToString() + " ";
                    }

                    writer.WriteLine(line);
                }
                writer.WriteLine();

                writer.WriteLine("[CollisionLayout]");
                for (int y = 0; y < HeightInCells; y++)
                {
                    string line = "";

                    for (int x = 0; x < WidthInCells; x++)
                    {
                        line += collisionLayout[y, x].ToString() + " ";
                    }

                    writer.WriteLine(line);
                }
                writer.WriteLine();

                if (Entities.Count > 0)
                {
                    writer.WriteLine("[Entities]");
                    Dictionary<string, List<Entity>> entityDict = new Dictionary<string, List<Entity>>();
                    foreach (Entity entity in Entities)
                    {
                        string entityTypeName = entity.GetType().ToString().Replace("Adventure.", "");
                        if (!entityDict.Keys.Contains(entityTypeName))
                            entityDict.Add(entityTypeName, new List<Entity>());
                        entityDict[entityTypeName].Add(entity);
                    }
                    foreach (string entityTypeName in entityDict.Keys)
                    {
                        string line = entityTypeName;

                        foreach (Entity entity in entityDict[entityTypeName])
                            line += " " + entity.ToString();

                        writer.WriteLine(line);
                    }
                    writer.WriteLine();
                }

                if (MapTransitions.Count > 0)
                {
                    writer.WriteLine("[MapTransitions]");
                    foreach (MapTransition mapTransition in MapTransitions)
                    {
                        string line = mapTransition.ToString();
                        writer.WriteLine(line);
                    }
                }
                
            }
        }

        public static Point ConvertPositionToCell(Vector2 position)
        {
            return new Point(
                (int)(Math.Round(position.X) / (float)TILE_WIDTH),
                (int)(Math.Round(position.Y) / (float)TILE_HEIGHT));
        }

        public static Rectangle CreateRectangleForCell(Point cell)
        {
            return new Rectangle(cell.X * TILE_WIDTH, cell.Y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT);
        }

        public TileCollision GetCollisionAtCell(int x, int y)
        {
            return (TileCollision)collisionLayout[y, x];
        }

        public TileCollision GetCollisionAtCell(Point cell)
        {
            if (CellInBounds(cell))
                return (TileCollision)collisionLayout[cell.Y, cell.X];
            return TileCollision.None;
        }

        public void SetCollisionAtCell(Point cell, TileCollision collision)
        {
            if (CellInBounds(cell))
                collisionLayout[cell.Y, cell.X] = (int)collision;
        }

        public void SetCollisionAtCell(Point cell, int collision)
        {
            if (CellInBounds(cell))
                collisionLayout[cell.Y, cell.X] = collision;
        }

        public int GetBackgroundTileIndexAtCell(Point cell)
        {
            if (CellInBounds(cell))
                return backgroundTileLayout[cell.Y, cell.X];
            return -1;
        }

        public void SetBackgroundTileIndexAtCell(Point cell, int index)
        {
            if (CellInBounds(cell))
                backgroundTileLayout[cell.Y, cell.X] = index;
        }

        public int GetForegroundTileIndexAtCell(Point cell)
        {
            if (CellInBounds(cell))
                return foregroundTileLayout[cell.Y, cell.X];
            return -1;
        }

        public void SetForegroundTileIndexAtCell(Point cell, int index)
        {
            if (CellInBounds(cell))
                foregroundTileLayout[cell.Y, cell.X] = index;
        }

        //public Entity GetEntityAtSpawnCell(Point cell)
        //{
        //    Rectangle cellRectangle = Area.CreateRectangleForCell(cell);

        //    foreach (Entity entity in Entities)
        //    {
        //        //if (cellRectangle.Contains(new Point((int)Math.Round(entity.Center.X), (int)Math.Round(entity.Center.Y))))
        //        if (entity.SpawnCell.X == cell.X && entity.SpawnCell.Y == cell.Y)
        //            return entity;
        //    }
        //    return null;
        //}

        public Entity GetEntityById(string id)
        {
            foreach (Entity entity in Entities)
            {
                if (entity.Id.Equals(id))
                    return entity;
            }
            return null;
        }

        public List<Entity> GetActiveEntitiesAtCell(Point cell)
        {
            List<Entity> entityList = new List<Entity>();
            Rectangle cellRectangle = Area.CreateRectangleForCell(cell);

            foreach (Entity entity in GetActiveEntities())
            {
                if (entity.BoundingBox.CollidesWith(cellRectangle))
                    entityList.Add(entity);
            }
            return entityList;
        }

        public bool CellInBounds(Point cell)
        {
            return (cell.X >= 0 && cell.Y >= 0 && cell.X < WidthInCells && cell.Y < HeightInCells);
        }

        public void DrawBackground(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < WidthInCells; x++)
            {
                for (int y = 0; y < HeightInCells; y++)
                {
                    int textureIndex = backgroundTileLayout[y, x];

                    if (textureIndex == -1)
                        continue;

                    Texture2D texture = tileTextures[textureIndex];

                    spriteBatch.Draw(
                        texture,
                        new Rectangle(
                            x * TILE_WIDTH,
                            y * TILE_HEIGHT,
                            TILE_WIDTH,
                            TILE_HEIGHT),
                        null,
                        Color.White,
                        0f, Vector2.Zero, SpriteEffects.None, 1f);
                }
            }
        }

        public void DrawForeground(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < WidthInCells; x++)
            {
                for (int y = 0; y < HeightInCells; y++)
                {
                    int textureIndex = foregroundTileLayout[y, x];

                    if (textureIndex == -1)
                        continue;

                    Texture2D texture = tileTextures[textureIndex];

                    spriteBatch.Draw(
                        texture,
                        new Rectangle(
                            x * TILE_WIDTH,
                            y * TILE_HEIGHT,
                            TILE_WIDTH,
                            TILE_HEIGHT),
                        null,
                        Color.White,
                        0f, Vector2.Zero, SpriteEffects.None, 0f);
                }
            }
        }

        public void DrawEntities(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            List<Entity> sortedEntities = GetActiveEntities();
            sortedEntities.Sort();
            foreach (Entity entity in sortedEntities)
            {
                if (entity.ShouldBeDrawnByArea)
                {
                    if (entity is ShadowOwner)
                    {
                        ShadowOwner shadowOwner = (ShadowOwner)entity;
                        int shadowTextureIndex = (int)shadowOwner.ShadowSize;

                        Texture2D shadowTexture = null;
                        if (shadowTextureIndex >= 0)
                            shadowTexture = shadowTextures[shadowTextureIndex];

                        if (shadowTexture != null)
                        {
                            spriteBatch.Draw(shadowTexture,
                                new Vector2(
                                    shadowOwner.ShadowCenter.X - (shadowTexture.Width / 2),
                                    shadowOwner.ShadowCenter.Y - (shadowTexture.Height / 2)),
                                Color.White);
                        }
                    }

                    entity.Draw(spriteBatch, changeColorsEffect);
                }
            }
        }

        public void SpawnPickup(PickupDropper pickupDropper)
        {
            float n = (float)GameWorld.Random.NextDouble();
            if (n < pickupDropper.PickupDropChance)
            {
                List<PickupType> possibleTypes = new List<PickupType>();
                possibleTypes.Add(PickupType.BronzeCoin);
                possibleTypes.Add(PickupType.SilverCoin);
                possibleTypes.Add(PickupType.GoldCoin);
                possibleTypes.Add(PickupType.Heart);

                Pickup pickup = new Pickup(game, this, possibleTypes.ElementAt(GameWorld.Random.Next(possibleTypes.Count)), true);
                pickup.Center = pickupDropper.PickupDropPosition;
                pickup.LoadContent();
                Entities.Add(pickup);
            }
        }

        public List<Entity> GetObstacleEntitiesInCollisionPathX(Entity entity, float xMovement)
        {
            List<Entity> entitiesInCollisionPathX = new List<Entity>();
            foreach (Entity other in GetActiveEntities())
            {
                if (other.IsObstacle && 
                    other.BoundingBox.Bottom > entity.ObstacleCollisionBox.Top &&
                    other.BoundingBox.Top < entity.ObstacleCollisionBox.Bottom)
                {
                    if ((xMovement > 0 && other.Center.X > entity.Center.X) ||
                        (xMovement < 0 && other.Center.X < entity.Center.X))
                        entitiesInCollisionPathX.Add(other);
                }
            }
            return entitiesInCollisionPathX;
        }

        public List<Entity> GetObstacleEntitiesInCollisionPathY(Entity entity, float yMovement)
        {
            List<Entity> entitiesInCollisionPathY = new List<Entity>();
            foreach (Entity other in GetActiveEntities())
            {
                if (other.IsObstacle &&
                    other.BoundingBox.Right > entity.ObstacleCollisionBox.Left &&
                    other.BoundingBox.Left < entity.ObstacleCollisionBox.Right)
                {
                    if ((yMovement > 0 && other.Center.Y > entity.Center.Y) ||
                        (yMovement < 0 && other.Center.Y < entity.Center.Y))
                        entitiesInCollisionPathY.Add(other);
                }
            }
            return entitiesInCollisionPathY;
        }

        public List<Entity> GetActiveEntities()
        {
            List<Entity> activeEntities = new List<Entity>();

            foreach (Entity entity in Entities)
            {
                if (entity.IsVisible)
                    activeEntities.Add(entity);
            }

            return activeEntities;
        }

        public bool IsEntityInside(Entity entity)
        {
            Rectangle areaRect = new Rectangle(0, 0, this.WidthInPixels, this.HeightInPixels);
            return entity.BoundingBox.CollidesWith(areaRect);
        }

        public int IsUsingTexture(Texture2D texture)
        {
            if (tileTextures.Contains(texture))
                return tileTextures.IndexOf(texture);
            return -1;
        }

        public void AddTexture(Texture2D texture)
        {
            tileTextures.Add(texture);
        }
    }
}
