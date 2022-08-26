using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class MouseState
{
    public MouseState(
        Vector2D<float> position,
        bool isRelative,
        Vector2D<float> scrollWheel,
        IReadOnlyDictionary<MouseButton, bool> previousButtonStates,
        IReadOnlyDictionary<MouseButton, bool> currentButtonStates)
    {
        Position = position;
        IsRelative = isRelative;
        ScrollWheel = scrollWheel;
        PreviousButtonStates = previousButtonStates;
        CurrentButtonStates = currentButtonStates;
    }

    public Vector2D<float> Position { get; }

    public bool IsRelative { get; }

    public Vector2D<float> ScrollWheel { get; }

    public IReadOnlyDictionary<MouseButton, bool> PreviousButtonStates { get; }

    public IReadOnlyDictionary<MouseButton, bool> CurrentButtonStates { get; }
    
    /// <summary>
    /// Get just the boolean true/false value of key
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public bool GetButtonValue(MouseButton button)
    {
        return CurrentButtonStates[button];
    }

    /// <summary>
    /// Get a state based on current and previous frame
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public ButtonState GetButtonState(MouseButton button)
    {
        if (PreviousButtonStates[button] == false)
        {
            if (CurrentButtonStates[button] == false)
            {
                return ButtonState.None;
            }
            else
            {
                return ButtonState.Pressed;
            }
        }
        else // Prev state must be 1
        {
            if (CurrentButtonStates[button] == false)
            {
                return ButtonState.Released;
            }
            else
            {
                return ButtonState.Held;
            }
        }
    }
}