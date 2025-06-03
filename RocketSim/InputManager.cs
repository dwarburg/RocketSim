using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class InputManager
{
    private double _escapeDebounceTime;
    private double _mDebounceTime;
    private readonly double _debounceDelay = 0.5;// Minimum delay (in seconds) between toggles  

    public void Update(GameTime gameTime)
    {
        _escapeDebounceTime -= gameTime.ElapsedGameTime.TotalSeconds;
        _mDebounceTime -= gameTime.ElapsedGameTime.TotalSeconds;
    }

    public bool IsEscapePressed(KeyboardState state)
    {
        if (state.IsKeyDown(Keys.Escape) && _escapeDebounceTime <= 0)
        {
            _escapeDebounceTime = _debounceDelay;
            return true;
        }
        return false;
    }

    public bool IsMPressed(KeyboardState state)
    {
        if (state.IsKeyDown(Keys.M) && _mDebounceTime <= 0)
        {
            _mDebounceTime = _debounceDelay;
            return true;
        }
        return false;
    }
}