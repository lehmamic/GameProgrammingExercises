using Silk.NET.Input;

namespace GameProgrammingExercises;

public class KeyboardState
{
    public KeyboardState(IReadOnlyDictionary<Key, bool> previousKeyStates, IReadOnlyDictionary<Key, bool> currentKeyStates)
    {
        PreviousKeyStates = previousKeyStates;
        CurrentKeyStates = currentKeyStates;
    }

    public IReadOnlyDictionary<Key, bool> PreviousKeyStates { get; }

    public IReadOnlyDictionary<Key, bool> CurrentKeyStates { get; }
    
    /// <summary>
    /// Get just the boolean true/false value of key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool GetKeyValue(Key key)
    {
        return CurrentKeyStates[key];
    }

    /// <summary>
    /// Get a state based on current and previous frame
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ButtonState GetKeyState(Key key)
    {
        if (PreviousKeyStates[key] == false)
        {
            if (CurrentKeyStates[key] == false)
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
            if (CurrentKeyStates[key] == false)
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