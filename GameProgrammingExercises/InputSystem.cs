using GameProgrammingExercises.Maths;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public sealed class InputSystem : IDisposable
{
    private readonly Game _game;
    private IInputContext _input;
    private IKeyboard _primaryKeyboard;
    private IMouse _primaryMouse;

    public InputSystem(Game game)
    {
        _game = game;

        var keyboardState = new KeyboardState(new Dictionary<Key, bool>(),new Dictionary<Key, bool>());
        var mouseState = new MouseState(
            Vector2D<float>.Zero,
            Vector2D<float>.Zero,
            false,
            Vector2D<float>.Zero,
            new Dictionary<MouseButton, bool>(),
            new Dictionary<MouseButton, bool>());

        State = new InputState(keyboardState, mouseState);
    }

    public InputState State { get; private set; }

    public void Initialize()
    {
        _input = _game.Window.CreateInput();

        // -------- Keyboard --------
        _primaryKeyboard = _input.Keyboards.First();
        var keyboardState = new KeyboardState(
            _primaryKeyboard.SupportedKeys.ToDictionary(k => k, _ => false),
            _primaryKeyboard.SupportedKeys.ToDictionary(k => k, _ => false));

        // -------- Mouse (just set everything to 0) --------
        _primaryMouse = _input.Mice.First();
        var mouseState = new MouseState(
            _primaryMouse.Position.ToGeneric(),
            Vector2D<float>.Zero,
            false,
            _primaryMouse.ScrollWheels.First().ToVector2D(),
            _primaryMouse.SupportedButtons.ToDictionary(k => k, _ => false),
            _primaryMouse.SupportedButtons.ToDictionary(k => k, _ => false));

        State = new InputState(keyboardState, mouseState);
    }

    public void SetRelativeMouseMode(bool value)
    {
        _primaryMouse.Cursor.CursorMode = CursorMode.Disabled;

        var mouseState = new MouseState(
            State.Mouse.Position,
            Vector2D<float>.Zero,
            value,
            State.Mouse.ScrollWheel,
            State.Mouse.PreviousButtonStates,
            State.Mouse.CurrentButtonStates);

        State = new InputState(State.Keyboard, mouseState);
    }

    // Called right after SDL_PollEvents loop
    public void Update()
    {
        // Update Keyboard State
        var previousKeyStates = State.Keyboard.CurrentKeyStates;
        var currentKeyStates = new Dictionary<Key, bool>();

        foreach (var key in _primaryKeyboard.SupportedKeys)
        {
            currentKeyStates[key] = _primaryKeyboard.IsKeyPressed(key);
        }

        var currentKeyboardState = new KeyboardState(previousKeyStates, currentKeyStates);

        // Update Mouse State
        var previousMouseButtonStates = State.Mouse.CurrentButtonStates;
        var currentMouseButtonStates = new Dictionary<MouseButton, bool>();

        foreach (var button in _primaryMouse.SupportedButtons)
        {
            currentMouseButtonStates[button] = _primaryMouse.IsButtonPressed(button);
        }

        var currentPosition = _primaryMouse.Position.ToGeneric();
        var relativePosition = currentPosition - State.Mouse.Position;

        var currentMouseState = new MouseState(
            currentPosition,
            relativePosition,
            State.Mouse.IsRelative,
            _primaryMouse.ScrollWheels.First().ToVector2D(),
            previousMouseButtonStates,
            currentMouseButtonStates);

        State = new InputState(currentKeyboardState, currentMouseState);
    }

    public void Dispose()
    {
        _input.Dispose();
    }
}