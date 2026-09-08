using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        Remaining = remaining > 0 ? remaining : _random.Next(10, 1500);
    }

    public void Update(GameTime gameTime)
    {

    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(OreTileset.OreTextures, new Rectangle((int)position.X, (int)position.Y, World.TileSize, World.TileSize), OreTileset.GetSourceRect(Type, Remaining), Color.White);
    }
}