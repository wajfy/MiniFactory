using System;
using MiniFactory.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

public class MoneyUI
{
    private UITheme theme;
    private const int numberSpacing = 10;
    private const int spaceAroundNumbers = 5;
    private const int borderOffset = 5;

    private float moneyTopLeftY;
    private int panelWidth;
    private int panelHeight;

    private string moneyDisplay;

    public MoneyUI(UITheme uiTheme)
    {
        theme = uiTheme;
    }
    public void Update(float money, float moneySlide)
    {
        moneyDisplay = money.ToString() + '$';
        Vector2 textSize = theme.FontMedium.MeasureString(moneyDisplay);
        panelWidth = numberSpacing;
        panelHeight = (int)textSize.Y + (2 * numberSpacing);
        foreach(char letter in moneyDisplay)
        {
            Vector2 letterSize = theme.FontMedium.MeasureString(letter.ToString());
            panelWidth += (int)letterSize.X + numberSpacing;
        }
        moneyTopLeftY = MathHelper.Lerp(-panelHeight, 0, moneySlide);
    }

    public void Draw(SpriteBatch spriteBatch, float moneySlide)
    {
        if (moneySlide == 0f) return;   

        spriteBatch.DrawRoundedRect(theme.Pixel, theme.CornerTexture, new Rectangle(0, (int)moneyTopLeftY + borderOffset, panelWidth + spaceAroundNumbers, panelHeight), theme.SlotBorder);
        spriteBatch.DrawRoundedRect(theme.Pixel, theme.CornerTexture, new Rectangle(0, (int)moneyTopLeftY, panelWidth + spaceAroundNumbers, panelHeight), theme.PanelBackground);
        
        float x = numberSpacing;
        for (int i = 0; i < moneyDisplay.Count(); i++)
        {
            char letter = moneyDisplay[i];
            Vector2 letterSize = theme.FontMedium.MeasureString(letter.ToString());
            Vector2 position = new Vector2(x, moneyTopLeftY + numberSpacing);

            if (letter != '.')
                spriteBatch.DrawRoundedRect(theme.Pixel, theme.CornerTexture, new Rectangle((int)position.X, (int)position.Y, (int)letterSize.X + spaceAroundNumbers, (int)letterSize.Y), theme.SlotColor);

            spriteBatch.DrawString(theme.FontMedium, letter.ToString(), new Vector2(position.X + (spaceAroundNumbers / 2), moneyTopLeftY + numberSpacing), theme.TextColor);
            x += letterSize.X + numberSpacing;
        }   
    }
}