using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class EditRocketPropertiesScreen(SpriteFont font, Texture2D pixel, RocketInitialProperties props, GraphicsDevice graphicsDevice, RocketSimGame rocketSimGame)
{
    private readonly List<(string Label, EditableField Field)> _fields =
    [
        ("Dry Mass", new EditableField(font, pixel, props.RocketDryMass, new Rectangle(100, 100, 200, 40))),
        ("Max Fuel", new EditableField(font, pixel, props.MaxFuel, new Rectangle(100, 160, 200, 40))),
        ("Thrust", new EditableField(font, pixel, props.ThrustPower, new Rectangle(100, 220, 200, 40))),
        ("Fuel Burn Rate", new EditableField(font, pixel, props.FuelBurnRate, new Rectangle(100, 280, 200, 40)))
    ];

    private KeyboardState _prevKeyboardState;

    public bool IsVisible { get; private set; }

    private Rectangle CommitChangesButton = new Rectangle(860, 500, 200, 50);
    private Rectangle ExitButton = new Rectangle(860, 700, 200, 50);

    private readonly Texture2D _buttonTexture = new Texture2D(graphicsDevice, 1, 1);

    public void Close()
    {
        IsVisible = false;
    }

    public void Open()
    {
        IsVisible = true;
    }

    public void Update()
    {
        var mouseState = Mouse.GetState();
        var keyboardState = Keyboard.GetState();

        //if esc is pressed, change IsVisible to false
        if (keyboardState.IsKeyDown(Keys.Escape) && _prevKeyboardState.IsKeyUp(Keys.Escape))
        {
            IsVisible = false;
            return;
        }

        // detect changes to each property 
        foreach (var (_, field) in _fields) field.Update(mouseState, keyboardState, _prevKeyboardState);
        props.SetRocketDryMass(_fields[0].Field.Value);
        props.SetMaxFuel(_fields[1].Field.Value); //maxFuel
        props.SetThrustPower(_fields[2].Field.Value); //Thrust
        props.SetFuelBurnRate(_fields[3].Field.Value); //FuelBurnRate

        // commit changes
        if (CommitChangesButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
            rocketSimGame.ResetRocket(props);

        // if exit button is clicked, close the screen
        if (ExitButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
        {
            IsVisible = false;
        }

        //save keyboardState
        _prevKeyboardState = keyboardState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {

        // Draw the commit button
        _buttonTexture.SetData([Color.White]);
        spriteBatch.Draw(_buttonTexture, CommitChangesButton, Color.Blue);
        spriteBatch.DrawString(font, "Save Changes", new Vector2(CommitChangesButton.X + 20, CommitChangesButton.Y + 15),
            Color.White);

        // Draw the exit button
        spriteBatch.Draw(_buttonTexture, ExitButton, Color.Red);
        spriteBatch.DrawString(font, "Exit", new Vector2(ExitButton.X + 20, ExitButton.Y + 15), Color.White);


        foreach (var (label, field) in _fields)
        {
            // Draw label
            var labelPos = new Vector2(field.Bounds.X - 90, field.Bounds.Y + 10);
            spriteBatch.DrawString(font, label + ":", labelPos, Color.White);

            // Draw field
            field.Draw(spriteBatch);
        }
    }
}

public class EditableField(SpriteFont font, Texture2D background, float initialValue, Rectangle bounds)
{
    public Rectangle Bounds = bounds;
    private string _inputBuffer = "";

    private bool _isEditing;
    public float Value = initialValue;

    public void Update(MouseState mouseState, KeyboardState keyboardState, KeyboardState prevKeyboardState)
    {
        // Start editing on click
        if (mouseState.LeftButton == ButtonState.Pressed && Bounds.Contains(mouseState.Position))
            if (!_isEditing)
            {
                _isEditing = true;
                _inputBuffer =
                    Value.ToString("0.###"); //inputBuffer = ""; // or use inputBuffer = Value.ToString("0.###");
            }

        if (_isEditing)
            foreach (var key in keyboardState.GetPressedKeys())
                if (!prevKeyboardState.IsKeyDown(key))
                {
                    switch (key)
                    {
                        case Keys.Enter:
                            if (float.TryParse(_inputBuffer, NumberStyles.Float, CultureInfo.InvariantCulture,
                                    out var result))
                                Value = result;
                            _isEditing = false;
                            break;
                        case Keys.Escape:
                            _isEditing = false;
                            break;
                        case Keys.Back when _inputBuffer.Length > 0:
                            _inputBuffer = _inputBuffer[..^1];
                            break;
                        default:
                            var c = HelperMethods.GetCharFromKey(key, keyboardState);
                            if (HelperMethods.IsValidFloatChar(c))
                            {
                                if (c == '.' && _inputBuffer.Contains('.')) break;
                                if (c == '-' && _inputBuffer.Length > 0) break;
                                _inputBuffer += c;
                            }
                            break;
                    }
                }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(background, Bounds, Color.DarkSlateGray);

        var textToDraw = _isEditing ? _inputBuffer + "|" : Value.ToString("0.###", CultureInfo.InvariantCulture);

        var textSize = font.MeasureString(textToDraw);
        var textPos = new Vector2(
            Bounds.X + (Bounds.Width - textSize.X) / 2,
            Bounds.Y + (Bounds.Height - textSize.Y) / 2
        );

        spriteBatch.DrawString(font, textToDraw, textPos, Color.White);
    }
}