using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class MenuScreen
{
    private SpriteFont _font;
    private Texture2D _buttonTexture;
    private Rectangle _exitButton, _resetButton, _startButton, _editPropertiesButton;
    private bool _isMenuActive;
    private Game _game; // Reference to the Game instance
    private bool _isEditingProperties = false; // Tracks if the Edit Properties screen is active


    public MenuScreen(SpriteFont font, GraphicsDevice graphicsDevice, Game game)
    {
        _font = font;
        _game = game;

        // Create a simple button texture
        _buttonTexture = new Texture2D(graphicsDevice, 1, 1);
        _buttonTexture.SetData(new[] { Color.White });

        // Define button positions and sizes
        _exitButton = new Rectangle(860, 400, 200, 50); // Centered on screen
        _resetButton = new Rectangle(860, 500, 200, 50); // Below the exit button
        _startButton = new Rectangle(860, 600, 200, 50);
        _editPropertiesButton = new Rectangle(860, 700, 200, 50); 

        _isMenuActive = true; // Start with the menu active
    }

    public bool IsMenuActive => _isMenuActive;

    public void ToggleMenu()
    {
        _isMenuActive = !_isMenuActive;
    }

    public void Update(Game game, RocketCurrentState rocketState, Vector2 initialRocketPosition, RocketInitialProperties rocketInitialProperties)
    {
        if (!_isMenuActive && !_isEditingProperties) return;

        var mouseState = Mouse.GetState();

        if (_isEditingProperties)
        {
            // Handle input for editing properties (e.g., sliders or buttons)
            HandleEditPropertiesInput(rocketInitialProperties);
            return;
        }

        // Check if the exit button is clicked
        if (_exitButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
        {
            game.Exit();
        }

        // Check if the reset button is clicked
        if (_resetButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
        {
            rocketState.ResetToInitialPosition(initialRocketPosition);
        }

        // Check if the start button is clicked
        if (_startButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
        {
            // Start the simulation
            _isMenuActive = false;
            game.IsMouseVisible = false; // Hide the mouse cursor
        }

        // Check if the edit properties button is clicked
        if (_editPropertiesButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
        {
            // Open the properties editing interface
            _isEditingProperties = true;
        }
    }

    private void HandleEditPropertiesInput(RocketInitialProperties rocketInitialProperties)
    {
        var keyboardState = Keyboard.GetState();

        // Example: Adjust ThrustPower using arrow keys
        
        if (keyboardState.IsKeyDown(Keys.Up))
            rocketInitialProperties.SetThrustPower(rocketInitialProperties.ThrustPower + 100f);
        if (keyboardState.IsKeyDown(Keys.Down))
            rocketInitialProperties.SetThrustPower(rocketInitialProperties.ThrustPower - 100f);


        // Close the Edit Properties screen with the Escape key
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            _isEditingProperties = false;
        }
    }



    public void Draw(SpriteBatch spriteBatch, RocketInitialProperties rocketInitialProperties)
    {
        if (!_isMenuActive && !_isEditingProperties) return;

        if (_isEditingProperties)
        {
            // Draw the Edit Properties screen
            DrawEditPropertiesScreen(spriteBatch, rocketInitialProperties);
            return;
        }


        // Make the mouse cursor visible
        _game.IsMouseVisible = true;

        // Draw the exit button
        spriteBatch.Draw(_buttonTexture, _exitButton, Color.Red);
        spriteBatch.DrawString(_font, "Exit", new Vector2(_exitButton.X + 60, _exitButton.Y + 15), Color.White);

        // Draw the reset button
        spriteBatch.Draw(_buttonTexture, _resetButton, Color.Blue);
        spriteBatch.DrawString(_font, "Reset Rocket", new Vector2(_resetButton.X + 20, _resetButton.Y + 15), Color.White);

        // Draw the start button
        spriteBatch.Draw(_buttonTexture, _startButton, Color.Green);
        spriteBatch.DrawString(_font, "Start Simulation", new Vector2(_startButton.X + 20, _startButton.Y + 15), Color.White);

        // Draw the edit properties button
        spriteBatch.Draw(_buttonTexture, _editPropertiesButton, Color.Yellow);
        spriteBatch.DrawString(_font, "Edit Properties", new Vector2(_editPropertiesButton.X + 20, _editPropertiesButton.Y + 15), Color.Black);
    }

    private void DrawEditPropertiesScreen(SpriteBatch spriteBatch, RocketInitialProperties rocketInitialProperties)
    {
        // Draw a background for the Edit Properties screen
        var backgroundRect = new Rectangle(400, 200, 1120, 600);
        spriteBatch.Draw(_buttonTexture, backgroundRect, Color.Gray);

        // Draw labels and values for each property
        spriteBatch.DrawString(_font, "Edit Rocket Properties", new Vector2(600, 220), Color.White);

        var yOffset = 300;
        rocketInitialProperties.ThrustPower.ToString("F1");
        spriteBatch.DrawString(_font, $"Thrust Power: {rocketInitialProperties.ThrustPower:F1}", new Vector2(450, yOffset), Color.White);
        spriteBatch.DrawString(_font, "Use Up/Down to adjust", new Vector2(800, yOffset), Color.White);
        yOffset += 50;

        spriteBatch.DrawString(_font, $"maxFuel: {rocketInitialProperties.MaxFuel:F1}", new Vector2(450, yOffset), Color.White);
        yOffset += 50;

        spriteBatch.DrawString(_font, $"maxFuel Burn Rate: {rocketInitialProperties.FuelBurnRate:F1}", new Vector2(450, yOffset), Color.White);
        yOffset += 50;

        spriteBatch.DrawString(_font, $"Rocket Mass: {rocketInitialProperties.RocketMass:F1}", new Vector2(450, yOffset), Color.White);
        yOffset += 50;

        spriteBatch.DrawString(_font, "Press Escape to return to the menu", new Vector2(600, 700), Color.White);
    }
}
