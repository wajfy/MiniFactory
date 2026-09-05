using System;
using MiniFactory.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class World
{
    public const int TileSize = 64;

    public Dictionary<Point, Tile> Terrain { get; set; }
    public Dictionary<Point, Building> Buildings { get; set; }
    public Dictionary<Point, ResourceTile> Resources { get; set; }

    public World()
    {
        Terrain = new Dictionary<Point, Tile>();
        Buildings = new Dictionary<Point, Building>();
        Resources = new Dictionary<Point, ResourceTile>();
    }

    public static Point PixelToGrid(Vector2 pixelPos)
    {
        return new Point((int)Math.Floor(pixelPos.X / TileSize), (int)Math.Floor(pixelPos.Y / TileSize));
    }
    public static Vector2 GridToPixel(Point gridPos)
    {
        return new Vector2(gridPos.X * TileSize, gridPos.Y * TileSize);
    }
    public bool TryGetResourceAt(Vector2 pixelPosition, out ResourceTile tile)
    {
        return Resources.TryGetValue(PixelToGrid(pixelPosition), out tile);
    }
    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture)
    {
        foreach(var resource in Resources)
        {
            resource.Value.Draw(spriteBatch, GridToPixel(resource.Key), texture);
        }
    }
}