using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine;
using Adventure;

namespace TileEditor
{
    public partial class Form1 : Form
    {
        const int MAX_FILL_CELLS = 1000;

        SpriteBatch spriteBatch;
        Texture2D tileBorderTexture;
        Texture2D collisionTileTexture;

        int cellX, cellY;

        int maxWidth = 0, maxHeight = 0;
        int fillCounter = 0;
        bool mouseDown = false;

        Area area = null;
        Camera camera = new Camera();
        string areaFileName = "";

        public GraphicsDevice GraphicsDevice { get { return tileDisplay1.GraphicsDevice; } }

        Dictionary<string, System.Drawing.Image> previewDict = new Dictionary<string, System.Drawing.Image>();
        Dictionary<string, Texture2D> tileTextureDict = new Dictionary<string, Texture2D>();
        Dictionary<TileCollision, System.Drawing.Color> tileCollisionColorDict =
            new Dictionary<TileCollision, System.Drawing.Color>
            {
                {TileCollision.Wall,System.Drawing.Color.Orange}
            };


        public Form1()
        {
            InitializeComponent();
            tileDisplay1.OnInitialize += new EventHandler(tileDisplay1_OnInitialize);
            tileDisplay1.OnDraw += new EventHandler(tileDisplay1_OnDraw);

            Application.Idle += delegate { tileDisplay1.Invalidate(); };
            vScrollBar1.ValueChanged += delegate { tileDisplay1.Invalidate(); };
            hScrollBar1.ValueChanged += delegate { tileDisplay1.Invalidate(); };

            Mouse.WindowHandle = tileDisplay1.Handle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (TileCollision collision in tileCollisionColorDict.Keys)
            {
                tileCollisionListBox.Items.Add(collision);
            }
            tileCollisionListBox.DrawMode = DrawMode.OwnerDrawFixed;
        }

        private void tileDisplay1_OnInitialize(object sender, EventArgs e)
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tileBorderTexture = Texture2D.FromStream(GraphicsDevice,
                new FileStream("Content/tile_border.png", FileMode.Open));
            collisionTileTexture = Texture2D.FromStream(GraphicsDevice,
                new FileStream("Content/collision_tile.png", FileMode.Open));
        }

        private void tileDisplay1_OnDraw(object sender, EventArgs e)
        {
            camera.Position.X = hScrollBar1.Value * Area.TILE_WIDTH;
            camera.Position.Y = vScrollBar1.Value * Area.TILE_HEIGHT;

            int mx = Mouse.GetState().X;
            int my = Mouse.GetState().Y;

            if (area != null)
            {
                if (mx >= 0 && mx < tileDisplay1.Width && my >= 0 && my < tileDisplay1.Height)
                {
                    cellX = mx / Area.TILE_WIDTH;
                    cellY = my / Area.TILE_HEIGHT;

                    cellX += hScrollBar1.Value;
                    cellY += vScrollBar1.Value;

                    cellX = (int)MathHelper.Clamp(cellX, 0, area.WidthInCells - 1);
                    cellY = (int)MathHelper.Clamp(cellY, 0, area.HeightInCells - 1);

                    if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        if (tabControl1.SelectedTab == tileLayoutTab)
                        {
                            if (drawButton.Checked && tileTextureListBox.SelectedItem != null)
                            {
                                Texture2D texture = tileTextureDict[tileTextureListBox.SelectedItem as string];

                                int index = area.IsUsingTexture(texture);

                                if (index == -1)
                                {
                                    area.AddTexture(texture);
                                    index = area.IsUsingTexture(texture);
                                }

                                if (fillCellsCheckBox.Checked)
                                {
                                    fillCounter = MAX_FILL_CELLS;
                                    FillCell(cellX, cellY, index);
                                }
                                else
                                {
                                    if (editBackgroundButton.Checked)
                                        area.SetBackgroundTileIndexAtCell(new Point(cellX, cellY), index);
                                    else if (editForegroundButton.Checked)
                                        area.SetForegroundTileIndexAtCell(new Point(cellX, cellY), index);
                                }
                            }
                            else if (eraseButton.Checked)
                            {
                                if (fillCellsCheckBox.Checked)
                                {
                                    fillCounter = MAX_FILL_CELLS;
                                    FillCell(cellX, cellY, -1);
                                }
                                else
                                {
                                    if (editBackgroundButton.Checked)
                                        area.SetBackgroundTileIndexAtCell(new Point(cellX, cellY), -1);
                                    else if (editForegroundButton.Checked)
                                        area.SetForegroundTileIndexAtCell(new Point(cellX, cellY), -1);
                                }
                            }
                        }

                        else if (tabControl1.SelectedTab == collisionLayoutTab)
                        {
                            if (addTileCollisionButton.Checked && tileCollisionListBox.SelectedItem != null)
                            {
                                TileCollision collision = (TileCollision)tileCollisionListBox.SelectedItem;
                                area.SetCollisionAtCell(new Point(cellX, cellY), collision);
                            }
                            else if (removeTileCollisionButton.Checked)
                            {
                                area.SetCollisionAtCell(new Point(cellX, cellY), TileCollision.None);
                            }
                        }
                    }
                }
                else
                {
                    cellX = cellY = -1;
                }
            }

