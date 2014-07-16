using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Adventure
{
    public class InventoryScreen
    {
        private const int NUM_ROWS = 3;
        private const int NUM_COLUMNS = 6;
        private const int ITEM_BOX_LENGTH = 38;
        private const int MARGIN_LENGTH = 5;
        private const int ITEM_BOX_OFFSET_X = 9;
        private const int ITEM_BOX_OFFSET_Y = 9;

        private Inventory inventory;
        private Vector2 position;
        private Point selectedItemPoint;
        private bool isDisplaying = false;
        public bool IsDisplaying { get { return isDisplaying; } }
        private GamePadState gamepadState;
        private GamePadState previousGamepadState;

        private Texture2D background;
        private Texture2D selectedItemOutline;

        public InventoryScreen(Inventory inventory)
        {
            this.inventory = inventory;
            position = Vector2.Zero;
            selectedItemPoint = new Point();
            gamepadState = GamePad.GetState(PlayerIndex.One);
            previousGamepadState = gamepadState;
        }

        public void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Screens/item_screen_background");
            selectedItemOutline = content.Load<Texture2D>("Screens/selected_item_outline");
        }

        public void StartDisplaying()
        {
            isDisplaying = true;
            position.X = (Adventure.SCREEN_WIDTH / 2) - (background.Width / 2);
            position.Y = (Adventure.SCREEN_HEIGHT / 2) - (background.Height / 2);
            selectedItemPoint = new Point(0, 0);
        }

        public void StopDisplaying()
        {
            isDisplaying = false;
        }

        public void Update()
        {
            gamepadState = GamePad.GetState(PlayerIndex.One);

            if (gamepadState.IsButtonDown(Buttons.LeftThumbstickUp) && previousGamepadState.IsButtonUp(Buttons.LeftThumbstickUp))
                selectedItemPoint.Y--;
            if (gamepadState.IsButtonDown(Buttons.LeftThumbstickDown) && previousGamepadState.IsButtonUp(Buttons.LeftThumbstickDown))
                selectedItemPoint.Y++;
            if (gamepadState.IsButtonDown(Buttons.LeftThumbstickLeft) && previousGamepadState.IsButtonUp(Buttons.LeftThumbstickLeft))
                selectedItemPoint.X--;
            if (gamepadState.IsButtonDown(Buttons.LeftThumbstickRight) && previousGamepadState.IsButtonUp(Buttons.LeftThumbstickRight))
                selectedItemPoint.X++;

            selectedItemPoint.X = (int)MathHelper.Clamp(selectedItemPoint.X, 0, NUM_COLUMNS - 1);
            selectedItemPoint.Y = (int)MathHelper.Clamp(selectedItemPoint.Y, 0, NUM_ROWS - 1);

            foreach (Buttons button in Inventory.EQUIPPED_ITEM_BUTTONS)
            {
                if (gamepadState.IsButtonDown(button) && previousGamepadState.IsButtonUp(button))
                {
                    EquippableItem item;
                    if ((item = inventory.GetEquippableItemAtPoint(selectedItemPoint)) != null)
                    {
                        inventory.EquipItem(item, button);
                    }
                }
            }

            previousGamepadState = gamepadState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, position, Color.White);

            Vector2 selectedItemOutlinePos = new Vector2(position.X + ITEM_BOX_OFFSET_X, position.Y + ITEM_BOX_OFFSET_Y);
            selectedItemOutlinePos.X += selectedItemPoint.X * (ITEM_BOX_LENGTH + MARGIN_LENGTH);
            selectedItemOutlinePos.Y += selectedItemPoint.Y * (ITEM_BOX_LENGTH + MARGIN_LENGTH);
            spriteBatch.Draw(selectedItemOutline, selectedItemOutlinePos, Color.White);

            for (int x = 0; x < NUM_COLUMNS; x++)
            {
                for (int y = 0; y < NUM_ROWS; y++)
                {
                    EquippableItem item;
                    if ((item = inventory.GetEquippableItemAtPoint(new Point(x, y))) != null)
                    {
                        Vector2 centerPos = new Vector2(position.X + ITEM_BOX_OFFSET_X, position.Y + ITEM_BOX_OFFSET_Y);
                        centerPos.X += (x * (ITEM_BOX_LENGTH + MARGIN_LENGTH)) + (ITEM_BOX_LENGTH / 2);
                        centerPos.Y += y * (ITEM_BOX_LENGTH + MARGIN_LENGTH) + (ITEM_BOX_LENGTH / 2);
                        spriteBatch.Draw(item.InventoryScreenIcon,
                            new Vector2(centerPos.X - (item.InventoryScreenIcon.Width / 2),
                                centerPos.Y - (item.InventoryScreenIcon.Height / 2)),
                                Color.White);
                    }
                }
            }

            spriteBatch.End();
        }
    }
}
