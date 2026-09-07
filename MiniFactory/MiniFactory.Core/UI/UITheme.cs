using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class UITheme
{
    public Texture2D Pixel;
    public Texture2D CornerTexture;
    public SpriteFont FontSmall;
    public SpriteFont FontMedium;
    public SpriteFont FontLarge;
    public Color PanelBackground = new Color(27, 38, 59);    // tmavě námořnická modrá — pozadí panelu
    public Color SlotColor = new Color(42, 63, 90);    // střední modrá — prázdný slot
    public Color SlotBorder = new Color(62, 92, 118);   // světlejší modrá — okraj slotu / hover
    public Color TextColor = new Color(232, 236, 239); // téměř bílá — text, kontrast na tmavém pozadí
    public Color Accent = new Color(255, 190, 60);  // teplá zlatá/oranžová — zvýraznění (progress bar, vybraný slot)

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
    {
            Pixel = new Texture2D(graphicsDevice, 1, 1);
            Pixel.SetData([Color.White]);
            CornerTexture = content.Load<Texture2D>("square");

            FontSmall = content.Load<SpriteFont>("Fonts/HudSmall");
            FontMedium = content.Load<SpriteFont>("Fonts/HudMedium");
            FontLarge = content.Load<SpriteFont>("Fonts/Hud");
    }
}