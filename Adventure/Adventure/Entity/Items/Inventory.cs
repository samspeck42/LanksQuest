using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class Inventory
    {
        public static Buttons[] EQUIPPED_ITEM_BUTTONS = new Buttons[] { Buttons.Y, Buttons.B };

        private List<EquippableItem> ownedEquippableItems;
        private int money;
        private int numKeys;
        private int numArrows;
        private int numBombs;
        private Dictionary<Buttons, EquippableItem> equippedItemsDict;

        public int Money { get { return money; } }
        public int NumKeys { get { return numKeys; } }
        public int NumArrows { get { return numArrows; } }
        public int NumBombs { get { return numBombs; } }

        public Inventory()
        {
            ownedEquippableItems = new List<EquippableItem>();
            money = 0;
            numKeys = 3;
            numArrows = 0;
            numBombs = 0;
            equippedItemsDict = new Dictionary<Buttons, EquippableItem>();
            foreach (Buttons button in EQUIPPED_ITEM_BUTTONS)
            {
                equippedItemsDict.Add(button, null);
            }
        }

        public void CollectPickup(Pickup pickup)
        {
            if (pickup.Type == PickupType.BronzeCoin ||
                    pickup.Type == PickupType.SilverCoin ||
                    pickup.Type == PickupType.GoldCoin)
            {
                money += pickup.Value;
            }
            else if (pickup.Type == PickupType.Key)
            {
                numKeys += pickup.Value;
            }
        }

        public void CollectEquippableItem(EquippableItem item)
        {
            if (!ownedEquippableItems.Contains(item))
                ownedEquippableItems.Add(item);
        }

        public void UseKey()
        {
            numKeys--;
        }

        public EquippableItem GetEquippedItemForButton(Buttons button)
        {
            return equippedItemsDict[button];
        }

        public EquippableItem GetEquippableItemAtPoint(Point point)
        {
            foreach (EquippableItem item in ownedEquippableItems)
            {
                if (item.InventoryScreenPoint.Equals(point))
                    return item;
            }
            return null;
        }

        public void EquipItem(EquippableItem item, Buttons button)
        {
            if (ownedEquippableItems.Contains(item) && EQUIPPED_ITEM_BUTTONS.Contains(button))
            {
                foreach (Buttons b in EQUIPPED_ITEM_BUTTONS)
                {
                    if (equippedItemsDict[b] != null && equippedItemsDict[b].Equals(item))
                        equippedItemsDict[b] = null;
                }

                equippedItemsDict[button] = item;
            }
        }
    }
}
