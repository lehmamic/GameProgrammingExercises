using GameProgrammingExercises.Maths;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class UIScreen : IDisposable
{
    private readonly Game _game;
    private readonly Texture _buttonOn;
    private readonly Texture _buttonOff;

    // State
    private UIScreenState _state;

    // Configure positions

    private readonly List<Button> _buttons = new();
    private Vector2D<float> _nextButtonPos = new(0.0f, 200.0f);

    public UIScreen(Game game)
    {
        _game = game;
        
        // Add to UI Stack
        Game.PushUI(this);
        Font = Game.GetFont("Assets/Carlito-Regular.ttf");
        _buttonOn = Game.Renderer.GetTexture("Assets/ButtonYellow.png");
        _buttonOff = Game.Renderer.GetTexture("Assets/ButtonBlue.png");
    }


    public Game Game => _game;

    public UIScreenState State => _state;

    public Font Font { get; }

    protected string? Title { get; set; }

    protected Vector2D<float> TitlePos { get; set; } = new(0.0f, 300.0f);

    protected float TitleScale { get; set; } = 1.0f;

    protected Vector3D<float> TitleColor { get; set; } = Color.White;

    protected Texture? Background { get; set; }

    protected Vector2D<float> BgPos { get; set; } = new(0.0f, 250.0f);

    protected Vector2D<float> NextButtonPos
    {
        get => _nextButtonPos;
        set => _nextButtonPos = value;
    }

    public virtual void Update(float deltaTime)
    {
    }

    public virtual void Draw(Shader shader)
    {
        // Draw background (if exists)
        if (Background is not null)
        {
            Game.Renderer.DrawTexture(Background, BgPos);
        }
        // Draw title (if exists)
        if (Title is not null)
        {
            Game.Renderer.DrawText(Font, Title, TitlePos, TitleScale, TitleColor);
        }
        // Draw buttons
        foreach (var b in _buttons)
        {
            // Draw background of button
            Texture tex = b.Highlighted ? _buttonOn : _buttonOff;
            _game.Renderer.DrawTexture(tex, b.Position);

            // Draw text of button
            Game.Renderer.DrawText(Font, b.Name, b.Position, 0.5f, Color.White);
        }
        // Override in subclasses to draw any textures
    }

    public virtual void ProcessInput(InputState state)
    {
        // Do we have buttons?
        if (_buttons.Any())
        {
            // Get position of mouse
            float x, y;
            x = state.Mouse.Position.X;
            y = state.Mouse.Position.Y;

            // Convert to (0,0) center coordinates
            var mousePos = new Vector2D<float>(x, y);
            mousePos.X -= Game.Renderer.ScreenWidth * 0.5f;
            mousePos.Y = Game.Renderer.ScreenHeight * 0.5f - mousePos.Y;

            // Highlight any buttons
            foreach (var b in _buttons)
            {
                if (b.ContainsPoint(mousePos))
                {
                    b.Highlighted = true;
                }
                else
                {
                    b. Highlighted = false;
                }
            }
        }
    }

    public virtual void HandleKeyPress(InputState state)
    {
        if (state.Mouse.GetButtonState(MouseButton.Left) == ButtonState.Pressed)
        {
            if (_buttons.Any())
            {
                foreach (var b in _buttons)
                {
                    if (b.Highlighted)
                    {
                        b.Click();
                    }
                }
            }
        }
    }

    public void Close()
    {
        _state = UIScreenState.Closing;
    }

    public void AddButton(string name, Action onClick)
    {
        Vector2D<float> dims = new(_buttonOn.Width, _buttonOn.Height);
        Button b = new Button(name, onClick, NextButtonPos, dims);
        _buttons.Add(b);

        // Update position of next button
        // Move down by height of button plus padding
        _nextButtonPos.Y -= _buttonOff.Height + 20.0f;
    }

    protected void SetRelativeMouseMode(bool relative)
    {
        Game.InputSystem.SetRelativeMouseMode(relative);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}