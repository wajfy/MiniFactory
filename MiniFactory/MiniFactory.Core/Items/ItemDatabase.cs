using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public static class ItemDatabase
{
    private static Dictionary<ItemType, ItemDefinition> Items;
    public static Texture2D Textures;

    public static ItemDefinition Get(ItemType type)
    {
        return Items[type];
    }

    public static Dictionary<ItemType, ItemDefinition> GetAll()
    {
        return Items;
    }

    public static void LoadContent(ContentManager content)
    {
        Textures = content.Load<Texture2D>("items_inventory");
        Items = new Dictionary<ItemType, ItemDefinition>()
        {
            {ItemType.Stone, new ItemDefinition("Stone", "For rock and stone!", 3, 1, GetSourceRectangle(0, 0))},
            {ItemType.IronOre, new ItemDefinition("Iron", "better than stone", 3, 3, GetSourceRectangle(1, 0))},
            {ItemType.CopperOre, new ItemDefinition("Copper", "good for electronics", 1, 2.5f, GetSourceRectangle(2, 0))},
            {ItemType.Coal, new ItemDefinition("Coal", "just black stone", 2, 1.5f, GetSourceRectangle(3, 0))},

            {ItemType.StoneBrick, new ItemDefinition("Stone brick", "just black stone", 2, 1.5f, GetSourceRectangle(0, 1))},
            {ItemType.IronSheet, new ItemDefinition("Iron sheet", "just black stone", 2, 1.5f, GetSourceRectangle(1, 1))},
            {ItemType.CopperSheet, new ItemDefinition("Copper sheet", "just black stone", 2, 1.5f, GetSourceRectangle(2, 1))},
            {ItemType.IronCogWheel, new ItemDefinition("Iron cog wheel", "just black stone", 2, 1.5f, GetSourceRectangle(3, 1))},

            {ItemType.CopperWire, new ItemDefinition("Copper wire", "just black stone", 2, 1.5f, GetSourceRectangle(0, 2))},
            {ItemType.FurnaceTierOne, new ItemDefinition("Furnace T1", "just black stone", 2, 1.5f, GetSourceRectangle(1, 2))},
            {ItemType.MinerTierOne, new ItemDefinition("Drill T1", "just black stone", 2, 1.5f, GetSourceRectangle(2, 2))},
        };
    }

    private static Rectangle GetSourceRectangle(int x, int y)
    {
        return new Rectangle(x * 16, y * 16, 16, 16);
    }
}