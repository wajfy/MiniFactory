using System;
using MiniFactory.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Player
{
    public Vector2 Position { get; private set; }

    private readonly Texture2D _texture;
    private readonly float _speed = 200f;
    public int MiningRange = 200;
    public float MiningSpeed = 3f;
    public Inventory Inventory;

    public Player(Vector2 startPosition, Texture2D texture)
    {
        Position = startPosition;
        _texture = texture;
        Inventory = new Inventory(9, 3);
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        Vector2 direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W))
            direction.Y -= 1;
        if (keyboard.IsKeyDown(Keys.S))
            direction.Y += 1;
        if (keyboard.IsKeyDown(Keys.A))
            direction.X -= 1;
        if (keyboard.IsKeyDown(Keys.D))
            direction.X += 1;
            
        Position += direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, 32, 32), Color.White);
    }
}