using System;
using MiniFactory.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class GameMenu
{
    private bool menuOpen = false;
    private float menuSlide = 0f;
    private float slideSpeed = 4f;

    private KeyboardState previousKeyboard;

    private InventoryUI inventoryUI;
    private MoneyUI moneyUI;

    public GameMenu(UITheme uiTheme)
    {
        inventoryUI = new InventoryUI(uiTheme);
        moneyUI = new MoneyUI(uiTheme);
    }
    public void Update(GameTime gameTime, Player player, MouseState mouse, KeyboardState keyboard, Viewport viewport)
    {
        if (keyboard.IsKeyDown(Keys.Tab) && previousKeyboard.IsKeyUp(Keys.Tab))
        {
            menuOpen = !menuOpen;
        }
        if (menuOpen)
            menuSlide = Math.Min(1f, menuSlide + slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
        else
            menuSlide = Math.Max(0f, menuSlide - slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

        inventoryUI.Update(player.Inventory, mouse, viewport, menuSlide);
        moneyUI.Update(player.Money, menuSlide);

        previousKeyboard = keyboard;
    }

    public void Draw(SpriteBatch spriteBatch, Player player, Vector2 screenMousePos)
    {
        inventoryUI.Draw(spriteBatch, player.Inventory, screenMousePos, menuSlide);
        moneyUI.Draw(spriteBatch, menuSlide);
    }
}