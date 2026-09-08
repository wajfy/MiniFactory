using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class InventoryUI
{
    private bool inventoryOpen = false;

    private const int slotSize = 36;
    private const int slotSpacing = 8;
    private const int borderOffset = 5;
    private int? draggingRow = null;
    private int? draggingCol = null;
    private float inventorySlide = 0f;
    private float slideSpeed = 4f;
    private Point inventoryTopLeft;
    private int panelWidth;
    private int panelHeight;

    private MouseState previousMouse;
    private KeyboardState previousKeyboard;

    private UITheme theme;

    public InventoryUI(UITheme theme)
    {
        this.theme = theme;
    }

    public void Update(GameTime gameTime, Inventory inventory, MouseState mouse, KeyboardState keyboard, Viewport viewport)
    {
        if (keyboard.IsKeyDown(Keys.Tab) && previousKeyboard.IsKeyUp(Keys.Tab))
        {
            inventoryOpen = !inventoryOpen;
        }
        if (inventoryOpen)
            inventorySlide = Math.Min(1f, inventorySlide + slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
        else
            inventorySlide = Math.Max(0f, inventorySlide - slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

        int rows = inventory.Slots.GetLength(0);
        int columns = inventory.Slots.GetLength(1);

        panelWidth = columns * slotSize + (columns + 1) * slotSpacing;
        panelHeight = rows * slotSize + (rows + 1) * slotSpacing;

        float inventoryTopLeftY = MathHelper.Lerp(viewport.Height, viewport.Height - panelHeight, inventorySlide);
        inventoryTopLeft = new Point((viewport.Width / 2) - (panelWidth / 2), (int)inventoryTopLeftY);

        bool mouseButtonPressed = mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released;
        bool mouseButtonReleased = mouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed;
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Rectangle slotRectangle = new Rectangle(inventoryTopLeft.X + slotSpacing + i * (slotSize + slotSpacing), inventoryTopLeft.Y + slotSpacing + j * (slotSize + slotSpacing), slotSize, slotSize);

                if (slotRectangle.Contains(mouse.Position) && mouseButtonPressed)
                {
                    if (inventory.Slots[j, i].Count > 0)
                    {
                        draggingRow = j;
                        draggingCol = i;
                    }
                }
                if (slotRectangle.Contains(mouse.Position) && mouseButtonReleased)
                {
                    if (draggingRow.HasValue && draggingCol.HasValue)
                        inventory.MoveItem((int)draggingRow, (int)draggingCol, j, i);
                }
            }
        }
        if (mouseButtonReleased)
        {
            draggingRow = null;
            draggingCol = null;
        }
        previousKeyboard = keyboard;
        previousMouse = mouse;
    }

    public void Draw(SpriteBatch spriteBatch, Inventory inventory, Vector2 currentMousePos)
    {
        if (inventorySlide > 0f)
        {
            spriteBatch.DrawRoundedRect(theme.Pixel, theme.CornerTexture, new Rectangle(inventoryTopLeft.X, inventoryTopLeft.Y - borderOffset, panelWidth, panelHeight), theme.SlotBorder);
            spriteBatch.DrawRoundedRect(theme.Pixel, theme.CornerTexture, new Rectangle(inventoryTopLeft.X, inventoryTopLeft.Y, panelWidth, panelHeight), theme.PanelBackground);

            ItemDefinition hoveredItem = null;
            for (int i = 0; i < inventory.Slots.GetLength(1); i++)
            {
                for (int j = 0; j < inventory.Slots.GetLength(0); j++)
                {
                    InventorySlot slot = inventory.Slots[j, i];
                    Rectangle slotRectangle = new Rectangle(inventoryTopLeft.X + slotSpacing + i * (slotSize + slotSpacing), inventoryTopLeft.Y + slotSpacing + j * (slotSize + slotSpacing), slotSize, slotSize);
                    
                    Rectangle slotBorderRectangle = slotRectangle;
                    slotBorderRectangle.Y -= borderOffset;
                    spriteBatch.DrawRoundedRect(theme.Pixel, theme.CornerTexture, slotBorderRectangle, theme.SlotBorder);
                    spriteBatch.DrawRoundedRect(theme.Pixel, theme.CornerTexture, slotRectangle, theme.SlotColor);
                    if (slot.Count > 0 && !(draggingRow == j && draggingCol == i))
                    {
                        Texture2D itemTexture = ItemDatabase.Get(slot.Type).ItemTexture;
                        Vector2 textSize = theme.FontSmall.MeasureString(slot.Count.ToString());
                        float textY = slotRectangle.Bottom - textSize.Y;
                        spriteBatch.Draw(itemTexture, slotRectangle, Color.White);
                        spriteBatch.DrawString(theme.FontSmall, slot.Count.ToString(), new Vector2(slotRectangle.X, (int)textY), theme.TextColor);
                    }

                    if (slotRectangle.Contains(currentMousePos) && slot.Count > 0)
                        hoveredItem = ItemDatabase.Get(slot.Type);
                }
            }
            if (hoveredItem != null)
            {
                Vector2 boxTextSize = theme.FontSmall.MeasureString(hoveredItem.Name);
                spriteBatch.Draw(theme.Pixel, new Rectangle((int)currentMousePos.X - ((int)boxTextSize.X / 2), (int)currentMousePos.Y, (int)boxTextSize.X, (int)boxTextSize.Y), new Color(0, 0, 0, 180));
                spriteBatch.DrawString(theme.FontSmall, hoveredItem.Name, new Vector2((int)currentMousePos.X - ((int)boxTextSize.X / 2), (int)currentMousePos.Y), theme.TextColor);
            }
            if (draggingRow.HasValue && draggingCol.HasValue)
            {
                spriteBatch.Draw(ItemDatabase.Get(inventory.Slots[(int)draggingRow, (int)draggingCol].Type).ItemTexture, new Rectangle((int)currentMousePos.X - (slotSize / 2), (int)currentMousePos.Y - (slotSize / 2), slotSize, slotSize), new Color(255, 255, 255, 180));
            }
        }
    }
}