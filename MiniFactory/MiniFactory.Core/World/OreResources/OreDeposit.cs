using System;
using MiniFactory.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class OreDeposit
{
    public Point Origin { get; set; }
    public ResourceTile[,] Tiles { get; set; }
    public int TotalRemaining
    {
        get
        {
            int total = 0;
            foreach (var tile in Tiles)
                total += tile.Remaining;
            return total;
        }
    }

    public OreDeposit(int x, int y, int width = 5, int height = 5, ItemType type = ItemType.Stone)
    {
        Origin = new Point(x, y);
        Tiles = new ResourceTile[width, height];
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Tiles[i, j] = new ResourceTile(this, type);
            }
        }
    }

    public Point GetWorldPosition(int localX, int localY)
    {
        return new Point(Origin.X + localX, Origin.Y + localY);
    }

    public void RegisterInWorld(World world)
    {
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                world.Resources[GetWorldPosition(x, y)] = Tiles[x, y];
            }
        }
    }

    public bool TryMine(Point worldGridPos, int amount)
    {
        throw new NotImplementedException();
    }

    public void Update(GameTime gameTime)
    {

    }

    public void Draw(SpriteBatch spriteBatch)
    {
    }
}