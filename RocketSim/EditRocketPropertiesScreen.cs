using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketSim;

public class EditRocketPropertiesScreen
{
    private readonly List<(string Label, EditableField Field)> _fields;
    private readonly SpriteFont _font;
    private readonly RocketInitialProperties _rocketProps;
    private Texture2D _pixel;

    private KeyboardState _prevKeyboardState;

    public EditRocketPropertiesScreen(SpriteFont font, Texture2D pixel, RocketInitialProperties props)
    {
        _font = font;
        _pixel = pixel;
        _rocketProps = props;


        _fields =
        [
            ("Dry Mass", new EditableField(font, pixel, props.RocketDryMass, new Rectangle(100, 100, 200, 40))),
            ("Max Fuel", new EditableField(font, pixel, props.MaxFuel, new Rectangle(100, 160, 200, 40))),
            ("Thrust", new EditableField(font, pixel, props.ThrustPower, new Rectangle(100, 220, 200, 40))),
            ("Fuel Burn Rate", new EditableField(font, pixel, props.FuelBurnRate, new Rectangle(100, 280, 200, 40)))
        ];
    }

    public bool IsVisible { get; set; }

    public void Update()
    {
        var mouse = Mouse.GetState();
        var keyboard = Keyboard.GetState();

        //if esc is pressed, change IsVisible to false
        if (keyboard.IsKeyDown(Keys.Escape) && _prevKeyboardState.IsKeyUp(Keys.Escape))
        {
            IsVisible = false;
            return;
        }

        foreach (var (_, field) in _fields) field.Update(mouse, keyboard, _prevKeyboardState);

        // Sync back to _rocketProps after editing
        _rocketProps.SetRocketDryMass(_fields[0].Field.Value);
        _rocketProps.SetMaxFuel(_fields[1].Field.Value); //maxFuel
        _rocketProps.SetThrustPower(_fields[2].Field.Value); //Thrust
        _rocketProps.SetFuelBurnRate(_fields[3].Field.Value); //FuelBurnRate

        _prevKeyboardState = keyboard;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var (label, field) in _fields)
        {
            // Draw label
            var labelPos = new Vector2(field.Bounds.X - 90, field.Bounds.Y + 10);
            spriteBatch.DrawString(_font, label + ":", labelPos, Color.White);

            // Draw field
            field.Draw(spriteBatch);
        }
    }
}

public class EditableField
{
    private readonly Texture2D _background;

    private readonly SpriteFont _font;
    public Rectangle Bounds;
    private string _inputBuffer = "";

    private bool _isEditing;
    public float Value;

    public EditableField(SpriteFont font, Texture2D background, float initialValue, Rectangle bounds)
    {
        _font = font;
        _background = background;
        Value = initialValue;
        Bounds = bounds;
    }

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
                    if (key == Keys.Enter)
                    {
                        if (float.TryParse(_inputBuffer, NumberStyles.Float, CultureInfo.InvariantCulture,
                                out var result))
                            Value = result;
                        _isEditing = false;
                    }
                    else if (key == Keys.Escape)
                    {
                        _isEditing = false;
                    }
                    else if (key == Keys.Back && _inputBuffer.Length > 0)
                    {
                        _inputBuffer = _inputBuffer.Substring(0, _inputBuffer.Length - 1);
                    }
                    else
                    {
                        var c = HelperMethods.GetCharFromKey(key, keyboardState);
                        if (HelperMethods.IsValidFloatChar(c))
                        {
                            if (c == '.' && _inputBuffer.Contains('.')) continue;
                            if (c == '-' && _inputBuffer.Length > 0) continue;
                            _inputBuffer += c;
                        }
                    }
                }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_background, Bounds, Color.DarkSlateGray);

        var textToDraw = _isEditing ? _inputBuffer + "|" : Value.ToString("0.###", CultureInfo.InvariantCulture);

        var textSize = _font.MeasureString(textToDraw);
        var textPos = new Vector2(
            Bounds.X + (Bounds.Width - textSize.X) / 2,
            Bounds.Y + (Bounds.Height - textSize.Y) / 2
        );

        spriteBatch.DrawString(_font, textToDraw, textPos, Color.White);
    }
}