using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class MenuScreen
{
    private readonly Texture2D _buttonTexture;
    private readonly EditRocketPropertiesScreen _editRocketPropertiesScreen; // Reference to the Edit Properties screen
    private readonly SpriteFont _font;
    private readonly RocketSimGame _rocketSimGame; // Reference to the RocketSimGame instance
    private Rectangle _exitButton, _resetButton, _startButton, _editPropertiesButton;
    private bool _waitForMouseRelease = false;

    public MenuScreen(SpriteFont font, GraphicsDevice graphicsDevice,
        EditRocketPropertiesScreen editRocketPropertiesScreen, RocketSimGame rocketSimGame)
    {
        _font = font;
        _editRocketPropertiesScreen = editRocketPropertiesScreen;

        // Create a simple button texture
        _buttonTexture = new Texture2D(graphicsDevice, 1, 1);
        _buttonTexture.SetData([Color.White]);

        // Define button positions and sizes

        _startButton = new Rectangle(860, 400, 200, 50);
        _resetButton = new Rectangle(860, 500, 200, 50);
        _editPropertiesButton = new Rectangle(860, 600, 200, 50);
        _exitButton = new Rectangle(860, 700, 200, 50);

        IsMenuActive = false; // Start with the menu inactive
        _rocketSimGame = rocketSimGame;
    }

    public bool IsMenuActive { get; private set; }

    public void OpenMenu()
    {
        IsMenuActive = true;
    }

    public void CloseCurrentMenu()
    {
        if (_editRocketPropertiesScreen.IsVisible)
            _editRocketPropertiesScreen.Close();
        else
            IsMenuActive = false;
    }

    public void Update(RocketCurrentState rocketState, Vector2 initialRocketPosition,
    RocketInitialProperties rocketInitialProperties)
    {
        if (!IsMenuActive && !_editRocketPropertiesScreen.IsVisible) return;

        var mouseState = Mouse.GetState();

        // If edit screen is visible, update it and set wait flag if it just closed
        if (_editRocketPropertiesScreen.IsVisible)
        {
            _editRocketPropertiesScreen.Update();
            _waitForMouseRelease = true; // Always set while editing
            return;
        }

        // If we are waiting for mouse release, only proceed when button is released
        if (_waitForMouseRelease)
        {
            if (mouseState.LeftButton == ButtonState.Released)
                _waitForMouseRelease = false;
            else
                return;
        }

        // respond to clicks on each button
        if (_exitButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
            _rocketSimGame.Exit();

        if (_resetButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
            _rocketSimGame.ResetRocket(rocketInitialProperties);

        if (_startButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
        {
            IsMenuActive = false;
            _rocketSimGame.IsMouseVisible = false;
        }

        if (_editPropertiesButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
            _editRocketPropertiesScreen.Open();
    }

    public void Draw(SpriteBatch spriteBatch, RocketInitialProperties rocketInitialProperties)
    {
        if (!IsMenuActive && !_editRocketPropertiesScreen.IsVisible) return;

        if (_editRocketPropertiesScreen.IsVisible)
        {
            // Draw the Edit Properties screen
            _editRocketPropertiesScreen.Draw(spriteBatch);
            return;
        }

        // Make the mouse cursor visible
        _rocketSimGame.IsMouseVisible = true;

        // Draw the start button
        spriteBatch.Draw(_buttonTexture, _startButton, Color.Green);
        spriteBatch.DrawString(_font, "Start", new Vector2(_startButton.X + 20, _startButton.Y + 15),
            Color.White);

        // Draw the reset button
        spriteBatch.Draw(_buttonTexture, _resetButton, Color.Blue);
        spriteBatch.DrawString(_font, "Reset Position", new Vector2(_resetButton.X + 20, _resetButton.Y + 15),
            Color.White);

        // Draw the edit properties button
        spriteBatch.Draw(_buttonTexture, _editPropertiesButton, Color.Yellow);
        spriteBatch.DrawString(_font, "Edit Properties",
            new Vector2(_editPropertiesButton.X + 20, _editPropertiesButton.Y + 15), Color.Black);

        // Draw the exit button
        spriteBatch.Draw(_buttonTexture, _exitButton, Color.Red);
        spriteBatch.DrawString(_font, "Exit", new Vector2(_exitButton.X + 60, _exitButton.Y + 15), Color.White);
    }
}