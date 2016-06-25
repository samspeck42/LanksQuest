using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Adventure.Entities.Items;

namespace Adventure.Screens
{
    public class InventoryScreen
    {
        private const int NUM_ROWS = 3;
        private const int NUM_COLUMNS = 6;
        private const int ITEM_BOX_LENGTH = 38;
        private const int MARGIN_LENGTH = 5;
        private const int ITEM_BOX_OFFSET_X = 9;
        private const int ITEM_BOX_OFFSET_Y = 9;
        private static string[] equippableItemIds = new string[] { 
            "bow",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
        };

        private Inventory inventory;
        private Vector2 position = Vector2.Zero;
        private Point selectedItemPoint = new Point();
        private bool isDisplaying = false;
        public bool IsDisplaying { get { return isDisplaying; } }
        private GamePadState gamepadState;
        private GamePadState previousGamepadState;

        private Texture2D background;
        private Texture2D selectedItemOutline;
        private Dictionary<string, Texture2D> equippableItemIconDict = new Dictionary<string,Texture2D>();

        public InventoryScreen(Inventory inventory)
        {
            this.inventory = inventory;
            gamepadState = GamePad.GetState(PlayerIndex.One);
            previousGamepadState = gamepadState;
        }

        public void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Screens/item_screen_background");
            selectedItemOutline = content.Load<Texture2D>("Screens/selected_item_outline");

            foreach (string id in equippableItemIds)
            {
                if (!string.IsNullOrEmpty(id))
                    equippableItemIconDict.Add(id, content.Load<Texture2D>("Screens/Icons/" + id));
            }
        }

        public Texture2D GetEquippableItemIcon(string id)
        {
            return equippableItemIconDict[id];
        }

        public void StartDisplaying()
        {
            isDisplaying = true;
            position.X = (AdventureGame.SCREEN_WIDTH / 2) - (background.Width / 2);
            position.Y = (AdventureGame.SCREEN_HEIGHT / 2) - (background.Height / 2);
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

            PlayerButtons itemButtonPressed = PlayerButtons.None;
            if (gamepadState.IsButtonDown(PlayerInput.PlayerButtonsToButtons(PlayerButtons.EquippedItem1)) &&
                previousGamepadState.IsButtonUp(PlayerInput.PlayerButtonsToButtons(PlayerButtons.EquippedItem1)))
                itemButtonPressed = PlayerButtons.EquippedItem1;
            else if (gamepadState.IsButtonDown(PlayerInput.PlayerButtonsToButtons(PlayerButtons.EquippedItem2)) &&
                previousGamepadState.IsButtonUp(PlayerInput.PlayerButtonsToButtons(PlayerButtons.EquippedItem2)))
                itemButtonPressed = PlayerButtons.EquippedItem2;

            if (itemButtonPressed != PlayerButtons.None)
            {
                string id = equippableItemIds[selectedItemPoint.Y * NUM_COLUMNS + selectedItemPoint.X];
                EquippableItem item;
                if ((item = inventory.GetOwnedEquippableItemByInventoryScreenId(id)) != null)
                {
                    int index = itemButtonPressed == PlayerButtons.EquippedItem1 ? 0 : 1;
                    inventory.EquipItem(item, index);
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

            //for (int x = 0; x < NUM_COLUMNS; x++)
            //{
            //    for (int y = 0; y < NUM_ROWS; y++)
            //    {
            //        EquippableItem item;
            //        if ((item = inventory.GetEquippableItemAtPoint(new Point(x, y))) != null)
            //        {
            //            Vector2 centerPos = new Vector2(position.X + ITEM_BOX_OFFSET_X, position.Y + ITEM_BOX_OFFSET_Y);
            //            centerPos.X += (x * (ITEM_BOX_LENGTH + MARGIN_LENGTH)) + (ITEM_BOX_LENGTH / 2);
            //            centerPos.Y += y * (ITEM_BOX_LENGTH + MARGIN_LENGTH) + (ITEM_BOX_LENGTH / 2);
            //            //spriteBatch.Draw(item.InventoryScreenIcon,
            //            //    new Vector2(centerPos.X - (item.InventoryScreenIcon.Width / 2),
            //            //        centerPos.Y - (item.InventoryScreenIcon.Height / 2)),
            //            //        Color.White);
            //        }
            //    }
            //}

            foreach (string id in equippableItemIds)
            {
                if (inventory.IsEquippableItemOwned(id))
                {
                    int index = Array.IndexOf(equippableItemIds, id);
                    int x = index % NUM_COLUMNS;
                    int y = index / NUM_COLUMNS;
                    Texture2D icon = equippableItemIconDict[id];

                    Vector2 centerPos = new Vector2(position.X + ITEM_BOX_OFFSET_X, position.Y + ITEM_BOX_OFFSET_Y);
                    centerPos.X += (x * (ITEM_BOX_LENGTH + MARGIN_LENGTH)) + (ITEM_BOX_LENGTH / 2);
                    centerPos.Y += y * (ITEM_BOX_LENGTH + MARGIN_LENGTH) + (ITEM_BOX_LENGTH / 2);
                    spriteBatch.Draw(icon,
                        new Vector2(centerPos.X - (icon.Width / 2),
                            centerPos.Y - (icon.Height / 2)),
                            Color.White);
                }
            }

            spriteBatch.End();
        }
    }
}
