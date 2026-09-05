using System.Collections.Generic;
using Microsoft.Xna.Framework;

public static class ItemDatabase
{
    private static readonly Dictionary<ItemType, ItemDefinition> Items = new Dictionary<ItemType, ItemDefinition>()
    {
        {ItemType.Stone, new ItemDefinition("Stone", "For rock and stone!", 3, 1, Color.Gray)},
        {ItemType.Coal, new ItemDefinition("Coal", "just black stone", 2, 1.5f, Color.Black)},
        {ItemType.Iron, new ItemDefinition("Iron", "better than stone", 3, 3, Color.SandyBrown)},
        {ItemType.Copper, new ItemDefinition("Copper", "good for electronics", 1, 2.5f, Color.OrangeRed)},
    };

    public static ItemDefinition Get(ItemType type)
    {
        return Items[type];
    }
}