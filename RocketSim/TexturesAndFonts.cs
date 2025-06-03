using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RocketSim;

public class TexturesAndFonts
{
    public Texture2D EarthMapView { get; private set; }
    public Texture2D EarthSurface { get; private set; }
    public Texture2D Pixel { get; private set; }
    public Texture2D Rocket { get; private set; }
    public Texture2D RocketNoFire { get; private set; }
    public SpriteFont Font { get; private set; }
    public void LoadTextures(ContentManager content, GraphicsDevice graphicsDevice)
    {
        Rocket = content.Load<Texture2D>("rocket");
        RocketNoFire = content.Load<Texture2D>("rocketNoFire");
        EarthSurface = content.Load<Texture2D>("earthSurface");
        EarthMapView = content.Load<Texture2D>("earthMapView");
        Pixel = HelperMethods.CreatePixel(graphicsDevice);
        try
        {
            Font = content.Load<SpriteFont>("DefaultFont");
        }
        catch
        {
            Font = null;
        }

    }
}