using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public static class ItemDatabase
{
    private static Dictionary<ItemType, ItemDefinition> Items;
    public static ItemDefinition Get(ItemType type)
    {
        return Items[type];
    }

    public static void LoadContent(ContentManager content)
    {
        Items = new Dictionary<ItemType, ItemDefinition>()
        {
            {ItemType.Stone, new ItemDefinition("Stone", "For rock and stone!", 3, 1, Color.Gray, content.Load<Texture2D>("stone"))},
            {ItemType.Coal, new ItemDefinition("Coal", "just black stone", 2, 1.5f, Color.Black, content.Load<Texture2D>("coal"))},
            {ItemType.Iron, new ItemDefinition("Iron", "better than stone", 3, 3, Color.SandyBrown, content.Load<Texture2D>("iron_ore"))},
            {ItemType.Copper, new ItemDefinition("Copper", "good for electronics", 1, 2.5f, Color.OrangeRed, content.Load<Texture2D>("copper_ore"))},
        };
    }
}