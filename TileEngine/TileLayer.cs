using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    public class TileLayer
    {
        private Tile[,] tileMap;
        private Dictionary<string, Texture2D> tileTextureDict;

        public int Width { get { return tileMap.GetLength(1); } }

        public int Height { get { return tileMap.GetLength(0); } }

        public int WidthInPixels { get { return Width * Tile.Width; } }

        public int HeightInPixels { get { return Height * Tile.Height; } }
        
        public TileLayer(int width, int height)
        {
            tileMap = new Tile[height, width];
            tileTextureDict = new Dictionary<string, Texture2D>();

            for (int r = 0; r < tileMap.GetLength(0); r++)
                for (int c = 0; c < tileMap.GetLength(1); c++)
                    tileMap[r, c] = new Tile();
        }

        public static Point ConvertPositionToCell(Vector2 position)
        {
            return new Point(
                (int)(position.X / (float)Tile.Width),
                (int)(position.Y / (float)Tile.Height));
        }

        public static TileLayer FromFile(GraphicsDevice graphicsDevice, string contentPath, string filename)
        {
            List<string> textureNames = new List<string>();
            TileLayer tileLayer = ProcessFile(filename, textureNames);

            foreach (string textureName in textureNames)
            {
                string path = contentPath + "/" + textureName;

                foreach (string ext in Tile.ImageExtensions)
                {
                    if (File.Exists(path + ext))
                    {
                        path += ext;
                        break;
                    }
                }

                Texture2D texture = Texture2D.FromStream(graphicsDevice,
                    new FileStream(path, FileMode.Open));
                tileLayer.tileTextureDict.Add(textureName, texture);
            }

            return tileLayer;
        }

        public static TileLayer FromFile(ContentManager content, string filename)
        {
            List<string> textureNames = new List<string>();
            TileLayer tileLayer = ProcessFile(filename, textureNames);

            //set textures
            foreach (string textureName in textureNames)
            {
                tileLayer.tileTextureDict.Add(textureName, content.Load<Texture2D>(textureName));
            }

            return tileLayer;
        }

        private static TileLayer ProcessFile(string filename, List<string> textureNames)
        {
            TileLayer tileLayer;
            List<Tile> tileList = new List<Tile>();
            List<List<int>> tileLayout = new List<List<int>>();

            using (StreamReader reader = new StreamReader(filename))
            {
                bool readingTileData = false;
                bool readingLayout = false;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    else if (line.Contains("[TileData]"))
                    {
                        readingTileData = true;
                        readingLayout = false;
                    }

                    else if (line.Contains("[Layout]"))
                    {
                        readingTileData = false;
                        readingLayout = true;
                    }

                    else if (readingTileData)
                    {
                        Tile tile;
                        string[] data = line.Split(' ');

                        string textureName = data[0];
                        TileCollision collision = (TileCollision)int.Parse(data[1]);

                        tile = new Tile(textureName, collision);
                        tileList.Add(tile);
                        textureNames.Add(textureName);
                    }

                    else if (readingLayout)
                    {
                        List<int> row = new List<int>();

                        string[] cells = line.Split(' ');

                        foreach (string c in cells)
                        {
                            if (!string.IsNullOrEmpty(c))
                                row.Add(int.Parse(c));
                        }

                        tileLayout.Add(row);
                    }
                }
            }

            int layerWidth = tileLayout[0].Count;
            int layerHeight = tileLayout.Count;

            tileLayer = new TileLayer(layerWidth, layerHeight);

            //set tiles
            for (int r = 0; r < layerHeight; r++)
            {
                for (int c = 0; c < layerWidth; c++)
                {
                    int tileIndex = tileLayout[r][c];
                    Tile empty = new Tile(null, TileCollision.None);

                    if (tileIndex <= 0)
                        tileLayer.SetTileAtCell(new Point(c, r), empty);
                    else
                        tileLayer.SetTileAtCell(new Point(c, r), tileList[tileIndex - 1]);
                }
            }

            return tileLayer;
        }

        public Tile GetTileAtCell(Point cell) 
        {
            if (CellInBounds(cell))
                return tileMap[cell.Y, cell.X];
            return new Tile(null, TileCollision.None);
        }

        public void SetTileAtCell(Point cell, Tile tile) 
        {
            if (CellInBounds(cell))
                tileMap[cell.Y, cell.X] = tile;
        }

        public TileCollision GetCollisionAtCell(Point cell) 
        {
            if (CellInBounds(cell))
                return tileMap[cell.Y, cell.X].Collision;
            return TileCollision.None;
        }

        public bool CellInBounds(Point cell)
        {
            return (cell.X >= 0 && cell.Y >= 0 && cell.X < Width && cell.Y < Height);
        }

        public static Rectangle CreateRectangleForCell(Point cell)
        {
            return new Rectangle(cell.X * Tile.Width, cell.Y * Tile.Height, Tile.Width, Tile.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int r = 0; r < tileMap.GetLength(0); r++)
            {
                for (int c = 0; c < tileMap.GetLength(1); c++)
                {
                    if (string.IsNullOrEmpty(tileMap[r, c].TextureName))
                        continue;

                    Texture2D texture = tileTextureDict[tileMap[r, c].TextureName];

                    spriteBatch.Draw(
                        texture,
                        new Rectangle(c * Tile.Width, r * Tile.Height, Tile.Width, Tile.Height),
                        Color.White);
                }
            }
        }

    }
}
