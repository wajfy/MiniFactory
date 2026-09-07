using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Camera
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; }

    public Camera (Vector2 position, float zoom)
    {
        Position = position;
        Zoom = zoom;
    }

    public Matrix GetTransformMatrix(Viewport viewport)
    {
        return Matrix.CreateTranslation(-Position.X, -Position.Y, 0f)
             * Matrix.CreateScale(Zoom)
             * Matrix.CreateTranslation(viewport.Width / 2, viewport.Height / 2, 0f);
    }
}