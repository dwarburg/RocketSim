using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Globalization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RocketSim
{
    public class EditRocketPropertiesScreen
    {
        private SpriteFont font;
        private Texture2D pixel;

        private List<(string Label, EditableField Field)> fields;
        private RocketInitialProperties rocketProps;

        private KeyboardState prevKeyboardState;

        public bool IsVisible { get; set; } = false;

        public EditRocketPropertiesScreen(SpriteFont font, Texture2D pixel, RocketInitialProperties props)
        {
            this.font = font;
            this.pixel = pixel;
            this.rocketProps = props;


            fields =
        [
            ("Dry Mass",      new EditableField(font, pixel, props.RocketDryMass,      new Rectangle(100, 100, 200, 40))),
            ("Max Fuel",      new EditableField(font, pixel, props.MaxFuel,      new Rectangle(100, 160, 200, 40))),
            ("Thrust",        new EditableField(font, pixel, props.ThrustPower,       new Rectangle(100, 220, 200, 40))),
            ("Fuel Burn Rate",new EditableField(font, pixel, props.FuelBurnRate, new Rectangle(100, 280, 200, 40)))
        ];
        }

        public void Update()
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            //if esc is presed, change IsVisible to false
            if (keyboard.IsKeyDown(Keys.Escape) && prevKeyboardState.IsKeyUp(Keys.Escape))
            {
                IsVisible = false;
                return;
            }

            foreach (var (_, field) in fields)
            {
                field.Update(mouse, keyboard, prevKeyboardState);
            }

            // Sync back to rocketProps after editing
            rocketProps.SetRocketDryMass(fields[0].Field.Value);
            rocketProps.SetMaxFuel(fields[1].Field.Value); //maxFuel
            rocketProps.SetThrustPower(fields[2].Field.Value); //Thrust
            rocketProps.SetFuelBurnRate(fields[3].Field.Value); //FuelBurnRate

            prevKeyboardState = keyboard;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var (label, field) in fields)
            {
                // Draw label
                Vector2 labelPos = new Vector2(field.Bounds.X - 90, field.Bounds.Y + 10);
                spriteBatch.DrawString(font, label + ":", labelPos, Color.White);

                // Draw field
                field.Draw(spriteBatch);
            }
        }
    }




    public class EditableField
    {
        public Rectangle Bounds;
        public float Value;

        private bool isEditing = false;
        private string inputBuffer = "";

        private SpriteFont font;
        private Texture2D background;

        public EditableField(SpriteFont font, Texture2D background, float initialValue, Rectangle bounds)
        {
            this.font = font;
            this.background = background;
            this.Value = initialValue;
            this.Bounds = bounds;
        }

        public void Update(MouseState mouseState, KeyboardState keyboardState, KeyboardState prevKeyboardState)
        {
            // Start editing on click
            if (mouseState.LeftButton == ButtonState.Pressed && Bounds.Contains(mouseState.Position))
            {
                if (!isEditing)
                {
                    isEditing = true;
                    inputBuffer = Value.ToString("0.###"); //inputBuffer = ""; // or use inputBuffer = Value.ToString("0.###");
                }
            }

            if (isEditing)
            {
                foreach (var key in keyboardState.GetPressedKeys())
                {
                    if (!prevKeyboardState.IsKeyDown(key))
                    {
                        if (key == Keys.Enter)
                        {
                            if (float.TryParse(inputBuffer, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                                Value = result;
                            isEditing = false;
                        }
                        else if (key == Keys.Escape)
                        {
                            isEditing = false;
                        }
                        else if (key == Keys.Back && inputBuffer.Length > 0)
                        {
                            inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 1);
                        }
                        else
                        {
                            char c = HelperMethods.GetCharFromKey(key, keyboardState);
                            if (HelperMethods.IsValidFloatChar(c))
                            {
                                if (c == '.' && inputBuffer.Contains('.')) continue;
                                if (c == '-' && inputBuffer.Length > 0) continue;
                                inputBuffer += c;
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Bounds, Color.DarkSlateGray);

            string textToDraw = isEditing ? inputBuffer + "|" : Value.ToString("0.###", CultureInfo.InvariantCulture);

            Vector2 textSize = font.MeasureString(textToDraw);
            Vector2 textPos = new Vector2(
                Bounds.X + (Bounds.Width - textSize.X) / 2,
                Bounds.Y + (Bounds.Height - textSize.Y) / 2
            );

            spriteBatch.DrawString(font, textToDraw, textPos, Color.White);
        }

    }

}
