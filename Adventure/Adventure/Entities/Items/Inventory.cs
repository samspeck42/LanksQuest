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
        private List<EquippableItem> ownedEquippableItems;
        private int money;
        private int numKeys;
        private int numArrows;
        private int numBombs;
        private EquippableItem[] equippedItems = new EquippableItem[2];

        public int Money { get { return money; } }
        public int NumKeys { get { return numKeys; } }
        public int NumArrows { get { return numArrows; } }
        public int NumBombs { get { return numBombs; } }

        public Inventory()
        {
            ownedEquippableItems = new List<EquippableItem>();
            money = 0;
            numKeys = 0;
            numArrows = 0;
            numBombs = 0;
        }

        //public void CollectPickup(Pickup pickup)
        //{
        //    if (pickup.Type == PickupType.BronzeCoin ||
        //            pickup.Type == PickupType.SilverCoin ||
        //            pickup.Type == PickupType.GoldCoin)
        //    {
        //        money += pickup.Value;
        //    }
        //    else if (pickup.Type == PickupType.Key)
        //    {
        //        numKeys += pickup.Value;
        //    }
        //}

        public void CollectEquippableItem(EquippableItem item)
        {
            if (!ownedEquippableItems.Contains(item))
                ownedEquippableItems.Add(item);
        }

        public void UseKey()
        {
            numKeys--;
        }

        public EquippableItem EquippedItemAtIndex(int index)
        {
            return equippedItems[index];
        }

        public int IndexOfEquippedItem(EquippableItem item)
        {
            if (equippedItems.Contains(item))
                return Array.IndexOf(equippedItems, item);
            return -1;
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

        public void EquipItem(EquippableItem item, int index)
        {
            if (ownedEquippableItems.Contains(item) && index >= 0 && index < equippedItems.Length)
            {
                for (int i = 0; i < equippedItems.Length; i++)
                {
                    if (equippedItems[i] != null && equippedItems[i].Equals(item))
                        equippedItems[i] = null;
                }

                equippedItems[index] = item;
            }
        }
    }
}
