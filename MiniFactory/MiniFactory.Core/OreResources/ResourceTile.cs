using System;
using MiniFactory.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class ResourceTile
{
    public ItemType Type { get; set; }
    private int _remaining;
    public int Remaining
    {
        get => _remaining;
        set => _remaining = Math.Max(0, value);
    }
    public OreDeposit ParentDeposit { get; set; }
    public float MiningDuration = 5f;
    private static readonly Random _random = new Random();

    public ResourceTile(OreDeposit parent, ItemType type, int remaining = 0)
    {
        ParentDeposit = parent;
        Type = type;
        Remaining = remaining > 0 ? remaining : _random.Next(5, 10);
    }

    private Color GetColor()
    {
        switch (Type)
        {
            case ItemType.Stone:
                return Color.Gray;
            case ItemType.Coal:
                return Color.Black;
            case ItemType.Iron:
                return Color.SandyBrown;
            case ItemType.Copper:
                return Color.OrangeRed;
            default:
                return Color.Pink;
        }
    }

    public void Update(GameTime gameTime)
    {

    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Texture2D texture)
    {
        spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, World.TileSize, World.TileSize), GetColor());
    }
}