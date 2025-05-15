using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class MenuScreen
{
    private SpriteFont _font;
    private Texture2D _buttonTexture;
    private Rectangle _exitButtonRect;
    private Rectangle _resetButtonRect;
    private Rectangle _startButtonRect;
    private bool _isMenuActive;
    private Game _game; // Reference to the Game instance

    public MenuScreen(SpriteFont font, GraphicsDevice graphicsDevice, Game game)
    {
        _font = font;
        _game = game;

        // Create a simple button texture
        _buttonTexture = new Texture2D(graphicsDevice, 1, 1);
        _buttonTexture.SetData(new[] { Color.White });

        // Define button positions and sizes
        _exitButtonRect = new Rectangle(860, 400, 200, 50); // Centered on screen
        _resetButtonRect = new Rectangle(860, 500, 200, 50); // Below the exit button
        _startButtonRect = new Rectangle(860, 600, 200, 50); 

        _isMenuActive = true; // Start with the menu active
    }

    public bool IsMenuActive => _isMenuActive;

    public void ToggleMenu()
    {
        _isMenuActive = !_isMenuActive;
    }

    public void Update(Game game, RocketCurrentState rocketState, Vector2 initialRocketPosition)
    {
        if (!_isMenuActive) return;

        var mouseState = Mouse.GetState();

        // Check if the exit button is clicked
        if (_exitButtonRect.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
        {
            game.Exit();
        }

        // Check if the reset button is clicked
        if (_resetButtonRect.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
        {
            rocketState.ResetToInitialPosition(initialRocketPosition);
        }

        // Check if the start button is clicked
        if (_startButtonRect.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
        {
            // Start the simulation
            _isMenuActive = false;
            game.IsMouseVisible = false; // Hide the mouse cursor
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_isMenuActive) return;

        // Make the mouse cursor visible
        _game.IsMouseVisible = true;

        // Draw the exit button
        spriteBatch.Draw(_buttonTexture, _exitButtonRect, Color.Red);
        spriteBatch.DrawString(_font, "Exit", new Vector2(_exitButtonRect.X + 60, _exitButtonRect.Y + 15), Color.White);

        // Draw the reset button
        spriteBatch.Draw(_buttonTexture, _resetButtonRect, Color.Blue);
        spriteBatch.DrawString(_font, "Reset Rocket", new Vector2(_resetButtonRect.X + 20, _resetButtonRect.Y + 15), Color.White);

        // Draw the start button
        spriteBatch.Draw(_buttonTexture, _startButtonRect, Color.Green);
        spriteBatch.DrawString(_font, "Start Simulation", new Vector2(_startButtonRect.X + 20, _startButtonRect.Y + 15), Color.White);
    }
}
