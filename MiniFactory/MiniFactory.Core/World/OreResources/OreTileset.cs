using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public static class OreTileset
{
    public static Texture2D OreTextures;
    private static int tileSize = 16;

    public static void LoadContent(ContentManager content)
    {
        OreTextures = content.Load<Texture2D>("ores");
    }

    public static Rectangle GetSourceRect(ItemType type, int remaining)
    {
        int column;
        int row = 0;
        if (remaining <= 100)
        {
            column = 0;
        }
        else if (remaining <= 400)
        {
            column = 1;
        }
        else if (remaining <= 1000)
        {
            column = 2;
        }
        else
        {
            column = 3;
        }

        switch (type)
        {
            case ItemType.Stone:
                row = 0;
                break;
            case ItemType.IronOre:
                row = 1;
                break;
            case ItemType.CopperOre:
                row = 2;
                break;
            case ItemType.Coal:
                row = 3;
                break;
        }

        return new Rectangle(column * tileSize, row * tileSize, tileSize, tileSize);
    }
}