            Render();
        }

        private void Render()
        {
            GraphicsDevice.Clear(Color.Black);

            if (area != null)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, camera.TransformMatrix);
                area.DrawBackground(spriteBatch);
                if (editForegroundButton.Checked)
                    area.DrawForeground(spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin();
                for (int y = 0; y < area.HeightInCells; y++)
                {
                    for (int x = 0; x < area.WidthInCells; x++)
                    {
                        if (area != null)
                        {
                            if (tabControl1.SelectedTab == collisionLayoutTab)
                            {
                                TileCollision collision = area.GetCollisionAtCell(x, y);
                                if (collision != TileCollision.None)
                                {
                                    System.Drawing.Color color = tileCollisionColorDict[collision];

                                    spriteBatch.Draw(
                                    collisionTileTexture,
                                    new Rectangle(
                                        x * Tile.Width - (int)camera.Position.X,
                                        y * Tile.Height - (int)camera.Position.Y,
                                        Tile.Width,
                                        Tile.Height),
                                    new Color(color.R, color.G, color.B));
                                }
                            }

                            if (((editBackgroundButton.Checked && area.GetBackgroundTileIndexAtCell(new Point(x, y)) == -1) ||
                            (editForegroundButton.Checked && area.GetForegroundTileIndexAtCell(new Point(x, y)) == -1)))
                            {
                                spriteBatch.Draw(
                                tileBorderTexture,
                                new Rectangle(
                                    x * Tile.Width - (int)camera.Position.X,
                                    y * Tile.Height - (int)camera.Position.Y,
                                    Tile.Width,
                                    Tile.Height),
                                Color.White);
                            }
                        }
                    }
                }
                if (cellX != -1 && cellY != -1)
                {
                    spriteBatch.Draw(
                            tileBorderTexture,
                            new Rectangle(
                                cellX * Tile.Width - (int)camera.Position.X,
                                cellY * Tile.Height - (int)camera.Position.Y,
                                Tile.Width,
                                Tile.Height),
                            Color.Red);
                }
                else
                {
                    cellX = cellY = -1;
                }
                spriteBatch.End();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Area File|*.area";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                string contentPath = fileName.Substring(0, fileName.LastIndexOf("Content") + 8);
                areaFileName = Path.GetFileName(fileName);

                string[] tileTextureNames;
                area = Area.FromFile(GraphicsDevice, contentPath, fileName, null, out tileTextureNames);

                tileTextureListBox.Items.Clear();
                previewDict.Clear();
                tileTextureDict.Clear();
                camera.Position = Vector2.Zero;

                addAllTileTexturesInFolder(contentPath + "Tiles\\");

                foreach (string tileTextureName in tileTextureNames)
                {
                    area.AddTexture(tileTextureDict[tileTextureName]);
                }

                adjustScrollBars();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewAreaForm form = new NewAreaForm();

            form.ShowDialog();

            if (form.OKPressed)
            {
                string name = form.nameTextBox.Text;
                int width = int.Parse(form.widthTextBox.Text);
                int height = int.Parse(form.heightTextBox.Text);
                string contentPath = form.contentPathTextBox.Text + "\\";

                areaFileName = name + ".area";
                area = new Area(null, null, width, height);

                tileTextureListBox.Items.Clear();
                previewDict.Clear();
                tileTextureDict.Clear();
                camera.Position = Vector2.Zero;

                addAllTileTexturesInFolder(contentPath + "Tiles\\");
                adjustScrollBars();
            }
        }

        private void addAllTileTexturesInFolder(string path)
        {
            List<string> fileNames = new List<string>();

            foreach (string ext in Area.TileTextureImageExtensions)
            {
                string[] textureFiles = Directory.GetFiles(path, "*" + ext);

                foreach (string file in textureFiles)
                    fileNames.Add(file);
            }

            foreach (string fileName in fileNames)
            {
                string tileTextureName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                tileTextureName = tileTextureName.Remove(tileTextureName.LastIndexOf("."));

                tileTextureListBox.Items.Add(tileTextureName);

                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                previewDict.Add(tileTextureName, image);
                Texture2D texture = Texture2D.FromStream(GraphicsDevice, fs);
                tileTextureDict.Add(tileTextureName, texture);
            }
        }

        private void adjustScrollBars()
        {
            if (area.WidthInPixels > tileDisplay1.Width)
            {
                maxWidth = (int)Math.Ceiling((double)(area.WidthInPixels - tileDisplay1.Width) / Area.TILE_WIDTH) - 1 + hScrollBar1.LargeChange;

                hScrollBar1.Visible = true;
                hScrollBar1.Minimum = 0;
                hScrollBar1.Maximum = maxWidth;

            }
            else
            {
                maxWidth = 0;
                hScrollBar1.Visible = false;
            }
            if (area.HeightInPixels > tileDisplay1.Height)
            {
                maxHeight = (int)Math.Ceiling((double)(area.HeightInPixels - tileDisplay1.Height) / Area.TILE_HEIGHT) - 1 + vScrollBar1.LargeChange;

                vScrollBar1.Visible = true;
                vScrollBar1.Minimum = 0;
                vScrollBar1.Maximum = maxHeight;

            }
            else
            {
                maxHeight = 0;
                vScrollBar1.Visible = false;
            }

            hScrollBar1.Value = 0;
            vScrollBar1.Value = 0;
        }

        public void FillCell(int x, int y, int desiredIndex)
        {
            int oldIndex = 0;
            if (editBackgroundButton.Checked)
                oldIndex = area.GetBackgroundTileIndexAtCell(new Point(x, y));
            else if (editForegroundButton.Checked)
                oldIndex = area.GetForegroundTileIndexAtCell(new Point(x, y));

            if (desiredIndex == oldIndex || fillCounter == 0)
                return;

            fillCounter--;

            if (editBackgroundButton.Checked)
                area.SetBackgroundTileIndexAtCell(new Point(x, y), desiredIndex);
            else if (editForegroundButton.Checked)
                area.SetForegroundTileIndexAtCell(new Point(x, y), desiredIndex);

            if (editBackgroundButton.Checked)
            {
                if (x > 0 && area.GetBackgroundTileIndexAtCell(new Point(x - 1, y)) == oldIndex)
                    FillCell(x - 1, y, desiredIndex);
                if (x < area.WidthInCells - 1 && area.GetBackgroundTileIndexAtCell(new Point(x + 1, y)) == oldIndex)
                    FillCell(x + 1, y, desiredIndex);
                if (y > 0 && area.GetBackgroundTileIndexAtCell(new Point(x, y - 1)) == oldIndex)
                    FillCell(x, y - 1, desiredIndex);
                if (y < area.HeightInCells - 1 && area.GetBackgroundTileIndexAtCell(new Point(x, y + 1)) == oldIndex)
                    FillCell(x, y + 1, desiredIndex);
            }
            else if (editForegroundButton.Checked)
            {
                if (x > 0 && area.GetForegroundTileIndexAtCell(new Point(x - 1, y)) == oldIndex)
                    FillCell(x - 1, y, desiredIndex);
                if (x < area.WidthInCells - 1 && area.GetForegroundTileIndexAtCell(new Point(x + 1, y)) == oldIndex)
                    FillCell(x + 1, y, desiredIndex);
                if (y > 0 && area.GetForegroundTileIndexAtCell(new Point(x, y - 1)) == oldIndex)
                    FillCell(x, y - 1, desiredIndex);
                if (y < area.HeightInCells - 1 && area.GetForegroundTileIndexAtCell(new Point(x, y + 1)) == oldIndex)
                    FillCell(x, y + 1, desiredIndex);
            }

        }

        private void tileTextureListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tileTextureListBox.SelectedItem != null)
            {
                string tileTextureName = tileTextureListBox.SelectedItem as string;
                tileTexturePreviewBox.Image = previewDict[tileTextureName];
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (area != null)
            {
                saveFileDialog1.FileName = areaFileName;

                Dictionary<int, string> utilizedTextures = new Dictionary<int, string>();

                foreach (string textureName in tileTextureListBox.Items)
                {
                    int index = area.IsUsingTexture(tileTextureDict[textureName]);

                    if (index != -1)
                    {
                        utilizedTextures.Add(index, textureName);
                    }
                }

                List<string> utilizedTextureList = new List<string>();

                for (int k = 0; k < utilizedTextures.Count; k++)
                    utilizedTextureList.Add(utilizedTextures[k]);

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    area.Save(saveFileDialog1.FileName, utilizedTextureList.ToArray());
            }
        }

        private void tileCollisionListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            TileCollision collision = (TileCollision)tileCollisionListBox.Items[e.Index];
            System.Drawing.Color color = tileCollisionColorDict[collision];

            System.Drawing.Brush brush = new System.Drawing.SolidBrush(color);

            e.Graphics.DrawString(tileCollisionListBox.Items[e.Index].ToString(), 
                e.Font, brush, e.Bounds, System.Drawing.StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }

        private void tileDisplay1_MouseClick(object sender, MouseEventArgs e)
        {
            mouseDown = true; 
        }    
    }
}
